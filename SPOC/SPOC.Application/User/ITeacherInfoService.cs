using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using SPOC.Common.Dto;
using SPOC.Common.EasyUI;
using SPOC.User.Dto.Common;
using SPOC.User.Dto.Teacher;

namespace SPOC.User
{
    
    public interface ITeacherInfoService : IApplicationService
    {

        EasyUiListResultDto<TeacherInfoDto> GetTeacherInfoByGuid(TeacherInfoInputDto input);

        TeacherInfoDto GetTeacherInfoDtoBuUserId(UserInfoQueryInputDto model);
        void UpdateTeacherInfo(CreateTeacherInfoInputDto input);
        [DisableValidation]
        Task CreateTeacherInfo(CreateTeacherInfoInputDto input);
        Task DeleteTeacherInfo(BatchRequestInput input);

        Task DeleteTeacherInfos(List<BatchDeleteRequestInputByUser> inputList);

        void UpdateTeacherInfoByUser(UpdateTeacherInfoInputDto input);
        Task ActiveTeacherInfo(List<BatchDeleteRequestInputByUser> inputList);
        Task SetTeacherRecommend(CreateTeacherInfoInputDto input);

        Task SetTeacherIsDisplay(CreateTeacherInfoInputDto input);

        TeacherInfoDto GetTeacherInfoByTeacherId(Guid teacherId);

        TeacherInfoDto GetTeacherInfoByUserId(Guid teacherId);
        string BindCmb();

        bool CheckNameExit(string name, string type, string oldname = "");

        TeacherInfoDto GetTeacherInfoList(string isRecommend = "");

        List<RecommendTeacherObj> GetRecommendTeacherList(TeacherInfoInputDto input,  ref int total);

        List<TeacherInfoDto> GetTeacherInfoByTIds(string tids);

        string GetTeacherTitle(string id);
        List<ImportFieldModel> GetTeacherInfoMap();

        System.IO.Stream GetTeacherInfoExportTemplate(string names);

        ImportResultOutputDto CreateTeacherInfoFromFile(Stream fileStream);

          List<TeacherInfoDto> GetTeacherInfoDtoList(List<BatchDeleteRequestInputByUser> inputList, TeacherInfoInputDto input, ref int total);
          Task CreateRecovryTeacherInfo(CreateTeacherInfoInputDto input);

        /// <summary>
        /// 为指定教师生成推荐码
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        Task AddInviteCode(Guid userId);
    }
}
