using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.User.Dto.Common;
using SPOC.User.Dto.StudentInfo;

namespace SPOC.User
{
  
    public interface IStudentInfoService : IApplicationService
    {

        EasyUiListResultDto<StudentInfoDto> GetStudentInfoByGuid(StudentInfoInputDto input);

        StudentInfoDto GetStudentInfo(UserInfoQueryInputDto model);
        [DisableValidation]
        void UpdateStudentInfo(CreateStudentInfoInputDto input);
        [DisableValidation]
        Task CreateStudentInfo(CreateStudentInfoInputDto input);


        Task DeleteStudentInfos(List<BatchDeleteRequestInputByUser> inputList);


        Task ActiveStudentInfo(List<BatchDeleteRequestInputByUser> inputList);

   
        System.IO.Stream GetStudentInfoExportTemplate(string names);

        ImportResultOutputDto CreateUserInfoFromFile(Stream fileStream, string departmentUid);

        List<ImportFieldModel> GetStudentInfoMap();

        List<StudentInfoDto> GetStudentInfoDtoList(List<BatchDeleteRequestInputByUser> inputList, StudentInfoInputDto input, ref int total);

        Task CreateRecovryStudentInfo(StudentInfo model);
        #region   学生注册审核
        /// <summary>
        /// 学生审核
        /// </summary>
        /// <param name="input">待审核的学生ID</param>
        /// <returns></returns>
        Task ApplyStudents(ApplyStudent input);
        /// <summary>
        /// 获取待审核的学生列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        EasyUiListResultDto<ApplyStudentOutDto> GetApplyStudentUiList(ApplyStudentInputDto input);
        #endregion
    }
}
