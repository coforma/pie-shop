using Microsoft.EntityFrameworkCore;
using PieShop.Api.Middleware;
using PieShop.Core.Services;
using PieShop.Core.StateMachine;
using PieShop.Infrastructure.Data;
using PieShop.Infrastructure.ExternalServices;
using PieShop.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Pie Shop API", Version = "v1" });
});

// Database configuration
// TODO: Move connection string to environment variables or Key Vault
var postgresConnection = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<PieShopDbContext>(options =>
    options.UseNpgsql(postgresConnection));

// MongoDB configuration
// TODO: Move MongoDB connection to environment variables
var mongoConnection = builder.Configuration.GetConnectionString("MongoDB") 
    ?? "mongodb://localhost:27017";
var mongoDatabase = builder.Configuration["MongoDB:Database"] ?? "pieshop";
builder.Services.AddSingleton(new MongoDbContext(mongoConnection, mongoDatabase));

// Register repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();

// Register services
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<OrderStateMachine>();

// Register external service clients
// TODO: Use IHttpClientFactory for better connection management
builder.Services.AddHttpClient<IFruitPickerClient, FruitPickerClient>(client =>
{
    // TODO: Move service URLs to configuration
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:FruitPickerUrl"] 
        ?? "http://localhost:8081");
});

builder.Services.AddHttpClient<IBakerClient, BakerClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:BakerUrl"] 
        ?? "http://localhost:8082");
});

builder.Services.AddHttpClient<IDeliveryClient, DeliveryClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:DeliveryUrl"] 
        ?? "http://localhost:8083");
});

// CORS configuration
// TODO: Configure CORS for production
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // WRONG! Should specify allowed origins
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// TODO: Add authentication middleware here
// app.UseAuthentication();
// app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

// Ensure database is created (not for production!)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PieShopDbContext>();
    dbContext.Database.EnsureCreated(); // TODO: Use migrations instead
}

app.Run();
