using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebAppNetCore.Models;

namespace WebAppNetCore
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
            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddControllers(options => options.EnableEndpointRouting = false);

            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("EmployeeDBConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>(Options =>
            {
                Options.Password.RequiredLength = 10;
                Options.Password.RequiredUniqueChars = 3;

            })
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "126046053480-f81mdk40mihfkdmnbikr60cqs7polbk6.apps.googleusercontent.com";
                options.ClientSecret = "bdBpm_fkjRODmcn6zIBWU0-P";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role"));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context =>
             context.User.IsInRole("Admin") &&
             context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
             context.User.IsInRole("Super Admin")));
            });
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithRedirects("/Error/{0}");

            }

            app.UseStaticFiles();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvc(routes =>
           {
               routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
           });
           



            //app.Run(async (context) =>
            //{
            //    await context.Response
            //    .WriteAsync("No Record Found");
            //});
        }
    }
}