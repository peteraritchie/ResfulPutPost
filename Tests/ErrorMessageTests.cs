using Newtonsoft.Json;
using WebAPI.Models;
using Xunit;

namespace Tests
{
	public class ErrorMessageTests
	{
		[Fact]
		public void JsonPropertyNamesAreCorrect()
		{
			var error = new ErrorResponse
			{
				Error = new Error
				{
					Code = "400.1",
					Message = new ErrorMessage
					{
						Text = "Field cannot be empty",
						Targets = new[] {"request.name"},
						Links = new HelpLinks
						{
							Help = new[]
							{
								new HelpLink {Href = "the URL", Name = "the name", Title = "the title"}
							}
						}
					}
				}
			};

			var json = JsonConvert.SerializeObject(error);

			Assert.NotNull(json);
			Assert.Equal(
				"{\"error\":{\"code\":\"400.1\",\"message\":{\"text\":\"Field cannot be empty\",\"targets\":[\"request.name\"],\"_links\":{\"help\":[{\"href\":\"the URL\",\"title\":\"the title\",\"name\":\"the name\"}]}}}}",
				json);
		}
	}
}