using System;
using System.Collections.Generic;
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

        // Tests if Server can receive short messages
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

        // Tests if Server can receive long messages
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

        // Tests if Server does not crash, when specified characters are read (others had this problem)
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

        // Tests if Server can send messages
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

        // Tests if RequestContext Constructor splits message accordingly
        [Test]
        public void RequestContextTest()
        {
            var dataList = new List<String>();
            string receivedData = @"PUT /messages/3 HTTP/1.1
Content - Type: text / plain
User - Agent: PostmanRuntime / 7.26.8
Accept: */*
Postman-Token: b4dfd06b-7ae4-4529-ab44-b7a202d33ef5
Accept-Encoding: gzip, deflate, br
Connection: keep-alive
Content-Length: 6

zweite";
            RequestContext processData = new RequestContext(receivedData, dataList);

            string actualHttpRequestValue = processData.HttpRequest;
            string actualHttpBodyValue = processData.HttpBody;
            string actualHttpVersionValue = processData.HttpVersion;

            Assert.AreEqual("/messages/3", actualHttpRequestValue);
            Assert.AreEqual("zweite", actualHttpBodyValue);
            Assert.AreEqual("HTTP/1.1", actualHttpVersionValue);
        }

        // Test if Server can handle non-HttpRequest messages
        [Test]
        public void RequestContextEmptyTest()
        {
            var dataList = new List<String>();
            string receivedData = "";
            RequestContext processData = new RequestContext(receivedData, dataList);

            string actualHttpRequestValue = processData.HttpRequest;
            string actualHttpBodyValue = processData.HttpBody;
            string actualHttpVersionValue = processData.HttpVersion;

            Assert.AreEqual(null, actualHttpRequestValue);
            Assert.AreEqual(null, actualHttpBodyValue);
            Assert.AreEqual(null, actualHttpVersionValue);
        }

        // Tests if a message can be created
        [Test]
        public void OneNewMessageTest()
        {
            var dataList = new List<String>();
            RequestContext processData = new RequestContext(dataList);
            processData.NewMessage("Hallo");

            Assert.AreEqual("Hallo", dataList[0]);
        }

        // Tests if two message can be created
        [Test]
        public void TwoNewMessageTest()
        {
            var dataList = new List<String>();
            RequestContext processData = new RequestContext(dataList);
            processData.NewMessage("Hallo");
            processData.NewMessage("Bye");

            Assert.AreEqual("Hallo", dataList[0]);
            Assert.AreEqual("Bye", dataList[1]);
        }

        // Tests if Server can cope with non-existing messages
        [Test]
        public void UpdateNonExistingMessageTest()
        {
            var dataList = new List<String>();
            RequestContext processData = new RequestContext(dataList);
            processData.UpdateMessage(1, "Not Existing");

            Assert.AreEqual("412 Precondition Failed", processData.StatusCode);
            Assert.AreEqual("", processData.Payload);
            Assert.AreEqual(-1, processData.MessageID);
        }

        // Tests if Server can update messages
        [Test]
        public void UpdateExistingMessageTest()
        {
            var dataList = new List<String>();
            RequestContext processData = new RequestContext(dataList);
            processData.NewMessage("Hallo");
            processData.NewMessage("Bye");

            Assert.AreEqual("Hallo", dataList[0]);
            Assert.AreEqual("Bye", dataList[1]);

            processData.UpdateMessage(1, "Not Existing");

            Assert.AreEqual("Not Existing", dataList[0]);
            Assert.AreEqual("Not Existing", processData.Payload);
            Assert.AreEqual(-1, processData.MessageID);
        }

        // Tests if Server can return messages to client
        [Test]
        public void ListMessagesTest()
        {
            var dataList = new List<String>();
            RequestContext processData = new RequestContext(dataList);
            processData.NewMessage("Hallo");
            processData.NewMessage("Bye");

            Assert.AreEqual("Hallo", dataList[0]);
            Assert.AreEqual("Bye", dataList[1]);

            processData.ListMessages();

            Assert.AreEqual("Hallo", dataList[0]);
            Assert.AreEqual("1: Hallo\r\n2: Bye\r\n", processData.Payload);
        }

        // Tests if Server can cope with request for not existing message
        [Test]
        public void ListNonExistingSingleMessageTest()
        {
            var dataList = new List<String>();
            RequestContext processData = new RequestContext(dataList);

            processData.ListSingleMessage(1);

            Assert.AreEqual("404 Not Found", processData.StatusCode);
            Assert.AreEqual("", processData.Payload);
        }

        // Tests if Server can return single messages
        [Test]
        public void ListSingleMessageTest()
        {
            var dataList = new List<String>();
            RequestContext processData = new RequestContext(dataList);
            processData.NewMessage("Hallo");
            processData.NewMessage("Bye");

            Assert.AreEqual("Hallo", dataList[0]);
            Assert.AreEqual("Bye", dataList[1]);

            processData.ListSingleMessage(1);

            Assert.AreEqual("Hallo", dataList[0]);
            Assert.AreEqual("200 OK", processData.StatusCode);
            Assert.AreEqual("Hallo", processData.Payload);
        }

        // Tests if Server can cope with remove request for not existing message
        [Test]
        public void RemoveNonExistingMessageTest()
        {
            var dataList = new List<String>();
            RequestContext processData = new RequestContext(dataList);

            processData.RemoveMessage(1);

            Assert.AreEqual("404 Not Found", processData.StatusCode);
            Assert.AreEqual("", processData.Payload);
        }

        // Tests if Server can remove messages
        [Test]
        public void RemoveMessageTest()
        {
            var dataList = new List<String>();
            RequestContext processData = new RequestContext(dataList);
            processData.NewMessage("Hallo");
            processData.NewMessage("Bye");

            Assert.AreEqual("Hallo", dataList[0]);
            Assert.AreEqual("Bye", dataList[1]);

            processData.RemoveMessage(1);

            Assert.AreEqual("", dataList[0]);
            Assert.AreEqual("200 OK", processData.StatusCode);
            Assert.AreEqual("", processData.Payload);
        }
    }
}