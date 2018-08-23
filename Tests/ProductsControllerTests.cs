using System;
using System.Collections.Generic;
#if !PUT_CREATE_SUPPORTED
using System.Linq;
using System.Net;
#endif
using Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;
using WebAPI.Models;
using Xunit;
using Product = Domain.Primitivies.Product;

namespace Tests
{
	public class ProductsControllerTests
	{
		[Fact/*[RFC2616.10.2.1]*/]
		public void GetProductsResultsInOk()
		{
			var mock = new Mock<IProductCatalog>();
			mock.SetupGet(m => m.Products).Returns(new[] { new Product() });
			var controller = new ProductsController(mock.Object);
			var result = controller.Get();
			var okResult = Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public void GetProductsResultsInCorrectProductCount()
		{
			var mock = new Mock<IProductCatalog>();
			mock.SetupGet(m => m.Products).Returns(new[] { new Product() });
			var controller = new ProductsController(mock.Object);
			var result = controller.Get();
			var okResult = Assert.IsType<OkObjectResult>(result);
			var sequence = Assert.IsAssignableFrom<IEnumerable<WebAPI.Models.Product>>(okResult.Value);
			Assert.Single(sequence);
		}

		// Get Response Content
		[Fact/*[RFC2616.10.2.1]*/]
		public void GetProductWithValidProductIdResultsInOk()
		{
			var mock = new Mock<IProductCatalog>();
			mock.Setup(m => m.FindById(It.Is<string>(s=>s=="1"))).Returns(new Product("TestName", 10.0m));
			var controller = new ProductsController(mock.Object);
			var result = controller.Get("1");
			var okResult = Assert.IsType<OkObjectResult>(result);

			var product = Assert.IsType<WebAPI.Models.Product>(okResult.Value);
			Assert.Equal("TestName", product.Name);
			Assert.Equal("10.0", product.Price);
		}

		[Fact/*[RFC2616.10.4.5]*/]
		public void GetProductWithInvalidProductIdResultsInNotFound()
		{
			var mock = new Mock<IProductCatalog>();
			mock.Setup(m => m.FindById(It.IsAny<string>())).Returns(default(Product));
			var controller = new ProductsController(mock.Object);
			var result = controller.Get("1");
			var notFoundResult = Assert.IsType<NotFoundResult>(result); // TODO: NotFoundObjectResult
		}

		[Theory/*[RFC2616.10.4.1]*/]
		[InlineData(default(string))]
		[InlineData("")]
		// TODO: other invalid types if necessary
		public void GetProductWithMalformedProductIdResultsInBadRequest(string id)
		{
			var mock = new Mock<IProductCatalog>();
			var controller = new ProductsController(mock.Object);
			var result = controller.Get(id);
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);

			// TODO: Error object
		}

		[Theory/*[RFC2616.10.4.1]*/]
		[InlineData(default(string))]
		[InlineData("")]
		// TODO: other invalid types if necessary
		public void PutProductWithMalformedProductIdResultsInBadRequest(string id)
		{
			var mock = new Mock<IProductCatalog>();
			var controller = new ProductsController(mock.Object);
			var result = controller.Put(id, new WebAPI.Models.Product());
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);

			// TODO: Error object
		}

		[Theory/*[RFC2616.10.4.1]*/]
		[InlineData(default(WebAPI.Models.Product))]
		// TODO: other invalid types if necessary
		public void PutProductWithMalformedProductResultsInBadRequest(WebAPI.Models.Product product)
		{
			var mock = new Mock<IProductCatalog>();
			var controller = new ProductsController(mock.Object);
			var result = controller.Put("1", product);
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);

			// TODO: Error object
		}

		// Put update, NoContent
		[Fact/*[RFC2616.9.6]*/]
		public void PutUpdateProductIdResultsInNoContent()
		{
			var mock = new Mock<IProductCatalog>();
			mock.Setup(m => m.FindById(It.Is<string>(s => s == "2"))).Returns(new Product("TestName", 10.0m));
			Product updatedProduct = null;
			mock.Setup(m => m.ReplaceExistingProduct(It.Is<string>(s => s == "2"), It.IsAny<Product>())).Callback((string i, Product p) => { updatedProduct = p; });
			var controller = new ProductsController(mock.Object);
			var result = controller.Put("2", new WebAPI.Models.Product {Name = "name 2", Price = "10.1"});
			var noContentResult = Assert.IsType<NoContentResult>(result);

			// todo: validate result content
			//var product = Assert.IsType<WebAPI.Models.Product>(noContentResult.Value);
			//Assert.Equal("name 2", product.Name);
			//Assert.Equal("10.1", product.Price);
			Assert.NotNull(updatedProduct);
		}

#if PUT_CREATE_SUPPORTED
		[Fact/*[RFC2616.9.6]*/]
		public void PutProductWithNewProductIdResultsInCreate()
		{
			var mock = new Mock<IProductCatalog>();
			mock.Setup(m => m.FindById(It.Is<string>(s => s == "2"))).Returns(default(Product));
			Product newProduct = null;
			mock.Setup(m => m.AddNewProduct(It.Is<string>(s => s == "2"), It.IsAny<Product>()))
				.Callback((string i, Product p) => { newProduct = p; });

			var controller = new ProductsController(mock.Object)
			{
				ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
			};
			var result = controller.Put("2", new WebAPI.Models.Product {Name = "name 2", Price = "10.1"});

			var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);

			var product = Assert.IsType<WebAPI.Models.Product>(createdAtRouteResult.Value);
			Assert.Equal("name 2", product.Name);
			Assert.Equal("10.1", product.Price);
			Assert.NotNull(newProduct);
		}

