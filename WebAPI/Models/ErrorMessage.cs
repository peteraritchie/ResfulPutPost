using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebAPI.Models
{
	[DataContract]
	public class ErrorMessage
	{
		[DataMember(Name = "text")]
		public string Text { get; set; }
		[DataMember(Name = "targets")]
		public IEnumerable<string> Targets { get; set; }
		[DataMember(Name = "_links")]
		public HelpLinks Links { get; set; }
	}
}