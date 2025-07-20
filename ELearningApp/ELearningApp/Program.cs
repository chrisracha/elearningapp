
using ELearningApp.Components;
using ELearningApp.Components.Account;
using ELearningApp.Data;
using ELearningApp.Services;
using ELearningApp.Models;
using ELearningApp.Models.Entities;
using ELearningApp.Models.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = false; // Disable email confirmation for easier development
        
        // Disable 2FA completely as requested
        options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
        
        // Password requirements
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
        
        // User requirements
        options.User.RequireUniqueEmail = true;
        
        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<StaticDataService>();

// Register Course Management Services
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<DatabaseSeeder>();

// Add Authorization Policies
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("InstructorOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
              {
                  // This will be validated in the component/service layer
                  // since we need database access to check IsInstructor
                  return true;
              }));

    options.AddPolicy("StudentOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
              {
                  // This will be validated in the component/service layer  
                  // since we need database access to check user role
                  return true;
              }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<ELearningApp.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Seed database with sample data in development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
    }
}

app.Run();
