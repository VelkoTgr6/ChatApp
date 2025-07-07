using FriChat.Extensions;
using FriChat.Infrastructure.Services.EmailSender;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FriChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAppDbContext(builder.Configuration);

            builder.Services.AddAppServices();

            builder.Services.AddAppIdentity(builder.Configuration);

            // Configure EmailSettings with values from the configuration
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            // Register EmailService as a singleton service
            builder.Services.AddSingleton<IEmailSender>(serviceProvider =>
            {
                var emailSettings = serviceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;
                return new EmailService(
                    emailSettings.SmtpServer,
                    emailSettings.SmtpPort,
                    emailSettings.SmtpUser,
                    emailSettings.SmtpPass
                );
            });

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseAuthentication();

            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value?.ToLowerInvariant();

                if (!context.User.Identity.IsAuthenticated && context.Request.Path == "/")
                {
                    // Only redirect to login if not already on the login page
                    if (path != "/identity/account/login")
                    {
                        context.Response.Redirect("/Identity/Account/Login");
                        return;
                    }
                }
                else if (context.User.IsInRole("User"))
                {
                    // Only redirect if the user is at the root ("/")
                    if (path == "/")
                    {
                        context.Response.Redirect("/AppUser/Index");
                        return;
                    }
                }

                await next();
            });

            // Log all 400 Bad Request errors and form data for debugging
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 400)
                {
                    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("400Logger");
                    logger.LogWarning("400 Bad Request for {Path}. Method: {Method}", context.Request.Path, context.Request.Method);
                    if (context.Request.HasFormContentType)
                    {
                        foreach (var key in context.Request.Form.Keys)
                        {
                            logger.LogWarning("Form field: {Key} = {Value}", key, context.Request.Form[key]);
                        }
                    }
                }
            });

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}
