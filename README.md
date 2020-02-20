# NetCore.JwtAuth.Demo
> 這個專案專注於利用 Json Web Token 來達到 API-based 的設計架構中實踐驗證與授權機制。  
> This project focus on the use of **Json Web Token** to achieve API-based design verification and authorization.  

練習 JWT Token-based authentication 實作於 .NET Core 3.1 上。  
To practice how jwt token-based authentication implement on .NET Core 3.1.  

## Running the project
The solution contains a Test project validating the application behaviour. You can run the tests from Visual Studio 2019 or by typing `dotnet test` in a command window.  
If you want to interactively test the application, you can use [Postman](https://www.getpostman.com/) or any other Http client.

1. Run the project from Visual Studio 2019 or by typing `dotnet run` in a command window
2. Launch _Postman_ and make a GET request as follows:

```
    GET https://localhost:5001/limitedapi HTTP/1.1
    cache-control: no-cache
    Accept: */*
    Host: localhost:5001
    accept-encoding: gzip, deflate, br
    Connection: keep-alive
```

This should return a 401 HTTP status code (_Unauthorized_)

3. Make a POST request like the following:

```
    POST https://localhost:5001/signin HTTP/1.1
    cache-control: no-cache
    Content-Type: application/json
    Accept: */*
    Host: localhost:5001
    accept-encoding: gzip, deflate, br
    content-length: 88
    Connection: keep-alive

    HTTP message body:
    { "Username": "neil", "Password": "secret", "Roles": ["User"], "ExpireMinutes": 30 }
```

It returns a JSON object like the following:

```
    {
       "token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJuZWlsIiwianRpIjoiZGMxODBiZGQtODBkZC00OTE4LWJiNmUtNjEwYjY4M2U3OWVmIiwicm9sZXMiOiJVc2VyIiwibmJmIjoxNTgyMTAyNTc4LCJleHAiOjE1ODIxMDQzNzgsImlhdCI6MTU4MjEwMjU3OCwiaXNzIjoiSnd0QXV0aElzc3VlciJ9.dnmIWJkBSFmE5rMUPiZhA0p6SdjC1xMu9RhHs-jrlkk"
    }
```

4. The following GET request

```
    GET https://localhost:5001/limitedapi HTTP/1.1
    cache-control: no-cache
    Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJuZWlsIiwianRpIjoiZGMxODBiZGQtODBkZC00OTE4LWJiNmUtNjEwYjY4M2U3OWVmIiwicm9sZXMiOiJVc2VyIiwibmJmIjoxNTgyMTAyNTc4LCJleHAiOjE1ODIxMDQzNzgsImlhdCI6MTU4MjEwMjU3OCwiaXNzIjoiSnd0QXV0aElzc3VlciJ9.dnmIWJkBSFmE5rMUPiZhA0p6SdjC1xMu9RhHs-jrlkk
    Accept: */*
    Host: localhost:5001
    accept-encoding: gzip, deflate, br
    Connection: keep-alive
```

returns the following response:

```
[
    "Jwt",
    "Get",
    "Authorized"
]
```

## Last Version
1.0.0.0 (February 19, 2020)

## Record
* 1.0.0.0
  * Initial Commit (初次上版)
  
## Refer Github
[JwtAuthDemo](https://github.com/doggy8088/JwtAuthDemo) by doggy8088  

## Refer Article
* English
  * [jwt.io](https://jwt.io/)  
  * [ASP.NET Core 3.1 - JWT Authentication Tutorial with Example API](https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api)  
* Chinese
  * [如何在 ASP.NET Core 3 使用 Token-based 身分驗證與授權 (JWT)](https://blog.miniasp.com/post/2019/12/16/How-to-use-JWT-token-based-auth-in-aspnet-core-31)  
  * [\[JWT\] 甚麼是 JWT](https://dotblogs.com.tw/yc421206/2019/01/07/what_is_jwt)
  * [在 ASP.NET Core WebAPI 中使用 JWT 驗證](https://poychang.github.io/authenticating-jwt-tokens-in-asp-net-core-webapi/)  
  * [淺談 Authentication 中集：Token-based authentication](https://medium.com/@xumingyo/%E6%B7%BA%E8%AB%87-authentication-%E4%B8%AD%E9%9B%86-token-based-authentication-90139fbcb897)
  * [JWT JSON Web Token 使用 ASP.NET Core 2.0 Web API 的逐步練習教學與各種情境測試](https://csharpkh.blogspot.com/2018/04/jwt-json-web-token-aspnet-core.html)
