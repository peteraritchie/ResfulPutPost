using System.Diagnostics.CodeAnalysis;
using Domain;
using Domain.Abstractions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		[ExcludeFromCodeCoverage/*TODO:if configuration added, remove this attribute*/]
		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().AddXmlDataContractSerializerFormatters();
			services.AddSingleton<IProductCatalog>(new ProductCatalog());
		}

		[ExcludeFromCodeCoverage]
		[UsedImplicitly]
		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

			app.UseMvc();
		}
	}
}