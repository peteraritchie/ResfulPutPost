using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

[assembly: InternalsVisibleTo("Tests")]

namespace WebAPI
{
	public class Program
	{
		[ExcludeFromCodeCoverage]
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		[ExcludeFromCodeCoverage]
		public static IWebHost BuildWebHost(string[] args) =>
			CreateWebHostBuilder(args).Build();

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}