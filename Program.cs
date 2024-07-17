using CloudContactApi;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
builder.Services.AddHttpClient();

var apiSettings = builder.Configuration.GetSection("ApiSettings");

builder.Services.AddSingleton(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    var tenantUrl = apiSettings["TenantUrl"];
    var clientId = apiSettings["ClientId"];
    var clientSecret = apiSettings["ClientSecret"];
    var scope = apiSettings["Scope"];
    var grantType = apiSettings["GrantType"];
    return new CloudContactApiService(httpClient, tenantUrl, clientId, clientSecret, scope, grantType);
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
