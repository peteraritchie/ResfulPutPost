using System;
using System.Runtime.Serialization;

namespace WebAPI.Models
{
	[DataContract(Name ="status")]
	public class Status
	{
		public Status(int pingAfterSeconds, string state = "pending", string message="Your request has been accepted for processing.")
		{
			PingAfterDateTime = DateTime.UtcNow.AddSeconds(pingAfterSeconds);
			State = state;
			Message = message;
		}

		public Status(string uri, string state = "done", string message = "Your request has been processed.")
		{
			Link = new Uri(uri);
			State = state;
			Message = message;
		}

		[DataMember(Name="state")]
		public string State { get; set; }
		[DataMember(Name = "link")]
		public Uri Link{ get; set; }
		[DataMember(Name = "message")]
		public string Message { get; set; }
		[DataMember(Name = "ping-after")]
		public DateTime? PingAfterDateTime { get; set; }
	}
}