using FluentValidation;
using FluentValidation.AspNetCore;
using HomeStore.API.DependencyInjection;
using HomeStore.API.Hubs;
using HomeStore.API.Services; // nơi chứa FileService
using HomeStore.API.Swagger;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseUrls("http://0.0.0.0:7257");

// ── DI: Repositories, Services, DbContext, AutoMapper ──
builder.Services.AddApplicationServices(builder.Configuration);

// ── SignalR Notification Services ──
builder.Services.AddScoped<ICartNotificationService, CartNotificationService>();

// ── File upload service ──
builder.Services.AddScoped<IFileService, HomeStore.BLL.Services.FileService>();

// ── SignalR ──
builder.Services.AddSignalR();

// ── Controllers + FluentValidation ──
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ── JWT Authentication ──
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                context.Token = accessToken;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ── Swagger ──
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeStore API", Version = "v1" });

    // Gom tất cả controller vào chung một doc
    c.TagActionsBy(apiDesc =>
    {
        if (apiDesc.GroupName != null)
            return new[] { apiDesc.GroupName }; // hiển thị group name như tag
        return new[] { apiDesc.ActionDescriptor.RouteValues["controller"]! };
    });

    // Cho phép tất cả group hiển thị trong v1
    c.DocInclusionPredicate((docName, apiDesc) => docName == "v1");

    // JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    // File upload
    c.OperationFilter<SwaggerFileOperationFilter>();
});
// ── CORS ──
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ── Database Schema Update & Seed ──
using (var scope = app.Services.CreateScope())
{
    var schemaUpdater = scope.ServiceProvider.GetRequiredService<IDatabaseSchemaUpdater>();
    await schemaUpdater.UpdateSchemaAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseDataSeeder>();
    await seeder.SeedAsync();
}

// ── Middleware Pipeline ──
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeStore API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // thêm để truy cập file upload
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── SignalR Hubs ──
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<OrderHub>("/hubs/orders");
app.MapHub<CartHub>("/hubs/cart");

app.Run();