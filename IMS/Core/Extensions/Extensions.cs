using IMS.Core.Services;
using IMS.Repositories.User;
using IMS.Services.Login;
using IMS.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text;

namespace IMS.Core.Extensions
{
    public static class Extensions
    {
        public static WebApplicationBuilder AddJwtAuthentication(this WebApplicationBuilder builder) 
        {
            var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
            var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = jwtIssuer,
                     ValidAudience = jwtIssuer,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
                 };
             });

            return builder;
        }
        public static WebApplicationBuilder AddPostGreSqlDbConnection(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("PostgreDB");
            builder.Services.AddScoped((provider) => new NpgsqlConnection(string.Format(connectionString!, "postgres", "postgres", "postgres")));

            return builder;
        }
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserContextService, UserContextService>();
            builder.Services.AddTransient<IHashingService, HashingService>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            return builder;
        }
    }
}
