﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;

namespace Downloader.Test
{
    [TestClass]
    public class DummyFileControllerTest
    {
        private int port = 3333;
        private string contentType = "application/octet-stream";

        public DummyFileControllerTest()
        {
            DummyHttpServer.HttpServer.Run(port);
        }

        ~DummyFileControllerTest()
        {
            DummyHttpServer.HttpServer.Stop();
        }

        [TestMethod]
        public void GetFileTest()
        {
            // arrange
            int size = 1024;
            byte[] bytes = new byte[size];
            string url = $"http://localhost:{port}/dummyfile/file/size/{size}";
            var dummyData = Helper.DummyData.GenerateOrderedBytes(size);

            // act
            var headers = ReadAndGetHeaders(url, bytes);

            // assert
            Assert.IsTrue(dummyData.SequenceEqual(bytes));
            Assert.AreEqual(size.ToString(), headers["Content-Length"]);
            Assert.AreEqual(contentType, headers["Content-Type"]);
        }

        [TestMethod]
        public void GetFileWithNameTest()
        {
            // arrange
            int size = 2048;
            byte[] bytes = new byte[size];
            string filename = "testfilename.dat";
            string url = $"http://localhost:{port}/dummyfile/file/{filename}?size={size}";
            var dummyData = Helper.DummyData.GenerateOrderedBytes(size);

            // act
            var headers = ReadAndGetHeaders(url, bytes);

            // assert
            Assert.IsTrue(dummyData.SequenceEqual(bytes));
            Assert.AreEqual(size.ToString(), headers["Content-Length"]);
            Assert.AreEqual(contentType, headers["Content-Type"]);
        }

        [TestMethod]
        public void GetFileWithoutHeaderTest()
        {
            // arrange
            int size = 2048;
            byte[] bytes = new byte[size];
            string filename = "testfilename.dat";
            string url = $"http://localhost:{port}/dummyfile/file/{filename}?size={size}&noheader=true";
            var dummyData = Helper.DummyData.GenerateOrderedBytes(size);

            // act
            var headers = ReadAndGetHeaders(url, bytes);

            // assert
            Assert.IsTrue(dummyData.SequenceEqual(bytes));
            Assert.IsNull(headers["Content-Length"]);
            Assert.IsNull(headers["Content-Type"]);
        }

        [TestMethod]
        public void GetFileWithContentDispositionTest()
        {
            // arrange
            int size = 1024;
            byte[] bytes = new byte[size];
            string filename = "testfilename.dat";
            string url = $"http://localhost:{port}/dummyfile/file/{filename}/size/{size}";
            var dummyData = Helper.DummyData.GenerateOrderedBytes(size);

            // act
            var headers = ReadAndGetHeaders(url, bytes);

            // assert
            Assert.IsTrue(dummyData.SequenceEqual(bytes));
            Assert.AreEqual(size.ToString(), headers["Content-Length"]);
            Assert.AreEqual(contentType, headers["Content-Type"]);
            Assert.IsTrue(headers["Content-Disposition"].Contains($"filename={filename};"));
        }

        private WebHeaderCollection ReadAndGetHeaders(string url, byte[] bytes)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            using HttpWebResponse downloadResponse = request.GetResponse() as HttpWebResponse;
            var respStream = downloadResponse.GetResponseStream();
            respStream.Read(bytes);

            return downloadResponse.Headers;
        }
    }
}
