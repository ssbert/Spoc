using System;
using System.Linq;
using Abp.Application.Services;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using SPOC.Common.Cookie;
using SPOC.Common.Helper;
using SPOC.Exam;
using SPOC.ExamPaper;
using SPOC.ExamPaper.Dto;
using SPOC.PolicyPaper.Dto;
using SPOC.User;

namespace SPOC.PolicyPaper
{
    /// <summary>
    /// 随机试卷基本信息服务
    /// </summary>
    public class ExamPolicyService:ApplicationService, IExamPolicyService
    {
        private readonly IExamPaperService _iExamPaperService;
        private readonly IRepository<TeacherInfo, Guid> _iTeacherInfoRep;
        private readonly IRepository<ExamExam, Guid> _iExamExamRep;
        private readonly IRepository<ExamPolicy, Guid> _iExamPolicyRep;
        private readonly IRepository<ExamPolicyNode, Guid> _iExamPolicyNodeRep;
        private readonly IRepository<ExamPolicyItem, Guid> _iExamPolicyItemRep;
        private readonly IRepository<Exam.ExamPaper, Guid> _iExamPaperRep;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExamPolicyService(IRepository<ExamPolicy, Guid> iExamPolicyRep, IRepository<TeacherInfo, Guid> iTeacherInfoRep, 
            IRepository<Exam.ExamPaper, Guid> iExamPaperRep, IExamPaperService iExamPaperService, 
            IRepository<ExamExam, Guid> iExamExamRep,
            IRepository<ExamPolicyNode, Guid> iExamPolicyNodeRep, IRepository<ExamPolicyItem, Guid> iExamPolicyItemRep)
        {
            _iExamPolicyRep = iExamPolicyRep;
            _iTeacherInfoRep = iTeacherInfoRep;
            _iExamPaperRep = iExamPaperRep;
            _iExamPaperService = iExamPaperService;
            _iExamExamRep = iExamExamRep;
            _iExamPolicyNodeRep = iExamPolicyNodeRep;
            _iExamPolicyItemRep = iExamPolicyItemRep; 
        }

        public async Task<ExamPolicyOutputDto> Get(Guid id)
        {
            try
            {
                var result = await _iExamPolicyRep.GetAsync(id);
                return await Task.FromResult(result.MapTo<ExamPolicyOutputDto>());
            }
            catch (Exception)
            {
                throw new UserFriendlyException("无效的随机试卷ID");
            }
        }

        public async Task<ExamPolicyOutputDto> Create(ExamPolicyInputDto input)
        {
            #region 验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            if (input.isCustomCode)
            {
                if (string.IsNullOrEmpty(input.policyCode))
                {
                    throw new UserFriendlyException("权限不够");
                }

                if (_iExamPolicyRep.GetAll().Any(a => a.policyCode == input.policyCode))
                {
                    throw new UserFriendlyException("已有相同的试卷编码");
                }
            }
            #endregion

            input.Id = Guid.NewGuid();
            var entity = input.MapTo<ExamPolicy>();
            entity.creatorUid = cookie.Id;
            entity.createTime = DateTime.Now;
            if (!entity.isCustomCode)
            {
                entity.policyCode = CreateNewCode();
            }
            await _iExamPolicyRep.InsertAsync(entity);

            var paperDto = entity.MapTo<ExamPaperDto>();
            paperDto.Id = entity.Id;//由policy产生的paper数据id一致
            paperDto.paperCode = entity.policyCode;
            paperDto.paperName = entity.policyName;
            paperDto.paperTypeCode = "random";
            paperDto.policyUid = entity.Id;
            paperDto.isCustomCode = false;

            input.Id = Guid.NewGuid();
            var paper = paperDto.MapTo<Exam.ExamPaper>();
            paper.createTime = DateTime.Now;
            paper.lastUpdateTime = entity.createTime;
            paper.creatorUid = cookie.Id;
            await _iExamPaperRep.InsertAsync(paper);

            return await Task.FromResult(entity.MapTo<ExamPolicyOutputDto>());
        }

        public async Task Update(ExamPolicyInputDto input)
        {
            #region 验证
            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }
            
            var entity = _iExamPolicyRep.GetAll().FirstOrDefault(a => a.Id == input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException("无效的随机试卷");
            }

            if (!entity.isCustomCode && input.isCustomCode)
            {
                if (string.IsNullOrEmpty(input.policyCode))
                {
                    throw new UserFriendlyException("试卷编码不可为空");
                }

                if (_iExamPolicyRep.GetAll().Any(a => a.policyCode == input.policyCode))
                {
                    throw new UserFriendlyException("已有相同的试卷编码");
                }
            }
            #endregion

            if (entity.isCustomCode && !input.isCustomCode)
            {
                input.policyCode = CreateNewCode();
            }
            input.MapTo(entity);
            entity.creatorUid = new Guid(cookie.UserUid);
            
            var paper = entity.MapTo<ExamPaperInputDto>();
            paper.Id = entity.Id;//由policy产生的paper数据id一致
            paper.paperCode = entity.policyCode;
            paper.paperName = entity.policyName;
            paper.paperTypeCode = "random";
            paper.policyUid = entity.Id.ToString();
            paper.isCustomCode = false;

            await _iExamPolicyRep.UpdateAsync(entity);

            await _iExamPaperService.Update(paper);
        }

        public async Task Delete(string ids)
        {
            #region 验证

            var cookie = CookieHelper.GetLoginInUserInfo();
            if (!cookie.IsLogin)
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");
            }

            if (!cookie.IsAdmin && !_iTeacherInfoRep.GetAll().Any(a => a.userId == cookie.Id))
            {
                throw new UserFriendlyException("权限不够");
            }

            var idArray = ids.Split(',').Select(a=>new Guid(a)).ToArray();
            foreach (var id in idArray)
            {
                if (!_iExamPolicyRep.GetAll().Any(a => a.Id == id))
                {
                    throw new UserFriendlyException("无效的试卷");
                }

                if (_iExamExamRep.GetAll().Any(a => a.paperUid == id))
                {
                    throw new UserFriendlyException("试卷已被使用");
                }
            }
            #endregion

            foreach (var uid in idArray)
            {
                var guid = uid;
                await _iExamPaperRep.DeleteAsync(a => a.Id == guid);
                await _iExamPaperRep.DeleteAsync(a => a.policyUid == guid && a.paperTypeCode == "fix_from_random");
                await _iExamPolicyRep.DeleteAsync(a => a.Id == guid);
                var guidList = _iExamPolicyNodeRep.GetAll().Where(a => a.policyUid == guid).Select(a => a.Id);
                foreach (var nodeUid in guidList)
                {
                    await _iExamPolicyItemRep.DeleteAsync(a => a.policyNodeUid == nodeUid);
                }
            }
        }

        private string CreateNewCode()
        {
            var code = "P000001";
            var entity = _iExamPaperRep.GetAll().Where(a => !a.isCustomCode).OrderByDescending(a => a.createTime).FirstOrDefault();
            if (entity != null)
            {
                code = entity.paperCode;
                do
                {
                    code = StringUtil.GetNextCodeByAuto(code);
                } while (_iExamPaperRep.GetAll().Any(a => a.paperCode == code));
            }
            return code;
        }
    }
}