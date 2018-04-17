using SPOC.Common.Dto;
using SPOC.Common.Pagination;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SPOC.Exam.Dto
{
    public class UserExamInputDto : EasyuiDto
    {
        public Guid userId { get; set; }

        public string courseName { get; set; }

        public string examName { get; set; }

        public Guid examId { get; set; }
        
    }

    public class MyExamViewModel:PagerViewModel
    {
        private List<ExamInfoObj> _ExamInfoObj = new List<ExamInfoObj>();
        public List<ExamInfoObj> ExamInfoObj
        {
            get { return _ExamInfoObj; }
            set { _ExamInfoObj = value; }
        }

        //private List<LearningPlatformDto> _PlateListItem = new List<LearningPlatformDto>();
        //public List<LearningPlatformDto> PlateListItem
        //{
        //    get { return _PlateListItem; }
        //    set { _PlateListItem = value; }
        //}

        /// <summary>
        /// 平台名称
        /// </summary>
        private Guid _PlatfromId = Guid.Empty;
        public Guid PlatfromId
        {
            get { return _PlatfromId; }
            set { _PlatfromId = value; }
        }

        /// <summary>
        /// 课程名称
        /// </summary>
        private string _CourseName = string.Empty;
        public string CourseName
        {
            get { return _CourseName; }
            set { _CourseName = value; }
        }

        /// <summary>
        /// 考试名称
        /// </summary>
        private string _ExamName = string.Empty;
        public string ExamName
        {
            get { return _ExamName; }
            set { _ExamName = value; }
        }
    }

    public class ExamDetailViewModel : PagerViewModel
    {
        private List<ExamDetailObj> _ExamDetailObj = new List<ExamDetailObj>();
        public List<ExamDetailObj> ExamDetailObj
        {
            get { return _ExamDetailObj; }
            set { _ExamDetailObj = value; }
        }

        /// <summary>
        /// 课时ID
        /// </summary>
        private Guid _LessonId = Guid.Empty;
        public Guid LessonId
        {
            get { return _LessonId; }
            set { _LessonId = value; }
        }

        /// <summary>
        /// 分期ID
        /// </summary>
        private Guid _BatchId = Guid.Empty;
        public Guid BatchId
        {
            get { return _BatchId; }
            set { _BatchId = value; }
        }

        /// <summary>
        /// 考试ID
        /// </summary>
        private Guid _ExamId = Guid.Empty;
        public Guid ExamId
        {
            get { return _ExamId; }
            set { _ExamId = value; }
        }
    }

    public class ExamInfoObj
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        private string _CourseId = string.Empty;
        public string CourseId
        {
            get { return _CourseId; }
            set { _CourseId = value; }
        }

        /// <summary>
        /// 课程名称
        /// </summary>
        private string _CourseName = string.Empty;
        public string CourseName
        {
            get { return _CourseName; }
            set { _CourseName = value; }
        }

        /// <summary>
        /// 考试名称
        /// </summary>
        private string _ExamName = string.Empty;
        public string ExamName
        {
            get { return _ExamName; }
            set { _ExamName = value; }
        }

        /// <summary>
        /// 分期名称
        /// </summary>
        private string _BatchName = string.Empty;
        public string BatchName
        {
            get { return _BatchName; }
            set { _BatchName = value; }
        }

        /// <summary>
        /// 最后一次考试成绩
        /// </summary>
        private decimal? _LastGrade = 0;
        public decimal? LastGrade
        {
            get { return _LastGrade; }
            set { _LastGrade = value; }
        }

        /// <summary>
        ///最高分
        /// </summary>
        private decimal _MaxGrade = 0;
        public decimal MaxGrade
        {
            get { return _MaxGrade; }
            set { _MaxGrade = value; }
        }

        /// <summary>
        /// 最后一次考试时间
        /// </summary>
        private string _LastExamTime =string.Empty;
        public string LastExamTime
        {
            get { return _LastExamTime; }
            set { _LastExamTime = value; }
        }

        /// <summary>
        /// 平台名称
        /// </summary>
        private string _PlatFormName = string.Empty;
        public string PlatFormName
        {
            get { return _PlatFormName; }
            set { _PlatFormName = value; }
        }

        /// <summary>
        /// 考试的id
        /// </summary>
        private Guid _GradeId = Guid.Empty;
        public Guid GradeId
        {
            get { return _GradeId; }
            set { _GradeId = value; }
        }

        /// <summary>
        /// 课时id
        /// </summary>
        private Guid _LessonId = Guid.Empty;
        public Guid LessonId
        {
            get { return _LessonId; }
            set { _LessonId = value; }
        }

        /// <summary>
        /// 媒体文件名称
        /// </summary>
        private string _MediaName = string.Empty;
        public string MediaName
        {
            get { return _MediaName; }
            set { _MediaName = value; }
        }

        private Guid _BatchId = Guid.Empty;
        public Guid BatchId
        {
            get { return _BatchId; }
            set { _BatchId = value; }
        }

        private DateTime _LastUpdateTime = new DateTime();
        [JsonConverter(typeof(DateFormat))]
        public DateTime LastUpdateTime
        {
            get { return _LastUpdateTime; }
            set { _LastUpdateTime = value; }
        }

        /// <summary>
        /// 分数
        /// </summary>
        private decimal? _Score = 0;
        public decimal? Score
        {
            get { return _Score; }
            set { _Score = value; }
        }

        /// <summary>
        /// 考试ID
        /// </summary>
        private Guid _ExamId = Guid.Empty;
        public Guid ExamId
        {
            get { return _ExamId; }
            set { _ExamId = value; }
        }
    }

    public class ExamDetailObj
    {
        /// <summary>
        /// 序号
        /// </summary>
        private int _No = 0;
        public int No
        {
            get { return _No; }
            set { _No = value; }
        }

        /// <summary>
        /// 考试名称
        /// </summary>
        private string _ExamName = string.Empty;
        public string ExamName
        {
            get { return _ExamName; }
            set { _ExamName = value; }
        }

        /// <summary>
        /// 考试的uid
        /// </summary>
        private Guid _ExamGradeUid = Guid.Empty;
        public Guid ExamGradeUid
        {
            get { return _ExamGradeUid; }
            set { _ExamGradeUid = value; }
        }

        /// <summary>
        /// 考试分数
        /// </summary>
        private decimal? _GradeScore = 0;
        public decimal? GradeScore
        {
            get { return _GradeScore; }
            set { _GradeScore = value; }
        }

        private string _GradeStatusCode = string.Empty;
        public string GradeStatusCode
        {
            get { return _GradeStatusCode; }
            set { _GradeStatusCode = value; }
        }

        /// <summary>
        /// 最后时间
        /// </summary>
        private string _LastUpdateTime = string.Empty;
        [JsonConverter(typeof(DateFormat))]
        public string LastUpdateTime
        {
            get { return _LastUpdateTime; }
            set { _LastUpdateTime = value; }
        }

        /// <summary>
        /// 分期名称
        /// </summary>
        private string _BatchName = string.Empty;
        public string BatchName
        {
            get { return _BatchName; }
            set { _BatchName = value; }
        }
    }
}
