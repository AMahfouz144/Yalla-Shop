
using Microsoft.EntityFrameworkCore;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;

using YallaShop.Infrastructure.Persistence;
using YallaShop.Infrastructure.Repositories;
using YallaShop.Domain.Entites;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YallaShop.Infrastructure.Settings;
using YallaShop.API.Mapping;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.OpenApi.Models;
using YallaShop.Infrastructure.Services;

namespace YallaShop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            #region Cors & identity & Authentication
            builder.Services.AddCors(options => {
                options.AddPolicy("MyPolicy",
                                  policy => policy.AllowAnyMethod()
                                  .AllowAnyOrigin()
                                  .AllowAnyHeader());
            });
            builder.Services.AddIdentity<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            })
             .AddEntityFrameworkStores<AppDbContext>()
             .AddDefaultTokenProviders();
            builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //[authore ]:found toke or not
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //account/loginresonse unauthorize
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            #endregion
            #region Swagger
            //builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(swagger =>
            {
                //This�is�to�generate�the�Default�UI�of�Swagger�Documentation����
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET�5�Web�API",
                    Description = " ITI Projrcy"
                });
                //�To�Enable�authorization�using�Swagger�(JWT)����
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter�'Bearer'�[space]�and�then�your�valid�token�in�the�text�input�below.\r\n\r\nExample:�\"Bearer�eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                    });
            });
            #endregion
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.Configure<MailSettings>(
            builder.Configuration.GetSection("MailSettings"));
            //Generic Repository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //Auth services & repos
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            //seller services & repos
            builder.Services.AddScoped<ISellerService, SellerService>();
            builder.Services.AddScoped<ISellerRepository, SellerRepository>();
            //Account
            builder.Services.AddScoped<IAccountService, AccountService>();
            //Review Service & Repo
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            //Wishlist Service & Repo
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
            builder.Services.AddScoped<IWishlistService, WishlistService>();
            //AutoMapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingConfigurations>();
                cfg.AddProfile<YallaShop.Application.Mapping.MappingConfig>();
            });
            //Email service
            builder.Services.AddScoped<IEmailSender, EmailService>();

            // Register Repositories
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();


            // Register Repositories
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register Services
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();

            // Register Services
            builder.Services.AddScoped<IOrderService, OrderService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
