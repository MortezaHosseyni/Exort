using System.Reflection;
using System.Text;
using Application.Services;
using Application.Tools;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Adapters.Email;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Hubs;

// Builder
var builder = WebApplication.CreateBuilder(args);

// Kestrel
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 150000000;
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});

// Authentication
builder.Services.AddAuthentication(schema =>
    {
        schema.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        schema.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

// Controllers
builder.Services.AddControllers();

// Repositories and Services
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<AppDatabase>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IFileManagerService, FileManagerService>();

builder.Services.AddScoped<IDatabaseConfiguration, DatabaseConfiguration>();

builder.Services.AddScoped<IEmailConfiguration, EmailConfiguration>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ICommunityRepository, CommunityRepository>();
builder.Services.AddScoped<ICommunityService, CommunityService>();

builder.Services.AddScoped<ICommunityPartRepository, CommunityPartRepository>();
builder.Services.AddScoped<ICommunityPartService, CommunityPartService>();

builder.Services.AddScoped<ICommunityChannelRepository, CommunityChannelRepository>();
builder.Services.AddScoped<ICommunityChannelService, CommunityChannelService>();

builder.Services.AddScoped<ICommunityMessageRepository, CommunityMessageRepository>();
builder.Services.AddScoped<ICommunityMessageService, CommunityMessageService>();

builder.Services.AddScoped<IUsersCommunityRepository, UsersCommunityRepository>();
builder.Services.AddScoped<IUsersCommunityService, UsersCommunityService>();

builder.Services.AddScoped<ICommunityRolesService, CommunityRolesService>();

builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<IFriendService, FriendService>();

builder.Services.AddScoped<IPrivateMessageRepository, PrivateMessageRepository>();
builder.Services.AddScoped<IPrivateMessageService, PrivateMessageService>();

// SignalR
builder.Services.AddSignalR();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Exort APIs",
        Description = "This application contains all services and endpoints of the Exort project.",
        Version = "v1",
    });

    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Model Mapper
builder.Services.AddSingleton<IMapper>(sp => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
}).CreateMapper());

// App
var app = builder.Build();

// CORS
app.UseCors(b =>
{
    b.WithOrigins(builder.Configuration.GetSection("Origins").Get<string[]>()!)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});

// Environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Exort APIs");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "Exort APIs";
        c.ConfigObject.AdditionalItems["syntaxHighlight"] = true;
        c.ConfigObject.AdditionalItems["displayOperationId"] = true;
        c.ConfigObject.AdditionalItems["filter"] = true;
    });
}


// Using
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Mapping
app.MapControllers();

// Static Files
app.UseStaticFiles();

// Endpoint & Hubs
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<VoiceChatHub>("/VoiceChat");
});

// Run
app.Run();
