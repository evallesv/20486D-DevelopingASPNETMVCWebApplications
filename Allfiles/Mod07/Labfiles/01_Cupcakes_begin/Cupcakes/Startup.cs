﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Cupcakes.Data;
using Microsoft.EntityFrameworkCore;
using Cupcakes.Repositories;

namespace Cupcakes
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<CupcakeContext>(options => options.UseSqlite("Data Source=cupcake.db"));
            services.AddDbContext<CupcakeContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<ICupcakeRepository, CupcakeRepository>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, CupcakeContext cupcakeContext)
        {
            //cupcakeContext.Database.EnsureDeleted();
            //cupcakeContext.Database.EnsureCreated();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "CupcakeRoute",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Cupcake", action = "Index" },
                    constraints: new { id = "[0-9]+" });
            });
        }
    }
}
