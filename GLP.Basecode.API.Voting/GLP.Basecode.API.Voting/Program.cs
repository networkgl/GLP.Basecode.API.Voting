using GLP.Basecode.API.Voting.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using GLP.Basecode.API.Voting.Repository;
using GLP.Basecode.API.Voting.Manager;
using GLP.Basecode.API.Voting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure strongly typed MailSettings
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Register application services
builder.Services.AddTransient<MailManager>();
builder.Services.AddScoped<AccountManager>();

// Register repositories
builder.Services.AddScoped(typeof(BaseRepository<>));// generic repo

// Register DbContext
builder.Services.AddDbContext<VotingAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
