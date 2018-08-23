using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using Xunit;
using WebAPI;

namespace Tests
{
	public class SmokeTests
	{
		[Fact]
		public async Task HomePageNotFound()
		{
			// Arrange
			var webHostBuilder = Program.CreateWebHostBuilder(Array.Empty<string>())
				.UseContentRoot(Path.GetFullPath("../../../../WebAPI"));

			var server = new TestServer(webHostBuilder);
			var client = server.CreateClient();

			// Act
			var response = await client.GetAsync("/");

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async Task StatusFeed()
		{
			var item = new SyndicationItem {Title = "status"};
			item.AddLink(new SyndicationLink(new Uri("http://www.w3.org/2005/Atom")));

			using (var writer = new StringWriter())
			{
				using (var xmlWriter = XmlWriter.Create(writer,
					new XmlWriterSettings() {Async = true, Indent = true, Encoding = Encoding.UTF8}))
				{
					var rssWriter = new RssFeedWriter(xmlWriter);

					await rssWriter.WriteTitle("This is the title");
					await rssWriter.WriteDescription("description");
					await rssWriter.Write(item);
				}

				var text = writer.ToString();
			}
		}
	}
}