using System;
using System.IO;
using System.Text;

namespace SixLabors.Fonts
{
    /// <summary>
    /// BinaryReader using bigendian encoding.
    /// </summary>
    internal class BinaryReader : IDisposable
    {
        /// <summary>
        /// Buffer used for temporary storage before conversion into primitives
        /// </summary>
        private readonly byte[] storageBuffer = new byte[16];

        private readonly bool leaveOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryReader" /> class.
        /// Constructs a new binary reader with the given bit converter, reading
        /// to the given stream, using the given encoding.
        /// </summary>
        /// <param name="stream">Stream to read data from</param>
        /// <param name="leaveOpen">if set to <c>true</c> [leave open].</param>
        public BinaryReader(Stream stream, bool leaveOpen)
        {
            this.BaseStream = stream;
            this.StartOfStream = stream.Position;
            this.leaveOpen = leaveOpen;
            this.BitConverter = new BigEndianBitConverter();
        }

        private long StartOfStream { get; }

        /// <summary>
        /// Gets the underlying stream of the EndianBinaryReader.
        /// </summary>
        private Stream BaseStream { get; }

        /// <summary>
        /// Gets the bit converter used to read values from the stream.
        /// </summary>
        internal BigEndianBitConverter BitConverter { get; }

        /// <summary>
        /// Closes the reader, including the underlying stream.
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Seeks within the stream.
        /// </summary>
        /// <param name="offset">Offset to seek to.</param>
        /// <param name="origin">Origin of seek operation.</param>
        public void Seek(long offset, SeekOrigin origin)
        {
            // if begin offsert the offset by the start of stream postion
            if (origin == SeekOrigin.Begin)
            {
                offset = offset + this.StartOfStream;
            }

            this.BaseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Seeks within the stream.
        /// </summary>
        /// <param name="header">The header.</param>
        public void Seek(Tables.TableHeader header)
        {
            this.BaseStream.Seek(header.Offset, SeekOrigin.Begin);
        }

        /// <summary>
        /// Reads a single byte from the stream.
        /// </summary>
        /// <returns>The byte read</returns>
        public byte ReadByte()
        {
            this.ReadInternal(this.storageBuffer, 1);
            return this.storageBuffer[0];
        }

        /// <summary>
        /// Reads a single signed byte from the stream.
        /// </summary>
        /// <returns>The byte read</returns>
        public sbyte ReadSByte()
        {
            this.ReadInternal(this.storageBuffer, 1);
            return unchecked((sbyte)this.storageBuffer[0]);
        }

        /// <summary>
        /// Reads a boolean from the stream. 1 byte is read.
        /// </summary>
        /// <returns>The boolean read</returns>
        public bool ReadBoolean()
        {
            this.ReadInternal(this.storageBuffer, 1);
            return this.BitConverter.ToBoolean(this.storageBuffer, 0);
        }

        public float ReadF2dot14()
        {
            const float F2Dot14ToFloat = 16384.0f;

            this.ReadInternal(this.storageBuffer, 2);
            return this.BitConverter.ToInt16(this.storageBuffer, 0) / F2Dot14ToFloat;
        }

        /// <summary>
        /// Reads a 16-bit signed integer from the stream, using the bit converter
        /// for this reader. 2 bytes are read.
        /// </summary>
        /// <returns>The 16-bit integer read</returns>
        public short ReadInt16()
        {
            this.ReadInternal(this.storageBuffer, 2);
            return this.BitConverter.ToInt16(this.storageBuffer, 0);
        }

        public TEnum ReadInt16<TEnum>()
        {
            return CastTo<TEnum>.From(this.ReadInt16());
        }

        public short ReadFWORD()
        {
            return this.ReadInt16();
        }

        public ushort ReadUFWORD()
        {
            return this.ReadUInt16();
        }

        /// <summary>
        /// Reads a 32-bit signed integer from the stream, using the bit converter
        /// for this reader. 4 bytes are read.
        /// </summary>
        /// <returns>The 32-bit integer read</returns>
        public float ReadFixed()
        {
            this.ReadInternal(this.storageBuffer, 4);
            var int32 = this.BitConverter.ToInt32(this.storageBuffer, 0);
            return this.BitConverter.Int32BitsToSingle(int32);
        }

        /// <summary>
        /// Reads a 64-bit signed integer from the stream, using the bit converter
        /// for this reader. 8 bytes are read.
        /// </summary>
        /// <returns>The 64-bit integer read</returns>
        public long ReadInt64()
        {
            this.ReadInternal(this.storageBuffer, 8);
            return this.BitConverter.ToInt64(this.storageBuffer, 0);
        }

        /// <summary>
        /// Reads a 16-bit unsigned integer from the stream, using the bit converter
        /// for this reader. 2 bytes are read.
        /// </summary>
        /// <returns>The 16-bit unsigned integer read</returns>
        public ushort ReadUInt16()
        {
            this.ReadInternal(this.storageBuffer, 2);
            return this.BitConverter.ToUInt16(this.storageBuffer, 0);
        }

        public TEnum ReadUInt16<TEnum>()
        {
            return CastTo<TEnum>.From(this.ReadUInt16());
        }

        /// <summary>
        /// Reads array or 16-bit unsigned integers from the stream, using the bit converter
        /// for this reader. 2 bytes are read.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>
        /// The 16-bit unsigned integer read
        /// </returns>
        public ushort[] ReadUInt16Array(int length)
        {
            var data = new ushort[length];
            for (var i = 0; i < length; i++)
            {
                data[i] = this.ReadUInt16();
            }

            return data;
        }

        /// <summary>
        /// Reads array or 16-bit unsigned integers from the stream, using the bit converter
        /// for this reader. 2 bytes are read.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>
        /// The 16-bit unsigned integer read
        /// </returns>
        public uint[] ReadUInt32Array(int length)
        {
            var data = new uint[length];
            for (var i = 0; i < length; i++)
            {
                data[i] = this.ReadUInt32();
            }

            return data;
        }

        /// <summary>
        /// Reads array or 16-bit unsigned integers from the stream, using the bit converter
        /// for this reader. 2 bytes are read.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>
        /// The 16-bit unsigned integer read
        /// </returns>
        public ushort[] Offset16Array(int length)
        {
            return this.ReadUInt16Array(length);
        }

        /// <summary>
        /// Reads array or 16-bit unsigned integers from the stream, using the bit converter
        /// for this reader. 2 bytes are read.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>
        /// The 16-bit unsigned integer read
        /// </returns>
        public uint[] Offset32Array(int length)
        {
            return this.ReadUInt32Array(length);
        }

        public byte[] ReadUInt8Array(int length)
        {
            var data = new byte[length];
            for (var i = 0; i < length; i++)
            {
                data[i] = this.ReadUInt8();
            }

            return data;
        }

        public TEnum[] ReadUInt8Array<TEnum>(int length)
             where TEnum : struct
        {
            var data = new TEnum[length];
            for (var i = 0; i < length; i++)
            {
                data[i] = CastTo<TEnum>.From(this.ReadUInt8());
            }

            return data;
        }

        /// <summary>
        /// Reads a 16-bit unsigned integer from the stream, using the bit converter
        /// for this reader. 2 bytes are read.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>
        /// The 16-bit unsigned integer read
        /// </returns>
        public short[] ReadInt16Array(int length)
        {
            var data = new short[length];
            for (var i = 0; i < length; i++)
            {
                data[i] = this.ReadInt16();
            }

            return data;
        }

        /// <summary>
        /// Reads a 8-bit unsigned integer from the stream, using the bit converter
        /// for this reader. 1 bytes are read.
        /// </summary>
        /// <returns>The 8-bit unsigned integer read</returns>
        public byte ReadUInt8()
        {
            this.ReadInternal(this.storageBuffer, 1);
            return this.storageBuffer[0];
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer from the stream, using the bit converter
        /// for this reader. 4 bytes are read.
        /// </summary>
        /// <returns>The 32-bit unsigned integer read</returns>
        public uint ReadUInt32()
        {
            this.ReadInternal(this.storageBuffer, 4);
            return this.BitConverter.ToUInt32(this.storageBuffer, 0);
        }

        public uint ReadOffset32()
        {
            return this.ReadUInt32();
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer from the stream, using the bit converter
        /// for this reader. 8 bytes are read.
        /// </summary>
        /// <returns>The 64-bit unsigned integer read</returns>
        public ulong ReadUInt64()
        {
            this.ReadInternal(this.storageBuffer, 8);
            return this.BitConverter.ToUInt64(this.storageBuffer, 0);
        }

        /// <summary>
        /// Reads a single-precision floating-point value from the stream, using the bit converter
        /// for this reader. 4 bytes are read.
        /// </summary>
        /// <returns>The floating point value read</returns>
        public float ReadSingle()
        {
            this.ReadInternal(this.storageBuffer, 4);
            return this.BitConverter.ToSingle(this.storageBuffer, 0);
        }

        /// <summary>
        /// Reads a double-precision floating-point value from the stream, using the bit converter
        /// for this reader. 8 bytes are read.
        /// </summary>
        /// <returns>The floating point value read</returns>
        public double ReadDouble()
        {
            this.ReadInternal(this.storageBuffer, 8);
            return this.BitConverter.ToDouble(this.storageBuffer, 0);
        }

        /// <summary>
        /// Reads a decimal value from the stream, using the bit converter
        /// for this reader. 16 bytes are read.
        /// </summary>
        /// <returns>The decimal value read</returns>
        public decimal ReadDecimal()
        {
            this.ReadInternal(this.storageBuffer, 16);
            return this.BitConverter.ToDecimal(this.storageBuffer, 0);
        }

        /// <summary>
        /// Reads the specified number of bytes into the given buffer, starting at
        /// the given index.
        /// </summary>
        /// <param name="buffer">The buffer to copy data into</param>
        /// <param name="index">The first index to copy data into</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The number of bytes actually read. This will only be less than
        /// the requested number of bytes if the end of the stream is reached.
        /// </returns>
        public int Read(byte[] buffer, int index, int count)
        {
            int read = 0;
            while (count > 0)
            {
                int block = this.BaseStream.Read(buffer, index, count);
                if (block == 0)
                {
                    return read;
                }

                index += block;
                read += block;
                count -= block;
            }

            return read;
        }

        /// <summary>
        /// Reads the specified number of bytes, returning them in a new byte array.
        /// If not enough bytes are available before the end of the stream, this
        /// method will return what is available.
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The bytes read</returns>
        public byte[] ReadBytes(int count)
        {
            byte[] ret = new byte[count];
            int index = 0;
            while (index < count)
            {
                int read = this.BaseStream.Read(ret, index, count - index);

                // Stream has finished half way through. That's fine, return what we've got.
                if (read == 0)
                {
                    byte[] copy = new byte[index];
                    Buffer.BlockCopy(ret, 0, copy, 0, index);
                    return copy;
                }

                index += read;
            }

            return ret;
        }

        /// <summary>
        /// Reads the specified number of bytes, returning them in a new byte array.
        /// If not enough bytes are available before the end of the stream, this
        /// method will throw an IOException.
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The bytes read</returns>
        public byte[] ReadBytesOrThrow(int count)
        {
            byte[] ret = new byte[count];
            this.ReadInternal(ret, count);
            return ret;
        }

        /// <summary>
        /// Reads a 7-bit encoded integer from the stream. This is stored with the least significant
        /// information first, with 7 bits of information per byte of value, and the top
        /// bit as a continuation flag. This method is not affected by the endianness
        /// of the bit converter.
        /// </summary>
        /// <returns>The 7-bit encoded integer read from the stream.</returns>
        public int Read7BitEncodedInt()
        {
            int ret = 0;
            for (int shift = 0; shift < 35; shift += 7)
            {
                int b = this.BaseStream.ReadByte();
                if (b == -1)
                {
                    throw new EndOfStreamException();
                }

                ret = ret | ((b & 0x7f) << shift);
                if ((b & 0x80) == 0)
                {
                    return ret;
                }
            }

            // Still haven't seen a byte with the high bit unset? Dodgy data.
            throw new IOException("Invalid 7-bit encoded integer in stream.");
        }

        /// <summary>
        /// Reads a string of a specific length, which specifies the number of bytes
        /// to read from the stream. These bytes are then converted into a string with
        /// the encoding for this reader.
        /// </summary>
        /// <param name="bytesToRead">The bytes to read.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        /// The string read from the stream.
        /// </returns>
        public string ReadString(int bytesToRead, Encoding encoding)
        {
            byte[] data = new byte[bytesToRead];
            this.ReadInternal(data, bytesToRead);
            return encoding.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// Reads the uint32 string.
        /// </summary>
        /// <returns>a 4 character long UTF8 encoded string</returns>
        public string ReadTag()
        {
            return this.ReadString(4, Encoding.UTF8);
        }

        /// <summary>
        /// Reads the given number of bytes from the stream, throwing an exception
        /// if they can't all be read.
        /// </summary>
        /// <param name="data">Buffer to read into</param>
        /// <param name="size">Number of bytes to read</param>
        private void ReadInternal(byte[] data, int size)
        {
            int index = 0;
            while (index < size)
            {
                int read = this.BaseStream.Read(data, index, size - index);
                if (read == 0)
                {
                    throw new EndOfStreamException($"End of stream reached with {size - index} byte{(size - index == 1 ? "s" : string.Empty)} left to read.");
                }

                index += read;
            }
        }

        /// <summary>
        /// Reads the given number of bytes from the stream if possible, returning
        /// the number of bytes actually read, which may be less than requested if
        /// (and only if) the end of the stream is reached.
        /// </summary>
        /// <param name="data">Buffer to read into</param>
        /// <param name="size">Number of bytes to read</param>
        /// <returns>Number of bytes actually read</returns>
        private int TryReadInternal(byte[] data, int size)
        {
            int index = 0;
            while (index < size)
            {
                int read = this.BaseStream.Read(data, index, size - index);
                if (read == 0)
                {
                    return index;
                }

                index += read;
            }

            return index;
        }

        public void Dispose()
        {
            if (!this.leaveOpen)
            {
                this.BaseStream?.Dispose();
            }
        }

        /// <summary>
        /// Class to cast to type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        private static class CastTo<T>
        {
            /// <summary>
            /// Casts <typeparamref name="S" /> to <typeparamref name="T" />.
            /// This does not cause boxing for value types.
            /// Useful in generic methods.
            /// </summary>
            /// <typeparam name="S">Source type to cast from. Usually a generic type.</typeparam>
            /// <param name="s">The s.</param>
            public static T From<S>(S s)
            {
                return Cache<S>.Caster(s);
            }

            private static class Cache<S>
            {
                public static readonly Func<S, T> Caster = Get();

                private static Func<S, T> Get()
                {
                    var p = System.Linq.Expressions.Expression.Parameter(typeof(S));
                    var c = System.Linq.Expressions.Expression.ConvertChecked(p, typeof(T));
                    return System.Linq.Expressions.Expression.Lambda<Func<S, T>>(c, p).Compile();
                }
            }
        }
    }
}