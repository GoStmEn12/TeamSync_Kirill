using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeamSync_Kirill.Models;
using TeamSync_Kirill.Services;
using TeamSync_Kirill.DbContext;

namespace TeamSync_Kirill
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// ��������� �������
			builder.Logging.ClearProviders();
			builder.Logging.AddConsole();
			builder.Logging.AddDebug();

			// Add service products DI container
			builder.Services.AddScoped<ICommentService, CommentService>();
			builder.Services.AddScoped<IFileService, FileService>();
			builder.Services.AddScoped<INotificationService, NotificationService>();
			builder.Services.AddScoped<IProjectService, ProjectService>();
			builder.Services.AddScoped<IProjectUserService, ProjectUserService>();
			builder.Services.AddScoped<ITaskService, TaskService>();
			builder.Services.AddScoped<ITaskStatusService, TaskStatusService>();
			builder.Services.AddScoped<ITaskUserService, TaskUserService>();
			builder.Services.AddScoped<IUserRoleService, UserRoleService>();
			builder.Services.AddScoped<IUserService, UserService>();


			//// ����������� ��������

			////����� ����� �������� ���  DbContext  ������ ApplicationDbContext
			//// 1. ��������� ����������� � ���� ������
			builder.Services.AddDbContext<AppDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			//// 2. ��������� Identity
			builder.Services.AddIdentity<User, UserRole>(options =>
			{
				options.SignIn.RequireConfirmedEmail = true;
				options.Password.RequireDigit = false;
				options.Password.RequiredLength = 4;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequiredUniqueChars = 0;
				options.Password.RequireNonAlphanumeric = false;
			})
			.AddEntityFrameworkStores<AppDbContext>() // �������� ApplicationDbContext �� ��� �������� ���� ������
			.AddDefaultTokenProviders();



			// 3. ��������� CORS
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowSpecificOrigin", policyBuilder =>
				{
					policyBuilder.WithOrigins("http://127.0.0.1:5500")
								 .AllowAnyHeader()
								 .AllowAnyMethod();
				});
			});

			// 4. ��������� �������������� (JWT + Cookie)
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddCookie(options =>
			{
				options.LoginPath = "/Account/Login";
				options.AccessDeniedPath = "/Account/AccessDenied";
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					ValidAudience = builder.Configuration["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
				};

				// ����������� ������� ��������������
				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
						logger.LogError("Authentication failed: {Message}", context.Exception.Message);
						return System.Threading.Tasks.Task.CompletedTask;
					},
					OnTokenValidated = context =>
					{
						var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
						logger.LogInformation("Token validated successfully.");
						return System.Threading.Tasks.Task.CompletedTask;
					}
				};
			});

			// 5. ����������� MVC
			builder.Services.AddControllersWithViews();

			// 6. ��������� �������� ����������� (��������, ���� �����)
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
			});

			var app = builder.Build();




			// ��������� middleware

			// 1. ��������� ������
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseHttpsRedirection();
			

			// 2. CORS
		//	app.UseCors("AllowSpecificOrigin");


			// �������� CORS ����� ������� UseAuthentication � UseAuthorization
			app.UseCors("AllowAll");
			app.UseAuthentication();
			app.UseAuthorization();


			app.UseStaticFiles();
			// 4. �������������
			app.UseRouting();

			// 5. ��������� ���������
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			// 6. ���������� �������� ����� API
			app.MapControllers();

			// ������ ����������
			app.Run();
		}
	}
}
