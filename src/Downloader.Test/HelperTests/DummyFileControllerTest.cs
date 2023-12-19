﻿using Downloader.DummyHttpServer;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Xunit;

namespace Downloader.Test.HelperTests;

public class DummyFileControllerTest
{
    private readonly string contentType = "application/octet-stream";
    private WebHeaderCollection headers;

    [Fact]
    public void GetFileTest()
    {
        // arrange
        int size = 1024;
        byte[] bytes = new byte[size];
        string url = DummyFileHelper.GetFileUrl(size);
        var dummyData = DummyData.GenerateOrderedBytes(size);

        // act
        ReadAndGetHeaders(url, bytes);

        // assert
        Assert.True(dummyData.SequenceEqual(bytes));
        Assert.Equal(size.ToString(), headers["Content-Length"]);
        Assert.Equal(contentType, headers["Content-Type"]);
    }

    [Fact]
    public void GetFileWithNameTest()
    {
        // arrange
        int size = 2048;
        byte[] bytes = new byte[size];
        string filename = "testfilename.dat";
        string url = DummyFileHelper.GetFileWithNameUrl(filename, size);
        var dummyData = DummyData.GenerateOrderedBytes(size);

        // act
        ReadAndGetHeaders(url, bytes);

        // assert
        Assert.True(dummyData.SequenceEqual(bytes));
        Assert.Equal(size.ToString(), headers["Content-Length"]);
        Assert.Equal(contentType, headers["Content-Type"]);
    }

    [Fact]
    public void GetSingleByteFileWithNameTest()
    {
        // arrange
        int size = 2048;
        byte fillByte = 13;
        byte[] bytes = new byte[size];
        string filename = "testfilename.dat";
        string url = DummyFileHelper.GetFileWithNameUrl(filename, size, fillByte);
        var dummyData = DummyData.GenerateSingleBytes(size, fillByte);

        // act
        ReadAndGetHeaders(url, bytes);

        // assert
        Assert.True(bytes.All(i => i == fillByte));
        Assert.True(dummyData.SequenceEqual(bytes));
        Assert.Equal(size.ToString(), headers["Content-Length"]);
        Assert.Equal(contentType, headers["Content-Type"]);
    }

    [Fact]
    public void GetFileWithoutHeaderTest()
    {
        // arrange
        int size = 2048;
        byte[] bytes = new byte[size];
        string filename = "testfilename.dat";
        string url = DummyFileHelper.GetFileWithoutHeaderUrl(filename, size);
        var dummyData = DummyData.GenerateOrderedBytes(size);

        // act
        ReadAndGetHeaders(url, bytes);

        // assert
        Assert.True(dummyData.SequenceEqual(bytes));
        Assert.Null(headers["Content-Length"]);
        Assert.Null(headers["Content-Type"]);
    }

    [Fact]
    public void GetSingleByteFileWithoutHeaderTest()
    {
        // arrange
        int size = 2048;
        byte fillByte = 13;
        byte[] bytes = new byte[size];
        string filename = "testfilename.dat";
        string url = DummyFileHelper.GetFileWithoutHeaderUrl(filename, size, fillByte);
        var dummyData = DummyData.GenerateSingleBytes(size, fillByte);

        // act
        ReadAndGetHeaders(url, bytes);

        // assert
        Assert.True(bytes.All(i => i == fillByte));
        Assert.True(dummyData.SequenceEqual(bytes));
        Assert.Null(headers["Content-Length"]);
        Assert.Null(headers["Content-Type"]);
    }

    [Fact]
    public void GetFileWithContentDispositionTest()
    {
        // arrange
        int size = 1024;
        byte[] bytes = new byte[size];
        string filename = "testfilename.dat";
        string url = DummyFileHelper.GetFileWithContentDispositionUrl(filename, size);
        var dummyData = DummyData.GenerateOrderedBytes(size);

        // act
        ReadAndGetHeaders(url, bytes);

        // assert
        Assert.True(dummyData.SequenceEqual(bytes));
        Assert.Equal(size.ToString(), headers["Content-Length"]);
        Assert.Equal(contentType, headers["Content-Type"]);
        Assert.Contains($"filename={filename};", headers["Content-Disposition"]);
    }

    [Fact]
    public void GetSingleByteFileWithContentDispositionTest()
    {
        // arrange
        int size = 1024;
        byte fillByte = 13;
        byte[] bytes = new byte[size];
        string filename = "testfilename.dat";
        string url = DummyFileHelper.GetFileWithContentDispositionUrl(filename, size, fillByte);
        var dummyData = DummyData.GenerateSingleBytes(size, fillByte);

        // act
        ReadAndGetHeaders(url, bytes);

        // assert
        Assert.True(bytes.All(i => i == fillByte));
        Assert.True(dummyData.SequenceEqual(bytes));
        Assert.Equal(size.ToString(), headers["Content-Length"]);
        Assert.Equal(contentType, headers["Content-Type"]);
        Assert.Contains($"filename={filename};", headers["Content-Disposition"]);
    }

