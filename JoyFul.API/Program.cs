using JoyFul.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    //Method that allow use to use Oauth 2 Authentication
    // Add security definition for OAuth2 token
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        // Set description for security scheme
        Description = "Please enter token",
        // Specify the location of the token in the request (header)
        In = ParameterLocation.Header,
        // Specify the name of the header containing the token
        Name = "Authorization",
        // Specify the type of security scheme (API key)
        Type = SecuritySchemeType.ApiKey
    });
    //Add Security Info to each Operation For OAuth2
    options.OperationFilter<SecurityRequirementsOperationFilter>();
}
);

//register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("JoyFulStringConnection");
    options.UseSqlServer(connectionString);
});


// Configuring JWT authentication with bearer tokens.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // Setting up token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate the token's issuer
        ValidateIssuer = true,

        // Validate the token's audience
        ValidateAudience = true,

        // Validate the token's lifetime
        ValidateLifetime = true,

        // Validate the token's issuer signing key
        ValidateIssuerSigningKey = true,

        // Specifying the valid issuer obtained from Appsetting.json
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

        // Specifying the valid audience obtained from Appsetting.json
        ValidAudience = builder.Configuration["JwtSettings:Audience"],

        // Creating the issuer signing key from the symmetric key obtained from Appsetting.json
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//enable static files
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
