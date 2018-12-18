using api.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApiRESTful.Models;
using WebApiRESTful.Secure;

namespace WebApiRESTful.Controllers
{
    [RoutePrefix("authoritydemo")]
    public class AuthorityDemoController : ApiController
    {
        public readonly string secret = System.Configuration.ConfigurationManager.AppSettings["Secret"]; //口令加密秘钥

        [HttpPost, Route("api/login"), ResponseType(typeof(LoginResult))]
        public IHttpActionResult Login([FromBody]LoginRequest request)
        {
            LoginResult rs = new LoginResult();
            //这是是获取用户名和密码的，这里只是为了模拟
            if (request.UserName == "123" && request.Password == "123")
            {
                AuthInfo info = new AuthInfo
                {
                    UserName = request.UserName,
                    Roles = new List<string> { "Admin", "Manage" },
                    IsAdmin = true,
                    ExpiryDateTime = DateTime.Now.AddHours(2)
                };
                try
                {
                    //secret需要加密
                    IJwtAlgorithm algorithm = new HMACSHA256Algorithm();//加密方式
                    IJsonSerializer serializer = new JsonNetSerializer();//序列化Json
                    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();//base64加解密
                    IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);//JWT编码
                    var token = encoder.Encode(info, secret);//生成令牌
                    rs.Message = "登陆成功";
                    rs.Token = token;
                    rs.Success = true;
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                rs.Message = "fail";
                rs.Success = false;
            }     
            return Ok(rs);
        }

        // GET: User
        [ApiAuthorize]
        [HttpGet, Route("api/getusername"), ResponseType(typeof(string))]
        public IHttpActionResult GetUserName()
        {
            //获取回用户信息(在ApiAuthorize中通过解析token的payload并保存在RouteData中)
            AuthInfo authInfo = this.RequestContext.RouteData.Values["Authorization"] as AuthInfo;
            string resMessage = string.Empty;
            if (authInfo == null)
                resMessage = "获取不到，失败";
            else
                resMessage = $"获取到了，Auth的Name是 {authInfo.UserName}";

            return Ok(resMessage);
        }
    }
}