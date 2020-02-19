using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JwtAuthCore.Helpers.Interface;
using JwtAuthCore.Model;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Utility;

namespace JwtAuthCore.Controllers
{
    [Authorize] // 要求 Authorization
    [ApiController]
    public class LoginController : ControllerBase
    {
        #region Properties

        private readonly IJwtHelper _jwt;

        #endregion

        #region Constructor

        public LoginController(IJwtHelper jwt)
        {
            _jwt = jwt;
        }

        #endregion

        [AllowAnonymous] // 不要求 Authorization
        [HttpPost("~/signin")]
        public IActionResult SignIn(LoginModel login)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            if (ValidateUser(login))
            {
                // 產生 JWT token
                string jwtToken = _jwt.GenerateToken(login.Username, login.Roles, login.ExpireMinutes);

                response.Content = new StringContent(JsonConvert.SerializeObject(new { token = jwtToken }));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return new HttpResponseMessageResult(response);
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// 驗證使用者
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private bool ValidateUser(LoginModel login)
        {
            return true; // TODO: 驗證使用者邏輯可撰寫於此...
        }

        /// <summary>
        /// 取得聲明資訊
        /// </summary>
        /// <returns></returns>
        [HttpGet("~/claims")]
        public IActionResult GetClaims()
        {
            return Ok(User.Claims.Select(p => new { p.Type, p.Value }));
        }

        /// <summary>
        /// 取得使用者名稱
        /// </summary>
        /// <returns></returns>
        [HttpGet("~/username")]
        public IActionResult GetUserName()
        {
            return Ok(User.Identity.Name);
        }

        /// <summary>
        /// 取得 JWT ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("~/jwtid")]
        public IActionResult GetUniqueId()
        {
            var jti = User.Claims.FirstOrDefault(p => p.Type == "jti");
            return Ok(jti.Value);
        }
    }
}
