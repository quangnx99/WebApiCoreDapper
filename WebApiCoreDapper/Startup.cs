using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace WebApiCoreDapper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title =  "RESTAPI DAPPER", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //cấu hình cho phép domain nào được truy cập api này
            //khi thực thi chương trình nếu có lỗi nó sẽ bắt lỗi là lỗi gì tên gì
            //sau đó thì ta có thể ghi ra log
            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (ex == null) return;

                    var error = new
                    {
                        message = ex.Message
                    };
                    context.Response.ContentType = "application/json";
                    context.Response.Headers.Add("Access-Control-Allow-Credential", new[] { "true" });
                    context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { Configuration["AllowedOrigins"] });
                    using (var writer=new StreamWriter(context.Response.Body))
                    {
                        new JsonSerializer().Serialize(writer, error);
                        await writer.FlushAsync().ConfigureAwait(false);
                    }
                }
                );
            });
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RESTAPI DAPPER V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
