using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tests.Common;
using WebAPI.Controllers;
using Xunit;

namespace Tests
{
	public class ExampleControllerTests
	{
		[Fact]
		public void MissingParameters400Result()
		{
			var mockFactory = new Mock<IHttpClientFactory>();
			mockFactory
				.Setup(m => m.CreateClient(It.IsAny<string>()))
				.Returns(new HttpClient(new StubMessageHandler("{\"result\": \"I don't know\"}")));
			var controller = new ExampleController(mockFactory.Object);
			var resultTask = controller.GetDefinition(new Dictionary<string, string>());
			var result = resultTask.Result;
			var badRequestResult = Assert.IsType<BadRequestResult>(result);
			// if particular response body is required, check it here
		}

		[Fact]
		public void MissingParameter400Result()
		{
			var mockFactory = new Mock<IHttpClientFactory>();
			mockFactory
				.Setup(m => m.CreateClient(It.IsAny<string>()))
				.Returns(new HttpClient(new StubMessageHandler("{\"result\": \"I don't know\"}")));
			var controller = new ExampleController(mockFactory.Object);
			var resultTask = controller.GetDefinition(new Dictionary<string, string>
			{
				// ReSharper disable once StringLiteralTypo
				["gibberashish"] = "gibberashish"
			});
			var result = resultTask.Result;
			var badRequestResult = Assert.IsType<BadRequestResult>(result);
			// if particular response body is required, check it here
		}

		[Fact]
		public void CorrectParameter200Result()
		{
			var mockFactory = new Mock<IHttpClientFactory>();
			mockFactory
				.Setup(m => m.CreateClient(It.IsAny<string>()))
				.Returns(new HttpClient(new StubMessageHandler("{\"result\": \"I don't know\"}")));
			var controller = new ExampleController(mockFactory.Object);
			var resultTask = controller.GetDefinition(new Dictionary<string, string> {{"w", "set"}});
			var result = resultTask.Result;
			var okResult = Assert.IsType<OkObjectResult>(result);
			// if particular response body is required, check it here
		}

		[Fact]
		public void BadUpstreamResponse502Result()
		{
			var mockFactory = new Mock<IHttpClientFactory>();
			mockFactory
				.Setup(m => m.CreateClient(It.IsAny<string>()))
				.Returns(new HttpClient(new StubMessageHandler("{\"result\": \"I don't know\"}", HttpStatusCode.BadRequest)));
			var controller = new ExampleController(mockFactory.Object);
			var resultTask = controller.GetDefinition(new Dictionary<string, string> {{"w", "set"}});
			var result = resultTask.Result;
			var objectResult = Assert.IsType<ObjectResult>(result);
			Assert.Equal((int)HttpStatusCode.BadGateway, objectResult.StatusCode);
			// if particular response body is required, check it here
		}
	}

	// something to provide canned answers/inputs
}