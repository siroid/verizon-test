using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Disposal;
using Pets.BL.Parsers;
using Pets.BL.Services;
using Pets.Web.Controllers;
using Pets.Web.Ninject;
using System;
using System.Net.Http;
using System.Threading;

namespace Pets.Web
{
    public class Startup
    {
        private readonly AsyncLocal<Scope> scopeProvider = new AsyncLocal<Scope>();
        private IKernel Kernel { get; set; }
        private object Resolve(Type type) => Kernel.Get(type);
        private object RequestScope(IContext context) => scopeProvider.Value;
        private sealed class Scope : DisposableObject { }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddRequestScopingMiddleware(() => scopeProvider.Value = new Scope());
            services.AddCustomControllerActivation(Resolve);
            services.AddCustomViewComponentActivation(Resolve);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            this.Kernel = this.RegisterApplicationComponents(app, loggerFactory);

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

        private IKernel RegisterApplicationComponents(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // IKernelConfiguration config = new KernelConfiguration();
            var kernel = new StandardKernel();

            // Register application services
            foreach (var ctrlType in app.GetControllerTypes())
            {
                kernel.Bind(ctrlType).ToSelf().InScope(RequestScope);
            }

            // This is where our bindings are configurated
            kernel.Bind<IService>().To<BreedsService>().WhenInjectedInto<BreedsController>().InScope(RequestScope);
            kernel.Bind<ILogger<BreedsController>>().To<Logger<BreedsController>>();
            kernel.Bind<ILoggerFactory>().ToConstant(loggerFactory).InScope(RequestScope);
            kernel.Bind<HttpClient>().ToSelf().InScope(RequestScope);
            kernel.Bind<IBreedsParser>().To<CatBreedsParser>().InScope(RequestScope).
                WithConstructorArgument("authKeyName", Configuration["Parsers:Cats:AuthKeyName"]).
                WithConstructorArgument("authKeyValue", Configuration["Parsers:Cats:AuthKeyValue"]).
                WithConstructorArgument("apiUrl", Configuration["Parsers:Cats:ApiUrl"]);

            kernel.Bind<IBreedsParser>().To<DogBreedsParser>().InScope(RequestScope).
                WithConstructorArgument("apiUrl", Configuration["Parsers:Dogs:ApiUrl"]);

            // Cross-wire required framework services
            kernel.BindToMethod(app.GetRequestService<IViewBufferScope>);

            return kernel;
        }
    }
}
