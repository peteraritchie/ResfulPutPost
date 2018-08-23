using System;
using WebAPI.Models;

namespace WebAPI.Abstractions
{
	public interface ICreateProductRequestJournal
	{
		// Create
		Guid Book(CreateProductRequest request);

		[Obsolete("Deprecated, use Book instead.", error:true)]
		Guid Create(CreateProductRequest request);

		// Get
		JournalEntry<CreateProductRequest> Lookup(Guid guid);

		// Update
		void TransitionState(Guid guid, TransactionState newState);

		// post/remove?
		[Obsolete("Deprecated, use Post instead.", error: true)]
		void Delete(Guid guid);

		void Post(Guid guid);
	}

	public enum TransactionState
	{
		Unknown = 0,
		Skipped = 1,
		Booked = 2,	// recorded, to be scheduled/assigned
		Scheduled = 4, // & assigned
		Started = 8,
		Suspended = 16,
		Resumed = 32,
		Aborted = 64, // & withdrawn
		Completed = 128
	}
}