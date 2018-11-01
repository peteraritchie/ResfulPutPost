using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable IntroduceOptionalParameters.Global

namespace Tests.Common
{
	/// <summary>
	/// A test stub to be used with HttpClient to provide canned response messages
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class StubMessageHandler : DelegatingHandler
	{
		private readonly HttpResponseMessage cannedHttpResponseMessage;

		public StubMessageHandler(HttpResponseMessage httpResponseMessage)
		{
			cannedHttpResponseMessage = httpResponseMessage;
		}

		public StubMessageHandler(byte[] byteArray)
		:this(byteArray, HttpStatusCode.OK)
		{
		}

		public StubMessageHandler(byte[] byteArray, HttpStatusCode httpStatusCode)
		{
			cannedHttpResponseMessage = new HttpResponseMessage(httpStatusCode)
			{
				Content = new ByteArrayContent(byteArray)
			};
		}

		public StubMessageHandler(string stringContent)
			: this(stringContent, HttpStatusCode.OK)
		{
		}

		public StubMessageHandler(string stringContent, HttpStatusCode httpStatusCode)
		{
			cannedHttpResponseMessage = new HttpResponseMessage(httpStatusCode)
			{
				Content = new StringContent(stringContent)
			};
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			return Task.FromResult(cannedHttpResponseMessage);
		}

		public StubMessageHandler(Stream stream)
			: this(stream, HttpStatusCode.OK)
		{
		}

		public StubMessageHandler(Stream stream, HttpStatusCode httpStatusCode)
		{
			cannedHttpResponseMessage = new HttpResponseMessage(httpStatusCode)
			{
				Content = new StreamContent(stream)
			};
		}

		public StubMessageHandler(IEnumerable<HttpContent> httpContents)
			: this(httpContents, HttpStatusCode.OK)
		{
		}

		public StubMessageHandler(IEnumerable<HttpContent> httpContents, HttpStatusCode httpStatusCode)
		{
			var multipartContent = new MultipartContent();
			foreach (var content in httpContents)
			{
				multipartContent.Add(content);
			}
			cannedHttpResponseMessage = new HttpResponseMessage(httpStatusCode)
			{
				Content = multipartContent
			};
		}

		public StubMessageHandler(HttpContent httpContent1, HttpContent httpContent2)
			: this(httpContent1, httpContent2, HttpStatusCode.OK)
		{
		}

		public StubMessageHandler(HttpContent httpContent1, HttpContent httpContent2, HttpStatusCode httpStatusCode)
		{
			cannedHttpResponseMessage = new HttpResponseMessage(httpStatusCode)
			{
				Content = new MultipartFormDataContent {httpContent1, httpContent2}
			};
		}
	}
}