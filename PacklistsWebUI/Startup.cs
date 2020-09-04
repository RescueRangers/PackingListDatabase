using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syncfusion.Blazor;
using PacklistsWebUI.Data;
using PacklistsWebUI.Repositories.Interfaces;
using PacklistsWebUI.Repositories;
using PacklistsWebUI.DataAdaptors;
using PacklistsWebUI.Services;

namespace PacklistsWebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSyncfusionBlazor();
            //services.AddSingleton<PdfService>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<IPackingListsRepository>(new PackingListsRepository(Configuration));
            services.AddSingleton<IMaterialsRepository>(new MaterialsRepository(Configuration));
            services.AddSingleton<IItemsRepository>(new ItemsRepository(Configuration));
            services.AddSingleton<PacklisteService>();

            services.AddScoped<PackingListItemsAdaptor>();
            services.AddScoped<PackingListAdaptor>();
            services.AddScoped<MaterialAmountAdaptor>();
            services.AddScoped<RawUsageAdaptor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var syncfusionKey = Configuration.GetValue<string>("SyncfusionLicense");
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionKey);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();

            }
            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