    [Fact]
    public void GetFileWithRangeTest()
    {
        // arrange
        int size = 1024;
        byte[] bytes = new byte[size];
        string url = DummyFileHelper.GetFileUrl(size);
        var dummyData = DummyData.GenerateOrderedBytes(size);

        // act
        ReadAndGetHeaders(url, bytes, justFirst512Bytes: true);

        // assert
        Assert.True(dummyData.Take(512).SequenceEqual(bytes.Take(512)));
        Assert.Equal(contentType, headers["Content-Type"]);
        Assert.Equal("512", headers["Content-Length"]);
        Assert.Equal("bytes 0-511/1024", headers["Content-Range"]);
        Assert.Equal("bytes", headers["Accept-Ranges"]);
    }

    [Fact]
    public void GetFileWithNoAcceptRangeTest()
    {
        // arrange
        int size = 1024;
        byte[] bytes = new byte[size];
        string filename = "testfilename.dat";
        string url = DummyFileHelper.GetFileWithNoAcceptRangeUrl(filename, size);
        var dummyData = DummyData.GenerateOrderedBytes(size);

        // act
        ReadAndGetHeaders(url, bytes, justFirst512Bytes: true);

        // assert
        Assert.True(dummyData.SequenceEqual(bytes));
        Assert.Equal(size.ToString(), headers["Content-Length"]);
        Assert.Equal(contentType, headers["Content-Type"]);
        Assert.Null(headers["Accept-Ranges"]);
    }

    [Fact]
    public void GetSingleByteFileWithNoAcceptRangeTest()
    {
        // arrange
        int size = 1024;
        byte fillByte = 13;
        byte[] bytes = new byte[size];
        string filename = "testfilename.dat";
        string url = DummyFileHelper.GetFileWithNoAcceptRangeUrl(filename, size, fillByte);
        var dummyData = DummyData.GenerateSingleBytes(size, fillByte);

        // act
        ReadAndGetHeaders(url, bytes, justFirst512Bytes: true);

        // assert
        Assert.True(bytes.All(i => i == fillByte));
        Assert.True(dummyData.SequenceEqual(bytes));
        Assert.Equal(size.ToString(), headers["Content-Length"]);
        Assert.Equal(contentType, headers["Content-Type"]);
        Assert.Null(headers["Accept-Ranges"]);
    }

    [Fact]
    public void GetFileWithNameOnRedirectTest()
    {
        // arrange
        int size = 2048;
        byte[] bytes = new byte[size];
        string filename = "testfilename.dat";
        string url = DummyFileHelper.GetFileWithNameOnRedirectUrl(filename, size);
        var dummyData = DummyData.GenerateOrderedBytes(size);

        // act
        ReadAndGetHeaders(url, bytes);

        // assert
        Assert.True(dummyData.SequenceEqual(bytes));
        Assert.Equal(size.ToString(), headers["Content-Length"]);
        Assert.Equal(contentType, headers["Content-Type"]);
        Assert.NotEqual(url, headers[nameof(WebResponse.ResponseUri)]);
    }

    [Fact]
    public void GetFileWithFailureAfterOffsetTest()
    {
        // arrange
        int size = 10240;
        int failureOffset = size / 2;
        byte[] bytes = new byte[size];
        string url = DummyFileHelper.GetFileWithFailureAfterOffset(size, failureOffset);

        // act
        void getHeaders() => ReadAndGetHeaders(url, bytes, false);

        // assert
        Assert.ThrowsAny<IOException>(getHeaders);
        Assert.Equal(size.ToString(), headers["Content-Length"]);
        Assert.Equal(contentType, headers["Content-Type"]);
        Assert.Equal(0, bytes[size - 1]);
    }

    [Fact]
    public void GetFileWithTimeoutAfterOffsetTest()
    {
        // arrange
        int size = 10240;
        int timeoutOffset = size / 2;
        byte[] bytes = new byte[size];
        string url = DummyFileHelper.GetFileWithTimeoutAfterOffset(size, timeoutOffset);

        // act
        void getHeaders() => ReadAndGetHeaders(url, bytes, false);

        // assert
        Assert.ThrowsAny<IOException>(getHeaders);
        Assert.Equal(size.ToString(), headers["Content-Length"]);
        Assert.Equal(contentType, headers["Content-Type"]);
        Assert.Equal(0, bytes[size - 1]);
    }

    private void ReadAndGetHeaders(string url, byte[] bytes, bool justFirst512Bytes = false)
    {
        try
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Timeout = 10000; // 10sec
            if (justFirst512Bytes)
                request.AddRange(0, 511);

            using HttpWebResponse downloadResponse = request.GetResponse() as HttpWebResponse;
            var respStream = downloadResponse.GetResponseStream();

            // keep response headers
            downloadResponse.Headers.Add(nameof(WebResponse.ResponseUri), downloadResponse.ResponseUri.ToString());
            headers = downloadResponse.Headers;

            // read stream data
            var readCount = 1;
            var offset = 0;
            while (readCount > 0)
            {
                var count = bytes.Length - offset;
                if (count <= 0)
                    break;

                readCount = respStream.Read(bytes, offset, count);
                offset += readCount;
            }
        }
        catch (Exception exp)
        {
            Console.Error.WriteLine(exp.Message);
            Debugger.Break();
            throw;
        }
    }
}
