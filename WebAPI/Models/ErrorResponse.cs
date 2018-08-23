using System.Net;
using System.Runtime.Serialization;

namespace WebAPI.Models
{
	[DataContract]
	public class ErrorResponse
	{
		[DataMember(Name = "error")]
		public Error Error { get; set; }

		public static ErrorResponse BadRequestError(int subCode, string messageText, params string[] targets)
		{
			return new ErrorResponse
			{
				Error=new Error
				{
					Code = $"{(int)HttpStatusCode.BadRequest}.{subCode}",
					Message = new ErrorMessage
					{
						Text = messageText,
						Targets = targets
					}
				}
			};
		}
	}
}