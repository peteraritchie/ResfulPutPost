using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WebAPI.Abstractions;
using WebAPI.Models;

namespace WebAPI.Controllers
{
	/// <summary>
	/// Support for asynchronously creating a product
	/// </summary>
	[Route("api/[controller]")]
	public class CreateProductRequestsController : ControllerBase
	{
		private readonly ICreateProductRequestJournal requestJournal;

		public CreateProductRequestsController(ICreateProductRequestJournal requestJournal)
		{
			this.requestJournal = requestJournal;
		}

		/// <summary>
		/// <code>POST api/products</code>
		/// </summary>
		/// <param name="request">The request to create a product, containing the product data</param>
		/// <returns>One of: 400</returns>
		[HttpPost]
		public IActionResult Post([FromBody] CreateProductRequest request)
		{
			if (request == null)
				return BadRequest(ErrorResponse.BadRequestError(1, "missing request", nameof(request)));

			var requestGuid = requestJournal.Book(request);

			// TODO: something to kick off an external process to do the work (or something that monitors the journal.

			return AcceptedAtRoute("GetCreateProductRequest", new {id = requestGuid});
		}

		/// <summary>
		/// Get zero or one requests
		/// <code>GET api/createproductrequests/5</code>
		/// <remarks>named route: "GetCreateProductRequest"</remarks>
		/// </summary>
		/// <param name="id">id of the product to get</param>
		/// <returns>One of: 200, 400, 404, 500</returns>
		[HttpGet("{id}", Name = "GetCreateProductRequest")]
		public IActionResult Get(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
				return BadRequest(ErrorResponse.BadRequestError(1, "missing request id", nameof(id)));

			if (!Guid.TryParse(id, out var guid))
				return BadRequest(ErrorResponse.BadRequestError(1, "malformed request id", nameof(id)));

			var entry = requestJournal.Lookup(guid);

			if (entry == null)
				return NotFound();

			if (entry.State != TransactionState.Completed)
				return Ok(new Status(1, "pending", "Your request currently being processed."));

			string url = Url.Action("GetProduct", nameof(ProductsController), new {id = entry.Result});

			Response.Headers.Add(HeaderNames.ContentLocation, url);
			return StatusCode(StatusCodes.Status303SeeOther, new Status(url, "done", "Your request has been processed."));
		}
	}
}