using System.Runtime.Serialization;

namespace WebAPI.Models
{
	[DataContract]
	public class HelpLink
	{
		[DataMember(Name = "href")]
		public string Href { get; set; }
		[DataMember(Name = "title")]
		public string Title { get; set; }
		[DataMember(Name = "name")]
		public string Name { get; set; }
	}
}