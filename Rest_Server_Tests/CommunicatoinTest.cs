using System.IO;
using Moq;
using NUnit.Framework;
using SWE1_REST_SERVER;

namespace Rest_Server_Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetHttpContentTest()
        {
            var mockTcpHandler = new Mock<ITcpHandler>();
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream) { AutoFlush = true };
            writer.Write("abc");
            memoryStream.Position = 0;

            int callCounter = 0;
            mockTcpHandler
                .Setup(c => c.DataAvailable())
                .Returns(() =>
                {
                    if (callCounter++ < 1)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                });

            mockTcpHandler
                .Setup(c => c.GetStream())
                .Returns(memoryStream);

            WebHandler contentReader = new WebHandler(mockTcpHandler.Object);

            string actualValue = contentReader.GetHttpContent();

            Assert.AreEqual("abc", actualValue);
        }
    }
}