using CloudContactApi.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var apiSettings = builder.Configuration.GetSection("ApiSettings");

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiSettings["TenantUrl"]);
});

builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("ApiClient"); // Используем именованный HttpClient
    var tenantUrl = apiSettings["TenantUrl"];
    var clientId = apiSettings["ClientId"];
    var clientSecret = apiSettings["ClientSecret"];
    var scope = apiSettings["Scope"];
    var grantType = apiSettings["GrantType"];
    return new AuthenticationService(httpClient, tenantUrl, clientId, clientSecret, scope, grantType);
});

builder.Services.AddSingleton<IRecordingsService, RecordingsService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("ApiClient"); // Используем именованный HttpClient
    var tenantUrl = apiSettings["TenantUrl"];
    var authService = sp.GetRequiredService<IAuthenticationService>();
    return new RecordingsService(httpClient, tenantUrl, authService);
});

builder.Services.AddSingleton<IMetadataService, MetadataService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("ApiClient"); // Используем именованный HttpClient
    var tenantUrl = apiSettings["TenantUrl"];
    var authService = sp.GetRequiredService<IAuthenticationService>();
    return new MetadataService(httpClient, tenantUrl, authService);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
