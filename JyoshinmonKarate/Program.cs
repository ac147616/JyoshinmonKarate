using JyoshinmonKarate.Areas.Identity.Data;
using JyoshinmonKarate.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("JyoshinmonKarateContextConnection") ?? throw new InvalidOperationException("Connection string 'JyoshinmonKarateContextConnection' not found.");;

builder.Services.AddDbContext<JyoshinmonKarateContext>(options => options.UseSqlServer(connectionString));

//Adding identity with roles
builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<JyoshinmonKarateContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    //first I seed the roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);

    // and then the database
    var context = scope.ServiceProvider.GetRequiredService<JyoshinmonKarateContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    DbInitializer.Initialize(context, userManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
