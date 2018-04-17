
namespace SPOC.User.Dto
{
    public class MyWorkbenchViewModel
    {
        private int _PrivateMsg = 0;
        public int PrivateMsg
        {
            get { return _PrivateMsg; }
            set { _PrivateMsg = value; }
        }

        private int _Notice = 0;
        public int Notice
        {
            get { return _Notice; }
            set { _Notice = value; }
        }

        private int _ShopNum = 0;
        public int ShopNum
        {
            get { return _ShopNum; }
            set { _ShopNum = value; ; }
        }

        private int _Identity = 0;
        public int Identity
        {
            get { return _Identity; }
            set { _Identity = value; }
        }

        private string _UserID = string.Empty;
        public string UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        private string _UserName = string.Empty;
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        private string _FirstPageUrl = string.Empty;
        public string FirstPageUrl
        {
            get { return _FirstPageUrl; }
            set { _FirstPageUrl = value; }
        }
    }
}
