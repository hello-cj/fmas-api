using FMAS.API.Data;
using FMAS.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// My 1st service.
builder.Services.AddDbContext<FMASDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// My 2nd service.
builder.Services.AddScoped<JournalEntryService>();

// My 3nd Swagger service.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// My 4th service. for Login and JWT token generation
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

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
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// My 5th service. for accessing current user info in services and controllers
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();

// Enable Backend CORS for development purposes (allowing all origins, headers, and methods)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});



builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthService>();

// Build the app.
var app = builder.Build();

// Middleware for CORS (AFTER BUILD)
app.UseCors("AllowAll");

// Enabling middleware for swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Run the app.
app.Run();
