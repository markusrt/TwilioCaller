using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.Text;
using System.Text.Json;
using TwilioCaller;

namespace TwilioCallerTests
{
    public class CallTests
    {
        [Test]
        public async Task CallWithMissingPayload_ReturnsBadRequest()
        {
            // Define constants
            var logger = Substitute.For<ILogger>();
            var request = CreateMockRequest(null);
            
            var result = await Call.Run(request, logger);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestCase("911")]
        [TestCase("1234")]
        [TestCase("1234678")]
        public async Task CallWithInvalidPhoneNumber_ReturnsBadRequest(string phoneNumber)
        {
            // Define constants
            var logger = Substitute.For<ILogger>();
            var request = CreateMockRequest(new {number = phoneNumber});
            
            var result = await Call.Run(request, logger);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        private static HttpRequest CreateMockRequest(object body)
        {
            var json = JsonSerializer.Serialize(body);
            var byteArray = Encoding.ASCII.GetBytes(json);
 
            var memoryStream = new MemoryStream(byteArray);
 
            var mockRequest = Substitute.For<HttpRequest>();
            mockRequest.Body.Returns(memoryStream);
 
            return mockRequest;
        }
    }
}