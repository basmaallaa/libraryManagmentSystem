using Library.BLL.Mapping;
using Library.BLL.Services.Implementations;
using Library.BLL.Services.Interfaces;
using Library.DAL.Context;
using Library.DAL.Models;
using Library.DAL.Repository.Implementations;
using Library.DAL.Repository.Interfaces;
using Library.DAL.Seed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            #region Connection string Configration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(connectionString)
            );
            #endregion

            #region Identity Configration 

            builder.Services.AddIdentityCore<ApplicationUser>(Option =>
            {
                Option.Password.RequiredLength = 8;
                Option.Password.RequireNonAlphanumeric = true;
                Option.Password.RequireUppercase = true;
                Option.Password.RequireLowercase = true;
                Option.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Home/AccessDenied";
            });

            #endregion

            builder.Services.AddScoped(typeof(IGenericRepo<>),typeof(GenericRepo<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddAutoMapper(x => x.AddProfile(new MappingProfile()));
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBorrowService, BorrowService>();
            builder.Services.AddScoped<ILibraryService, LibraryService>();
            builder.Services.AddScoped<IAdminService, AdminService>();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                await DbInitializer.SeedRolesAndAdminAsync(roleManager, userManager);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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
          

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
