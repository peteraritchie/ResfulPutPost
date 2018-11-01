using System.Collections.Generic;
using System.Linq;
using Domain.Abstractions;
#if PUT_CREATE_SUPPORTED
using System;
#else
using Microsoft.AspNetCore.Http;
#endif
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WebAPI.Models;


// link elements contain the url (href), the relation (rel) and the type [https://tools.ietf.org/html/rfc4287#section-4.2.7]
// links as a single link object or array of link object [https://groups.google.com/d/msg/api-craft/i6gJNsIGLG4/m41dL-b5ZX4J]
// "Another thing which will help you while building RESTful APIs is that query based
//	API results should be represented by a list of links with summary information, not 
//	by arrays of original resource representations because query is not a substitute for 
//	identification of resources." https://restfulapi.net/

/*
 collection response:
{
	"_links": {
		"self": {"href": "/products"},
		"first": {"href": "/products"},
		"next": {"href": "/products?page=1"},
		"last": {"href": "/products?page=99"}
	  }
	},
	"_embedded": {
		"products" : [ // resource objects
		{
			"_links": {
				"self": {"href": "/products/1"},
			},
			"name" : "product 1",
			"price" : "10.99",
		},
		{
			"_links": {
				"self": {"href": "/products/2"},
			},
			"name" : "product 2",
			"price" : "99.99",
		}
		]
	},
	"count": 2,
	"total": 199
}
 */

/*
 * all responses are a Resource Object that may have reserved properties :_links, _embededed.  All other root properties should represent the current state of the resource
 * https://tools.ietf.org/html/draft-kelly-json-hal-08#section-4
 */
/*
 * Link Object
 * "href": required
 * "templated": optional
 * "type": optional
 * "deprecation": optional
 * "name": optional
 * "profile": optional
 * "hreflang": optional
 * https://tools.ietf.org/html/draft-kelly-json-hal-08#section-5
 */
// https://www.iana.org/assignments/link-relations/link-relations.xhtml RE: "rel"

namespace WebAPI.Controllers
{
	/// <summary>
	/// our api/products controller
	/// </summary>
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		// a repository for "products"
		private readonly IProductCatalog productCatalog;

		public ProductsController(IProductCatalog productCatalog)
		{
			this.productCatalog = productCatalog;
		}

		/// <summary>
		/// Get all products
		/// <code>GET api/products</code>
		/// <remarks>Named route: "GetProducts"</remarks>
		/// </summary>
		/// <returns>One of: 200, 500</returns>
		[HttpGet(Name = "GetProducts")]
		public IActionResult Get()
		{
			var productCatalogProducts = productCatalog.Products ?? new Domain.Primitivies.Product[0];
			return Ok(productCatalogProducts.Select(product => product.MapTo<Product>()));
		}

		/// <summary>
		/// Get zero or one product
		/// <code>GET api/products/5</code>
		/// <remarks>named route: "GetProduct"</remarks>
		/// </summary>
		/// <param name="id">id of the product to get</param>
		/// <returns>One of: 200, 400, 404, 500</returns>
		[HttpGet("{id}", Name="GetProduct")]
		public IActionResult Get(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
				return BadRequest(ErrorResponse.BadRequestError(1, "missing product id", nameof(id)));

			var product = productCatalog.FindById(id);

			if (product == null)
				return NotFound();
			return Ok(product.MapTo<Product>()); 
		}

		/// <summary>
		/// <code>POST api/products</code>
		/// </summary>
		/// <param name="product"></param>
		///<returns>One of: 201, 400, 500</returns>
		[HttpPost]
		public IActionResult Post([FromBody] Product product)
		{
			if (product == null)
				return BadRequest(ErrorResponse.BadRequestError(1, "missing product", nameof(product)));

			product.Map(out Domain.Primitivies.Product domainProduct);
			try
			{
				string id = productCatalog.AddNewProduct(domainProduct);
				// respond with 201, including a link to the get action with id and the product
				return CreatedAtRoute("GetProduct", new {id}, domainProduct.MapTo<Product>());
			}
			catch (ProductConstraintViolatedException ex)
			{
				return BadRequest(ErrorResponse.BadRequestError(2, $"invalid product body {ex.Message}", nameof(product)));
			}
			// let controller/global exception filter kick in.

		}

		/// <summary>
		/// Replace a product
		/// <code>PUT api/products/5</code>
		/// </summary>
		/// <param name="id">id of product to replace</param>
		/// <param name="product">mandatory product information</param>
#if PUT_CREATE_SUPPORTED
		///<returns>One of: 204, 400, 405, 500</returns>
#else
		///<returns>One of: 204, 201, 400, 500</returns>
#endif
		[HttpPut("{id}")]
		public IActionResult Put(string id, [FromBody] Product product)
		{
			if (product == null)
				return BadRequest(ErrorResponse.BadRequestError(1, "missing product", nameof(product)));
			if(string.IsNullOrWhiteSpace(id))
				return BadRequest(ErrorResponse.BadRequestError(1, "missing product id", nameof(id)));

			try
			{
				var domainProduct = product.MapTo<Domain.Primitivies.Product>();
				var existingProduct = productCatalog.FindById(id);
				if (existingProduct == null)
				{
					// Define PUT_CREATE_SUPPORTED *if* you want to support the PUT method
					// creating products when the id doesn't already exist.
					// NOTE: Only do this if the client (and not this service) owns creating
					// product ids!
#if PUT_CREATE_SUPPORTED
					try
					{
						productCatalog.AddNewProduct(id, domainProduct);
						// respond with 201, including a link to the get action with id and the product
						return CreatedAtRoute("GetProduct", new { id }, domainProduct.MapTo<Product>());
					}
					catch (InvalidOperationException)
					{
						// fall through and try to update due to race condition
					}
#else
					// respond with 405 and tell the client that only GET and POST are supported when including an id
					Response.Headers.Add(HeaderNames.Allow, "GET, POST");
					return StatusCode(StatusCodes.Status405MethodNotAllowed);
#endif
				}

				productCatalog.ReplaceExistingProduct(id, domainProduct);
				return NoContent();
			}
			catch (ProductConstraintViolatedException ex)
			{
				return BadRequest(ErrorResponse.BadRequestError(2, $"invalid product body {ex.Message}", nameof(product)));
			}
			// let controller/global exception filter kick in.

		}
	}
}