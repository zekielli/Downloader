﻿using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Downloader.Test
{
    [TestClass]
    public class ChunkTest
    {
        private readonly byte[] _testData = DummyData.GenerateOrderedBytes(1024);

        [TestMethod]
        public void ClearTest()
        {
            // arrange
            var chunk = new Chunk(0, 1000) { Position = 100, Timeout = 100 };
            chunk.CanTryAgainOnFailover();

            // act
            chunk.Clear();

            // assert
            Assert.AreEqual(0, chunk.Position);
            Assert.AreEqual(0, chunk.Timeout);
            Assert.AreEqual(0, chunk.FailoverCount);
        }

        [TestMethod]
        public void ClearFileStorageTest()
        {
            // arrange
            var chunk = new Chunk(0, 1000) { Storage = new FileStorage("") };
            chunk.Storage.WriteAsync(_testData, 0, 5).Wait();

            // act
            chunk.Clear();

            // assert
            Assert.AreEqual(0, chunk.Storage.GetLength());
        }

        [TestMethod]
        public void ClearMemoryStorageTest()
        {
            // arrange
            var chunk = new Chunk(0, 1000) { Storage = new MemoryStorage() };
            chunk.Storage.WriteAsync(_testData, 0, 5).Wait();

            // act
            chunk.Clear();

            // assert
            Assert.AreEqual(0, chunk.Storage.GetLength());
        }

        [TestMethod]
        public void IsDownloadCompletedOnBeginTest()
        {
            // arrange
            var size = 1024;
            var chunk = new Chunk(0, size) { Storage = (new MemoryStorage()) };

            // act
            bool isDownloadCompleted = chunk.IsDownloadCompleted();

            // assert
            Assert.IsFalse(isDownloadCompleted);
        }

        [TestMethod]
        public void IsDownloadCompletedWhenNoStorageTest()
        {
            // arrange
            var size = 1024;
            var chunk = new Chunk(0, size) {
                Position = size-1
            };

            // act
            bool isDownloadCompleted = chunk.IsDownloadCompleted();

            // assert
            Assert.IsFalse(isDownloadCompleted);
        }

        [TestMethod]
        public void IsDownloadCompletedWhenFileStorageNoDataTest()
        {
            // arrange
            var size = 1024;
            var chunk = new Chunk(0, size) { Position = size - 1, Storage = (new FileStorage("")) };

            // act
            bool isDownloadCompleted = chunk.IsDownloadCompleted();

            // assert
            Assert.IsFalse(isDownloadCompleted);
        }

        [TestMethod]
        public void IsDownloadCompletedWhenMemoryStorageNoDataTest()
        {
            // arrange
            var size = 1024;
            var chunk = new Chunk(0, size) { Position = size - 1, Storage = (new MemoryStorage()) };

            // act
            bool isDownloadCompleted = chunk.IsDownloadCompleted();

            // assert
            Assert.IsFalse(isDownloadCompleted);
        }

        [TestMethod]
        public void IsDownloadCompletedWhenMemoryStorageDataIsExistTest()
        {
            // arrange
            var chunk = new Chunk(0, _testData.Length - 1) { Position = _testData.Length - 1, Storage = new MemoryStorage() };
            chunk.Storage.WriteAsync(_testData, 0, _testData.Length).Wait();

            // act
            bool isDownloadCompleted = chunk.IsDownloadCompleted();

            // assert
            Assert.IsTrue(isDownloadCompleted);
        }

        [TestMethod]
        public void IsDownloadCompletedWhenFileStorageDataIsExistTest()
        {
            // arrange
            var chunk = new Chunk(0, _testData.Length - 1) { Position = _testData.Length - 1, Storage = new FileStorage("") };
            chunk.Storage.WriteAsync(_testData, 0, _testData.Length).Wait();

            // act
            bool isDownloadCompleted = chunk.IsDownloadCompleted();

            // assert
            Assert.IsTrue(isDownloadCompleted);
        }

        [TestMethod]
        public void IsValidPositionWithMemoryStorageTest()
        {
            // arrange
            var size = 1024;
            var chunk = new Chunk(0, size) { Storage = new MemoryStorage() };

            // act
            bool isValidPosition = chunk.IsValidPosition();

            // assert
            Assert.IsTrue(isValidPosition);
        }

        [TestMethod]
        public void IsValidPositionWithFileStorageTest()
        {
            // arrange
            var size = 1024;
            var chunk = new Chunk(0, size) { Storage = new FileStorage("") };

            // act
            bool isValidPosition = chunk.IsValidPosition();

            // assert
            Assert.IsTrue(isValidPosition);
        }

        [TestMethod]
        public void IsValidPositionOnOverflowTest()
        {
            // arrange
            var chunk = new Chunk(0, _testData.Length - 1) {
                Position = _testData.Length,
                Storage = new MemoryStorage() // overflowed
            };

            // act
            bool isValidPosition = chunk.IsValidPosition();

            // assert
            Assert.IsFalse(isValidPosition);
        }

        [TestMethod]
        public void IsValidPositionWithEqualStorageSizeTest()
        {
            // arrange
            var chunk = new Chunk(0, _testData.Length - 1) { Position = 7, Storage = new MemoryStorage() };
            chunk.Storage.WriteAsync(_testData, 0, 7);

            // act
            bool isValidPosition = chunk.IsValidPosition();

            // assert
            Assert.IsTrue(isValidPosition);
        }

        [TestMethod]
        public void IsValidPositionWithNoEqualStorageSizeTest()
        {
            // arrange
            var size = 1024;
            var chunk = new Chunk(0, size - 1) { Position = 10, Storage = (new MemoryStorage()) };
            chunk.Storage.WriteAsync(new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 }, 0, 10);

            // act
            bool isValidPosition = chunk.IsValidPosition();

            // assert
            Assert.IsFalse(isValidPosition);
        }

        [TestMethod]
        public void IsValidPositionWhenNoStorageAndPositivePositionTest()
        {
            // arrange
            var chunk = new Chunk(0, 1024) {
                Position = 1
            };

            // act
            bool isValidPosition = chunk.IsValidPosition();

            // assert
            Assert.IsFalse(isValidPosition);
        }

        [TestMethod]
        public void IsValidPositionWhenNoStorageAndZeroPositionTest()
        {
            // arrange
            var chunk = new Chunk(0, 1024) {
                Position = 0
            };

            // act
            bool isValidPosition = chunk.IsValidPosition();

            // assert
            Assert.IsTrue(isValidPosition);
        }

        [TestMethod]
        public void IsValidPositionOnZeroSizeTest()
        {
            // arrange
            var chunk = new Chunk(0, -1) { Position = 0, Storage = new MemoryStorage() };

            // act
            bool isValidPosition = chunk.IsValidPosition();

            // assert
            Assert.IsTrue(isValidPosition);
        }

        [TestMethod]
        public void SetValidPositionOnOverflowTest()
        {
            // arrange
            var chunk = new Chunk(0, 1023) {
                Position = 1024,
                Storage = new MemoryStorage() // overflowed
            };

            // act
            chunk.SetValidPosition();

            // assert
            Assert.AreEqual(0, chunk.Position);
        }

        [TestMethod]
        public void SetValidPositionWhenNoStorageAndPositivePositionTest()
        {
            // arrange
            var chunk = new Chunk(0, 1024) {
                Position = 1
            };

            // act
            chunk.SetValidPosition();

            // assert
            Assert.AreEqual(0, chunk.Position);
        }

        [TestMethod]
        public void SetValidPositionWithStorageAndPositivePositionTest()
        {
            // arrange
            var chunk = new Chunk(0, 1024) { Position = 1, Storage = new MemoryStorage() };

            // act
            chunk.SetValidPosition();

            // assert
            Assert.AreEqual(0, chunk.Position);
        }

        [TestMethod]
        public void ChunkSerializationWhenFileStorageTest()
        {
            // arrange
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StorageConverter());
            var chunk = new Chunk(1024, 1024 + _testData.Length) {
                Position = 1, 
                Timeout = 1000,
                MaxTryAgainOnFailover = 3000,
                Storage = new FileStorage()
            };
            chunk.Storage.WriteAsync(_testData, 0, _testData.Length).Wait();

            // act
            var serializedChunk = JsonConvert.SerializeObject(chunk);
            chunk.Storage.Close();
            var deserializedChunk = JsonConvert.DeserializeObject<Chunk>(serializedChunk, settings);

            // assert
            ChunksAreEqual(chunk, deserializedChunk);

            chunk.Clear();
        }

        [TestMethod]
        public void ChunkBinarySerializationWhenFileStorageTest()
        {
            // arrange
            IFormatter formatter = new BinaryFormatter();

            var chunk = new Chunk(1024, 1024 + _testData.Length) {
                Position = 1,
                Timeout = 1000,
                MaxTryAgainOnFailover = 3000,
                Storage = new FileStorage()
            };
            chunk.Storage.WriteAsync(_testData, 0, _testData.Length).Wait();
            using var serializedChunk = new MemoryStream();

            // act
            formatter.Serialize(serializedChunk, chunk);
            chunk.Storage.Close();
            serializedChunk.Flush();
            serializedChunk.Seek(0, SeekOrigin.Begin);
            var deserializedChunk = formatter.Deserialize(serializedChunk) as Chunk;

            // assert
            ChunksAreEqual(chunk, deserializedChunk);

            chunk.Clear();
        }

        private void ChunksAreEqual(Chunk source, Chunk destination)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(destination);
            Assert.AreEqual(source.Id, destination.Id);
            Assert.AreEqual(source.Start, destination.Start);
            Assert.AreEqual(source.End, destination.End);
            Assert.AreEqual(source.Length, destination.Length);
            Assert.AreEqual(source.Position, destination.Position);
            Assert.AreEqual(source.Timeout, destination.Timeout);
            Assert.AreEqual(source.MaxTryAgainOnFailover, destination.MaxTryAgainOnFailover);
            Assert.AreEqual(source.Storage.GetLength(), destination.Storage.GetLength());
        }
    }
}
