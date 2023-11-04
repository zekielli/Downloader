﻿using Xunit;
using System;
using System.Linq;

namespace Downloader.Test.Helper;

public static class AssertHelper
{
    public static void DoesNotThrow<T>(Action action) where T : Exception
    {
        try
        {
            action();
        }
        catch (T)
        {
            Assert.Fail($"Expected no {typeof(T).Name} to be thrown");
        }
        catch
        {
            return;
        }
    }

    public static void AreEquals(Chunk source, Chunk destination)
    {
        Assert.NotNull(source);
        Assert.NotNull(destination);

        foreach (var prop in typeof(Chunk).GetProperties().Where(p => p.CanRead && p.CanWrite))
        {
            Assert.Equal(prop.GetValue(source), prop.GetValue(destination));
        }
    }

    public static void AreEquals(DownloadPackage source, DownloadPackage destination)
    {
        Assert.NotNull(source);
        Assert.NotNull(destination);
        Assert.NotNull(source.Chunks);
        Assert.NotNull(destination.Chunks);
        Assert.Equal(source.FileName, destination.FileName);
        Assert.Equal(source.ReceivedBytesSize, destination.ReceivedBytesSize);
        Assert.Equal(source.TotalFileSize, destination.TotalFileSize);
        Assert.Equal(source.IsSaving, destination.IsSaving);
        Assert.Equal(source.IsSaveComplete, destination.IsSaveComplete);
        Assert.Equal(source.SaveProgress, destination.SaveProgress);
        Assert.Equal(source.Chunks?.Length, destination.Chunks?.Length);
        Assert.Equal(source.IsSupportDownloadInRange, destination.IsSupportDownloadInRange);
        Assert.Equal(source.InMemoryStream, destination.InMemoryStream);
        Assert.Equal(source.Storage.Path, destination.Storage.Path);
        //Assert.Equal(source.Storage.Length, destination.Storage.Length);
        Assert.True(source.Urls.SequenceEqual(destination.Urls));

        for (int i = 0; i < source.Chunks.Length; i++)
        {
            AreEquals(source.Chunks[i], destination.Chunks[i]);
        }
    }
}
