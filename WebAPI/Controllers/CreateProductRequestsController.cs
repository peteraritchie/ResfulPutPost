using Microsoft.AspNetCore.Mvc;
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
		/// <returns>One of: 400, 303</returns>
		[HttpPost]
		public IActionResult Post([FromBody] CreateProductRequest request)
		{
			if (request == null)
				return BadRequest(ErrorResponse.BadRequestError(1, "missing request", nameof(request)));

			var requestGuid = requestJournal.Book(request);

			// TODO: something to kick off an external process to do the work (or something that monitors the journal.

			return AcceptedAtAction("Get", "CreateProductRequestQueue",
				new {id = requestGuid});

		}
	}
}