// FROZEN FILE (owner: Aidan Smith / team lead). Wiring only - each module registers itself in Composition/*.
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
    await DbSeeder.SeedAsync(app.Services);   // safe no-op until Michael-John implements it
}

app.Run();

// Makes the entry point visible to WebApplicationFactory<Program> in integration tests.
public partial class Program { }
