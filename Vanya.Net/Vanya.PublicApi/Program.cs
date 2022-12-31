using Autofac;
using Autofac.Extensions.DependencyInjection;
using Vanya.Application.Extensions;
using Vanya.PublicApi.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    //Register modules and services
    builder.RegisterModule<Vanya.Application.Module>();
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(conf =>
    {
        conf.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHubF<MarketTrackingHub>("market");

app.Run();
