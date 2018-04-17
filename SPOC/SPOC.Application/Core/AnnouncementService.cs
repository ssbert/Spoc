using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Abp.Application.Services;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Castle.Core.Logging;
using SPOC.Common.Cookie;
using SPOC.Common.EasyUI;
using SPOC.Common.Extensions;
using SPOC.Common.File;
using SPOC.Common.Helper;
using SPOC.Common.Pagination;
using SPOC.Core.Dto;
using SPOC.Core.Dto.Announcement;
using SPOC.User;

namespace SPOC.Core
{
    /// <summary>
    /// 公告服务接口实现
    /// </summary>
    public class AnnouncementService : SPOCAppServiceBase,IAnnouncementService
    {
        private readonly IRepository<Announcement, Guid> _iAnnouncementRep;
        private readonly IRepository<ClassAnnouncement, Guid> _iClassAnnouncementRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<Class, Guid> _iClassRep;
        private readonly IRepository<ClassStudent, Guid> _iClassStudentRep;
        private readonly IRepository<UploadFile, Guid> _iUploadFileRep;
        private readonly IRepository<AnnouncementFile, Guid> _iAnnouncementFileRep;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        /// <summary>
        /// 构造函数
        /// </summary>
        public AnnouncementService(IRepository<Announcement, Guid> iAnnouncementRep, IRepository<ClassAnnouncement, Guid> iClassAnnouncementRep, IRepository<UserBase, Guid> iUserBaseRep, IUnitOfWorkManager unitOfWorkManager, IRepository<ClassStudent, Guid> iClassStudentRep, IRepository<Class, Guid> iClassRep, IRepository<UploadFile, Guid> iUploadFileRep, IRepository<AnnouncementFile, Guid> iAnnouncementFileRep)
        {
            _iAnnouncementRep = iAnnouncementRep;
            _iClassAnnouncementRep = iClassAnnouncementRep;
            _iUserBaseRep = iUserBaseRep;
            _unitOfWorkManager = unitOfWorkManager;
            _iClassStudentRep = iClassStudentRep;
            _iClassRep = iClassRep;
            _iUploadFileRep = iUploadFileRep;
            _iAnnouncementFileRep = iAnnouncementFileRep;
        }
        /// <summary>
        /// 公告管理列表
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public EasyUiListResultDto<AnnouncementDto> GetAnnouncementList(AnnouncementInputDto inputDto)
        {
            EasyUiListResultDto<AnnouncementDto> announcements = new EasyUiListResultDto<AnnouncementDto>();
            try
            {
                if (!LoginValidation.IsLogin())
                {
                    throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
                }
                var cookie = CookieHelper.GetLoginInUserInfo();
                //根据发布的班级过滤
                var announcementIds = _iClassAnnouncementRep.GetAll()
                    .Where(c => inputDto.classId.Contains(c.classId.ToString())).Select(c => c.announcementId).ToList();
                inputDto.pageSize = inputDto.rows;
                inputDto.skip = (inputDto.rows * (inputDto.page-1));
                var data = (from c in _iAnnouncementRep.GetAll().WhereIf(!string.IsNullOrWhiteSpace(inputDto.classId),a=> announcementIds.Contains(a.Id))
                        .WhereIf(inputDto.enable<0, a => inputDto.enable.Equals(a.enable)).WhereIf(!string.IsNullOrWhiteSpace(inputDto.title), a => a.title.Contains(inputDto.title))
                            join u in _iUserBaseRep.GetAll() on c.createUserId equals u.Id into st
                    from tmp in st.DefaultIfEmpty()
                    orderby c.createTime descending
                    select new AnnouncementDto
                    {
                        id = c.Id,
                        content=c.content,
                        title = c.title,
                        updateTime= c.updateTime,
                        enable=c.enable,
                        userId=c.createUserId,
                        userName =tmp !=null? (!string.IsNullOrEmpty(tmp.userFullName)? tmp.userFullName: tmp.userLoginName):"",
                     
                    });
                if (!data.Any())
                {
                    return announcements;
                }
                //当前用户不是管理员只能查看自己创建的公告
                data = data.WhereIf(!cookie.IsAdmin, a => a.userId == cookie.Id);
                //公告发布班级统计
                var classAnnouncements = from ca in _iClassAnnouncementRep.GetAll()
                    join cl in _iClassRep.GetAll() on ca.classId equals cl.Id
                    select new
                    {
                        ca.announcementId,
                        ca.classId,
                        cl.name
                    };
                announcements.rows = data.OrderBy(inputDto.OrderExpression).Skip(inputDto.skip).Take(inputDto.pageSize).ToList();
                announcements.total = data.Count();
                announcements.rows.ForEach(a =>
                    {
                        var classAnnouncement = classAnnouncements.Where(d => d.announcementId == a.id).Select(p => p.name).ToList();
                        a.classNames = classAnnouncement.Any() ? classAnnouncement.Aggregate((right, left) => right + "," + left) : "";
                        classAnnouncement = classAnnouncements.Where(d => d.announcementId == a.id).Select(p => p.classId.ToString()).ToList();
                        a.classIds = classAnnouncement.Any() ? classAnnouncement.Aggregate((right, left) => right + "," + left) : "";
                        var file = (from af in _iAnnouncementFileRep.GetAll()
                            join f in _iUploadFileRep.GetAll() on af.UploadFileId equals f.Id
                            where af.AnnouncementId == a.id
                            select f).FirstOrDefault();
                        if (file != null)
                        {
                            a.fileName = file.FileName;
                            a.fileId = file.Id;
                        }
                    });

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return announcements;
        }
        /// <summary>
        /// 新增公告
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task InsertAnnouncement(AnnouncementInputDto input)
        {
            //try
            //{
                if (!LoginValidation.IsLogin())
                {
                    throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
                }
                var cookie = CookieHelper.GetLoginInUserInfo();
                var dto = new Announcement
                {
                    content = HttpUtility.UrlDecode(input.content),
                    createTime = DateTime.Now,
                    enable = 1,//(byte) input.enable,
                    Id = Guid.NewGuid(),
                    title = input.title,
                    createUserId = new Guid(cookie.UserUid),
                    updateTime = DateTime.Now
                };

                await _iAnnouncementRep.InsertAsync(dto);
                //公告发布到班级
                var classIds = input.classId.Split(',');
                var classAnnouncement = _iClassAnnouncementRep.GetAll().Where(ca =>  ca.announcementId.Equals(dto.Id)).ToList();
                foreach (var classId in classIds)
                {
                    if (string.IsNullOrEmpty(classId))
                        continue;
                    if (!classAnnouncement.Any(ca => ca.classId.Equals(new Guid(classId)) && ca.announcementId.Equals(dto.Id)))
                    {
                        var caDto = new ClassAnnouncement()
                        {
                            Id = Guid.NewGuid(),
                            announcementId = dto.Id,
                            classId = classId.TryParseGuid(),
                            createTime = DateTime.Now

                        };
                        await _iClassAnnouncementRep.InsertAsync(caDto);
                    }
                }

            if (input.tempFileId != Guid.Empty)
            {
                await AddFile(new AddAnnouncementFileInputDto
                {
                    Id = dto.Id,
                    FileName = input.fileName,
                    TempFileId = input.tempFileId.ToString()
                });
            }
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error(ex.ToString());
            //}
        }
        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task ModifyAnnouncement(AnnouncementInputDto input)
        {
            try
            {
                if (!LoginValidation.IsLogin())
                {
                    throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
                }
   
                var announcement = _iAnnouncementRep.FirstOrDefault(new Guid(input.id));

                announcement.content = HttpUtility.UrlDecode(input.content);
                announcement.updateTime = DateTime.Now;
                announcement.enable = 1; //(byte) input.enable;
                announcement.title = input.title;
                await _iAnnouncementRep.UpdateAsync(announcement);
                //公告发布到班级
                //没有发布班级时表示当前公告不发布到任何班级 删除所有公告关联的班级
                if (string.IsNullOrEmpty(input.classId))
                {
                    _iClassAnnouncementRep.Delete(a=>a.announcementId.Equals(announcement.Id));
                    return;
                }
                var classIds = input.classId.Split(',');
                //先根据编辑界面删除项 删除关联表中的数据
                _iClassAnnouncementRep.Delete(
                    a => !input.classId.Contains(a.classId.ToString()) && a.announcementId.Equals(announcement.Id));
                var classAnnouncement = _iClassAnnouncementRep.GetAll().Where(ca => ca.announcementId.Equals(announcement.Id)).ToList();
                await _unitOfWorkManager.Current.SaveChangesAsync();
                foreach (var classId in classIds)
                {
                    if (string.IsNullOrEmpty(classId))
                        continue;
                    if (!classAnnouncement.Any(ca => ca.classId.Equals(new Guid(classId)) && ca.announcementId.Equals(announcement.Id)))
                    {
                        var caDto = new ClassAnnouncement()
                        {
                            Id = Guid.NewGuid(),
                            announcementId = announcement.Id,
                            classId = classId.TryParseGuid(),
                            createTime = DateTime.Now

                        };
                        await _iClassAnnouncementRep.InsertAsync(caDto);
                    }
                }

                //原来已有附件，需要删除附件
                if (input.fileId == Guid.Empty &&
                    await _iAnnouncementFileRep.GetAll().AnyAsync(a => a.AnnouncementId == announcement.Id))
                {
                    await DeleteFile(announcement.Id);
                }

                if (input.tempFileId != Guid.Empty)
                {
                    await AddFile(new AddAnnouncementFileInputDto
                    {
                        Id = announcement.Id,
                        FileName = input.fileName,
                        TempFileId = input.tempFileId.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteAnnouncement(BatchRequestInput input)
        {
            try
            {
                if (input == null)
                {
                    return;
                }
                for (int i = 0; i < input.Id.Split(',').Length; i++)
                {
                    if(string.IsNullOrEmpty((input.Id.Split(',')[i])))
                        continue;
                    var uid = new Guid(input.Id.Split(',')[i]);
                    await _iClassAnnouncementRep.DeleteAsync(c=>c.announcementId.Equals(uid));
                    await _iAnnouncementRep.DeleteAsync(uid);
                    //原来已有附件，需要删除附件
                    if (await _iAnnouncementFileRep.GetAll().AnyAsync(a => a.AnnouncementId == uid))
                    {
                        await DeleteFile(uid);
                    }
                }
               
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        /// <summary>
        /// 获取公告详细信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public AnnouncementDto GetAnnouncement(AnnouncementInputDto input)
        {
            AnnouncementDto dto = new AnnouncementDto();
            try
            {
                Announcement ca = _iAnnouncementRep.FirstOrDefault(d => d.Id.ToString() == input.id);
                if (ca == null)
                {
                    return dto;
                }
                dto.id = ca.Id;
                dto.title = ca.title;
                dto.content = ca.content;
                var classAnnouncements = _iClassAnnouncementRep.GetAll()
                    .Where(a => a.announcementId.Equals(ca.Id)).Select(p => p.classId.ToString()).ToList();
                dto.classIds = classAnnouncements.Any() ? classAnnouncements.Aggregate((right, left) => right + "," + left) : "";
                dto.enable = ca.enable;
                var file = (from af in _iAnnouncementFileRep.GetAll()
                    join f in _iUploadFileRep.GetAll() on af.UploadFileId equals f.Id
                    where af.AnnouncementId == ca.Id
                    select f).FirstOrDefault();
                if (file != null)
                {
                    dto.fileName = file.FileName;
                    dto.fileId = file.Id;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return dto;
        }

        #region 附件操作
        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddFile(AddAnnouncementFileInputDto input)
        {
            await DeleteFile(input.Id);

            var fileInfo = new FileInfo(Path.Combine(UploadHelper.TempDirPath, input.TempFileId));
            //添加现有文件
            var uploadFile = new UploadFile
            {
                Id = Guid.NewGuid(),
                FileName = input.FileName,
                Source = "announcement",
                Size = fileInfo.Length
            };
            UploadHelper.CreateFile(input.TempFileId, uploadFile.Id.ToString());
            await _iUploadFileRep.InsertAsync(uploadFile);
            await _iAnnouncementFileRep.InsertAsync(new AnnouncementFile
            {
                Id = Guid.NewGuid(),
                AnnouncementId = input.Id,
                UploadFileId = uploadFile.Id
            });
        }

        /// <summary>
        /// 删除已有附件
        /// </summary>
        /// <param name="id">announcementId</param>
        /// <returns></returns>
        private async Task DeleteFile(Guid id)
        {
            var list = await _iAnnouncementFileRep.GetAll().Where(a => a.AnnouncementId == id).ToListAsync();
            foreach (var announcementFile in list)
            {
                await _iUploadFileRep.DeleteAsync(announcementFile.UploadFileId);
                UploadHelper.DeleteFile(announcementFile.UploadFileId.ToString());
                await _iAnnouncementFileRep.DeleteAsync(announcementFile);
            }
        }
        #endregion

        #region 前台
        /// <summary>
        /// 获取我的公告
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public MyAnnouncementViewDto GetMyAnnouncement(PaginationInputDto input)
        {
            try
            {
                if (!LoginValidation.IsLogin())
                {
                    throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
                
                }
                var cookie = CookieHelper.GetLoginInUserInfo();
                var uid = cookie.UserUid.TryParseGuid();
                var data = (from c in _iAnnouncementRep.GetAll().Where(a => a.enable == 1)
                    join ca in _iClassAnnouncementRep.GetAll() on c.Id equals ca.announcementId
                            join stu in _iClassStudentRep.GetAll() on ca.classId equals stu.ClassId
                    join user in _iUserBaseRep.GetAll() on c.createUserId equals user.Id
                            where stu.UserId == uid
                    orderby ca.createTime descending
                    select new MyAnnouncementDto
                    {
                        id = c.Id,
                        content = c.content,
                        title = c.title,
                        createTime = ca.createTime,
                        createUser= (!string.IsNullOrEmpty(user.userFullName) ? user.userFullName : user.userLoginName),
                        userHeadImg=user.smallAvatar

                    });
                if (string.IsNullOrWhiteSpace(input.OrderExpression))
                {
                    input.sort = "createTime";
                    input.order = "desc";
                }
                var list = data.OrderBy(input.OrderExpression).Skip(input.skip).Take(input.pageSize).ToList();
                list.ForEach(a =>
                {
                    var content = StringUtil.NoHTML(a.content);
                    a.content = content.Length > 200 ? content.Substring(0, 200) + "..." : content;
                });
                return new MyAnnouncementViewDto{MyAnnouncementList= list ,SumPage= data.Count()% input.pageSize>0? data.Count() / input.pageSize+1: data.Count() / input.pageSize, CurrentPage = input.page+1,Total = data.Count() };
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString);
            }
            return new MyAnnouncementViewDto();
        }
        /// <summary>
        /// 前台公告详细信息
        /// </summary>
        /// <param name="id">公告ID</param>
        /// <returns></returns>
        public AnnouncementFrontViewDto AnnouncementDetail(Guid id)
        {
            AnnouncementFrontViewDto dto = new AnnouncementFrontViewDto();
            try
            {
                var data = from c in _iAnnouncementRep.GetAll()
                    join user in _iUserBaseRep.GetAll() on c.createUserId equals user.Id
                    where c.createUserId == user.Id && c.Id == id
                           select new AnnouncementFrontViewDto
                    {
                        updateTime=c.updateTime,
                        id=c.Id,
                        title =c.title,
                        content = c.content,
                        userName = (!string.IsNullOrEmpty(user.userFullName) ? user.userFullName : user.userLoginName)
                           };
                dto = data.FirstOrDefault();
                var file = (from af in _iAnnouncementFileRep.GetAll()
                    join f in _iUploadFileRep.GetAll() on af.UploadFileId equals f.Id
                    where af.AnnouncementId == id
                    select f).FirstOrDefault();
                if (file != null && dto != null)
                {
                    dto.fileName = file.FileName;
                    dto.fileId = file.Id;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return dto;
        }
        #endregion
    }
}

