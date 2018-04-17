using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using Abp.UI;
using NPOI.HPSF;
using SPOC.Category;
using SPOC.Common.Cookie;
using SPOC.Common.EasyUI;
using SPOC.Common.Helper;
using SPOC.Common.Pagination;
using SPOC.Faqs.Dtos;
using SPOC.QuestionBank.Dto;
using SPOC.SystemSet;

namespace SPOC.Faqs
{
    /// <summary>
    /// Faq应用层服务的接口实现方法
    /// </summary>
    public class FaqAppService : SPOCAppServiceBase, IFaqAppService
    {
      
        private readonly IRepository<Faq, Guid> _faqRepository;
        private readonly IRepository<NvFolder, Guid> _nvFolderRepository;
        /// <summary>
        /// 构造函数
        /// </summary>
        public FaqAppService(IRepository<Faq, Guid> faqRepository, IRepository<NvFolder, Guid> nvFolderRepository)
        {
            _faqRepository = faqRepository;
            _nvFolderRepository = nvFolderRepository;
        }

        /// <summary>
        /// 获取Faq的分页列表信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<EasyUiListResultDto<FaqListDto>> GetPagedFaqs(FaqInputDto input)
        {

            var query = _faqRepository.GetAll();
            var faqs =       from faq in query
                             join f in _nvFolderRepository.GetAll() on faq.folderId equals f.Id 
                             into tempF
                             from temp in tempF.DefaultIfEmpty()
                             select new FaqListDto
                             {
                                 Id=faq.Id,
                                 content=faq.content,
                                 title = faq.title,
                                 folderName = temp!=null? temp.folderName:"",
                                 folderId = temp != null ? temp.Id:Guid.Empty,
                                 updateTime = faq.updateTime,
                                 userFul = faq.userFul,
                                 userLess = faq.userLess,
                                 seq=faq.seq,
                                 IsActive = faq.IsActive
                             };
            faqs = faqs.WhereIf(!string.IsNullOrWhiteSpace(input.content), a => a.content.Contains(input.content))
                .WhereIf(!string.IsNullOrWhiteSpace(input.title), a => a.title.Contains(input.title))
                .WhereIf(input.folderId.Any(), a => input.folderId.Contains(a.folderId));
            faqs = !string.IsNullOrEmpty(input.sort) ? faqs.OrderBy(input.sort + " " + input.order) : faqs.OrderBy(a => a.seq);
            var rows = await faqs.Skip(input.skip).Take(input.pageSize).ToListAsync();
                rows.ForEach(a =>
            {
                var content = StringUtil.NoHTML(a.content);
                a.content = content.Length > 50 ? content.Substring(0, 50) + "..." : content;
            });
            return new EasyUiListResultDto<FaqListDto>
            {
                rows = rows,
                total = await faqs.CountAsync()
            };
           

        }

        /// <summary>
        /// 通过指定id获取FaqListDto信息
        /// </summary>
        public async Task<FaqItemDto> GetFaqByIdAsync(EntityDto<Guid> input)
        {
            var entity = await _faqRepository.GetAsync(input.Id);

            return entity.MapTo<FaqItemDto>();
        }

  
        /// <summary>
        /// MPA版本才会用到的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<GetFaqForEditOutput> GetFaqForEdit(NullableIdDto<Guid> input)
        {
            var output = new GetFaqForEditOutput();
            FaqEditDto faqEditDto;

            if (input.Id.HasValue)
            {
                var entity = await _faqRepository.GetAsync(input.Id.Value);

                faqEditDto = entity.MapTo<FaqEditDto>();
            }
            else
            {
                faqEditDto = new FaqEditDto();
            }

            output.Faq = faqEditDto;
            return output;

        }

        /// <summary>
        /// 添加或者修改Faq的公共方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateFaq(FaqEditDto input)
        {
            if (!LoginValidation.IsLogin())
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");

            }
            if (input.Id.HasValue)
            {
                await UpdateFaqAsync(input);
            }
            else
            {
                await CreateFaqAsync(input);
            }
        }

        /// <summary>
        /// 新增Faq
        /// </summary>
        protected virtual async Task<FaqEditDto> CreateFaqAsync(FaqEditDto input)
        {
           
            var entity = ObjectMapper.Map<Faq>(input);

            entity = await _faqRepository.InsertAsync(entity);
            return entity.MapTo<FaqEditDto>();
        }

