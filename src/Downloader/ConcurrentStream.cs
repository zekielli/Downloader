﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Downloader
{
    public class ConcurrentStream : IDisposable
    {
        private readonly SemaphoreSlim _queueConsumerLocker = new SemaphoreSlim(0);
        private readonly ManualResetEventSlim _completionEvent = new ManualResetEventSlim(true);
        private readonly ManualResetEventSlim _addingBlocker = new ManualResetEventSlim(true);
        private readonly ConcurrentBag<Packet> _inputQueue = new ConcurrentBag<Packet>();
        private long _maxMemoryBufferBytes = 0;
        private bool _disposed;
        private Stream _stream;
        private string _path;

        public string Path
        {
            get => _path;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _path = value;
                    _stream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                }
            }
        }

        public byte[] Data
        {
            get
            {
                Flush();
                if (_stream is MemoryStream mem)
                    return mem.ToArray();

                return null;
            }
            set
            {
                if (value != null)
                    _stream = new MemoryStream(value, true);
            }
        }

        public long Length => _stream?.Length ?? 0;

        public long MaxMemoryBufferBytes
        {
            get => _maxMemoryBufferBytes;
            set
            {
                _maxMemoryBufferBytes = (value <= 0) ? long.MaxValue : value;
            }
        }

        public ConcurrentStream(Stream stream, long maxMemoryBufferBytes = 0)
        {
            _stream = stream;
            MaxMemoryBufferBytes = maxMemoryBufferBytes;
            Initial();
        }

        public ConcurrentStream(string filename, long initSize, long maxMemoryBufferBytes = 0)
        {
            _path = filename;
            _stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            MaxMemoryBufferBytes = maxMemoryBufferBytes;

            if (initSize > 0)
                _stream.SetLength(initSize);

            Initial();
        }

        // parameterless constructor for deserialization
        public ConcurrentStream()
        {
            _stream = new MemoryStream();
            Initial();
        }

        private void Initial()
        {
            Task.Run(Watcher).ConfigureAwait(false);
        }

        public Stream OpenRead()
        {
            _completionEvent.Wait();
            if (_stream?.CanSeek == true)
                _stream.Seek(0, SeekOrigin.Begin);

            return _stream;
        }

        public void WriteAsync(long position, byte[] bytes, int length)
        {
            _addingBlocker.Wait();
            _inputQueue.Add(new Packet(position, bytes, length));
            _completionEvent.Reset();
            _queueConsumerLocker.Release();
            StopWritingToQueueIfLimitIsExceeded(length);
        }

        private async Task Watcher()
        {
            while (!_disposed)
            {
                ResumeWriteOnQueueIfBufferEmpty();
                await _queueConsumerLocker.WaitAsync().ConfigureAwait(false);
                if (_inputQueue.TryTake(out Packet packet))
                {
                    await WritePacketOnFile(packet).ConfigureAwait(false);
                }
            }
        }

        private void StopWritingToQueueIfLimitIsExceeded(long packetSize)
        {
            if (MaxMemoryBufferBytes < packetSize * _inputQueue.Count)
            {
                // Stop writing packets to the queue until the memory is free
                _addingBlocker.Reset();
            }
        }

        private void ResumeWriteOnQueueIfBufferEmpty()
        {
            if (_inputQueue.IsEmpty)
            {
                // resume writing packets to the queue
                _addingBlocker.Set();
                _completionEvent.Set();
            }
        }

        private async Task WritePacketOnFile(Packet packet)
        {
            if (_stream.CanSeek)
            {
                _stream.Position = packet.Position;
                await _stream.WriteAsync(packet.Data, 0, packet.Length).ConfigureAwait(false);
                packet.Dispose();
            }
        }

        public void Flush()
        {
            _completionEvent.Wait();
            _stream?.Flush();
            GC.Collect();
        }

        public void Dispose()
        {
            if (_disposed == false)
            {
                Flush();
                _disposed = true;
                _queueConsumerLocker.Dispose();
                _stream.Dispose();
            }
        }
    }
}
