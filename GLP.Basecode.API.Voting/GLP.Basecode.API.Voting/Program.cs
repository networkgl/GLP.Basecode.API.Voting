using GLP.Basecode.API.BLL.Services;
using Microsoft.EntityFrameworkCore;
using GLP.Basecode.API.DAL.DAC.Repository;
using GLP.Basecode.API.BLL.Managers;
using GLP.Basecode.API.Helper;
using GLP.Basecode.API.Voting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using GLP.Basecode.API.DAL.Data;


var builder = WebApplication.CreateBuilder(args);


// Register JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<JwtService>();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer ?? string.Empty,
        ValidAudience = jwtSettings?.Audience ?? string.Empty,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key ?? string.Empty )),
        RoleClaimType = ClaimTypes.Role 
    };

});

//Enabling Authorize in Swaggers
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "GLP Voting System Backend API", Version = "v1" });

    // Add JWT Authentication to Swagger
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter JWT Bearer token only (without 'Bearer' prefix).",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            jwtSecurityScheme,
            Array.Empty<string>()
        }
    });
});




// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure strongly typed MailSettings
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Stateless utility class: safe to use transient
builder.Services.AddTransient<EmailService>();
builder.Services.AddTransient<ImageFileHelper>();
builder.Services.AddTransient<ExceptionMessageHelper>();


// Manager classes: correctly scoped
builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped<PartyListManager>();
builder.Services.AddScoped<CandidateManager>();


// Generic repository: correctly scoped
builder.Services.AddScoped(typeof(BaseRepository<>));


// Register DbContext
builder.Services.AddDbContext<VotingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();//middle ware
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.Run();
