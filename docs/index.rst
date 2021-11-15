Downloader
=======================================

[![Windows x64](https://github.com/bezzad/Downloader/workflows/Windows%20x64/badge.svg)](https://github.com/bezzad/Downloader/actions/workflows/dotnet.yml)
[![Ubuntu x64](https://github.com/bezzad/Downloader/workflows/Ubuntu%20x64/badge.svg)](https://github.com/bezzad/Downloader/actions/workflows/dotnet-core.yml)
[![Build Status](https://ci.appveyor.com/api/projects/status/github/bezzad/downloader?branch=master&svg=true)](https://ci.appveyor.com/project/bezzad/downloader) 
[![codecov](https://codecov.io/gh/bezzad/downloader/branch/master/graph/badge.svg)](https://codecov.io/gh/bezzad/downloader)
[![NuGet](https://img.shields.io/nuget/dt/downloader.svg)](https://www.nuget.org/packages/downloader) 
[![NuGet](https://img.shields.io/nuget/vpre/downloader.svg)](https://www.nuget.org/packages/downloader)
[![CodeFactor](https://www.codefactor.io/repository/github/bezzad/downloader/badge/master)](https://www.codefactor.io/repository/github/bezzad/downloader/overview/master)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/f7cd6e24f75c45c28e5e6fab2ef8d219)](https://www.codacy.com/gh/bezzad/Downloader/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=bezzad/Downloader&amp;utm_campaign=Badge_Grade)
[![License](https://img.shields.io/github/license/bezzad/downloader.svg)](https://github.com/bezzad/downloader/blob/master/LICENSE)
[![Generic badge](https://img.shields.io/badge/support-.Net%20Framework_&_.Net%20Core-blue.svg)](https://github.com/bezzad/Downloader)

:rocket: Fast and reliable multipart downloader with **.Net Core 3.1+** supporting :rocket:

Downloader is a modern, fluent, asynchronous, testable and portable library for .NET. This is a multipart downloader with asynchronous progress events.
This library can added in your `.Net Core v3.1` and later or `.Net Framework v4.5` or later projects.

Downloader is compatible with .NET Standard 2.0 and above, running on Windows, Linux, and macOS, in full .NET Framework or .NET Core.

----------------------------------------------------

## Sample Console Application
![sample-project](https://github.com/bezzad/Downloader/raw/master/sample.gif)

----------------------------------------------------

## Features at a glance

- Simple interface to make download request.
- Download files async and non-blocking.
- Download any type of files like image, video, pdf, apk and etc.
- Cross-platform library to download any files with any size.
- Get real-time progress info of each block.
- Download file multipart as parallel.
- Handle all the client-side and server-side exceptions non-stopping.
- Config your `ChunkCount` to define the parts count of the download file.
- Download file multipart as `in-memory` or `in-temp files` cache mode.
- Store download package object to resume the download when you want.
- Get download speed or progress percentage in each progress event.
- Get download progress events per chunk downloads.
- Pause and Resume your downloads with package object.
- Supports large file download.
- Set a speed limit on downloads.
- Download files without storing on disk and get a memory stream for each downloaded file.
- Serializable download package (to/from `JSON` or `Binary`)
- Live streaming support, suitable for playing music at the same time as downloading.
- Download Manager to download and order many files as Parallel

----------------------------------------------------

## How to use

Get it on [NuGet](https://www.nuget.org/packages/Downloader):

    PM> Install-Package Downloader

Or via the .NET Core command line interface:

    dotnet add package Downloader

Create your custom configuration:
```csharp
var downloadOpt = new DownloadConfiguration()
{
    BufferBlockSize = 10240, // usually, hosts support max to 8000 bytes, default values is 8000
    ChunkCount = 8, // file parts to download, default value is 1
    MaximumBytesPerSecond = 1024 * 1024, // download speed limited to 1MB/s, default values is zero or unlimited
    MaxTryAgainOnFailover = int.MaxValue, // the maximum number of times to fail
    OnTheFlyDownload = false, // caching in-memory or not? default values is true
    ParallelDownload = true, // download parts of file as parallel or not. Default value is false
    TempDirectory = "C:\\temp", // Set the temp path for buffering chunk files, the default path is Path.GetTempPath()
    Timeout = 1000, // timeout (millisecond) per stream block reader, default values is 1000
    RequestConfiguration = // config and customize request headers
    {
        Accept = "*/*",
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        CookieContainer =  new CookieContainer(), // Add your cookies
        Headers = new WebHeaderCollection(), // Add your custom headers
        KeepAlive = false,
        ProtocolVersion = HttpVersion.Version11, // Default value is HTTP 1.1
        UseDefaultCredentials = false,
        UserAgent = $"DownloaderSample/{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}"
    }
};
```

So, declare download service instance per download and pass your config:
```csharp
var downloader = new DownloadService(downloadOpt);
```

Then handle download progress and completed events:
```csharp
// Provide `FileName` and `TotalBytesToReceive` at the start of each downloads
downloader.DownloadStarted += OnDownloadStarted;    

// Provide any information about chunker downloads, like progress percentage per chunk, speed, total received bytes and received bytes array to live streaming.
downloader.ChunkDownloadProgressChanged += OnChunkDownloadProgressChanged;

// Provide any information about download progress, like progress percentage of sum of chunks, total speed, average speed, total received bytes and received bytes array to live streaming.
downloader.DownloadProgressChanged += OnDownloadProgressChanged;

// Download completed event that can include occurred errors or cancelled or download completed successfully.
downloader.DownloadFileCompleted += OnDownloadFileCompleted;    
```

__Start the download asynchronously__
```csharp
string file = @"Your_Path\fileName.zip";
string url = @"https://file-examples.com/fileName.zip";
await downloader.DownloadFileTaskAsync(url, file);
```

__Download into a folder without file name__
```csharp
DirectoryInfo path = new DirectoryInfo("Your_Path");
string url = @"https://file-examples.com/fileName.zip";
await downloader.DownloadFileTaskAsync(url, path); // download into "Your_Path\fileName.zip"
```

__Download on MemoryStream__
```csharp
Stream destinationStream = await downloader.DownloadFileTaskAsync(url);
```

The ‍`DownloadService` class has a property called `Package` that stores each step of the download. To stopping or pause the download you must call the `CancelAsync` method, and if you want to continue again, you must call the same `DownloadFileTaskAsync` function with the `Package` parameter to resume your download! 
For example:

Keep `Package` file to resume from last download positions:
```csharp
DownloadPackage pack = downloader.Package; 
```

__Stop or Pause Download:__
```csharp
downloader.CancelAsync(); 
```

__Resume Download:__
```csharp
await downloader.DownloadFileTaskAsync(pack); 
```

So that you can even save your large downloads with a very small amount in the Package and after restarting the program, restore it again and start continuing your download. In fact, the packages are your instant download snapshots. If your download config has OnTheFlyDownload, the downloaded bytes ​​will be stored in the package itself, but otherwise, only the downloaded file address will be included and you can resume it whenever you like. 
For more detail see [StopResumeDownloadTest](https://github.com/bezzad/Downloader/blob/master/src/Downloader.Test/DownloadIntegrationTest.cs#L79) method


> Note: for complete sample see `Downloader.Sample` project from this repository.

----------------------------------------------------

## How to serialize and deserialize downloader package

Serialize download packages to `JSON` text or `Binary`, after stopping download to keep download data and resuming that every time you want. 
You can serialize packages even using memory storage for caching download data which is used `MemoryStream`.

__Serialize and Deserialize into Binary with [BinaryFormatter](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.formatters.binary.binaryformatter)__

To serialize or deserialize the package into a binary file, just you need to a [BinaryFormatter](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.formatters.binary.binaryformatter) of [IFormatter](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.iformatter) and then create a stream to write bytes on that:
```csharp
DownloadPackage pack = downloader.Package; 
IFormatter formatter = new BinaryFormatter();
Stream serializedStream = new MemoryStream();
```

Serializing package:
```csharp
formatter.Serialize(serializedStream, pack);
```

Deserializing into the new package:
```csharp
var newPack = formatter.Deserialize(serializedStream) as DownloadPackage;
```

For more detail see [PackageSerializationTest](https://github.com/bezzad/Downloader/blob/46167082b8de99d8e6ad21329c3a32a6e26cfd3e/src/Downloader.Test/DownloadPackageTest.cs#L51) method


__Serialize and Deserialize into `JSON` text with [Newtonsoft.Json](https://www.newtonsoft.com)__

Serializing the package to `JSON` is very simple like this:
```csharp
var serializedJson = Newtonsoft.Json.JsonConvert.SerializeObject(pack);
```

But to deserializing the [IStorage Storage](https://github.com/bezzad/Downloader/blob/e4ab807a2e107c9ae4902257ba82f71b33494d91/src/Downloader/Chunk.cs#L28) property of chunks you need to declare a [JsonConverter](https://github.com/bezzad/Downloader/blob/78085b7fb418e6160de444d2e97a5d2fa6ed8da0/src/Downloader.Test/StorageConverter.cs#L7) to override the Read method of `JsonConverter`. So you should add the below converter to your application:

```csharp
public class StorageConverter : Newtonsoft.Json.JsonConverter<IStorage>
{
    public override void WriteJson(JsonWriter writer, IStorage value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override IStorage ReadJson(JsonReader reader, Type objectType, IStorage existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var obj = JObject.Load(reader); // Throws an exception if the current token is not an object.
        if (obj.ContainsKey(nameof(FileStorage.FileName)))
        {
            var filename = obj[nameof(FileStorage.FileName)]?.Value<string>();
            return new FileStorage(filename);
        }

        if (obj.ContainsKey(nameof(MemoryStorage.Data)))
        {
            var data = obj[nameof(MemoryStorage.Data)]?.Value<string>();
            return new MemoryStorage() { Data = data };
        }

        return null;
    }
}
```

Then you can deserialize your packages from `JSON`:
```csharp
var settings = new Newtonsoft.Json.JsonSerializerSettings();
settings.Converters.Add(new StorageConverter());
var newPack = Newtonsoft.Json.JsonConvert.DeserializeObject<DownloadPackage>(serializedJson, settings);
```

For more detail see [PackageSerializationTest](https://github.com/bezzad/Downloader/blob/46167082b8de99d8e6ad21329c3a32a6e26cfd3e/src/Downloader.Test/DownloadPackageTest.cs#L34) method

----------------------------------------------------

### License
```
   MIT License

   Copyright (c) 2021 Behzad Khosravifar

   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all
   copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
   FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE 
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
   SOFTWARE.
```

### Contribute

Welcome to contribute, feel free to change and open a **PullRequest** to develop branch.

### IMPORTANT NOTES!

You can use either the latest version of Visual Studio or Visual Studio Code and .NET CLI for Windows, Mac and Linux.

**Note for Pull Requests (PRs):** We accept pull request from the community. When doing it, please do it onto the `develop` branch which is the consolidated work-in-progress branch. Do not request it onto `master` branch.
