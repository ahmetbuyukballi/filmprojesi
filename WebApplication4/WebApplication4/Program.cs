using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using static WebApplication4.Data.VeriTabani;
using static WebApplication4.Data.VeriTabani.FilmContext;// VeriTabani sýnýfýnýzýn namespace'ini ekleyin

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Auth0
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect("Auth0", options =>
{
    options.Authority = $"https://{builder.Configuration["Auth0:dev-3sv0egof53yu1s33.us.auth0.com"]}";
    options.ClientId = builder.Configuration["Auth0:znfKRde2Ps8jcTgGF9fYVTnXo4W0H8bx"];
    options.ClientSecret = builder.Configuration["Auth0:pR5uyyir_N84wP8DPWm3oB50gdcWzv5K6c99QDxGedFpjRhVlnvc3jCAJDW7VB_h"];
    options.ResponseType = "code";

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.CallbackPath = new PathString("/callback");
    options.ClaimsIssuer = "Auth0";

    options.SaveTokens = true;

    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProviderForSignOut = (context) =>
        {
            var logoutUri = $"https://{builder.Configuration["Auth0:dev-3sv0egof53yu1s33.us.auth0.com"]}/v2/logout?client_id={builder.Configuration["Auth0:znfKRde2Ps8jcTgGF9fYVTnXo4W0H8bx"]}";

            var postLogoutUri = context.Properties.RedirectUri;
            if (!string.IsNullOrEmpty(postLogoutUri))
            {
                if (postLogoutUri.StartsWith("/"))
                {
                    var request = context.Request;
                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                }
                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
            }

            context.Response.Redirect(logoutUri);
            context.HandleResponse();

            return Task.CompletedTask;
        }
    };
});

// Add DbContext
builder.Services.AddDbContext<VeriTabani.FilmContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
