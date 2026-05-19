using FMAS.API.Data;
using FMAS.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// My 1st service.
builder.Services.AddDbContext<FMASDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddControllers();

// My 2nd service.
builder.Services.AddScoped<JournalEntryService>();

// My 3rd Swagger service.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FMAS API",
        Version = "v1"
    });

    // JWT Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

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
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(key),

        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    };
});

// My 5th service. for accessing current user info in services and controllers
builder.Services.AddScoped<DashboardService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();

// My 6th service. for user management (create, read, update users)
builder.Services.AddScoped<UserManagementService>();

// My 7th service. for account management (create,read, update accounts)
builder.Services.AddScoped<AccountService>();

// My 8th service. for ledger retrieval
builder.Services.AddScoped<LedgerService>();

// My 9th service. for trial balance retrieval
builder.Services.AddScoped<TrialBalanceService>();

// My 10th service. for financial statements retrieval
builder.Services.AddScoped<FinancialStatementService>();

// My 11th service. for audit logging
builder.Services.AddScoped<AuditService>();

// My 12th service. for AP/AR invoice and customer management
builder.Services.AddScoped<APInvoiceService>();
builder.Services.AddScoped<ARInvoiceService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<VendorService>();

// My 13th service. for AP/AR payment management
builder.Services.AddScoped<ARPaymentService>();
builder.Services.AddScoped<APPaymentService>();

// My 14th service. for budget management
builder.Services.AddHttpContextAccessor();

// My last service. for super admin functionalities (managing organizations and secret account)
builder.Services.AddScoped<SuperAdminService>();

// hide the bug HAHAAHHAAHAHHA
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

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

// Seed initial data on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FMASDbContext>();
    
    Console.WriteLine("SEED STARTING...");

    DbSeeder.SeedRoles(context);
    Console.WriteLine("ROLES DONE");

    DbSeeder.SeedSuperAdmin(context);
    Console.WriteLine("SECRET ACCOUNT DONE ADDED");
    //DbSeeder.SeedFinanceData(context);

    //Console.WriteLine("🔥 FINANCE DONE");
}

// Middleware for CORS (AFTER BUILD)
app.UseCors("AllowAll");

// Enabling middleware for swagger
    app.UseSwagger();
    app.UseSwaggerUI();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Render fix
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Add($"http://0.0.0.0:{port}");

// Run the app.
app.Run();
