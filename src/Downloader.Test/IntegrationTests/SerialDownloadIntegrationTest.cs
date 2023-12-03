﻿using Xunit.Abstractions;

namespace Downloader.Test.IntegrationTests;

public class SerialDownloadIntegrationTest : DownloadIntegrationTest
{
    public SerialDownloadIntegrationTest(ITestOutputHelper output) : base(output)
    {
        Config = new DownloadConfiguration {
            ParallelDownload = false,
            BufferBlockSize = 1024,
            ParallelCount = 4,
            ChunkCount = 4,
            MaxTryAgainOnFailover = 100
        };

        Downloader = new DownloadService(Config);
        Downloader.DownloadFileCompleted += DownloadFileCompleted;
    }
}
