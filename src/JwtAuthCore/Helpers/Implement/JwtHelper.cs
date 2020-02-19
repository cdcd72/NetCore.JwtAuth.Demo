using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtAuthCore.Helpers.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthCore.Helpers.Implement
{
    /// <summary>
    /// JWT Helper
    /// </summary>
    public class JwtHelper : IJwtHelper
    {
        #region Properties

        private readonly IConfiguration _config;

        #endregion

        #region Constructor

        public JwtHelper(IConfiguration configuration)
        {
            _config = configuration;
        }

        #endregion

        /// <summary>
        /// 產生 Token
        /// </summary>
        /// <param name="userName">使用者名稱</param>
        /// <param name="userRoles">使用者規則</param>
        /// <param name="expireMinutes">過期時間(分鐘)</param>
        /// <returns></returns>
        public string GenerateToken(string userName, List<string> userRoles, int expireMinutes = 30)
        {
            var issuer = _config.GetValue<string>("JwtSettings:Issuer");
            var signKey = _config.GetValue<string>("JwtSettings:SignKey");

            // 設定要加入到 JWT Token 中的聲明資訊(Claims)
            var claims = new List<Claim>
            {
                // RFC 7519 規格中總共定義了 7 個預設的 Claims，目前只用到兩種！

                //new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Sub, userName), // User.Identity.Name
                //new Claim(JwtRegisteredClaimNames.Aud, "The Audience"),
                //new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString()),
                //new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), // 必須為數字
                //new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), // 必須為數字
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
            };

            #region 擴充 "roles" 加入登入者該有的角色

            foreach (string userRole in userRoles)
            {
                claims.Add(new Claim("roles", userRole));
            }

            #endregion

            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立一組對稱式加密的金鑰，主要用於 JWT 簽章之用
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // HmacSha256 有要求必須要大於 128 bits，所以 signKey 不能太短，至少要 16 字元以上
            // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 建立 SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                //Audience = issuer, // 由於你的 API 受眾通常沒有區分特別對象，因此通常不太需要設定，也不太需要驗證
                //NotBefore = DateTime.Now, // 預設值就是 DateTime.Now
                //IssuedAt = DateTime.Now, // 預設值就是 DateTime.Now
                Subject = userClaimsIdentity,
                Expires = DateTime.Now.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            // 產出所需要的 JWT securityToken 物件，並取得序列化後的 Token 結果(字串格式)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
