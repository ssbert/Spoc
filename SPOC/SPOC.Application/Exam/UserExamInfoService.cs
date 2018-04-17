using Abp.Application.Services;
using Abp.Domain.Repositories;
using SPOC.Common.Extensions;
using SPOC.Exam.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPOC.Exam
{
    public class UserExamInfoService : ApplicationService, IUserExamInfoService
    {
        private readonly IRepository<ExamGrade, Guid> _iExamGradeRepository;

        private readonly IRepository<ExamExam, Guid> _iExamExamRepository;

        

        public UserExamInfoService(IRepository<ExamGrade, Guid> iExamGradeRepository, IRepository<ExamExam, Guid> iExamExamRepository)
        {
            _iExamGradeRepository = iExamGradeRepository;
            _iExamExamRepository = iExamExamRepository;
        }

        public List<ExamInfoObj> GetExams(UserExamInputDto input,ref int total)
        {
            List<ExamInfoObj> examInfoList = new List<ExamInfoObj>();
            try
            {
                var data = (from eg in _iExamGradeRepository.GetAll()
                            join ee in _iExamExamRepository.GetAll() on eg.examUid equals ee.Id
                            where eg.userUid == input.userId && eg.gradeStatusCode == "release"
                            orderby eg.lastUpdateTime descending
                            select new ExamInfoObj
                            {
                                ExamId = ee.Id,
                                ExamName = ee.ExamName,
                                LastUpdateTime = eg.lastUpdateTime,
                                Score = eg.gradeScore,
                                GradeId = eg.Id
                            }).ToList();

                //if(!data.Any())
                //{
                //    return examInfoList??new List<ExamInfoObj>();    
                //}
                //examInfoList = data.ToList();
                var disData = data.DistinctBy(x => x.ExamId).ToList();
                foreach (var item in disData)
                {
                    ExamInfoObj obj = new ExamInfoObj();
                    obj.ExamName = item.ExamName;
                    obj.CourseId = item.CourseId;
                    obj.CourseName = item.CourseName;
                    var lastExam = data.Where(d => d.ExamId == item.ExamId).OrderByDescending(d => d.LastUpdateTime).FirstOrDefault();
                    obj.LastExamTime = lastExam.LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    obj.LastGrade = lastExam.Score;
                    obj.MaxGrade = data.Where(d => d.ExamId == item.ExamId).Max(d => d.Score).Value;
                    obj.MediaName = item.MediaName;
                    obj.LessonId = item.LessonId;
                    obj.BatchId = item.BatchId;
                    obj.BatchName = item.BatchName;
                    obj.ExamId = item.ExamId;
                    examInfoList.Add(obj);
                }

                if (!string.IsNullOrWhiteSpace(input.courseName))
                {
                    examInfoList = examInfoList.Where(d=>d.CourseName.Contains(input.courseName)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(input.examName))
                {
                    examInfoList = examInfoList.Where(d => d.ExamName.Contains(input.examName)).ToList();
                }

                total = examInfoList.Count();
                examInfoList = examInfoList.ToList().Skip(input.Skip).Take(input.PageSize).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

            return examInfoList;
        }

        public List<ExamDetailObj> GetExamDetails(UserExamInputDto input, ref int total)
        {
            List<ExamDetailObj> list = new List<ExamDetailObj>();
            try
            {
                var examId = input.examId;
                var userId = input.userId;
                var result = from eg in _iExamGradeRepository.GetAll()
                             join ee in _iExamExamRepository.GetAll() on eg.examUid equals ee.Id
                             into st from tp in st.DefaultIfEmpty()
                             where eg.examUid == examId && eg.userUid == userId
                             orderby eg.lastUpdateTime descending
                             select new
                             {
                                 eg.Id,
                                 eg.gradeScore,
                                 eg.gradeStatusCode,
                                 eg.lastUpdateTime,
                                 tp.ExamName  
                             };

                if (!result.Any())
                {
                    return list;
                }

                int index = 0;
                foreach (var item in result)
                {
                    index++;
                    ExamDetailObj obj = new ExamDetailObj();
                    obj.No = index ;
                    obj.ExamGradeUid = item.Id;
                    obj.GradeScore = item.gradeScore ?? 0;
                    obj.GradeStatusCode = GetExamStatus(item.gradeStatusCode);
                    obj.LastUpdateTime = item.lastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    obj.ExamName = item.ExamName;
                    list.Add(obj);
                }              

                total = list.Count;
                list = list.Skip(input.Skip).Take(input.PageSize).ToList();

            }
            catch(Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return list;

        }

        public string GetExamStatus(string code)
        {
            switch (code)
            {
                case "release":
                    return "已考完";
                case "examing":
                    return "考试中";
                case "submitted":
                    return "已提交";
                case "judging":
                    return "评卷中";
                case "judged":
                    return "已评分";
                default:
                    return "已考完";
           
            }
        }
    }
}
