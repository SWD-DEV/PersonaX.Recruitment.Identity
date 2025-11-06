using Duende.IdentityServer;
using Persol.Marketplace.Data;
using Persol.Marketplace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Persol.Marketplace;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var identityServerBuilder = builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = db =>
                    db.UseSqlServer(builder.Configuration.GetConnectionString("IdentityServerConnection"),
                        sql => sql.MigrationsAssembly(typeof(HostingExtensions).Assembly.FullName));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = db =>
                    db.UseSqlServer(builder.Configuration.GetConnectionString("IdentityServerConnection"),
                        sql => sql.MigrationsAssembly(typeof(HostingExtensions).Assembly.FullName));
                options.EnableTokenCleanup = true;
            })
            .AddAspNetIdentity<ApplicationUser>();

        builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = "copy client ID from Google here";
                options.ClientSecret = "copy client secret from Google here";
            });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}