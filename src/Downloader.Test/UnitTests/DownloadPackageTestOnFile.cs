﻿using Downloader.DummyHttpServer;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Downloader.Test.UnitTests;

public class DownloadPackageTestOnFile : DownloadPackageTest
{
    private string _path;

    public override async Task InitializeAsync()
    {
        _path = Path.GetTempFileName();

        Package = new DownloadPackage() {
            FileName = _path,
            Urls = new[] { DummyFileHelper.GetFileWithNameUrl(DummyFileHelper.SampleFile16KbName, DummyFileHelper.FileSize16Kb) },
            TotalFileSize = DummyFileHelper.FileSize16Kb
        };

        await base.InitializeAsync();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        File.Delete(_path);
    }

    [Theory]
    [InlineData(true)]  // BuildStorageWithReserveSpaceTest
    [InlineData(false)] // BuildStorageTest
    public void BuildStorageTest(bool reserveSpace)
    {
        // arrange
        _path = Path.GetTempFileName();
        Package = new DownloadPackage() {
            FileName = _path,
            Urls = new[] { DummyFileHelper.GetFileWithNameUrl(DummyFileHelper.SampleFile16KbName, DummyFileHelper.FileSize16Kb) },
            TotalFileSize = DummyFileHelper.FileSize16Kb
        };

        // act
        Package.BuildStorage(reserveSpace, 1024 * 1024);
        using var stream = Package.Storage.OpenRead();

        // assert
        Assert.IsType<FileStream>(stream);
        Assert.Equal(reserveSpace ? DummyFileHelper.FileSize16Kb : 0, Package.Storage.Length);
    }
}
