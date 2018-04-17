using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Abp.Web.Models;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.SysSetting.RoleManageDTO;
using SPOC.User.Dto.Department;
using SPOC.User.Dto.Teacher;

namespace SPOC.User
{
    /// <summary>
    /// 院系 专业 班级 组织架构服务接口
    /// </summary>
    public interface IDepartmentService : IApplicationService
    {

        /// <summary>
        /// 获取院系列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        EasyUiListResultDto<FacultyOutDto> GetFacultyUiList(FacultyInputDto input);
        /// <summary>
        /// 新增院系
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        Task InsertFaculty(FacultyInputDto input);
        /// <summary>
        /// 删除院系
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> DeleteFaculty(BatchRequestInput input);
        /// <summary>
        /// 修改院系
        /// </summary>
        /// <param name="input"></param>
        void ModifyFaculty(FacultyInputDto input);

        /// <summary>
        /// 获取专业列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        EasyUiListResultDto<MajorOutDto> GetMajorUiList(MajorInputDto input);
        /// <summary>
        /// 新增专业
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        Task InsertMajor(MajorInputDto input);
        /// <summary>
        /// 删除专业
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> DeleteMajor(BatchRequestInput input);
        /// <summary>
        /// 修改专业
        /// </summary>
        /// <param name="input"></param>
        void ModifyMajor(MajorInputDto input);

        /// <summary>
        /// 获取班级列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        EasyUiListResultDto<ClassOutDto> GetClassUiList(ClassInputDto input);
        /// <summary>
        /// 获取行政班级列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        EasyUiListResultDto<ClassOutDto> GetAdministrativeClassUiList(ClassInputDto input);
        /// <summary>
        /// 新增班级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableValidation]
        Task InsertClass(ClassInputDto input);
        /// <summary>
        /// 删除班级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> DeleteClass(BatchRequestInput input);
        /// <summary>
        /// 删除行政班级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> DeleteAdministrativeClass(BatchRequestInput input);
        /// <summary>
        /// 修改班级
        /// </summary>
        /// <param name="input"></param>
        void ModifyClass(ClassInputDto input);
        /// <summary>
        /// 获取班级下教师列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DontWrapResult]
        EasyUiListResultDto<ClassTeacherOutDto> GetTeachersByClassId(ClassInputDto input);
        /// <summary>
        /// 班级关联教师
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task AddTeachers(RigthTeacherInputDto input);
        /// <summary>
        /// 删除班级下关联的教师
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DelClassTeacher(BatchRequestInput input);
        /// <summary>
        /// 班级设置教师获取教师待选框列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [DontWrapResult]
        EasyUiListResultDto<LeftTeacherInfoOutDto> GetLeftTeachers(LeftTeacherInfoDto dto);
        /// <summary>
        /// 院系下拉选项
        /// </summary>
        /// <returns></returns>
        [DontWrapResult]
        IEnumerable<object> CmbFaculty();

        /// <summary>
        /// 专业下拉选项
        /// </summary>
        /// <returns></returns>
        [DontWrapResult]
        [HttpGet]   
        IEnumerable<object> CmbMajor(string facultyId);
        /// <summary>
        /// 班级下拉选项
        /// </summary>
        /// <returns></returns>
        [DontWrapResult]
        [HttpGet]
        IEnumerable<object> CmbClass(string majorId);
        /// <summary>
        /// 行政班级下拉列表
        /// </summary>
        /// <param name="majorId"></param>
        /// <returns></returns>
        [DontWrapResult]
        [HttpGet]
        IEnumerable<object> CmbAdministrativeClass(string majorId);
        /// <summary>
        /// 根据班级权限获取组织架构树(院系、专业、班级)
        /// </summary>
        /// <returns></returns>
        [DontWrapResult]
        IEnumerable<SelectListItemDto> GetDepartmentTree();

        /// <summary>
        /// 获取组织架构树(院系、专业、班级) 直接获取所有节点不进行班级权限过滤
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItemDto> GetAllDepartmentTree();
    

        /// <summary>
        /// 若用户为管理员，则获取所有班级
        /// 若用户为教师，则获取关联的所有班级
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        Task<List<ClassOutDto>> GetAllClass();

        /// <summary>
        /// 根据Class的Id列表获取Class的相关信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<ClassOutDto>> GetClassesByIdList(IdListInputDto input);

        Task<IEnumerable<System.Web.Mvc.SelectListItem>> GetAllClassSelectItem(bool loadEmpty);
    }
}
