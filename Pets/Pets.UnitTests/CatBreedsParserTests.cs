using Moq;
using Moq.Protected;
using NUnit.Framework;
using Pets.BL.Parsers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Pets.UnitTests
{
    public class CatBreedsParserTests
    {

        [SetUp]
        public void Setup()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{'name':'1'},{'name':'2'}]"),
                })
               .Verifiable();
            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            var parser = new CatBreedsParser(httpClient, "test", "test", "test");
            var task = parser.Run();
            task.Wait();
        }

        [Test]
        public void CheckIf()
        {
            Assert.Pass();
        }
    }
}