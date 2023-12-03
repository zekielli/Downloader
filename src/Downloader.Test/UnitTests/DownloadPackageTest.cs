﻿using Downloader.DummyHttpServer;
using Downloader.Test.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Downloader.Test.UnitTests;

public abstract class DownloadPackageTest : IAsyncLifetime
{
    protected DownloadConfiguration Config { get; set; }
    protected DownloadPackage Package { get; set; }
    protected byte[] Data { get; set; }

    public virtual async Task InitializeAsync()
    {
        Config = new DownloadConfiguration() { ChunkCount = 8 };
        Data = DummyData.GenerateOrderedBytes(DummyFileHelper.FileSize16Kb);
        Package.BuildStorage(false, 1024 * 1024);
        new ChunkHub(Config).SetFileChunks(Package);
        await Package.Storage.WriteAsync(0, Data, DummyFileHelper.FileSize16Kb);
        await Package.Storage.FlushAsync();
    }

    public virtual Task DisposeAsync()
    {
        Package?.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public void PackageSerializationTest()
    {
        // act
        var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(Package);
        Package.Storage.Dispose();
        var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<DownloadPackage>(serialized);
        var destData = new byte[deserialized.TotalFileSize];
        deserialized.Storage.OpenRead().Read(destData, 0, destData.Length);

        // assert
        AssertHelper.AreEquals(Package, deserialized);
        Assert.True(Data.SequenceEqual(destData));

        deserialized.Clear();
        deserialized.Storage.Dispose();
    }

    [Fact]
    public void ClearChunksTest()
    {
        // act
        Package.Clear();

        // assert
        Assert.Null(Package.Chunks);
    }

    [Fact]
    public void ClearPackageTest()
    {
        // act
        Package.Clear();

        // assert
        Assert.Equal(0, Package.ReceivedBytesSize);
    }

    [Fact]
    public void PackageValidateTest()
    {
        // arrange
        Package.Chunks[0].Position = Package.Storage.Length;

        // act
        Package.Validate();

        // assert
        Assert.Equal(0, Package.Chunks[0].Position);
    }

    [Fact]
    public void TestPackageValidateWhenDoesNotSupportDownloadInRange()
    {
        // arrange
        Package.Chunks[0].Position = 1000;
        Package.IsSupportDownloadInRange = false;

        // act
        Package.Validate();

        // assert
        Assert.Equal(0, Package.Chunks[0].Position);
    }
}
