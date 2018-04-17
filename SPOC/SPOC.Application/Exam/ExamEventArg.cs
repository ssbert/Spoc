using System;

namespace SPOC.Exam
{
    public class ExamEventArg
    {
        private string _changeType = string.Empty;
        private object _changeObject = string.Empty;
        private Guid _objectUid = Guid.Empty;
        private string _relativeUid = string.Empty;
        private string _operatorName = string.Empty;
        private Guid _operatorUid = Guid.Empty;

        /// <summary>
        /// 操作者姓名
        /// </summary>
        public string OperatorName
        {
            get { return _operatorName; }
            set { _operatorName = value; }
        }

        /// <summary>
        /// 操作者系统编号
        /// </summary>
        public Guid OperatorUid
        {
            get { return _operatorUid; }
            set { _operatorUid = value; }
        }

        /// <summary>
        /// 变更类型
        /// </summary>
        public string ChangeType
        {
            get { return _changeType; }
            set { _changeType = value; }
        }

        /// <summary>
        /// 发生变动的对象系统编号
        /// </summary>
        public object ChangeObject
        {
            get { return _changeObject; }
            set { _changeObject = value; }
        }

        /// <summary>
        /// 发生变动的对象系统编号
        /// </summary>
        public Guid ObjectUid
        {
            get { return _objectUid; }
            set { _objectUid = value; }
        }

        /// <summary>
        /// 与变动对象相关联的系统编号
        /// </summary>
        public string RelativeUid
        {
            get { return _relativeUid; }
            set { _relativeUid = value; }
        }
    }
}