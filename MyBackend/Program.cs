using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.Extensions;
using MyBackend.Middlewares;
using MyBackend.Settings;

var builder = WebApplication.CreateBuilder(args);

// 1. Core Framework Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// 2. Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Custom Extension Methods
builder.Services.AddApplicationServices();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddRateLimiterConfig();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// 4. Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 1. Connect appsettings.json section to the CloudinarySettings class
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

var firebasePath = Path.Combine(Directory.GetCurrentDirectory(), "firebase-config.json");
if (File.Exists(firebasePath))
    FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile(firebasePath) });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TokenBlacklistMiddleware>();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();