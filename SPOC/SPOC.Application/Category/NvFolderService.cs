using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Category.Dto;
using SPOC.Common.Cookie;
using SPOC.Common.Extensions;
using SPOC.Common.Helper;
using SPOC.Exam;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SPOC.Category
{
    public class NvFolderService : ApplicationService, INvFolderService
    {
        private readonly IRepository<NvFolder, Guid> _iNvFolderRep;
        private readonly IRepository<NvFolderType, Guid> _iNvFolderTypeRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<ExamQuestion, Guid> _iExamQuestionRep;
        private readonly IRepository<Exam.ExamPaper, Guid> _iExamPaperRep;
     

        public NvFolderService(IRepository<NvFolder, Guid> iNvFolderRep, IRepository<NvFolderType, Guid> iNvFolderTypeRep,
            IRepository<TeacherInfo, Guid> iTeacherInfoRep, IRepository<ExamQuestion, Guid> iExamQuestionRep,
            IRepository<Exam.ExamPaper, Guid> iExamPaperRep)
        {
            _iNvFolderRep = iNvFolderRep;
            _iNvFolderTypeRep = iNvFolderTypeRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iExamQuestionRep = iExamQuestionRep;
            _iExamPaperRep = iExamPaperRep;
           
        }

        /// <summary>
        /// 创建新的分类节点
        /// </summary>
        /// <param name="input"></param>
        public async Task Create(NvFolderInputDto input)
        {
            //todo:缺少验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            NvFolder parentNvFolder = new NvFolder();
            #region 验证

            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (input.parentUid != Guid.Empty)
            {
                parentNvFolder = _iNvFolderRep.Get(input.parentUid);
                if (parentNvFolder == null)
                {
                    throw new UserFriendlyException("父分类不存在！");
                }
            }

            if (input.isCustomCode)
            {
                if (string.IsNullOrEmpty(input.folderCode))
                {
                    throw new UserFriendlyException("自定义分类编号不能为空");
                }

                if (_iNvFolderRep.GetAll().Any(a => a.folderCode == input.folderCode && a.folderTypeCode == input.folderTypeCode))
                {
                    throw new UserFriendlyException("已有相同的分类编号");
                }
            }
            #endregion

            #region 节点排序
            //获取需要挪动排序的列表
            var entity =
                _iNvFolderRep.GetAll().Where(
                    a =>
                        a.folderTypeCode == input.folderTypeCode && a.parentUid == input.parentUid &&
                        a.listOrder >= input.listOrder).OrderByDescending(a => a.listOrder).FirstOrDefault();

            if (entity != null)
            {
                //向后移动一位
                ListOrderOffset(input.folderTypeCode, input.parentUid, input.listOrder, entity.listOrder, 1)
                    .ForEach(a => _iNvFolderRep.UpdateAsync(a));
            }
            #endregion

            var id = Guid.NewGuid();

            if (!input.isCustomCode)
            {
                input.folderCode = CreateNewCode(input.folderTypeCode);
            }

            var newEntity = new NvFolder()
            {
                Id = id,
                parentUid = input.parentUid,
                fullPath = id.ToString(),
                folderCode = input.folderCode,
                isCustomCode = input.isCustomCode,
                folderTypeCode = input.folderTypeCode,
                folderName = input.folderName,
                createTime = DateTime.Now,
                folderLevel = input.folderLevel,
                listOrder = input.listOrder,
                remarks = input.remarks,
                creatorUid = cookie.Id
            };

            if (input.parentUid != Guid.Empty)
            {
                var parentFullPath = _iNvFolderRep.GetAll()
                    .Where(a => a.Id == input.parentUid)
                    .Select(a => a.fullPath)
                    .FirstOrDefault();
                newEntity.fullPath = parentFullPath + "," + id;
            }
            await _iNvFolderRep.InsertAsync(newEntity);

            if (parentNvFolder.Id != Guid.Empty)
            {
                parentNvFolder.hasChild = "Y";
                await _iNvFolderRep.UpdateAsync(parentNvFolder);
            }

        }
        /// <summary>
        /// 更新一个节点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Update(NvFolderInputDto input)
        {
            //todo:缺少验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            var entity = _iNvFolderRep.Get(input.Id);
            #region 验证
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (entity == null)
            {
                throw new UserFriendlyException("无效的数据");
            }


            #endregion

            #region 节点排序

            var parentFullPath = _iNvFolderRep.GetAll()
                .Where(a => a.Id == input.parentUid)
                .Select(a => a.fullPath)
                .FirstOrDefault();
            var newFullPath = parentFullPath + "," + entity.Id;
            //父节点改变
            if (entity.parentUid != input.parentUid)
            {
                //思路：获取节点与所有子级节点后对所有folder_level进行修改
                var childList = _iNvFolderRep.GetAll().Where(a => a.fullPath.StartsWith(entity.fullPath + ",")).ToList(); ;
                if (childList.Any())
                {
                    var offset = input.folderLevel - entity.folderLevel;
                    childList.ForEach(a =>
                    {
                        a.folderLevel += offset;
                        a.fullPath = a.fullPath.Replace(entity.fullPath, newFullPath);
                    });
                }
                //原父节点层级要减少一个节点
                var lastEntity1 =
                    _iNvFolderRep.GetAll().Where(a =>
                        a.folderTypeCode == input.folderTypeCode && a.parentUid == entity.parentUid &&
                        a.listOrder > entity.listOrder).OrderByDescending(a => a.listOrder).FirstOrDefault();

                if (lastEntity1 != null)
                {
                    //移动走的节点后的所有节点向前移动一位
                    ListOrderOffset(entity.folderTypeCode, entity.parentUid, entity.listOrder,
                        lastEntity1.listOrder, -1).ForEach(a => _iNvFolderRep.UpdateAsync(a));
                }

                //新层级要增加一个节点
                var lastEntity2 =
                    _iNvFolderRep.GetAll().Where(a =>
                        a.folderTypeCode == input.folderTypeCode && a.parentUid == input.parentUid &&
                        a.listOrder >= input.listOrder).OrderByDescending(a => a.listOrder).FirstOrDefault();

                if (lastEntity2 != null)
                {
                    //被插入位置所影响的节点向后移动一位
                    ListOrderOffset(input.folderTypeCode, input.parentUid, input.listOrder, lastEntity2.listOrder, 1)
                        .ForEach(a => _iNvFolderRep.UpdateAsync(a));
                }

                childList.ForEach(a => _iNvFolderRep.UpdateAsync(a));
            }
            else//父节点不变
            {
                //但是位置改变
                if (entity.listOrder != input.listOrder)
                {

                    IQueryable<NvFolder> list;
                    int offset;
                    if (entity.listOrder > input.listOrder) //该节点向前移动了
                    {
                        list = _iNvFolderRep.GetAll().Where(a =>
                            a.folderTypeCode == input.folderTypeCode && a.parentUid == input.parentUid &&
                            a.listOrder >= input.listOrder && a.listOrder < entity.listOrder);
                        offset = 1;
                    }
                    else //该节点向后移动了
                    {
                        list = _iNvFolderRep.GetAll().Where(a =>
                            a.folderTypeCode == input.folderTypeCode && a.parentUid == input.parentUid &&
                            a.listOrder <= input.listOrder && a.listOrder > entity.listOrder);
                        offset = -1;
                    }
                    NvFolder startEntity = list.OrderBy(a => a.listOrder).FirstOrDefault();
                    NvFolder endEntity = list.OrderByDescending(a => a.listOrder).FirstOrDefault();
                    if (startEntity != null && endEntity != null)
                    {
                        ListOrderOffset(entity.folderTypeCode, entity.parentUid, startEntity.listOrder,
                            endEntity.listOrder, offset).ForEach(a => _iNvFolderRep.UpdateAsync(a));
                    }
                }
            }

            #endregion

            entity.fullPath = newFullPath;
            entity.parentUid = input.parentUid;
            entity.folderCode = input.folderCode;
            entity.folderTypeCode = input.folderTypeCode;
            entity.folderName = input.folderName;
            entity.folderLevel = input.folderLevel;
            entity.listOrder = input.listOrder;
            entity.remarks = input.remarks;

            await _iNvFolderRep.UpdateAsync(entity);
        }
        /// <summary>
        /// 根据ID删除节点
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task Delete(string ids)
        {
            //todo:缺少验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            var idArray = ids.Split(',').Select(a=>new Guid(a)).ToArray();
            #region 验证
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (idArray.Any(id => !_iNvFolderRep.GetAll().Any(a => a.Id == id)))
            {
                throw new UserFriendlyException("无效的分类删除条件");
            }

            var checkedIdList = new List<Guid>();
            foreach (var id in idArray)
            {
                var guid = id;
                if (checkedIdList.Contains(guid))
                {
                    continue;
                }
                var folder = _iNvFolderRep.Get(guid);
                var children = _iNvFolderRep.GetAll().Where(a => a.fullPath.StartsWith(folder.fullPath)).ToList();
                children.ForEach(f =>
                {
                    checkedIdList.Add(f.Id);
                    if ((folder.folderTypeCode == "question_bank" && _iExamQuestionRep.GetAll().Any(a => a.folderUid == f.Id))
                    || (folder.folderTypeCode == "exam_paper" && _iExamPaperRep.GetAll().Any(a => a.folderUid == f.Id)) )
                    {
                        throw new UserFriendlyException("[" + f.folderName + "] 分类已被引用，无法进行删除操作！");
                    }
                });

            }

            #endregion

            //先删除子节点
            foreach (var id in checkedIdList)
            {
                if (idArray.Contains(id))
                {//遇到父节点跳过
                    continue;
                }
                await _iNvFolderRep.DeleteAsync(id);
            }
            //再删除父节点
            foreach (var id in idArray)
            {
                #region 节点排序
                //获取需要挪动排序的列表
                var guid = id;
                var entity = _iNvFolderRep.Get(guid);
                var lastEntity = _iNvFolderRep.GetAll().Where(
                    a =>
                        a.folderTypeCode == entity.folderTypeCode && a.parentUid == entity.parentUid &&
                        a.listOrder >= entity.listOrder).OrderByDescending(a => a.listOrder).FirstOrDefault();

                if (lastEntity != null)
                {
                    //向前移动一位
                    ListOrderOffset(entity.folderTypeCode, entity.parentUid, entity.listOrder, lastEntity.listOrder, -1)
                        .ForEach(a => _iNvFolderRep.UpdateAsync(a));
                }
                #endregion

                await _iNvFolderRep.DeleteAsync(guid);
            }
        }
        /// <summary>
        /// 根据分页条件获取分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<NvFolderPaginationOutputDto> GetPagination(NvFolderPaginationInputDto input)
        {
            var list = _iNvFolderRep.GetAll()
                .Where(a => a.folderTypeCode == input.nvFolderTypeCode);

            return await Task.FromResult(new NvFolderPaginationOutputDto()
            {
                total = list.Count(),
                rows = list.Skip(input.skip).Take(input.pageSize).MapTo<List<NvFolderItemOutputDto>>()
            });
        }

        /// <summary>
        /// 根据code获取分类数据
        /// </summary>
        /// <param name="folderTypeCode"></param>
        /// <returns></returns>
        public async Task<List<NvFolderItemOutputDto>> Get(string folderTypeCode)
        {
            #region 验证
            //验证分类是否存在
            if (!_iNvFolderTypeRep.GetAll().Any(a => a.folderTypeCode == folderTypeCode))
            {
                throw new UserFriendlyException("分类类型不存在");
            }
            #endregion

            var queryable = await _iNvFolderRep.GetAllListAsync(a => a.folderTypeCode == folderTypeCode);
            var list = queryable.MapTo<List<NvFolderItemOutputDto>>();
            return await Task.FromResult(list);
        }

        

        /// <summary>
        /// 同级节点偏移，用于新增、删除、移动节点后已有节点顺序需要修改时
        /// </summary>
        /// <param name="typeCode">分类类型</param>
        /// <param name="parentUid">父节点ID</param>
        /// <param name="beginListOrder">需要修改的开始排序号</param>
        /// <param name="endListOrder">需要修改的结束排序号</param>
        /// <param name="offset">偏移量可正可负</param>
        protected IQueryable<NvFolder> ListOrderOffset(string typeCode, Guid parentUid, int beginListOrder, int endListOrder, int offset)
        {
            var list = _iNvFolderRep.GetAll()
                    .Where(
                        a =>
                            a.folderTypeCode == typeCode && a.parentUid == parentUid &&
                            a.listOrder >= beginListOrder && a.listOrder <= endListOrder);
            list.ForEach(a => a.listOrder += offset);
            return list;
        }

        /// <summary>
        /// 获取给定节点的直系父节点
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        public async Task<List<NvFolder>> GetAllParent(Guid childId)
        {
            var nvFolder = await _iNvFolderRep.FirstOrDefaultAsync(d => d.Id == childId);
            if (nvFolder == null)
            {
                throw new UserFriendlyException("无效的子节点");
            }

            var guidList = nvFolder.fullPath.Split(',').Select(a => a.TryParseGuid()).ToList();
            return await _iNvFolderRep.GetAll().Where(a => guidList.Contains(a.Id)).OrderBy(a=>a.folderLevel).ToListAsync();
        }
        
        
        private string CreateNewCode(string folderTypeCode)
        {
            var code = "F000001";
            var entity = _iNvFolderRep.GetAll().Where(a => !a.isCustomCode && a.folderTypeCode == folderTypeCode).OrderByDescending(a => a.createTime).FirstOrDefault();
            if (entity != null)
            {
                code = entity.folderCode;
                do
                {
                    code = StringUtil.GetNextCodeByAuto(code);
                } while (_iNvFolderRep.GetAll().Any(a => a.folderCode == code));
            }
            return code;
        }
    }
}