        /// <summary>
        /// 编辑Faq
        /// </summary>
        protected virtual async Task UpdateFaqAsync(FaqEditDto input)
        {
           
            var entity = await _faqRepository.GetAsync(input.Id.Value);
            input.MapTo(entity);

            // ObjectMapper.Map(input, entity);
            await _faqRepository.UpdateAsync(entity);
        }

    

        /// <summary>
        /// 批量删除Faq的方法
        /// </summary>
        public async Task BatchDeleteFaqsAsync(BatchRequestInput input)
        {
            if (!LoginValidation.IsLogin())
            {
                throw new UserFriendlyException("未登录系统或登录已经失效，请重新登录");

            }
            if (input == null)
            {
                return;
            }
            for (int i = 0; i < input.Id.Split(',').Length; i++)
            {
                if (string.IsNullOrEmpty((input.Id.Split(',')[i])))
                    continue;
                var uid = new Guid(input.Id.Split(',')[i]);
                await _faqRepository.DeleteAsync(s => uid.Equals(s.Id));
            }
           
        }
        #region FAQ前端接口实现
        /// <summary>
        /// 前端获取FAQ分类
        /// </summary>
        /// <returns></returns>
        public async Task<List<FaqFolderDto>> GetFaqFolder()
        {
            var result = new List<FaqFolderDto>();
            var queryable = await _nvFolderRepository.GetAll().Where(a => a.folderTypeCode == "faq").OrderBy(a => a.listOrder).ToListAsync();
            var rootNode=queryable.FirstOrDefault(a => a.parentUid.Equals(Guid.Empty));
            if (rootNode == null)
                return result;
            rootNode.folderName = "全部问题";
            //根分类
            result.Add(new FaqFolderDto{id = rootNode.Id,name = rootNode.folderName, faqFolders=new List<FaqFolderDto>()});
            //第二级子分类
            var childNode = queryable.Where(a => a.parentUid.Equals(rootNode.Id));
            foreach (var node in childNode)
            {
                result.Add(new FaqFolderDto{id=node.Id,name =node.folderName,faqFolders = BuildChildNode(node.Id, queryable) });
            }
            return result;
        }

        private List<FaqFolderDto> BuildChildNode(Guid rootFolderId,List<NvFolder> folders)
        {
            var result = new List<FaqFolderDto>();
            var childNode = folders.Where(a => a.parentUid.Equals(rootFolderId));
            foreach (var node in childNode)
            {
                result.Add(new FaqFolderDto
                {
                    id = node.Id,
                    name = node.folderName,
                    faqFolders = BuildChildNode(node.Id, folders)
                });
            }
            return result;
        }
        /// <summary>
        /// 前端获取FAQ列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PaginationOutputDto<FaqItemDto>> GetPagination(FaqInputDto input)
        {
            var query = _faqRepository.GetAll().Where(a=>a.IsActive);
            var emptyGuid = Guid.Empty;
            var folderId = input.folderId.Any()?input.folderId[0].ToString(): "";
            var faqs = from faq in query
                join f in _nvFolderRepository.GetAll() on faq.folderId equals f.Id
                into tempF
                from temp in tempF.DefaultIfEmpty()
                       select new 
                {
                    faq.Id,
                    faq.content,
                    faq.title,
                    faq.updateTime,
                    folderName = temp != null ? temp.folderName : "",
                    folderId = temp != null ? temp.Id : emptyGuid,
                    faq.seq,
                    fullPath=temp != null ? temp.fullPath : "",
                };
            faqs = faqs.WhereIf(!string.IsNullOrWhiteSpace(input.content),
                a => a.content.Contains(input.content) || a.title.Contains(input.content)).WhereIf(input.folderId.Any(), f => f.fullPath.Contains(folderId));
            var queryResult = faqs.Select(a => new FaqItemDto
            {
                Id = a.Id,
                content = a.content,
                folderId = a.folderId,
                folderName = a.folderName,
                seq = a.seq,
                title = a.title,
                updateTime = a.updateTime
            });
      
            var list = await queryResult.OrderBy(a => a.seq).Skip(input.skip).Take(input.pageSize).ToListAsync();
            list.ForEach(a =>
            {
                var content = StringUtil.NoHTML(a.content);
                a.content = content.Length > 200 ? content.Substring(0, 200) + "..." : content;
            });
            return new PaginationOutputDto<FaqItemDto>
            {
                rows = list,
                total = queryResult.Count()

            };
        }
        #endregion 
    }
}

