using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValueController : ControllerBase
    {
        private readonly ILogger<ValueController> _logger;

        public ValueController(ILogger<ValueController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Jwt", "Get( Not Authorize )" };
        }

        [HttpGet("~/notlimitedapi")]
        public IEnumerable<string> GetNotLimitedAPI()
        {
            return new string[] { "Jwt", "Get( Not Authorize )" };
        }

        [Authorize] // 要求 Authorization
        [HttpGet("~/limitedapi")]
        public IEnumerable<string> GetLimitedAPI()
        {
            return new string[] { "Jwt", "Get( Authorized )" };
        }

        [Authorize(Roles = "Admin")] // 要求 Authorization 且要有 Admin 規則
        [HttpGet("~/limitedwithadminroleapi")]
        public IEnumerable<string> GetLimitedWithAdminRoleAPI()
        {
            return new string[] { "Jwt", "Get( Authorized With Role = Admin )" };
        }

        [Authorize(Roles = "Admin,User")] // 要求 Authorization 且要有 Admin 或 User 規則
        [HttpGet("~/limitedwithuserroleapi")]
        public IEnumerable<string> GetLimitedWithAdminAndUserRoleAPI()
        {
            return new string[] { "Jwt", "Get( Authorized With Role = Admin or User )" };
        }
    }
}
