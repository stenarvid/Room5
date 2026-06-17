var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    if (path.StartsWith("/_blazor") ||
        path.StartsWith("/_framework") ||
        path.Contains("."))
    {
        await next();
        return;
    }

    // 1. Skapa en lista med alla godkända host-namn (utan http:// eller snedstreck)
    var allowedHosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "escape-room-4.runasp.net", // Rum 4
        "escape-room-5.runasp.net", // Ditt eget rum (så att omstart/F5 fungerar!)
        "localhost"                 // Bra att ha med under lokal utveckling
    };

    // 2. Kontrollera om Referer finns och är en giltig URL
    if (context.Request.Headers.TryGetValue("Referer", out var refererValue) &&
        Uri.TryCreate(refererValue.ToString(), UriKind.Absolute, out var refererUri))
    {
        // 3. Kolla om den aktuella hosten finns med i din lista
        if (allowedHosts.Contains(refererUri.Host))
        {
            await next();
            return;
        }
    }

    // Om Referer saknas eller inte finns i listan -> Skicka till Rum 1
    context.Response.Redirect("http://room1.runasp.net/");
});

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Room5}/{id?}")
    .WithStaticAssets();


app.Run();
