using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Abp.EntityFramework;
using SPOC.Attribute;
using SPOC.Category;
using SPOC.Core;
using SPOC.Exam;
using SPOC.SystemSet;
using SPOC.User;
using SPOC.Exercises;
using SPOC.Lib;

namespace SPOC.EntityFramework
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class SPOCDbContext : AbpDbContext
    {
        //TODO: Define an IDbSet for each Entity...

        //Example:
        //public virtual IDbSet<User> Users { get; set; }

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public SPOCDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in SPOCDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of SPOCDbContext since ABP automatically handles it.
         */
        public SPOCDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public SPOCDbContext(DbConnection existingConnection)
         : base(existingConnection, false)
        {

        }

        public SPOCDbContext(DbConnection existingConnection, bool contextOwnsConnection)
         : base(existingConnection, contextOwnsConnection)
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Add(new DecimalPrecisionAttributeConvention());
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); //移除复数表名
            modelBuilder.Entity<AdminInfo>().ToTable("admin_info");
            modelBuilder.Entity<ExamAnswer>().HasKey(e => new { e.Id, e.examGradeUid, e.questionUid }).ToTable("exam_answer");
            modelBuilder.Entity<ExamCacheTask>().ToTable("exam_cache_task");
            modelBuilder.Entity<ExamTaskClass>().HasKey(e => new {e.Id, e.ClassId}).ToTable("exam_task_class");
            modelBuilder.Entity<ExamExam>().ToTable("exam_exam");
            modelBuilder.Entity<ExamExamPaper>().HasKey(e => new { e.Id, e.examUid, e.paperUid }).ToTable("exam_exam_paper");
            modelBuilder.Entity<ExamGrade>().ToTable("exam_grade");
            modelBuilder.Entity<ExamInfoChange>().ToTable("exam_info_change");
            modelBuilder.Entity<ExamJudge>().HasKey(e => new { e.Id, e.examUid, e.ownerUid }).ToTable("exam_judge");
            modelBuilder.Entity<ExamJudgeMany>().ToTable("exam_judge_many");
            modelBuilder.Entity<ExamJudgePaperNode>().HasKey(e => new { e.Id, e.examUid, e.paperNodeUid, e.judgeUserUid }).ToTable("exam_judge_paper_node");
            modelBuilder.Entity<ExamJudgePolicy>().ToTable("exam_judge_policy");
            modelBuilder.Entity<ExamJudgeQuestion>().ToTable("exam_judge_question");
            modelBuilder.Entity<ExamJudgeQuestionGrade>().ToTable("exam_judge_question_grade");
            modelBuilder.Entity<ExamJudgeUser>().HasKey(e => new { e.Id, e.examGradeUid, e.judgeUserUid }).ToTable("exam_judge_user");
            modelBuilder.Entity<ExamPaper>().ToTable("exam_paper");
            modelBuilder.Entity<ExamPaperNode>().ToTable("exam_paper_node");
            modelBuilder.Entity<ExamPaperNodeQuestion>().HasKey(e => new { e.Id, e.paperNodeUid, e.questionUid }).ToTable("exam_paper_node_question");
            modelBuilder.Entity<ExamPaperRelative>().ToTable("exam_paper_relative");
            modelBuilder.Entity<ExamPolicy>().ToTable("exam_policy");
            modelBuilder.Entity<ExamPolicyItem>().ToTable("exam_policy_item");
            modelBuilder.Entity<ExamPolicyNode>().ToTable("exam_policy_node");
            modelBuilder.Entity<ExamPublish>().HasKey(e => new { e.Id, e.examUid, e.ownerUid }).ToTable("exam_publish");
            modelBuilder.Entity<ExamQuestion>().ToTable("exam_question");
            modelBuilder.Entity<ExamQuestionFeedback>().ToTable("exam_question_feedback");
            modelBuilder.Entity<ExamQuestionType>().ToTable("exam_question_type");
            modelBuilder.Entity<ExamUser>().HasKey(e => new { e.Id, e.examUid, e.ownerUid }).ToTable("exam_user");
            modelBuilder.Entity<ExamUserAnswer>().ToTable("exam_user_answer");

            modelBuilder.Entity<ExerciseClass>().HasKey(e => new { e.Id, e.ClassId }).ToTable("exercise_class");

            modelBuilder.Entity<NvFolder>().ToTable("nv_folder");
            modelBuilder.Entity<NvFolderType>().ToTable("nv_folder_type");
            modelBuilder.Entity<NotificationClass>().HasKey(e => new { e.Id, e.ClassId }).ToTable("notification_class");
            modelBuilder.Entity<RecordOfReadNotification>().HasKey(e => new { e.Id, e.UserId }).ToTable("record_of_read_notification");

            modelBuilder.Entity<RoleManage>().ToTable("role_manage");
            modelBuilder.Entity<RolePermission>().ToTable("role_permission");
            modelBuilder.Entity<UserRole>().ToTable("user_role");
            modelBuilder.Entity<SystemSet.Site>().ToTable("site");
            modelBuilder.Entity<SiteSet>().ToTable("site_set");
            modelBuilder.Entity<StudentInfo>().ToTable("student_info");

            modelBuilder.Entity<SystemLog>().ToTable("system_log");

            modelBuilder.Entity<TeacherInfo>().ToTable("teacher_info");

            modelBuilder.Entity<UserBase>().ToTable("users");

            modelBuilder.Entity<SiteVersion>().ToTable("site_version");

            modelBuilder.Entity<Menu>().ToTable("menu");

        }

        public virtual IDbSet<AdminInfo> AdminInfos { get; set; }

        public virtual IDbSet<CityArea> CityAreas { get; set; }
        public virtual IDbSet<Cloud> Clouds { get; set; }


        public virtual IDbSet<ExamTask> ExamTasks { get; set; }
        public virtual IDbSet<ExamTaskClass> ExamTaskClasses { get; set; }
        public virtual IDbSet<ExamAnswer> ExamAnswers { get; set; }
        public virtual IDbSet<ExamCacheTask> ExamCacheTasks { get; set; }
        public virtual IDbSet<ExamExam> ExamExams { get; set; }
        public virtual IDbSet<ExamExamPaper> ExamExamPapers { get; set; }
        public virtual IDbSet<ExamGrade> ExamGrades { get; set; }
        public virtual IDbSet<ExamInfoChange> ExamInfoChanges { get; set; }
        public virtual IDbSet<ExamJudge> ExamJudges { get; set; }
        public virtual IDbSet<ExamJudgeInfo> ExamJudgeInfos { get; set; }
        public virtual IDbSet<ExamJudgeMany> ExamJudgeManys { get; set; }
        public virtual IDbSet<ExamJudgePaperNode> ExamJudgePaperNodes { get; set; }
        public virtual IDbSet<ExamJudgePolicy> ExamJudgePolicys { get; set; }
        public virtual IDbSet<ExamJudgeQuestion> ExamJudgeQuestions { get; set; }
        public virtual IDbSet<ExamJudgeQuestionGrade> ExamJudgeQuestionGrades { get; set; }
        public virtual IDbSet<ExamJudgeUser> ExamJudgeUsers { get; set; }
        public virtual IDbSet<ExamPaper> ExamPapers { get; set; }
        public virtual IDbSet<ExamPaperNode> ExamPaperNodes { get; set; }
        public virtual IDbSet<ExamPaperNodeQuestion> ExamPaperNodeQuestions { get; set; }
        public virtual IDbSet<ExamPaperRelative> ExamPaperRelatives { get; set; }
        public virtual IDbSet<ExamPolicy> ExamPolicys { get; set; }
        public virtual IDbSet<ExamPolicyItem> ExamPolicyItems { get; set; }
        public virtual IDbSet<ExamPolicyItemLabel> ExamPolicyItemLabels { get; set; }
        public virtual IDbSet<ExamPolicyNode> ExamPolicyNodes { get; set; }
        public virtual IDbSet<ExamPublish> ExamPublishs { get; set; }
        public virtual IDbSet<ExamQuestion> ExamQuestions { get; set; }
        public virtual IDbSet<ExamQuestionFeedback> ExamQuestionFeedbacks { get; set; }
        public virtual IDbSet<ExamQuestionType> ExamQuestionTypes { get; set; }
        public virtual IDbSet<ExamUser> ExamUsers { get; set; }
        public virtual IDbSet<ExamUserAnswer> ExamUserAnswers { get; set; }
        public virtual IDbSet<ExamProgramResult> ExamProgramResults { get; set; }

        public virtual IDbSet<Exercise> Exercises { get; set; }
        public virtual IDbSet<ExerciseRecord> ExerciseRecords { get; set; }
        public virtual IDbSet<ExerciseAnswer> ExerciseAnswers { get; set; }
        public virtual IDbSet<ExerciseClass> ExerciseClasses { get; set; }

        public virtual IDbSet<Menu> Menus { get; set; }

        public virtual IDbSet<Notification> Notifications { get; set; }
        public virtual IDbSet<NotificationClass> NotificationClasses { get; set; }
        public virtual IDbSet<NotificationType> NotificationTypes { get; set; }
        public virtual IDbSet<RecordOfReadNotification> RecordOfReadNotifications { get; set; }

        public virtual IDbSet<NvFolder> NvFolders { get; set; }
        public virtual IDbSet<NvFolderType> NvFolderTypes { get; set; }


        public virtual IDbSet<RoleManage> RoleManages { get; set; }
        public virtual IDbSet<RolePermission> RolePermissions { get; set; }
        public virtual IDbSet<UserRole> UserRole { get; set; }

        public virtual IDbSet<SPOC.SystemSet.Site> Sites { get; set; }
        public virtual IDbSet<SiteSet> SiteSets { get; set; }
        public virtual IDbSet<StudentInfo> StudentInfos { get; set; }

        public virtual IDbSet<SystemLog> SystemLogs { get; set; }
        public virtual IDbSet<Faq> Faqs { get; set; }

        public virtual IDbSet<TeacherInfo> TeacherInfos { get; set; }

        public virtual IDbSet<UserBase> Userss { get; set; }

        public virtual IDbSet<UserLoginRemember> UserLoginRemembers { get; set; }


        public virtual IDbSet<SiteVersion> SiteVersions { get; set; }

        public virtual IDbSet<Faculty> Facultys { get; set; }
        public virtual IDbSet<Major> Majors { get; set; }
        public virtual IDbSet<Class> Classes { get; set; }
        public virtual IDbSet<AdministrativeClass> AdministrativeClasses { get; set; }
        public virtual IDbSet<ClassStudent> ClassStudents { get; set; }
        
        public virtual IDbSet<ClassTeacher> ClassTeachers { get; set; }
        public virtual IDbSet<ClassAnnouncement> ClassAnnouncements { get; set; }
        public virtual IDbSet<Announcement> Announcements { get; set; }
        public virtual IDbSet<AnnouncementFile> AnnouncementFiles { get; set; }
        public virtual IDbSet<UploadFile> UploadFiles { get; set; }
        public virtual IDbSet<ChallengeQuestion> ChallengeQuestions { get; set; }
        public virtual IDbSet<ChallengeGrade> ChallengeGrades { get; set; }
        public virtual IDbSet<QuestionStandardCode> QuestionStandardCodes { get; set; }
        #region 标签库
        public virtual IDbSet<Label> Labels { get; set; }
        public virtual IDbSet<LabelRule> LabelRules { get; set; }
        public virtual IDbSet<QuestionLabel> QuestionLabels { get; set; }
        public virtual IDbSet<UserAnswerRecords> UserAnswerRecords { get; set; }
        public virtual IDbSet<UserLabelScore> UserLabelScore { get; set; }
        public virtual IDbSet<StructureMap> StructureMaps { get; set; }
        #endregion
    }
}
