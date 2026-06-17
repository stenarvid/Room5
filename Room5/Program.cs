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
    var allowedReferrer = new Uri("http://escape-room-4.runasp.net/");

    if (!context.Request.Headers.TryGetValue("Referer", out var refererValue) ||
        !Uri.TryCreate(refererValue.ToString(), UriKind.Absolute, out var refererUri) ||
        refererUri.Host != allowedReferrer.Host)
    {
        context.Response.Redirect("http://room1.runasp.net/");
        return;
    }
    await next();
});

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Room5}/{id?}")
    .WithStaticAssets();


app.Run();
