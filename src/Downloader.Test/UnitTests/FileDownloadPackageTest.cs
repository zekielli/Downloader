﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Downloader.Test.UnitTests
{
    [TestClass]
    public class FileDownloadPackageTest : DownloadPackageTest
    {
        [TestInitialize]
        public override async Task Initial()
        {
            Config = new DownloadConfiguration() { OnTheFlyDownload = false };
            await base.Initial();
        }
    }
}