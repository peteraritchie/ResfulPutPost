using System.Runtime.Serialization;

namespace WebAPI.Models
{
	[DataContract]
	public class Error
	{
		[DataMember(Name = "code")]
		public string Code { get; set; }
		[DataMember(Name = "message")]
		public ErrorMessage Message { get; set; }
	}
}