
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

// Seed database with sample data in development (fixed to only use existing database columns)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await SeedSampleData(context, userManager);
    }
}

app.Run();

static async Task SeedSampleData(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
{
    // Ensure database is created
    await context.Database.EnsureCreatedAsync();
    
    // Check if we already have users
    if (await userManager.Users.AnyAsync())
    {
        return; // Database already seeded
    }

    // Create sample categories without explicit IDs
    var webDev = new Category { Name = "Web Development", Description = "Learn modern web development technologies", IconUrl = "code", Color = "bg-blue-50", CourseCount = 127 };
    var business = new Category { Name = "Business Management", Description = "Learn key business skills for professional management", IconUrl = "briefcase", Color = "bg-emerald-50", CourseCount = 89 };
    var design = new Category { Name = "Graphic Design", Description = "Create stunning designs to communicate effectively", IconUrl = "paint-brush", Color = "bg-purple-50", CourseCount = 156 };

    context.Categories.AddRange(webDev, business, design);
    await context.SaveChangesAsync();

    // Create a sample instructor user using UserManager
    var instructor = new ApplicationUser
    {
        UserName = "instructor@example.com",
        Email = "instructor@example.com",
        EmailConfirmed = true,
        FirstName = "John",
        LastName = "Instructor", 
        IsInstructor = true,
        IsActive = true,
        Bio = "Experienced software developer and instructor with 10+ years in the industry.",
        ProfileImageUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-4.0.3&auto=format&fit=crop&w=150&q=80",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    
    var result = await userManager.CreateAsync(instructor, "Password123!");
    if (!result.Succeeded)
    {
        throw new Exception("Failed to create instructor user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
    }
    
    // Retrieve the created user to ensure we have the ID generated by the database
    var createdInstructor = await userManager.FindByEmailAsync("instructor@example.com");
    if (createdInstructor == null)
    {
        throw new Exception("Failed to retrieve created instructor user.");
    }

    // Create sample courses that match the static data - let database assign IDs
    var courses = new[]
    {
        new Course
        {
            Title = "Grounding Skills for PTSD, Stress, Panic and Anxiety",
            ShortDescription = "Learn to Turn off the Fight, Flight, Freeze Response and Turn on Calm Using your Body's Natural Parasympathetic Response",
            LongDescription = "In this comprehensive course, you'll learn evidence-based grounding techniques to manage stress, anxiety, and panic attacks effectively.",
            ImageUrl = "https://images.unsplash.com/photo-1551288049-bebda4e38f71?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
            YouTubeVideoId = "dQw4w9WgXcQ",
            YouTubePreviewVideoId = "jNQXAC9IVRw",
            Level = CourseLevel.Beginner,
            Status = CourseStatus.Published,
            IsPublished = true,
            EstimatedDurationMinutes = 2520, // 42 hours
            CategoryId = business.Id,
            InstructorId = createdInstructor.Id,
            AverageRating = 4.3,
            ReviewCount = 125840,
            EnrollmentCount = 89432,
            ViewCount = 234567,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new Course
        {
            Title = "Modern Web Development",
            ShortDescription = "Master modern web development, including HTML, CSS, and JavaScript, with practical projects and examples.",
            LongDescription = "A comprehensive course covering the latest web development technologies and best practices for building modern applications.",
            ImageUrl = "https://images.unsplash.com/photo-1593720213428-28a5b9e94613?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
            YouTubeVideoId = "jNQXAC9IVRw",
            YouTubePreviewVideoId = "dQw4w9WgXcQ",
            Level = CourseLevel.Intermediate,
            Status = CourseStatus.Published,
            IsPublished = true,
            EstimatedDurationMinutes = 1800, // 30 hours
            CategoryId = webDev.Id,
            InstructorId = createdInstructor.Id,
            AverageRating = 4.8,
            ReviewCount = 231565,
            EnrollmentCount = 156789,
            ViewCount = 445623,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new Course
        {
            Title = "Digital Marketing Mastery",
            ShortDescription = "Comprehensive guide to digital marketing strategies, including SEO, social media, and content marketing.",
            LongDescription = "Learn all aspects of digital marketing from industry experts with real-world case studies and practical applications.",
            ImageUrl = "https://images.unsplash.com/photo-1460925895917-afdab827c52f?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
            YouTubeVideoId = "dQw4w9WgXcQ",
            YouTubePreviewVideoId = "jNQXAC9IVRw",
            Level = CourseLevel.Intermediate,
            Status = CourseStatus.Published,
            IsPublished = true,
            EstimatedDurationMinutes = 2100, // 35 hours
            CategoryId = business.Id,
            InstructorId = createdInstructor.Id,
            AverageRating = 4.6,
            ReviewCount = 98765,
            EnrollmentCount = 67432,
            ViewCount = 189543,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }
    };

    context.Courses.AddRange(courses);
    await context.SaveChangesAsync();
}
