using ExpressMeals.Domains.helpers;
using ExpressMeals.Infrastructures;
using ExpressMeals.Infrastructures.Context;
using ExpressMeals.WebServer;
using ExpressMeals.WebServer.Helpers;
using ExpressMeals.WebServer.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructureConfiguration(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = TokenHelper.Issuer,
            ValidAudience = TokenHelper.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(TokenHelper.Secret)),
            ClockSkew = TimeSpan.Zero
        };

    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("Cors",
        b => b
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true) 
            .AllowCredentials());   
});

builder.Services.AddControllers();
builder.Services.AddTransient<IDataContextSeeder, DataContextSeeder>();
builder.Services.AddSingleton<HttpContextAccessor>();
builder.Services.AddScoped<IFileService, FileLocalService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.Configure<EmailSenderOption>(builder.Configuration.GetSection(nameof(EmailSenderOption)));
builder.Services.AddSingleton<IEmailSender, EmailSenderService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseResponseCaching();

app.UseCors("Cors");

app.UseRouting();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MigrateDatabase().Run();
