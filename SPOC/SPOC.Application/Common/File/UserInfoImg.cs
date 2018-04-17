namespace SPOC.Common.File
{
   public  class UserInfoImg
    {
       public static string GetDefaultUserAvator(string gander) {
           //return gander == "1" ? "/files/UserInfo/smallImgMan.png" : (gander == "2 " ? "/files/UserInfo/smailImgWomen.png" : "/files/UserInfo/samilImgTotal.jpg");
           return   "/files/UserInfo/smallImgMan.png";

       }
    }
}
