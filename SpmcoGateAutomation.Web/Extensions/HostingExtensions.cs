using Microsoft.EntityFrameworkCore;
using AutoMapper;
using SpmcoGateAutomation.Web.Middlewares;
using SpmcoBctsDatabaseMigrator.Infrastructure;
using SpmcoGateAutomation.Web.BackgroundServices;
using SpmcoGateAutomation.Contract;
using SpmcoGateAutomation.Application;
using SpmcoGateAutomation.Domain.DomainServices;
using SpmcoGateAutomation.Domain.IRepositories;
using SpmcoGateAutomation.Infrastructure.EFCore.IRepositories;
using SpmcoGateAutomation.Domain.Abstracts;
using SpmcoGateAutomation.Infrastructure.EFCore.Abstracts;
using SpmcoGateAutomation.Common.Configurations;
using SpmcoGateAutomation.ExternalServices;
using Serilog;
using MassTransit;
using SpmcoGateAutomation.Consumer;
using SpmcoGateAutomation.Producer;
using Microsoft.AspNetCore.SignalR;

namespace SpmcoGateAutomation.Web.Extensions
{
    public static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            #region [- Add JsonFile and Environment Setting ans Set Gate Configuration -]

            builder.Host.ConfigureAppConfiguration((hostingcontext, buildere) =>
            {
                buildere.AddJsonFile("appsettings.json");
                buildere.AddEnvironmentVariables();
            });

            GateConfiguration gateConfiguration = new GateConfiguration();
            builder.Configuration.GetSection("GateConfiguration").Bind(gateConfiguration);
            builder.Services.AddSingleton<GateConfiguration>(gateConfiguration);
            #endregion

            #region [- Add DBContext Service -]
            builder.Services.AddDbContext<BctsDatabaseContext>(c => c.UseSqlServer(builder.Configuration.GetConnectionString("BctsDatabaseCnn")));
            #endregion

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region [- Add Auto Mapper Service -]

            builder.Services.AddAutoMapper(typeof(Program));
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            }).CreateMapper();
            builder.Services.AddSingleton(mappingConfig);

            #endregion

            #region [- Configure Services With Attribute -]

            var ConsigneeGateInLightApiUri = builder.Configuration.GetSection("GateConfiguration")["ConsigneeGateInLightApiUri"];
            var ConsigneeGateOutLightApiUri = builder.Configuration.GetSection("GateConfiguration")["ConsigneeGateOutLightApiUri"];
            builder.Services.AddHttpClient("ConsigneeGateInLightApiUri", c => c.BaseAddress = new Uri(ConsigneeGateInLightApiUri));
            builder.Services.AddHttpClient("ConsigneeGateOutLightApiUri", c => c.BaseAddress = new Uri(ConsigneeGateOutLightApiUri));
           
            builder.Services.AddScoped<PrinterService>();
            builder.Services.AddScoped<PlateService>();
            builder.Services.AddTransient<LightService>();
            builder.Services.AddScoped<ContainerService>();
            builder.Services.AddScoped<GateOutLogProducer>();
            builder.Services.AddScoped<GateInLogProducer>();
            
            builder.Services.AddHostedService<GateInService>();
            builder.Services.AddHostedService<GateOutService>();

            builder.Services.AddScoped<ApiExceptionMiddleware>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IContainerInfoRepository, ContainerInfoRepository>();
            builder.Services.AddScoped<IGeneralTableRepository, GeneralTableRepository>();
            builder.Services.AddScoped<IDeliveryPermissionCntrRepository, DeliveryPermissionCntrRepository>();
            builder.Services.AddScoped<IActCntrRepository,ActCntrRepository>();
            builder.Services.AddScoped<IGateLogRepository, GateLogRepository>();
            builder.Services.AddScoped<GeneralTableDomainService>();
            builder.Services.AddScoped<ContainerDomainService>();
            builder.Services.AddScoped<GateDomainService>();
            builder.Services.AddScoped<ActCntrDomainService>();
            builder.Services.AddScoped<IGateApplicationService, GateApplicationService>();

            //builder.Services.AddServicesWithAttributeOfType<ScopedServiceAttribute>();
            //builder.Services.AddServicesWithAttributeOfType<SingletonServiceAttribute>();

            #endregion

            #region [- Serilog Configuration  -]
            var logger = new LoggerConfiguration()
                  .ReadFrom.Configuration(builder.Configuration)
                  .Enrich.FromLogContext()
                  .WriteTo.Console()
                  .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
            #endregion

            #region [- MassTransit Configurations -] 

            var rabbitMQSection = builder.Configuration.GetSection("RabbitMQ");

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<GateOutLogConsumer>();
                x.AddConsumer<GateInLogConsumer>();
                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(config =>
                {

                    config.Host(new Uri(rabbitMQSection["ConnectionUrl"]), h =>
                    {
                        h.Username(rabbitMQSection["Username"]);
                        h.Password(rabbitMQSection["Password"]);
                    });

                    config.ReceiveEndpoint("GateOutLogConsumerQueue", ep =>
                    {
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.ConfigureConsumer<GateOutLogConsumer>(context);
                    });
                    config.ReceiveEndpoint("GateInLogConsumerQueue", ep =>
                    {
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.ConfigureConsumer<GateInLogConsumer>(context);
                    });

                }));
            });
            builder.Services.AddMassTransitHostedService(true);

            #endregion ------------------------------------------------------------------------


            return builder.Build();
        }
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            //var cnn = app.Configuration.GetSection("ConnectionStrings");
            //var rediscnn = cnn["Rabbit"];
            //var rediscnnvalue = app.Configuration["ConnectionStrings:Redis"];

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseMiddleware<ApiExceptionMiddleware>();
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //app.MapControllers();
            app.MapHub<GateInHubSignalRService>("/GateInHub");
            app.MapHub<GateOutHubSignalRService>("/GateOutHub");

            //app.Use(async (context, next) =>
            //{
            //    var hubContext = context.RequestServices
            //                            .GetRequiredService<IHubContext<GateInHubSignalRService>>();

            //    if (next != null)
            //    {
            //        await next.Invoke();
            //    }
            //});

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            return app;
        }
    }
}
