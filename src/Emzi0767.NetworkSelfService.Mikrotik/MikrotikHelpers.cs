using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Provides various helper methods for the API encoder/decoder.
/// </summary>
internal static class MikrotikHelpers
{
    /// <summary>
    /// Gets the default encoding instance for the library.
    /// </summary>
    public static Encoding DefaultEncoding { get; } = new UTF8Encoding(false);

    /// <summary>
    /// Attempts to encode a given length to Mikrotik format.
    /// </summary>
    /// <param name="length">Length to encode.</param>
    /// <param name="dest">Destination buffer to encode to.</param>
    /// <param name="bytesWritten">Number of bytes written to the buffer.</param>
    /// <returns>Whether the operation succeeded.</returns>
    public static bool TryEncodeLength(this long length, Span<byte> dest, out int bytesWritten)
    {
        bytesWritten = 0;
        if (length < 0 || length > uint.MaxValue)
            return false;

        switch (length)
        {
            case >= 0 and <= 0x7F:
                if (!_requireLength(dest, 1, out bytesWritten))
                    return false;

                dest[0] = (byte)length;
                return true;

            case >= 0x80 and <= 0x3FFF:
                if (!_requireLength(dest, 2, out bytesWritten))
                    return false;

                BinaryPrimitives.WriteUInt16BigEndian(dest, (ushort)(length | 0x8000u));
                return true;

            case >= 0x400 and <= 0x1FFFFF:
                if (!_requireLength(dest, 3, out bytesWritten))
                    return false;

                length |= 0xC00000;
                dest[0] = (byte)(length >> 16);
                dest[1] = (byte)((length >> 8) & 0xFF);
                dest[2] = (byte)(length & 0xFF);
                return true;

            case >= 0x200000 and <= 0xFFFFFFF:
                if (!_requireLength(dest, 4, out bytesWritten))
                    return false;

                BinaryPrimitives.WriteUInt32BigEndian(dest, (uint)(length | 0xE0000000u));
                return true;

            default:
                if (!_requireLength(dest, 5, out bytesWritten))
                    return false;

                dest[0] = 0xF0;
                BinaryPrimitives.WriteUInt32BigEndian(dest[1..], (uint)length);
                return true;
        }

        static bool _requireLength(Span<byte> _d, int _c, out int _w)
        {
            _w = 0;
            if (_d.Length < _c)
                return false;

            _w = _c;
            return true;
        }
    }

    /// <summary>
    /// Attempts to encode a given length to Mikrotik format.
    /// </summary>
    /// <param name="length">Length to encode.</param>
    /// <param name="dest">Destination buffer to encode to.</param>
    /// <returns>Whether the operation succeeded.</returns>
    public static bool TryEncodeLength(this long length, IMemoryBuffer<byte> dest)
    {
        var destLen = length switch
        {
            >= 0 and <= 0x7F => 1,
            >= 0x80 and <= 0x3FFF => 2,
            >= 0x400 and <= 0x1FFFFF => 3,
            >= 0x200000 and <= 0xFFFFFFF => 4,
            >= 0x10000000 and <= uint.MaxValue => 5,
            _ => -1,
        };
        
        if (destLen < 0)
            return false;
        
        var span = dest.GetSpan(destLen);
        if (!length.TryEncodeLength(span, out var bytesWritten))
            return false;
        
        if (bytesWritten != destLen)
            return false;
        
        dest.Advance(bytesWritten);
        return true;
    }
    
    /// <summary>
    /// Attempts to decode a given buffer as a length.
    /// </summary>
    /// <param name="src">Source buffer to decode from.</param>
    /// <param name="length">Decoded length or <see cref="long.MinValue"/> on failure.</param>
    /// <param name="bytesRead">Number of bytes read or -1 on failure.</param>
    /// <returns>Whether the operation was successful.</returns>
    public static bool TryDecodeLength(this Span<byte> src, out long length, out int bytesRead)
        => ((ReadOnlySpan<byte>)src).TryDecodeLength(out length, out bytesRead);

    /// <summary>
    /// Attempts to decode a given buffer as a length.
    /// </summary>
    /// <param name="src">Source buffer to decode from.</param>
    /// <param name="length">Decoded length or <see cref="long.MinValue"/> on failure.</param>
    /// <param name="bytesRead">Number of bytes read or -1 on failure.</param>
    /// <returns>Whether the operation was successful.</returns>
    public static bool TryDecodeLength(this ReadOnlySpan<byte> src, out long length, out int bytesRead)
    {
        if (src[0] <= 0x7F)
        {
            bytesRead = 1;
            length = src[0];
            return true;
        }

        bytesRead = -1;
        length = long.MinValue;
        switch (src[0] & 0xF0)
        {
            case 0x80:
                if (!_requireLength(src, 2, out bytesRead))
                    return false;

                length = BinaryPrimitives.ReadUInt16BigEndian(src) & ~0x8000u;
                return true;

            case 0xC0:
                if (!_requireLength(src, 3, out bytesRead))
                    return false;

                length = (uint)((src[0] << 16) | (src[1] << 8) | src[2]) & ~0xC00000u;
                return true;

            case 0xE0:
                if (!_requireLength(src, 4, out bytesRead))
                    return false;

                length = BinaryPrimitives.ReadUInt32BigEndian(src) & ~0xE0000000u;
                return true;

            case 0xF0:
                if (!_requireLength(src, 5, out bytesRead))
                    return false;

                length = BinaryPrimitives.ReadUInt32BigEndian(src[1..]);
                return true;

            default:
                return false;
        }

        static bool _requireLength(ReadOnlySpan<byte> _d, int _c, out int _w)
        {
            _w = -1;
            if (_d.Length < _c)
                return false;

            _w = _c;
            return true;
        }
    }

    /// <summary>
    /// Computes the length of the encoded string.
    /// </summary>
    /// <param name="s">String to measure.</param>
    /// <param name="encoding">Encoding to use for computing.</param>
    /// <returns>Computed number of bytes required to encode the string.</returns>
    public static int ComputeEncodedLength(this string s, Encoding encoding = default)
        => (encoding ?? DefaultEncoding).GetByteCount(s);

    /// <summary>
    /// Attempts to encode the given string to the specified buffer.
    /// </summary>
    /// <param name="s">String to encode.</param>
    /// <param name="destination">Buffer to encode to.</param>
    /// <param name="bytesWritten">Number of bytes actually encoded.</param>
    /// <param name="encoding">Encoding to use for the transform.</param>
    /// <returns>Whether the operation was a success.</returns>
    public static bool TryEncodeTo(this string s, Span<byte> destination, out int bytesWritten, Encoding encoding = default)
        => (encoding ?? DefaultEncoding).TryGetBytes(s, destination, out bytesWritten);

    /// <summary>
    /// Attempts to encode the given string to the specified buffer.
    /// </summary>
    /// <param name="s">String to encode.</param>
    /// <param name="destination">Buffer to encode to.</param>
    /// <param name="encoding">Encoding to use for the transform.</param>
    /// <returns>Whether the operation was a success.</returns>
    public static bool TryEncodeTo(this string s, IBufferWriter<byte> destination, Encoding encoding = default)
    {
        encoding ??= DefaultEncoding;
        var written = encoding.GetBytes(s, destination);
        return written == s.ComputeEncodedLength();
    }
}
