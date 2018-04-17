using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Runtime.Validation;
using SPOC.Common;
using SPOC.User.Dto.UserInfo;

namespace SPOC.User
{
    public interface IUserInfoApiService : IApplicationService
    {
        [DisableValidation]
        Task<ApiResponseResult<ApiUserInfoDto>> Login(MBasicRequestParamsDTO basicParamsDto, string uName, string pwd, string clientId = "");
        [DisableValidation]
        Task<ApiResponseResult<string>> Register(MBasicRequestParamsDTO basicParamsDto, UserRegisterModel registDto);

        [DisableValidation]
        Task<ApiResponseResult<string>> UserRegisterByList(MBasicRequestParamsDTO basicParamsDto, List<UserRegisterModel> registDtoList);
        [DisableValidation] 
        Task<ApiResponseResult<object>> CheckMobile(MBasicRequestParamsDTO basicParamsDto, UserRegisterModel registDto);
        [DisableValidation]
        Task<ApiResponseResult<object>> CheckEmail(MBasicRequestParamsDTO basicParamsDto, UserRegisterModel registDto);
        [DisableValidation]
        Task<ApiResponseResult<object>> CheckUserName(MBasicRequestParamsDTO basicParamsDto, UserRegisterModel registDto);
        [DisableValidation] 
        Task<ApiResponseResult<object>>  ModifryUserByMobile(MBasicRequestParamsDTO basicParamsDto, EditDto editDto);
        [DisableValidation] 
        Task<ApiResponseResult<object>>  ModifryUserMobile(MBasicRequestParamsDTO basicParamsDto, ModifyMobile editDto);

        [DisableValidation]
        Task<ApiResponseResult<ApiUserInfoDto>> UserSearch(MBasicRequestParamsDTO basicParamsDto, string userName,  string email,  string mobile);

        [DisableValidation]
        Task<ApiResponseResult<ApiUserInfoDto>> GetUserInfo(MBasicRequestParamsDTO basicParamsDto, string mobile);

    }
}
