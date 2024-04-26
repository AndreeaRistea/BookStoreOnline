using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proiect.Search;
using Proiect_CE.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();
builder.Services.AddDbContext<BookStoreContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("BookStoreDb")));
builder.Services.AddScoped<BookSearchEngine>();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<BookStoreContext>();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                   .AddRoles<IdentityRole>()
                  .AddEntityFrameworkStores<BookStoreContext>();
//builder.Services.AddIdentity<User, IdentityRole>//(options => options.SignIn.RequireConfirmedAccount = true).
//                    (options =>
//                    {
//                        options.User.RequireUniqueEmail = false;
//                    })
//                    .AddEntityFrameworkStores<BookStoreContext>()
//                    .AddDefaultTokenProviders();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
