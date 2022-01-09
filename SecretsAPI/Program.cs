using Ro.SQLite.Data;
using SecretsAPI.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
	    Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
	    In = ParameterLocation.Header,
	    Name = "Authorization",
	    Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
	        options.TokenValidationParameters = new TokenValidationParameters
		{
		    ValidateIssuerSigningKey = true,
		    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
								.GetBytes(builder.Configuration.GetSection("JWT_TOKEN_KEY").Value)),
		    ValidateIssuer = false,
		    ValidateAudience = false
		};
    });

var connectionString = builder.Configuration.GetSection("DefaultConnection").Value;
builder.Services.AddTransient<IDbAsync>( svc => 
{
    return new Database(connectionString);
});

builder.Services.AddScoped<IUsersRepo, UsersRepo>();
builder.Services.AddScoped<ISecretsRepo, SecretsRepo>();

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
