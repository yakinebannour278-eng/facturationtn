using Microsoft.EntityFrameworkCore;
using FacturationApp.Components;
using FacturationApp.Data;
using FacturationApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=facturation.db"));

builder.Services.AddScoped<IClientService,    ClientService>();
builder.Services.AddScoped<IProduitService,   ProduitService>();
builder.Services.AddScoped<IParametreService, ParametreService>();
builder.Services.AddScoped<IFactureService,   FactureService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
