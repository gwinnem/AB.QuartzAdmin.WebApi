using AB.QuartzAdmin.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Quartz;
using Quartz.Impl;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Reflection;

// ReSharper disable StringLiteralTypo

namespace AspNetCoreWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Gzip compression
            services.AddResponseCompression();
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            services.AddSingleton(Configuration);
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Quartz.Net Admin API",
                    Version = "v1",
                    Description = "Api for the Quartz.Net Scheduler",
                    Contact = new Contact
                    {
                        Name = "Geirr Winnem",
                        Email = "gwinnem@gmail.com",
                        Url = new Uri("https://twitter.com/gwinnem").ToString(),
                    },
                    License = new License
                    {
                        Name = "Use under MIT",
                        Url = new Uri( "https://github.com/gwinnem/AB.QuartzAdmin.WebApi/blob/master/LICENSE" ).ToString(),
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddOptions();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());


            // Setting up the DemoScheduler
            var properties = new NameValueCollection
            {
                {"quartz.serializer.type", "json"},
                {"quartz.scheduler.instanceName", "TestScheduler"},
                {"quartz.scheduler.instanceId", "ABQuartzAdmin"},
                {"quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz"},
                {"quartz.threadPool.threadCount", "10"}
            };

            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            var scheduler = sf.GetScheduler().GetAwaiter().GetResult();
            services.AddSingleton(scheduler);
            scheduler.Clear();
            DemoScheduler.Create(scheduler, true).GetAwaiter().GetResult();

            // Adding the Api
            services.AddQuartzAdmin(scheduler);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Adding swagger to expose the api  methods and documentation.
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quartz.Net Admin API V1"); });

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
