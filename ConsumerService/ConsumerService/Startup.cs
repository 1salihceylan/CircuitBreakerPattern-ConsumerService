using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsumerService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private void Log(string message, ConsoleColor consoleColor)
        {
            Console.BackgroundColor = consoleColor;
            Console.WriteLine($"{DateTime.Now:hh:mm:ss.fffff}\t{message}");
            throw new Exception(message);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //for all errors(tüm hata tipleri için)

            //services.AddHttpClient("producerService", c => { c.BaseAddress = new Uri("http://localhost:5000"); })
            //    .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(
            //handledEventsAllowedBeforeBreaking: 3,
            //durationOfBreak: TimeSpan.FromMinutes(1),
            //onBreak: (_, duration) => Log($"Circuit tripped. Circuit is open and requests won't be allowed through for duration={duration}", ConsoleColor.DarkRed),
            //onReset: () => Log("Circuit closed. Requests are now allowed through", ConsoleColor.DarkGreen),
            //onHalfOpen: () => Log("Circuit is now half-opened and will test the service with the next request", ConsoleColor.DarkYellow)
            //)
            //    );





            //policy is formed according to error type(hata tipine göre policy þekillendiriliyor)


            var policy = Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.RequestTimeout).CircuitBreakerAsync(
           handledEventsAllowedBeforeBreaking: 3,
           durationOfBreak: TimeSpan.FromMinutes(1),
           onBreak: (_, duration) => Log($"Circuit tripped. Circuit is open and requests won't be allowed through for duration={duration}", ConsoleColor.DarkRed),
           onReset: () => Log("Circuit closed. Requests are now allowed through", ConsoleColor.DarkGreen),
           onHalfOpen: () => Log("Circuit is now half-opened and will test the service with the next request", ConsoleColor.DarkYellow)
           );

            services.AddHttpClient("producerService", c => { c.BaseAddress = new Uri("http://localhost:5000"); })
    .AddPolicyHandler(request => policy);



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
