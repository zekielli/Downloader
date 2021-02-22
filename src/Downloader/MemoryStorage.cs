﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Downloader
{
    [Serializable]
    public class MemoryStorage : IStorage, IDisposable, ISerializable
    {
        [NonSerialized] private MemoryStream _dataStream;
        public string Data
        {
            get
            {
                if (_dataStream?.CanRead == true)
                {
                    return Convert.ToBase64String(_dataStream.ToArray());
                }

                return null;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value) == false)
                {
                    Close();
                    _dataStream = new MemoryStream(Convert.FromBase64String(value));
                }
            }
        }

        public MemoryStorage()
        {
            _dataStream = new MemoryStream();
        }

        public MemoryStorage(SerializationInfo info, StreamingContext context)
        {
            if(info.ObjectType == typeof(MemoryStorage))
            {
                Data = info.GetValue(nameof(Data),typeof(string)) as string;
            }
        }

        public Stream OpenRead()
        {
            _dataStream.Flush();
            _dataStream.Seek(0, SeekOrigin.Begin);
            return _dataStream;
        }

        public async Task WriteAsync(byte[] data, int offset, int count)
        {
            count = Math.Min(count, data.Length);
            await _dataStream.WriteAsync(data, offset, count);
        }

        public void Clear()
        {
            Close();
            _dataStream = null;
        }

        public void Close()
        {
            _dataStream?.Dispose();
        }

        public long GetLength()
        {
            return _dataStream?.Length ?? 0;
        }

        public void Dispose()
        {
            Clear();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Data), Data, typeof(string));
        }
    }
}
