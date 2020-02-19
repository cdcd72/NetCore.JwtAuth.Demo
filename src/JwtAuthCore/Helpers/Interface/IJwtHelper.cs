using System.Collections.Generic;

namespace JwtAuthCore.Helpers.Interface
{
    public interface IJwtHelper
    {
        /// <summary>
        /// 產生 Token
        /// </summary>
        /// <param name="userName">使用者名稱</param>
        /// <param name="userRoles">使用者規則</param>
        /// <param name="expireMinutes">過期時間(分鐘)</param>
        /// <returns></returns>
        string GenerateToken(string userName, List<string> userRoles, int expireMinutes = 30);
    }
}