#else

		[Fact/*[RFC2616.9.6]*/]
		public void PutProductWithNewProductIdResultsInCreate()
		{
			var mock = new Mock<IProductCatalog>();
			mock.Setup(m => m.FindById(It.Is<string>(s => s == "2"))).Returns(default(Product));

			var controller = new ProductsController(mock.Object)
			{
				ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
			};
			var result = controller.Put("2", new WebAPI.Models.Product {Name = "name 2", Price = "10.1"});

			var statusResult = Assert.IsType<StatusCodeResult>(result);
			Assert.Equal((int) HttpStatusCode.MethodNotAllowed, statusResult.StatusCode);
			Assert.True(controller.Response.Headers.ContainsKey("Allow"));
			Assert.Equal(new[] {"GET", "POST"},
				controller.Response.Headers["Allow"].Select(e => e).Select(e => e.Split(", ", StringSplitOptions.RemoveEmptyEntries)).SelectMany(e => e).ToArray());
		}
#endif

		[Fact/*[RFC2616.10.4.1]*/]
		public void PostWithMissingProductResultsInBadRequest()
		{
			var mock = new Mock<IProductCatalog>();
			var controller = new ProductsController(mock.Object);
			var result = controller.Post(null);
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact/*[RFC2616.9.5]*/]
		public void PostProductResultsInCreate()
		{
			var mock = new Mock<IProductCatalog>();
			mock.Setup(m => m.FindById(It.Is<string>(s => s == "2"))).Returns(default(Product));
			Product newProduct = null;
			mock.Setup(m => m.AddNewProduct(It.IsAny<Product>())).Callback((Product p) => { newProduct = p; });
			var controller = new ProductsController(mock.Object);
			var result = controller.Post(new WebAPI.Models.Product {Name = "name 3", Price = "10.2"});
			var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);

			var product = Assert.IsType<WebAPI.Models.Product>(createdAtRouteResult.Value);
			Assert.Equal("name 3", product.Name);
			Assert.Equal("10.2", product.Price);
			Assert.NotNull(newProduct);
		}


		// post bad product object, Bad Constraint, BadRequest
		[Fact/*[RFC2616.10.4.1]*/]
		public void PostProductConstraintViolationResultsInBadRequest()
		{
			var mock = new Mock<IProductCatalog>();
			mock.Setup(m => m.FindById(It.Is<string>(s => s == "2"))).Returns(default(Product));
			mock.Setup(m => m.AddNewProduct(It.IsAny<Product>())).Throws<ProductConstraintViolatedException>();
			var controller = new ProductsController(mock.Object);
			var result = controller.Post(new WebAPI.Models.Product {Name = "name 3", Price = "10.2"});
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);

			var errorResponse = Assert.IsType<ErrorResponse>(badRequestObjectResult.Value); // TODO: Error object
		}

		// Put bad product object, Bad Constraint, BadRequesxt
		[Fact/*[RFC2616.10.4.1]*/]
		public void PutUpdateProductConstraintViolationResultsInBadRequest()
		{
			var mock = new Mock<IProductCatalog>();
			mock.Setup(m => m.FindById(It.Is<string>(s => s == "2"))).Returns(new Product("TestName", 10.0m));
			mock.Setup(m => m.ReplaceExistingProduct(It.Is<string>(s => s == "2"), It.IsAny<Product>())).Throws<ProductConstraintViolatedException>();
			var controller = new ProductsController(mock.Object);
			var result = controller.Put("2", new WebAPI.Models.Product {Name = "name 2", Price = "10.1"});
			var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);

			var errorResponse = Assert.IsType<ErrorResponse>(badRequestObjectResult.Value);
		}

		// TODO: Post response content

#if PUT_CREATE_SUPPORTED
		// race condition (InvalidOperationException, NoContent);
		[Fact/*[RFC2616.9.5]*/]
		public void PutCreateInvalidOperationResultsInNoContent()
		{
			var mock = new Mock<IProductCatalog>();
			Product updatedProduct = null;
			mock.Setup(m => m.FindById(It.Is<string>(s => s == "2"))).Returns(default(Product));
			mock.Setup(m => m.AddNewProduct(It.Is<string>(s => s == "2"), It.IsAny<Product>())).Throws<InvalidOperationException>();
			mock.Setup(m => m.ReplaceExistingProduct(It.Is<string>(s => s == "2"), It.IsAny<Product>())).Callback((string i, Product p) => { updatedProduct = p; });
			var controller = new ProductsController(mock.Object)
			{
				ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
			};
			var result = controller.Put("2", new WebAPI.Models.Product {Name = "name 2", Price = "10.1"});
			var noContentResult = Assert.IsType<NoContentResult>(result);
		}
#endif
	}
}