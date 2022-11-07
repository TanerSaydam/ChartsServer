using ChartsServer.Context;
using ChartsServer.Hubs;
using ChartsServer.Models;
using ChartsServer.Subscription;
using ChartsServer.Subscription.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChartDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddSingleton<DatabaseSubscription<Satis>>();
builder.Services.AddSingleton<DatabaseSubscription<Personel>>();
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(b => b.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(b => true));
});
builder.Services.AddSignalR();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDatabaseSubscription<DatabaseSubscription<Satis>>("Satislar");
app.UseDatabaseSubscription<DatabaseSubscription<Personel>>("Personeller");
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<SatisHub>("/satishub");

app.Run();
