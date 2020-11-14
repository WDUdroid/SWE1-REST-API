using System;
using System.IO;
using System.Text;
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
        public void GetHttpContentShortMessageTest()
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

        [Test]
        public void GetHttpContentLongMessageTest()
        {
            var message = @"GET /messages HTTP/1.1
                            Content - Type: text / plain
                            User - Agent: PostmanRuntime / 7.26.8
                            Accept: */*
                            Postman-Token: 18f78a05-ec79-4a35-8bca-8752b54e96a0
                            Host: localhost:8000
                            Accept-Encoding: gzip, deflate, br
                            Connection: keep-alive
                            Content-Length: 12

                            Long Message";

            var mockTcpHandler = new Mock<ITcpHandler>();
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream) { AutoFlush = true };
            writer.Write(message);

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

            Assert.AreEqual(message, actualValue);
        }

        [Test]
        public void GetHttpContentSpecialCharacterMessageTest()
        {
            var mockTcpHandler = new Mock<ITcpHandler>();
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream) { AutoFlush = true };
            writer.Write("ÄÜÖäüö");
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

            Assert.AreEqual("????????????",actualValue);
        }

        [Test]
        public void SendHttpContentTest()
        {
            var mockTcpHandler = new Mock<ITcpHandler>();
            var mockRequestContext = new Mock<IRequestContext>();
            var memoryStream = new MemoryStream();
            var reader = new StreamReader(memoryStream);
            memoryStream.Position = 0;

            mockTcpHandler
                .Setup(c => c.GetStream())
                .Returns(memoryStream);

            mockRequestContext
                .Setup(c => c.MessageID)
                .Returns(1);
            mockRequestContext
                .Setup(c => c.StatusCode)
                .Returns("200 OK");
            mockRequestContext
                .Setup(c => c.Payload)
                .Returns("SendHttpContentTest");

            WebHandler contentWriter = new WebHandler(mockTcpHandler.Object, mockRequestContext.Object);

            contentWriter.SendHttpContent();

            string expectedValue = @"HTTP/1.1 200 OK
Server: RESTful-Server
Content-Type: text/plain
Content-Length: 21

1 SendHttpContentTest
";

            string actualValue = Encoding.ASCII.GetString(memoryStream.ToArray());

            Console.WriteLine(actualValue);
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}