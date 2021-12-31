using Ro.SQLite.Data;
using SecretsAPI.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetSection("DefaultConnection").Value;
builder.Services.AddTransient<IDbAsync>( svc => 
{
    return new Database(connectionString);
});

builder.Services.AddScoped<IUsersRepo, UsersRepo>();
builder.Services.AddScoped<ISecretsRepo, SecretsRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
