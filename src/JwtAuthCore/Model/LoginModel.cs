using System.Collections.Generic;

namespace JwtAuthCore.Model
{
    /// <summary>
    /// 登入資料模型
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 使用者密碼
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 使用者規則
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// 過期時間(分鐘數)
        /// </summary>
        public int ExpireMinutes { get; set; } = 30;
    }
}
