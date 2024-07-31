using Umbraco13Demo.Helpers;
using Umbraco13Demo.Hubs;
using Umbraco13Demo.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(option =>
{
    if (builder.Environment.IsDevelopment())
    {
        option.EnableDetailedErrors = true;
    }
});

//builder.Services.AddScoped(typeof(UserManager<MemberIdentityUser>));

builder.Services.AddScoped<IBackofficeUserAccessor, BackofficeUserAccessor>();
builder.Services.AddScoped<IContentProvider, ContentProvider>();
builder.Services.AddSingleton<NotificationHub>();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

app.UseHttpsRedirection();

//IBackOfficeUserManager

//IUmbracoUserManager

app.MapHub<NotificationHub>("/notificationHub");

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseInstallerEndpoints();
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();



