using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Unlimitedinf.Apis.Server
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
            var ts = new TableStorage(this.Configuration);
            services.AddSingleton(typeof(TableStorage), ts);

            services.AddMvc(o =>
            {
                o.Filters.Add(typeof(Filters.ValidateViewModelAttribute));
                o.Filters.Add(typeof(Filters.AiExceptionFilterAttribute));
                o.RequireHttpsPermanent = true;
            });

            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
            });

            services.AddApplicationInsightsTelemetry(o => o.ApplicationVersion = FileVersionInfo.GetVersionInfo(typeof(Startup).Assembly.Location).FileVersion);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
