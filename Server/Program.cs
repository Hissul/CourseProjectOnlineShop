using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Server.Data;
using Server.Services;
using System.Text;


Log.Logger = new LoggerConfiguration ()
    .ReadFrom.Configuration (new ConfigurationBuilder ()
        .AddJsonFile ("appsettings.json")
        .Build ())
    .Enrich.FromLogContext ()
    .CreateLogger ();




try {
    Log.Information ("Starting up");

    WebApplicationBuilder builder = WebApplication.CreateBuilder (args);
    builder.Host.UseSerilog ();

    string? connectionString = builder.Configuration.GetConnectionString ("DefaultConnection");
    string? keyString = builder.Configuration["Jwt:Secret"];
    string? issuer = builder.Configuration["Jwt:Issuer"];
    string? audience = builder.Configuration["Jwt:Audience"];

    byte[] key = Encoding.UTF8.GetBytes (keyString);

    builder.Services.AddScoped<AuthService> ();
    builder.Services.AddScoped<StoreService> ();
    builder.Services.AddScoped<CartService> ();
    builder.Services.AddScoped<OrderService> ();
    builder.Services.AddScoped<ProductService> ();
    builder.Services.AddScoped<UserService> ();

    builder.Services.AddCors (options => {
        options.AddPolicy ("AllowAll",
            policy => policy
                .AllowAnyOrigin ()
                .AllowAnyMethod ()
                .AllowAnyHeader ());
    });

    builder.Services.AddDbContext<ApplicationDbContext> (options => options.UseSqlServer (connectionString));

    builder.Services.Configure<IdentityOptions> (options => {
        options.SignIn.RequireConfirmedAccount = false;
    });

    builder.Services.AddIdentity<ApplicationUser, IdentityRole> ()
        .AddEntityFrameworkStores<ApplicationDbContext> ()
        .AddDefaultTokenProviders ();

    builder.Services.AddControllers ();

    builder.Services.AddAuthentication (options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer (options => {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey (key),
        };
    });

    builder.Services.AddAuthorization (options => {
        options.AddPolicy ("RequireAdministratorRole", policy => policy.RequireRole ("Admin"));
    });

    WebApplication app = builder.Build ();

    //using (var scope = app.Services.CreateScope ()) {
    //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext> ();
    //    dbContext.Database.EnsureDeleted (); // ������� ���� ������
    //    dbContext.Database.EnsureCreated (); // ������� ���� ������ ������
    //}

    app.UseCors ("AllowAll");

    //using (IServiceScope scope = app.Services.CreateScope ()) {
    //    IServiceProvider services = scope.ServiceProvider;
    //    await CreateRole (services);
    //}

    await using var scope = app.Services.CreateAsyncScope ();
    await CreateRole (scope.ServiceProvider);

    app.UseAuthentication ();
    app.UseAuthorization ();

    app.MapControllers ();

    app.Run ();
}
catch (Exception ex) {
    Log.Fatal (ex, "Application start-up failed");
}
finally {
    Log.CloseAndFlush ();
}


async Task CreateRole (IServiceProvider serviceProvider) {
    RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>> ();
    UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>> ();

    string[] roles = ["Admin", "User"];

    foreach (string role in roles) {
        if (!await roleManager.RoleExistsAsync (role)) {
            await roleManager.CreateAsync (new IdentityRole (role));
        }
    }

    string adminEmail = "admin@example.com";
    ApplicationUser? adminUser = await userManager.FindByEmailAsync (adminEmail);

    if (adminUser == null) {
        ApplicationUser newAdmin = new ApplicationUser {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "I'm the Boss"
        };
        await userManager.CreateAsync (newAdmin, "Admin123!");
        await userManager.AddToRoleAsync (newAdmin, "Admin");
    }
}

