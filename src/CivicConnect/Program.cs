// FROZEN FILE. Wiring only - each module registers itself in Composition/*.
// If you add a service, register it in YOUR module's Composition file, never here.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddCivicConnectData(builder.Configuration);
builder.Services.AddCivicConnectSecurity();
builder.Services.AddCivicConnectAudit();
builder.Services.AddCivicConnectRequests();
builder.Services.AddCivicConnectReporting();

var app = builder.Build();

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
    pattern: "{controller=ServiceRequests}/{action=Create}/{id?}");

if (app.Environment.IsDevelopment())
{
    await DbSeeder.SeedAsync(app.Services);   // Development only: applies migrations AND creates demo users
}
else if (app.Environment.IsStaging() && app.Configuration.GetValue<bool>("SeedDemoData"))
{
    // Staging only, and only when SeedDemoData=true is set (environment variable): migrate AND create demo users.
    // Never runs in Production.
    await DbSeeder.SeedAsync(app.Services);
}
else
{
    // Staging / Production without demo data: create or update the schema on startup. The in-memory test provider
    // is not relational, so the check below skips it and the "Testing" environment is unaffected.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational())
    {
        await db.Database.MigrateAsync();
    }
}

app.Run();

// Makes the entry point visible to WebApplicationFactory<Program> in integration tests.
public partial class Program { }
