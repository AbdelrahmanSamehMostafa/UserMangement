using com.gbg.modules.utility.Extentions;
using UserManagmentRazor.Extentions;

var builder = WebApplication.CreateBuilder(args);

// Register HttpClient for API consumption
ServiceRegiteration.RegisterService(builder);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Make the cookie essential
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session middleware before defining the map for root
app.UseSession();
app.UseMiddleware<TokenValidationMiddleware>();

// Redirect from root to login page
app.MapGet("/", (HttpContext context) =>
{
    context.Response.Redirect("/Authentication/Login");
    return Task.CompletedTask;
});

// Map Razor Pages
app.MapRazorPages();

app.UseAuthorization();

app.Run();