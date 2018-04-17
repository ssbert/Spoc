using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Dto;
using SPOC.Lib.Dto;
using SPOC.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SPOC.Lib
{
    /// <summary>
    /// 知识图谱服务接口实现
    /// </summary>
    public class StructureMapService:SPOCAppServiceBase, IStructureMapService
    {
        private readonly IRepository<StructureMap, Guid> _iStructureMapRep;
        private readonly IRepository<UserBase, Guid> _iUserBaseRep;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public StructureMapService(IRepository<StructureMap, Guid> iStructureMapRep, IRepository<UserBase, Guid> iUserBaseRep, IRepository<TeacherInfo, Guid> iTeacherInfoRep)
        {
            _iStructureMapRep = iStructureMapRep;
            _iUserBaseRep = iUserBaseRep;
            _iTeacherInfoRep = iTeacherInfoRep;
        }

        #endregion

        /// <summary>
        /// 获取单个知识图谱数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<StructureMapDto> Get(Guid id)
        {
            var result = await _iStructureMapRep.FirstOrDefaultAsync(id);
            return result.MapTo<StructureMapDto>();
        }

        /// <summary>
        /// 获取所有知识图谱列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<StructureMapItem>> GetList()
        {
            var list = await (from map in _iStructureMapRep.GetAll()
                join user in _iUserBaseRep.GetAll() on map.CreatorId equals user.Id
                select new StructureMapItem
                {
                    Id = map.Id,
                    CreatorId = map.CreatorId,
                    Title = map.Title,
                    UserLoginName = user.userLoginName,
                    UserFullName = user.userFullName,
                    IsShow = map.IsShow,
                    IsMain = map.IsMain,
                    CreateTime = map.CreateTime
                }).ToListAsync();
            return list;
        }

        /// <summary>
        /// 创建知识图谱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<StructureMapDto> Create(StructureMapDto input)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }
            var isTeacher = await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id);
            if (!cookie.IsAdmin && !isTeacher)
            {
                throw new UserFriendlyException("没有权限");
            }

            #endregion

            var entity = input.MapTo<StructureMap>();
            entity.Id = Guid.NewGuid();
            entity.CreatorId = cookie.Id;
            entity.CreateTime = DateTime.Now;
            entity.IsShow = true;
            entity.IsMain = await _iStructureMapRep.GetAll().AllAsync(a => !a.IsMain);

            await _iStructureMapRep.InsertAsync(entity);

            return entity.MapTo<StructureMapDto>();
        }

        /// <summary>
        /// 更新知识图谱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Update(StructureMapDto input)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }
            var isTeacher = await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id);
            if (!cookie.IsAdmin && !isTeacher)
            {
                throw new UserFriendlyException("没有权限");
            }

            var entity = await _iStructureMapRep.FirstOrDefaultAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的数据");
            }

            #endregion

            input.MapTo(entity);
            await _iStructureMapRep.UpdateAsync(entity);
        }

        /// <summary>
        /// 更新知识图谱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateData(StructureMapDataInputDto input)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }
            var isTeacher = await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id);
            if (!cookie.IsAdmin && !isTeacher)
            {
                throw new UserFriendlyException("没有权限");
            }

            var entity = await _iStructureMapRep.FirstOrDefaultAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的数据");
            }

            #endregion

            input.MapTo(entity);
            await _iStructureMapRep.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除知识图谱
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task Delete(IdListInputDto input)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }
            var isTeacher = await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id);
            if (!cookie.IsAdmin && !isTeacher)
            {
                throw new UserFriendlyException("没有权限");
            }

            #endregion

            foreach (var guid in input.idList)
            {
                await _iStructureMapRep.DeleteAsync(guid);
            }
        }

        /// <summary>
        /// 设置为主图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task SetIsMain(Guid id)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }
            var isTeacher = await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id);
            if (!cookie.IsAdmin && !isTeacher)
            {
                throw new UserFriendlyException("没有权限");
            }
            var entity = await _iStructureMapRep.FirstOrDefaultAsync(id);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的数据");
            }

            #endregion
            
            var mainMaps = await _iStructureMapRep.GetAll().Where(a => a.IsMain).ToListAsync();
            foreach (var map in mainMaps)
            {
                map.IsMain = false;
                await _iStructureMapRep.UpdateAsync(map);
            }

            entity.IsMain = true;
            await _iStructureMapRep.UpdateAsync(entity);
        }

        /// <summary>
        /// 更新设置是否显示
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateIsShow(KeyValuePair<Guid, bool> input)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录或登录已过期");
            }
            var isTeacher = await _iTeacherInfoRep.GetAll().AnyAsync(a => a.userId == cookie.Id);
            if (!cookie.IsAdmin && !isTeacher)
            {
                throw new UserFriendlyException("没有权限");
            }
            var entity = await _iStructureMapRep.FirstOrDefaultAsync(input.Key);
            if (entity == null)
            {
                throw new UserFriendlyException("无效的数据");
            }

            #endregion

            await _iStructureMapRep.UpdateAsync(input.Key, map =>
            {
                map.IsShow = input.Value;
                return Task.FromResult(map);
            });
        }

        /// <summary>
        /// 获取主知识图谱数据
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetMainMapData()
        {
            var entity = await _iStructureMapRep.FirstOrDefaultAsync(a => a.IsMain);
            if (entity == null)
            {
                return "";
            }
            return entity.MapData;
        }
    }
}