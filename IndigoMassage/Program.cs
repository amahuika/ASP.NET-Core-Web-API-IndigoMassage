using Azure.Storage.Blobs;
using Indigo.BuisnessLogic.Services;
using Indigo.BuisnessLogic.Services.DbSeeder;
using Indigo.DataAccess.Data;
using Indigo.Models.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// connection string options for sql server application db context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),

    sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    }));



// add cors allow any ||| Change later ||| ////////////////////******
string CorsPolicy = "myPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy,
               builder =>
               {
                   builder.WithOrigins("http://localhost:3000").WithHeaders("Content-Type", "Authorization")
                   .AllowAnyMethod();



               });

});





// set up authentication and jwt bearer token 

var Key = builder.Configuration.GetValue<string>("JwtKey:SecretKey");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(u =>
{
    u.RequireHttpsMetadata = false;
    u.SaveToken = true;
    u.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key!)),
        ValidateIssuer = false,
        ValidateAudience = false,

    };
});


// Dependency Injection
// add Blob storage service connection settings
var blobConnectionString = builder.Configuration.GetConnectionString("BlobConnectionString");
builder.Services.AddSingleton(u => new BlobServiceClient(blobConnectionString));


builder.Services.AddScoped<IBlobServices, BlobServices>();

builder.Services.AddScoped<IBookingService, BookingService>();
// mailSender
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddScoped<IDtoMappingToModel, DtoMappingToModel>();


builder.Services.AddSingleton<IConfiguration>(builder.Configuration);





// add identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// add Controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(CorsPolicy);


app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}
else
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}



app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Seed database
var serviceProvider = app.Services;
await DbSeeder.SeedDatabase(serviceProvider);

app.MapControllers();

app.Run();

public partial class Program { }
