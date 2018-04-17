using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using SPOC.User;

namespace SPOC.Common.Helper
{
    public class InviteCodeHelper
    {
        private const string Pattern = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// 生成6位推荐码
        /// </summary>
        /// <returns></returns>
        public static async Task<string> NewTeacherInviteCode(IRepository<TeacherInfo, Guid> teacherInfoRep)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var n = Pattern.Length;
            string code;
            do
            {
                code = "";
                for (var i = 0; i < 6; i++)
                {
                    var rnd = random.Next(0, n);
                    code += Pattern[rnd];
                }
            } while (await teacherInfoRep.GetAll().AnyAsync(a => a.teacherInviteCode == code));
            return code;
        }
    }
}