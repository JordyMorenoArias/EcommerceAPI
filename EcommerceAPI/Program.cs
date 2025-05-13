using EcommerceAPI.Data;
using EcommerceAPI.Middlewares;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Address;
using EcommerceAPI.Services.Address.Interfaces;
using EcommerceAPI.Services.Auth;
using EcommerceAPI.Services.Auth.Interfaces;
using EcommerceAPI.Services.Cart;
using EcommerceAPI.Services.Cart.Interfaces;
using EcommerceAPI.Services.CartItem;
using EcommerceAPI.Services.CartItem.Interfaces;
using EcommerceAPI.Services.Infrastructure;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Order;
using EcommerceAPI.Services.Order.Interfaces;
using EcommerceAPI.Services.OrderItem;
using EcommerceAPI.Services.OrderItem.Interfaces;
using EcommerceAPI.Services.Payment;
using EcommerceAPI.Services.Payment.Interfaces;
using EcommerceAPI.Services.PaymentGateway;
using EcommerceAPI.Services.PaymentGateway.Interfaces;
using ProductService = EcommerceAPI.Services.Product.ProductService;
using EcommerceAPI.Services.Product.Interfaces;
using EcommerceAPI.Services.Security;
using EcommerceAPI.Services.Security.Interfaces;
using EcommerceAPI.Services.User;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;
using Elastic.Clients.Elasticsearch;
using EcommerceAPI.Services.ElasticService.Interfaces;
using EcommerceAPI.Services.ElasticProductService;
using EcommerceAPI.Services.Category.Interfaces;
using EcommerceAPI.Services.Category;
using EcommerceAPI.Services.ElasticService;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Tag;
using EcommerceAPI.Services.Tag;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen( options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "EcommerceAPI", 
        Version = "v1",
        Description = "A simple e-commerce API",
    });
});

builder.Services.AddDbContext<EcommerceContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

builder.Services.AddSingleton(sp =>
{
    var elasticSearchUri = builder.Configuration["ElasticSearch:Uri"];

    if (string.IsNullOrEmpty(elasticSearchUri))
    {
        throw new Exception("ElasticSearch:Uri is missing in appsettings.json");
    }

    var settings = new ElasticsearchClientSettings(new Uri(elasticSearchUri));
    return new ElasticsearchClient(settings);
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IProductTagRepository, ProductTagRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Singleton and Infrastructure Services
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IOAuthProviderService, GoogleAuthService>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// Domain Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentGatewayService, MockPaymentGatewayService>();
builder.Services.AddScoped<IElasticProductService, ElasticProductService>();
builder.Services.AddScoped<IElasticTagService, ElasticTagService>();

// Elasticsearch Generic Services
builder.Services.AddSingleton<IElasticGenericService<ProductElasticDto>>(sp =>
    new ElasticGenericService<ProductElasticDto>(
        sp.GetRequiredService<ElasticsearchClient>(),
        "products"
    ));

builder.Services.AddSingleton<IElasticGenericService<TagDto>>(sp =>
    new ElasticGenericService<TagDto>(
        sp.GetRequiredService<ElasticsearchClient>(),
        "tags"
    ));

//  JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("Jwt:Key is missing in appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully!");
            return Task.CompletedTask;
        }
    };
});

// Entity Framework Core Identity
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EcommerceContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.Run();