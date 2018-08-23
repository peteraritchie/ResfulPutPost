using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebAPI.Models
{
	[DataContract]
	public class HelpLinks
	{
		[DataMember(Name = "help")]
		public IEnumerable<HelpLink> Help { get; set; }
	}
}