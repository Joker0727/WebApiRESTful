using JWT;
using JWT.Serializers;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using WebApiRESTful.Models;

namespace WebApiRESTful.Secure
{
    /// <summary>
    /// 权限特性
    /// </summary>
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        public readonly string secret = System.Configuration.ConfigurationManager.AppSettings["Secret"];//口令加密秘钥
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //前端请求api时会将token存放在名为"auth"的请求头中           
            var authHeader = actionContext.Request.Headers.Where(w => w.Key == "Authorization")
                .Select(s => s.Value.FirstOrDefault()).ToList();
            if (authHeader != null)
            {
                string token = authHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        //secret需要加密
                        IJsonSerializer serializer = new JsonNetSerializer();//序列化Json
                        IDateTimeProvider provider = new UtcDateTimeProvider();
                        IJwtValidator validator = new JwtValidator(serializer, provider);
                        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                        IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                        AuthInfo authinfo = decoder.DecodeToObject<AuthInfo>(token, secret, verify: true);
                        if (authinfo != null)
                        {
                            if (authinfo.ExpiryDateTime == null || authinfo.ExpiryDateTime < DateTime.Now)
                                return false;
                            actionContext.RequestContext.RouteData.Values.Add("Authorization", authinfo);
                            return true;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}