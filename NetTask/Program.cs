using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetTask;
using NetTask.Utilities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 设置JSON序列化选项，使对象属性名原样输出
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// 获取appsettings.json配置文件对象
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

// 注入DbContext
builder.Services.AddDbContext<NetTaskDbContext>(options =>
    options.UseSqlServer(configBuilder.GetConnectionString("NetTask")));

// Swagger + JWT授权配置
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 定义安全方案
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "请输入带有Bearer的Token，形如 \"Bearer {Token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header
    });
    // 指定方案应用范围
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "readAccess", "writeAccess" }
        }
    });
});

// 配置JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
    };
});

// 添加用于访问Header数据的依赖项
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// 添加TokenService依赖项
builder.Services.AddTransient<ITokenService, TokenService>();

// 导入权限管理整套自定义类并依赖注入
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddTransient<IAuthorizationHandler, PermissionRequirementHandler>();

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 启用JWT
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
