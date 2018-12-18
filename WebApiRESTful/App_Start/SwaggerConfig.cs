using System.Web.Http;
using WebActivatorEx;
using WebApiRESTful;
using Swashbuckle.Application;
using WebApiRESTful.Filter;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WebApiRESTful
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "WebApiRESTful");
                        c.IncludeXmlComments(GetXmlCommentsPath());//读取WebApiRESTful.XML
                        c.DescribeAllEnumsAsStrings();
                        c.OperationFilter<HttpHeaderFilter>();  // 权限过滤
                        c.OperationFilter<UploadFilter>();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("系统开发接口");
                        // 使用中文
                        c.InjectJavaScript(thisAssembly, "WebApiRESTful.scripts.Swagger.Swagger_lang.js");
                    });
        }
        /// <summary>
        /// 读取WebApiRESTful.XML
        /// </summary>
        /// <returns></returns>
        private static string GetXmlCommentsPath()
        {
            return string.Format("{0}/bin/WebApiRESTful.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
