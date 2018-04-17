using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Category.Dto;
using SPOC.Common.Cookie;
using SPOC.Common.Extensions;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOC.Category
{
    public class NvFolderTypeService:ApplicationService, INvFolderTypeService
    {
        private readonly IRepository<NvFolder, Guid> _iNvFolderRep;
        private readonly IRepository<NvFolderType, Guid> _iNvFolderTypeRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;

        private readonly INvFolderService _iNvFolderService;

        public NvFolderTypeService(IRepository<NvFolder, Guid> iNvFolderRep,
            IRepository<NvFolderType, Guid> iNvFolderTypeRep,
            IRepository<TeacherInfo, Guid> iTeacherInfoRep,
            INvFolderService iNvFolderService)
        {
            _iNvFolderRep = iNvFolderRep;
            _iNvFolderTypeRep = iNvFolderTypeRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iNvFolderService = iNvFolderService;
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<NvFolderTypePaginationOutputDto> GetPagination(NvFolderTypePaginationInputDto input)
        {
            input.folderTypeCode = input.folderTypeCode.Trim();
            input.folderTypeName = input.folderTypeName.Trim();
            var folderTypes = _iNvFolderTypeRep.GetAll();
            var folders = _iNvFolderRep.GetAll();
            var queryable = (from ft in folderTypes
                join f in folders on ft.folderTypeCode equals f.folderTypeCode
                where f.parentUid == Guid.Empty &&
                (string.IsNullOrEmpty(input.folderTypeName) || ft.folderTypeName.Contains(input.folderTypeName)) &&
                (string.IsNullOrEmpty(input.folderTypeCode) || ft.folderTypeCode.Contains(input.folderTypeCode))
                orderby ft.listOrder
                select new NvFolderTypeDto()
                {
                    Id = ft.Id,
                    folderTypeName = ft.folderTypeName,
                    folderTypeCode = ft.folderTypeCode,
                    remarks = ft.remarks,
                    folderCode = f.folderCode,
                    folderName = f.folderName,
                    listOrder = ft.listOrder
                });

            return await Task.FromResult(new NvFolderTypePaginationOutputDto()
            {
                rows = queryable.Skip(input.skip).Take(input.pageSize).ToList(),
                total = queryable.Count()
            });
        }

        public async Task<List<NvFolderTypeDto>> GetAll(NvFolderQueryInputDto input)
        {
            input.folderTypeCode = input.folderTypeCode.Trim();
            input.folderTypeName = input.folderTypeName.Trim();
            var folderTypes = _iNvFolderTypeRep.GetAll();
            var folders = _iNvFolderRep.GetAll();
            var queryable = (from ft in folderTypes
                             join f in folders on ft.folderTypeCode equals f.folderTypeCode
                             where f.parentUid == Guid.Empty &&
                             (string.IsNullOrEmpty(input.folderTypeName) || ft.folderTypeName.Contains(input.folderTypeName)) &&
                             (string.IsNullOrEmpty(input.folderTypeCode) || ft.folderTypeCode.Contains(input.folderTypeCode))
                             orderby ft.listOrder
                             select new NvFolderTypeDto()
                             {
                                 Id = ft.Id,
                                 folderTypeName = ft.folderTypeName,
                                 folderTypeCode = ft.folderTypeCode,
                                 remarks = ft.remarks,
                                 folderCode = f.folderCode,
                                 folderName = f.folderName,
                                 listOrder = ft.listOrder
                             });
            return await Task.FromResult(queryable.ToList());
        }

        public async Task Create(NvFolderTypeDto input)
        {
            //TODO:缺少验证
            var cookie = CookieHelper.GetLoginInUserInfo();

            #region 验证
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (_iNvFolderTypeRep.GetAll().Any(a => a.folderTypeCode == input.folderTypeCode))
            {
                throw new UserFriendlyException("已有同样的类型编号");
            }
            
            #endregion


            var listOrder = input.listOrder == -1 ? _iNvFolderTypeRep.GetAll().Count() : input.listOrder;
            var nvFolderType = new NvFolderType()
            {
                Id = Guid.NewGuid(),
                folderTypeName = input.folderTypeName,
                folderTypeCode = input.folderTypeCode,
                remarks = input.remarks,
                listOrder = listOrder
            };

            #region 节点排序
            if (input.listOrder != -1)
            {
                //获取需要挪动排序的列表
                var lastEntity = _iNvFolderTypeRep.GetAll().Where(
                    a => a.listOrder >= nvFolderType.listOrder).OrderByDescending(a => a.listOrder).FirstOrDefault();

                if (lastEntity != null)
                {
                    //向后移动一位
                    ListOrderOffset(nvFolderType.listOrder, lastEntity.listOrder, 1)
                        .ForEach(a => _iNvFolderTypeRep.UpdateAsync(a));
                }
            }
            #endregion

            await _iNvFolderTypeRep.InsertAsync(nvFolderType);
            await _iNvFolderService.Create(new NvFolderInputDto()
            {
                parentUid = Guid.Empty,
                folderCode = input.folderCode,
                folderName = input.folderName,
                folderTypeCode = input.folderTypeCode,
                isCustomCode = input.isCustomCode,
                listOrder = _iNvFolderRep.GetAll().Count(a => a.parentUid == Guid.Empty)
            });

           

        }

        public async Task Update(NvFolderTypeDto input)
        {
            //TODO:缺少验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            var entity = _iNvFolderTypeRep.Get(input.Id);

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

            if (_iNvFolderTypeRep.GetAll().Any(a => a.folderTypeCode == input.folderTypeCode && a.Id != input.Id))
            {
                throw new UserFriendlyException("已有同样的类型编号");
            }

            #endregion

            #region 节点排序
            //但是位置改变
            if (entity.listOrder != input.listOrder)
            {

                IQueryable<NvFolderType> list;
                int offset;
                if (entity.listOrder > input.listOrder) //该节点向前移动了
                {
                    list =
                        _iNvFolderTypeRep.GetAll()
                            .Where(a => a.listOrder >= input.listOrder && a.listOrder < entity.listOrder);
                    offset = 1;
                }
                else //该节点向后移动了
                {
                    list =
                        _iNvFolderTypeRep.GetAll()
                            .Where(a => a.listOrder <= input.listOrder && a.listOrder > entity.listOrder);
                    offset = -1;
                }
                NvFolderType startEntity = list.OrderBy(a => a.listOrder).FirstOrDefault();
                NvFolderType endEntity = list.OrderByDescending(a => a.listOrder).FirstOrDefault();
                if (startEntity != null && endEntity != null)
                {
                    ListOrderOffset(startEntity.listOrder, endEntity.listOrder, offset)
                        .ForEach(a => _iNvFolderTypeRep.UpdateAsync(a));
                }
            }

            #endregion
            //若是分类类型、组织架构、学科有改动，则要更新分类的信息
            if (entity.folderTypeCode != input.folderTypeCode) 
            {
                var queryables = _iNvFolderRep.GetAll().Where(a=>a.folderTypeCode == entity.folderTypeCode);
                queryables.ForEach(a =>
                {
                    a.folderTypeCode = input.folderTypeCode;
                    _iNvFolderRep.UpdateAsync(a);
                });
            }

            entity.folderTypeName = input.folderTypeName;
            entity.folderTypeCode = input.folderTypeCode;
            entity.remarks = input.remarks;
            entity.listOrder = input.listOrder;
            await _iNvFolderTypeRep.UpdateAsync(entity);
        }

        public async Task Delete(string ids)
        {
            //todo:缺少验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            var idArray = ids.Split(',');
            #region 验证
            if (cookie == null || cookie.Id == Guid.Empty)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            //1、id是否存在
            //2、根节点是否有子节点
            //3、根节点是否已经被相关的表引用
            foreach (var id in idArray)
            {
                var guid = new Guid(id);
                var entity = _iNvFolderTypeRep.Get(guid);
                if (entity == null)
                {
                    throw new UserFriendlyException("无效的id");
                }

                var rootFolder =
                    _iNvFolderRep.FirstOrDefault(
                        a => a.parentUid == Guid.Empty && a.folderTypeCode == entity.folderTypeCode);
                if (rootFolder.hasChild == "Y")
                {
                    throw new UserFriendlyException("根分类已有子分类，不可删除，请先删除子分类！");
                }
                //todo:3号验证条件需要补全
            }
            #endregion

            foreach (var id in idArray)
            {
                #region 节点排序

                //获取需要挪动排序的列表
                var guid = new Guid(id);
                var entity = _iNvFolderTypeRep.Get(guid);
                var lastEntity = _iNvFolderTypeRep.GetAll().Where(
                    a => a.listOrder >= entity.listOrder).OrderByDescending(a => a.listOrder).FirstOrDefault();

                if (lastEntity != null)
                {
                    //向前移动一位
                    ListOrderOffset(entity.listOrder, lastEntity.listOrder, -1)
                        .ForEach(a => _iNvFolderTypeRep.UpdateAsync(a));
                }

                #endregion

                var rootFolder =
                    _iNvFolderRep.FirstOrDefault(
                        a => a.parentUid == Guid.Empty && a.folderTypeCode == entity.folderTypeCode);

                await _iNvFolderService.Delete(rootFolder.Id.ToString());
                await _iNvFolderTypeRep.DeleteAsync(guid);
            }
        }
        /// <summary>
        /// 排序偏移，用于新增、删除、移动节点后已有节点顺序需要修改时
        /// </summary>
        /// <param name="beginListOrder">需要修改的开始排序号</param>
        /// <param name="endListOrder">需要修改的结束排序号</param>
        /// <param name="offset">偏移量可正可负</param>
        /// <returns></returns>
        protected IQueryable<NvFolderType> ListOrderOffset(int beginListOrder, int endListOrder, int offset)
        {
            var list =
                _iNvFolderTypeRep.GetAll().Where(a => a.listOrder >= beginListOrder && a.listOrder <= endListOrder);
            list.ForEach(a=>a.listOrder += offset);

            return list;
        }
    }
}