using Umbraco13Demo.Helpers;
using Umbraco13Demo.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();

builder.Services.AddControllersWithViews();


//builder.Services.AddScoped(typeof(UserManager<MemberIdentityUser>));

builder.Services.AddScoped<IBackofficeUserAccessor, BackofficeUserAccessor>();
builder.Services.AddScoped<IContentProvider, ContentProvider>();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

app.UseHttpsRedirection();

//IBackOfficeUserManager

//IUmbracoUserManager

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



