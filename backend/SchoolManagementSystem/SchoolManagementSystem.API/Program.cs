using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

DotEnv.Load(new DotEnvOptions(
    probeForEnv: true
));


builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(option => 
option.UseSqlServer(builder.Configuration.GetConnectionString("StudentManagementSystemConnectionString")));

builder.Services.AddDbContext<AuthDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("StudentManagementSystemAuthConnectionString")));

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
