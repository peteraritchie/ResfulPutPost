using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
#if !DEBUG
#error example code included in non-debug build
#else
	[Route("api/[controller]")]
	public class ExampleController : ControllerBase
	{
		private HttpClient httpClient;

		public ExampleController(IHttpClientFactory httpClientFactory)
		{
			httpClient = httpClientFactory.CreateClient();
		}

		[HttpGet]
		public async Task<IActionResult> GetDefinition([FromQuery] IDictionary<string, string> queryParameters)
		{
			if (!queryParameters.Any())
				return BadRequest();

			var nonThrowingDictionary = (IDictionary) queryParameters;
			string word = (string)(nonThrowingDictionary["word"] ?? nonThrowingDictionary["w"] ?? nonThrowingDictionary["lemma"]);
			if (string.IsNullOrWhiteSpace(word))
				return BadRequest();

			string url = $"https://googledictionaryapi.eu-gb.mybluemix.net/?define={word}";
			var responseMessage = await httpClient.GetAsync(url);
			if (!responseMessage.IsSuccessStatusCode)
			{
				return new ObjectResult("{}") // TODO:
				{
					StatusCode = (int?) HttpStatusCode.BadGateway
				};
			}

			return Ok(await responseMessage.Content.ReadAsStringAsync());
		}
	}
#endif // DEBUG
}