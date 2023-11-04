﻿using Downloader.DummyHttpServer;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Downloader.Test.HelperTests;

public class DummyLazyStreamTest
{
    [Fact]
    public void GenerateOrderedBytesStreamTest()
    {
        // arrange
        int size = 1024;
        byte[] bytes = Enumerable.Range(0, size).Select(i => (byte)i).ToArray();
        var memBuffer = new MemoryStream();

        // act
        var dummyData = new DummyLazyStream(DummyDataType.Order, size).ToArray();

        // assert
        Assert.Equal(size, dummyData.Length);
        Assert.True(dummyData.SequenceEqual(bytes));
    }

    [Fact]
    public void GenerateOrderedBytesStreamLessThan1Test()
    {
        // arrange
        int size = 0;

        // act
        void act() => new DummyLazyStream(DummyDataType.Order, size);

        // assert
        Assert.ThrowsAny<ArgumentException>(act);
    }

    [Fact]
    public void GenerateRandomBytesStreamTest()
    {
        // arrange
        int size = 1024;

        // act
        var dummyData = new DummyLazyStream(DummyDataType.Random, size).ToArray();

        // assert
        Assert.Equal(size, dummyData.Length);
        Assert.Contains(dummyData, i => i > 0);
    }

    [Fact]
    public void GenerateRandomBytesLessThan1Test()
    {
        // arrange
        int size = 0;

        // act
        void act() => new DummyLazyStream(DummyDataType.Random, size);

        // assert
        Assert.ThrowsAny<ArgumentException>(act);
    }

    [Fact]
    public void GenerateSingleBytesTest()
    {
        // arrange
        int size = 1024;
        byte fillByte = 13;

        // act
        var dummyData = new DummyLazyStream(DummyDataType.Single, size, fillByte).ToArray();

        // assert
        Assert.Equal(size, dummyData.Length);
        Assert.True(dummyData.All(i => i == fillByte));
    }
}
