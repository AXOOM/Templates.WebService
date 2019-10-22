using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace MyVendor.MyService.Infrastructure
{
    public static class RestApi
    {
        public static IServiceCollection AddRestApi(this IServiceCollection services)
        {
            services.AddMvcCore(options =>
                     {
                         options.EnableEndpointRouting = false;
                         options.Filters.Add(typeof(ApiExceptionFilterAttribute));
                     })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                    .AddApiExplorer()
                    .AddFormatterMappings()
                    .AddDataAnnotations()
                    .AddNewtonsoftJson(options =>
                     {
                         options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                         options.SerializerSettings.Converters.Add(new StringEnumConverter {NamingStrategy = new CamelCaseNamingStrategy()});
                     });

//            services.AddSwaggerGen(options =>
//            {
//                options.SwaggerDoc("v1",
//                    new Info
//                    {
//                        Title = "My Service",
//                        Version = "v1"
//                    });
//                options.IncludeXmlComments(Path.Combine(ApplicationEnvironment.ApplicationBasePath, "MyVendor.MyService.xml"));
//                options.IncludeXmlComments(Path.Combine(ApplicationEnvironment.ApplicationBasePath, "MyVendor.MyService.Dto.xml"));
//                options.DescribeAllEnumsAsStrings();
//            });

            return services;
        }

        public static IApplicationBuilder UseRestApi(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(TrustExternalProxy())
               .UseStatusCodePages();

//            app.UseSwagger()
//               .UseSwaggerUI(options =>
//                {
//                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My Service API v1");
//                    options.OAuthAppName("My Service Swagger UI");
//                    options.OAuthClientId("myvendor-myservice");
//                });

            return app.UseMvc();
        }

        private static ForwardedHeadersOptions TrustExternalProxy()
        {
            var options = new ForwardedHeadersOptions {ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto};
            options.KnownProxies.Clear();
            options.KnownNetworks.Clear();
            return options;
        }
    }
}
