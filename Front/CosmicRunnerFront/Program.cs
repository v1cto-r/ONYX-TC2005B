using CosmicRunnerFront.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IInicioApiService, InicioApiService>()
    .ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }
    );

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    }
);

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();




app.Run();
