using SPOC.Common.Const;

namespace SPOC.QuestionBank.Const
{
    public class QuestionStautsConst:BaseConst<QuestionBankService>
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const string Normal = "normal";
        /// <summary>
        /// 已过期
        /// </summary>
        public const string Outdated = "outdated";
        /// <summary>
        /// 禁用
        /// </summary>
        public const string Disabled = "disabled";

        /// <summary>
        /// 草稿
        /// </summary>
        public const string Draft = "draft";
    }
}