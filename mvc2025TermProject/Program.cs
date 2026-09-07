using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using EmailServices;
using NETCore.MailKit.Core;
using Azure.Identity;
using Azure.Storage.Blobs;

namespace mvc2025TermProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add saving images to the blob storage
            var storageAccountName = builder.Configuration["AzureBlobStorage:AccountName"];
            var blobServiceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
            builder.Services.AddSingleton(
                new BlobServiceClient(
                    blobServiceUri,
                    new DefaultAzureCredential()
                )
            );

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("TermProjectConnection") ??
                throw new InvalidOperationException("Connection string 'TermProjectConnection' not found.");

            // Add Email service
            var emailConfig = builder.Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();

            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailSender, EmailSender>();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseLazyLoadingProxies() // Lazy loading
                    .UseSqlServer(
                        connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(10),
                                errorNumbersToAdd: null);
                        }));

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            }

            builder.Services
                .AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false; //Change it to =true when configure an email service

                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365 * 100); // 100 years
                    options.Lockout.MaxFailedAccessAttempts = 3;

                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
