using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Abstractions;
using WebAPI.Controllers;
using WebAPI.Models;
using Xunit;

namespace Tests
{
	public class RequestControllerTests
	{
		[Fact/*[RFC2616.10.4.1]*/]
		public void PostRequestWithNullRequestResultsInBadRequest()
		{
			var mock = new Mock<ICreateProductRequestJournal>();
			mock.Setup(m => m.Book(It.IsAny<CreateProductRequest>())).Returns(new Guid("12345678901234567890123456789012"));
			var controller = new CreateProductRequestsController(mock.Object);

			var result = controller.Post(null);

			var acceptedResult = Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact/*ALLAMARAJU.1.10*/]
		public void PostRequestResultsInAcceptedWithValidGuid()
		{
			var mock = new Mock<ICreateProductRequestJournal>();
			Product product;
			mock
				.Setup(m => m.Book(It.IsAny<CreateProductRequest>()))
				.Callback((CreateProductRequest m) => product = m.Product)
				.Returns(new Guid("12345678901234567890123456789012"));

			var controller = new CreateProductRequestsController(mock.Object);

			var result = controller.Post(new CreateProductRequest {Product = new Product {Name = "the name", Price = "66.66"}});

			var acceptedResult = Assert.IsType<AcceptedAtActionResult>(result);
			Assert.True(acceptedResult.RouteValues.ContainsKey("id"));
			Guid guid = Assert.IsType<Guid>(acceptedResult.RouteValues["id"]);
			Assert.NotEqual(Guid.Empty, guid);
		}
	}
}