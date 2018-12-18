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
                        c.IncludeXmlComments(GetXmlCommentsPath());//��ȡWebApiRESTful.XML
                        c.DescribeAllEnumsAsStrings();
                        c.OperationFilter<HttpHeaderFilter>();  // Ȩ�޹���
                        c.OperationFilter<UploadFilter>();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("ϵͳ�����ӿ�");
                        // ʹ������
                        c.InjectJavaScript(thisAssembly, "WebApiRESTful.scripts.Swagger.Swagger_lang.js");
                    });
        }
        /// <summary>
        /// ��ȡWebApiRESTful.XML
        /// </summary>
        /// <returns></returns>
        private static string GetXmlCommentsPath()
        {
            return string.Format("{0}/bin/WebApiRESTful.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
