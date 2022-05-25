using FluentValidation;
using FluentValidation.AspNetCore;
using NLog;
using HomeBudget;
using HomeBudget.Authentication;
using HomeBudget.Authorization;
using HomeBudget.Entities;
using HomeBudget.Middleware;
using HomeBudget.Models;
using HomeBudget.Models.Validators;
using HomeBudget.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using NLog.Web;
using HomeBudget.Cache;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseNLog();
// utworzenie bazy danych
builder.Services.AddDbContext<BudgetDbContext>();

var authenticationSettings = new AuthenticationSettings();

builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);    
builder.Services.AddSingleton(authenticationSettings);

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = "Bearer";
    opt.DefaultScheme = "Bearer";
    opt.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
    };
});

// Add services to the container.

var cacheSettings = new CacheSettings();
builder.Configuration.GetSection("CacheConfiguration").Bind(cacheSettings);
builder.Services.AddSingleton(cacheSettings);

builder.Services.AddMemoryCache();
builder.Services.AddTransient<MemoryCacheService>();
builder.Services.AddTransient<RedisCacheService>();
builder.Services.AddTransient<ICacheService, MemoryCacheService>(); 
//builder.Services.AddTransient<Func<CacheTech, ICacheService>>
//(serviceProvider => key =>
//{
//    switch (key)
//    {
//        case CacheTech.Memory:
//            return serviceProvider.GetService<MemoryCacheService>();
//        case CacheTech.Redis:
//            return serviceProvider.GetService<RedisCacheService>();
//        default:
//            return serviceProvider.GetService<MemoryCacheService>();
//    }
//});






builder.Services.AddControllers().AddFluentValidation();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IValidator<RegisterUserModel>, RegisterUserModelValidator>();
builder.Services.AddScoped<IValidator<RequestParams>, RequestParamsValidator>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddHttpContextAccessor();





builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var scope = app.Services.CreateScope();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
