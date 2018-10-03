using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WebAPI.Abstractions;
using WebAPI.Models;

namespace WebAPI.Controllers
{
	/// <summary>
	/// Support for asynchronously creating a product, and the status of the request
	/// </summary>
	[Route("api/[controller]")]
	public class CreateProductRequestQueueController : ControllerBase
	{
		private readonly ICreateProductRequestJournal requestJournal;

		public CreateProductRequestQueueController(ICreateProductRequestJournal requestJournal)
		{
			this.requestJournal = requestJournal;
		}
		/// <summary>
		/// Get zero or one requests
		/// <code>GET api/createproductrequestqueue/fd33edfe-69c3-4aa7-8df7-1d4da74002d5</code>
		/// <remarks>named route: "GetCreateProductRequest"</remarks>
		/// </summary>
		/// <param name="id">id of the product to get</param>
		/// <returns>One of: 200, 400, 404, 500</returns>
		[HttpGet("{id}", Name = "GetCreateProductRequestQueue")]
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

			string url = Url.Action("Get", "Products", new { id = entry.Result });

			Response.Headers.Add(HeaderNames.ContentLocation, url);
			return StatusCode(StatusCodes.Status303SeeOther, new Status(url, "done", "Your request has been processed."));
		}
	}
}