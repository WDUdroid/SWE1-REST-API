using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Interfaces;
using Moq;
using NUnit.Framework;
using REST_API.API;

namespace REST_API.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TcpHandlerTest()
        {
            //var aTcpHandler = new TcpHandler(mockTcpClient.Object, IPAddress.Loopback);
            //mockTcpHandler.Setup(c => c.GetHttpContent()).Returns("abc");

            //var writer = new StreamWriter(stream){AutoFlush = true};
            //writer.WriteLine("abc");

            //Assert.AreEqual("abc", aTcpHandler.GetHttpContent());

            var mockTcpHandler = new Mock<ITcpHandler>();
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream) { AutoFlush = true };
            writer.Write("abc");
            memoryStream.Position = 0;

            mockTcpHandler
                .Setup(c => c.GetStream())
                .Returns(memoryStream);

            WebContentReader contentReader = new WebContentReader(mockTcpHandler.Object);

            string actualValue = contentReader.GetHttpContent();

            Assert.AreEqual("abc", actualValue);
        }
    }
}