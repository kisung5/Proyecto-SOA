using System;
using System.Net;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using API_service.Classes;
using API_service.Models;
using API_service.Services;
using System.Threading.Tasks;

namespace API_service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // SQL database
            services.AddDbContext<CoreDbContext>(op => op.UseSqlServer(Configuration.GetConnectionString("Database")));

            // NoSQL database
            services.Configure<AnalyzerDatabaseSettings>(Configuration.GetSection(nameof(AnalyzerDatabaseSettings)));
            services.AddSingleton<IAnalyzerDatabaseSettings>(sp => sp.GetRequiredService<IOptions<AnalyzerDatabaseSettings>>().Value);
            services.AddSingleton<DocumentService>();

            // Enable CORS
            services.AddCors();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_service", Version = "v1" });
            });

            // RabbitMQ Publisher
            services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@127.0.0.1:5672"));
            services.AddSingleton<IPublisher>(x => new Publisher(x.GetService<IConnectionProvider>(),
                    "request.dx",
                    ExchangeType.Topic));

            // RabbitMQ Suscriber
            services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(),
                "response.dx",
                "analysis_results",
                "analysis.results",
                ExchangeType.Topic));
            services.AddHostedService<AnalysisResultsCollector>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            };

            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                        {
                            var socketFinishedTcs = new TaskCompletionSource<object>();

                            // The background processor should call socketFinishedTcs.TrySetResult(null) when it has finished with the socket
                            AnalysisResultsCollector.AddSocket(webSocket, socketFinishedTcs);

                            // Wait for the BackgroundSocketProcessor to finish with the socket before concluding the request.
                            await socketFinishedTcs.Task;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    await next();
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // Allow any origin
                .AllowCredentials());               // Allow credentials

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

    }
}
