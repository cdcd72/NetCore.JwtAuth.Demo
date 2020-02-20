using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Web.Tests
{
    public class ApplicationTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ApplicationTests()
        {
            // 取得 config
            var config = new ConfigurationBuilder().SetBasePath(Path.GetFullPath(@"../../../../Web"))
                                                   .AddJsonFile("appsettings.json", optional: false)
                                                   .Build();

            // 模擬 Server-side
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>()
                                                         .UseConfiguration(config));

            // 模擬 Client-side
            _client = _server.CreateClient();
        }

        [SetUp]
        public void Setup()
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// 取得 Token
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetToken()
        {
            string token = await GetJwtToken(@"{ ""Username"": ""neil"", ""Password"": ""secret"", ""Roles"": [], ""ExpireMinutes"": 30 }");

            // Token 不是 null
            Assert.NotNull(token);
        }

        #region GetNotLimitedAPITest

        /// <summary>
        /// 無需經過授權 - 取用 API 成功
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetNotLimitedAPI_Success()
        {
            var response = await _client.GetAsync("/notlimitedapi");

            // 預期回應為 200
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JArray.Parse(responseString);

            // 預期為 "Not Authorize" 字串
            Assert.AreEqual("Not Authorize", responseJson[2].ToString());
        }

        #endregion

        #region GetLimitedAPITest

        /// <summary>
        /// 已經過授權但無角色規則 - 取用 API 成功
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetLimitedAPI_Success()
        {
            string token = await GetJwtToken(@"{ ""Username"": ""neil"", ""Password"": ""secret"", ""Roles"": [], ""ExpireMinutes"": 30 }");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/limitedapi");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(requestMessage);

            // 預期回應為 200
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JArray.Parse(responseString);

            // 預期為 "Authorized" 字串
            Assert.AreEqual("Authorized", responseJson[2].ToString());
        }

        /// <summary>
        /// 因未經過授權 - 取用 API 失敗
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetLimitedAPI_Failed()
        {
            var response = await _client.GetAsync("/limitedapi");

            // 預期回應為 401
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region GetLimitedWithAdminRoleAPITest

        /// <summary>
        /// 已經過授權且有 Admin 角色規則 - 取用 API 成功
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetLimitedWithAdminRoleAPI_Success()
        {
            string token = await GetJwtToken(@"{ ""Username"": ""neil"", ""Password"": ""secret"", ""Roles"": [""Admin""], ""ExpireMinutes"": 30 }");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/limitedwithadminroleapi");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(requestMessage);

            // 預期回應 200
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JArray.Parse(responseString);

            // 預期為 "Authorized With Role = Admin" 字串
            Assert.AreEqual("Authorized With Role = Admin", responseJson[2].ToString());
        }

        /// <summary>
        /// 已經過授權且有 User 角色規則 - 取用 API 失敗
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetLimitedWithAdminRoleAPI_Failed()
        {
            string token = await GetJwtToken(@"{ ""Username"": ""neil"", ""Password"": ""secret"", ""Roles"": [""User""], ""ExpireMinutes"": 30 }");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/limitedwithadminroleapi");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(requestMessage);

            // 預期回應 403
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        #endregion

        #region GetLimitedWithAdminOrUserRoleAPITest

        /// <summary>
        /// 已經過授權且有 Admin 角色規則 - 取用 API 成功
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetLimitedWithAdminOrUserRoleAPI_WithAdminRole_Success()
        {
            string token = await GetJwtToken(@"{ ""Username"": ""neil"", ""Password"": ""secret"", ""Roles"": [""Admin""], ""ExpireMinutes"": 30 }");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/limitedwithadminoruserroleapi");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(requestMessage);

            // 預期回應 200
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JArray.Parse(responseString);

            // 預期為 "Authorized With Role = Admin or User" 字串
            Assert.AreEqual("Authorized With Role = Admin or User", responseJson[2].ToString());
        }

        /// <summary>
        /// 已經過授權且有 User 角色規則 - 取用 API 成功
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetLimitedWithAdminOrUserRoleAPI_WithUserRole_Success()
        {
            string token = await GetJwtToken(@"{ ""Username"": ""neil"", ""Password"": ""secret"", ""Roles"": [""User""], ""ExpireMinutes"": 30 }");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/limitedwithadminoruserroleapi");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(requestMessage);

            // 預期回應 200
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JArray.Parse(responseString);

            // 預期為 "Authorized With Role = Admin or User" 字串
            Assert.AreEqual("Authorized With Role = Admin or User", responseJson[2].ToString());
        }

        /// <summary>
        /// 已經過授權但無角色規則 - 取用 API 失敗
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetLimitedWithAdminOrUserRoleAPI_NotRoles_Failed()
        {
            string token = await GetJwtToken(@"{ ""Username"": ""neil"", ""Password"": ""secret"", ""Roles"": [], ""ExpireMinutes"": 30 }");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/limitedwithadminoruserroleapi");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(requestMessage);

            // 預期回應 403
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 取得 JWT Token
        /// </summary>
        /// <param name="bodyString">HTTP message body</param>
        /// <returns></returns>
        private async Task<string> GetJwtToken(string bodyString)
        {
            // POST
            var response = await _client.PostAsync("/signin", new StringContent(bodyString, Encoding.UTF8, "application/json"));

            // 讀取 response content
            var responseString = await response.Content.ReadAsStringAsync();

            // 將字串轉為 JSON 格式
            var responseJson = JObject.Parse(responseString);

            // 取得其中之 Token
            return (string)responseJson["token"];
        }

        #endregion
    }
}