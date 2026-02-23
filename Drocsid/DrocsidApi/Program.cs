using DrocsidApi.Managers;
using DrocsidApi.Services;
using LogicLibrary.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DrocsidApi
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

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme =
                option.DefaultChallengeScheme =
                option.DefaultForbidScheme =
                option.DefaultScheme =
                option.DefaultSignInScheme =
                option.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = new TimeSpan(0),
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))
                };
            });

            builder.Services.AddSingleton<IDatabaseManager, DatabaseManager>();
            builder.Services.AddScoped<IRefreshTokenManager, RefreshTokenManager>();
            builder.Services.AddScoped<IUserManager, UserManager>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<PasswordService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
