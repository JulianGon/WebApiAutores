using WebApiAutores;

var builder = WebApplication.CreateBuilder(args);

var startup = new StartUp(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

var servLogger = (ILogger<StartUp>)app.Services.GetService(typeof(ILogger<StartUp>));
startup.Configure(app, app.Environment, servLogger);

app.Run();
