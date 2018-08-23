using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using static WebAPI.Abstractions.TransactionState;

namespace WebAPI.Abstractions
{
	public class JournalEntry<T> where T:class 
	{
		[ExcludeFromCodeCoverage/* TODO: in real service this would be used,*/]
		private void ValidateTransition(TransactionState state, TransactionState newState)
		{
			// should never happen, but just in case a ref*cktoring occurs
			if(state == Unknown || newState == Unknown)
			{
				throw new InvalidOperationException("Transaction state must not transition from nor to Unknown.");
			}

			switch ((int) state | (int) newState)
			{
				case (int) Booked | (int) Scheduled:
				case (int) Scheduled | (int) Skipped:
				case (int) Started | (int) Skipped:
				case (int) Suspended | (int) Skipped:
				case (int) Resumed | (int) Skipped:
				case (int) Scheduled | (int) Started:
				case (int) Started | (int) Aborted:
				case (int) Suspended | (int) Aborted:
				case (int) Resumed | (int) Aborted:
				case (int) Started | (int) Completed:
				case (int) Resumed | (int) Completed:
					return;
				default:
					throw new InvalidOperationException($"Transaction state must not transition from {state} to {newState}.");
			}
		}

		public JournalEntry([NotNull] T detail, DateTime utcDateTime, TransactionState state = Booked)
		{
			Detail = detail ?? throw new ArgumentNullException(nameof(detail));

			UtcDateTime = utcDateTime;
			State = state != Unknown
				? state
				: throw new ArgumentOutOfRangeException(nameof(state), "initiate state should not be Unknown");
		}

		[ExcludeFromCodeCoverage/* TODO: in real service this would be used,*/]
		public void TransitionTo(TransactionState newState)
		{
			if (newState == Unknown)
			{
				throw new ArgumentOutOfRangeException(nameof(newState), "New state should not be Unknown");
			}

			ValidateTransition(State, newState);

			State = newState;
		}

		public void SetResult(string resultText)
		{
			Result = resultText;
		}

		[ExcludeFromCodeCoverage/* TODO: in real service this would be used,*/]
		public DateTime UtcDateTime { get; }
		public TransactionState State { get; private set; }
		[ExcludeFromCodeCoverage/* TODO: in real service this would be used,*/]
		[NotNull] public T Detail { get; }

		public string Result { get; private set; }
	}
}