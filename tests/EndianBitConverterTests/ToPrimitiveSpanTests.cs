using System;
using FluentAssertions;
using MiscUtil.Conversion;
using NUnit.Framework;

namespace EndianBitConverterTests
{
    [TestFixtureSource(typeof(Sources), nameof(Sources.BitConverters))]
    public class ToPrimitiveSpanTests
    {
        private readonly Random _rng = new();
        private readonly EndianBitConverter _bitConverter;

        public ToPrimitiveSpanTests(EndianBitConverter bitConverter) { _bitConverter = bitConverter; }

        [TestCase(new byte[] { 0x00 }), TestCase(new byte[] { 0x01 }), TestCase(new byte[] { 0x17 })]
        public void ToBoolean(byte[] value)
        {
            var expected = _bitConverter.ToBoolean(value, 0);

            var span = value.AsSpan();
            var actual = _bitConverter.ToBoolean(span);

            actual.Should().Be(expected);
        }

        [TestCase(new byte[] { 0x00, 0x00 }), TestCase(new byte[] { 0x12, 0x34 }), TestCase(new byte[] { 0xFF, 0xFF })]
        public void ToChar(byte[] value)
        {
            var expected = _bitConverter.ToChar(value, 0);

            var span = value.AsSpan();
            var actual = _bitConverter.ToChar(span);

            actual.Should().Be(expected);
        }

        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF })]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
        public void ToDouble(byte[] value)
        {
            var expected = _bitConverter.ToDouble(value, 0);

            var span = value.AsSpan();
            var actual = _bitConverter.ToDouble(span);

            actual.Should().Be(expected);
        }

        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78 })]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
        public void ToSingle(byte[] value)
        {
            var expected = _bitConverter.ToSingle(value, 0);

            var span = value.AsSpan();
            var actual = _bitConverter.ToSingle(span);

            actual.Should().Be(expected);
        }

        [TestCase(new byte[] { 0x00, 0x00 })]
        [TestCase(new byte[] { 0x12, 0x34 })]
        [TestCase(new byte[] { 0xFF, 0xFF })]
        public void ToInt16(byte[] value)
        {
            var expected = _bitConverter.ToInt16(value, 0);

            var span = value.AsSpan();
            var actual = _bitConverter.ToInt16(span);

            actual.Should().Be(expected);
        }

        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78 })]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
        public void ToInt32(byte[] value)
        {
            var expected = _bitConverter.ToInt32(value, 0);

            var span = value.AsSpan();
            var actual = _bitConverter.ToInt32(span);

            actual.Should().Be(expected);
        }

        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF })]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
        public void ToInt64(byte[] value)
        {
            var expected = _bitConverter.ToInt64(value, 0);

            var span = value.AsSpan();
            var actual = _bitConverter.ToInt64(span);

            actual.Should().Be(expected);
        }

        [TestCase(new byte[] { 0x00, 0x00 })]
        [TestCase(new byte[] { 0x12, 0x34 })]
        [TestCase(new byte[] { 0xFF, 0xFF })]
        public void ToUInt16(byte[] value)
        {
            var expected = _bitConverter.ToUInt16(value, 0);

            var span = value.AsSpan();
            var actual = _bitConverter.ToUInt16(span);

            actual.Should().Be(expected);
        }

        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78 })]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
        public void ToUInt32(byte[] value)
        {
            var expected = _bitConverter.ToUInt32(value, 0);

            var span = value.AsSpan();
            var actual = _bitConverter.ToUInt32(span);

            actual.Should().Be(expected);
        }

        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF })]
        [TestCase(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
        public void ToUInt64(byte[] value)
        {
            var expected = _bitConverter.ToUInt64(value, 0);

            var span = value.AsSpan();
            var actual = _bitConverter.ToUInt64(span);

            actual.Should().Be(expected);
        }

        [Test]
        public void ToBoolean_sliced_buffer()
        {
            const int INDEX = 4;
            Span<byte> value = stackalloc byte[16];
            _rng.NextBytes(value);
            var expected = _bitConverter.ToBoolean(value.ToArray(), INDEX);

            var actual = _bitConverter.ToBoolean(value.Slice(INDEX, sizeof(bool)));

            actual.Should().Be(expected);
        }

        [Test]
        public void ToChar_sliced_buffer()
        {
            const int INDEX = 4;
            Span<byte> value = stackalloc byte[16];
            _rng.NextBytes(value);
            var expected = _bitConverter.ToChar(value.ToArray(), INDEX);

            var actual = _bitConverter.ToChar(value.Slice(INDEX, sizeof(char)));

            actual.Should().Be(expected);
        }

        [Test]
        public void ToDouble_sliced_buffer()
        {
            const int INDEX = 4;
            Span<byte> value = stackalloc byte[16];
            _rng.NextBytes(value);
            var expected = _bitConverter.ToDouble(value.ToArray(), INDEX);

            var actual = _bitConverter.ToDouble(value.Slice(INDEX, sizeof(double)));

            actual.Should().Be(expected);
        }

        [Test]
        public void ToSingle_sliced_buffer()
        {
            const int INDEX = 4;
            Span<byte> value = stackalloc byte[16];
            _rng.NextBytes(value);
            var expected = _bitConverter.ToSingle(value.ToArray(), INDEX);

            var actual = _bitConverter.ToSingle(value.Slice(INDEX, sizeof(float)));

            actual.Should().Be(expected);
        }

        [Test]
        public void ToInt16_sliced_buffer()
        {
            const int INDEX = 4;
            Span<byte> value = stackalloc byte[16];
            _rng.NextBytes(value);
            var expected = _bitConverter.ToInt16(value.ToArray(), INDEX);

            var actual = _bitConverter.ToInt16(value.Slice(INDEX, sizeof(short)));

            actual.Should().Be(expected);
        }

        [Test]
        public void ToInt32_sliced_buffer()
        {
            const int INDEX = 4;
            Span<byte> value = stackalloc byte[16];
            _rng.NextBytes(value);
            var expected = _bitConverter.ToInt32(value.ToArray(), INDEX);

            var actual = _bitConverter.ToInt32(value.Slice(INDEX, sizeof(int)));

            actual.Should().Be(expected);
        }

        [Test]
        public void ToInt64_sliced_buffer()
        {
            const int INDEX = 4;
            Span<byte> value = stackalloc byte[16];
            _rng.NextBytes(value);
            var expected = _bitConverter.ToInt64(value.ToArray(), INDEX);

            var actual = _bitConverter.ToInt64(value.Slice(INDEX, sizeof(long)));

            actual.Should().Be(expected);
        }

        [Test]
        public void ToUInt16_sliced_buffer()
        {
            const int INDEX = 4;
            Span<byte> value = stackalloc byte[16];
            _rng.NextBytes(value);
            var expected = _bitConverter.ToUInt16(value.ToArray(), INDEX);

            var actual = _bitConverter.ToUInt16(value.Slice(INDEX, sizeof(ushort)));

            actual.Should().Be(expected);
        }

        [Test]
        public void ToUInt32_sliced_buffer()
        {
            const int INDEX = 4;
            Span<byte> value = stackalloc byte[16];
            _rng.NextBytes(value);
            var expected = _bitConverter.ToUInt32(value.ToArray(), INDEX);

            var actual = _bitConverter.ToUInt32(value.Slice(INDEX, sizeof(uint)));

            actual.Should().Be(expected);
        }

        [Test]
        public void ToUInt64_sliced_buffer()
        {
            const int INDEX = 4;
            Span<byte> value = stackalloc byte[16];
            _rng.NextBytes(value);
            var expected = _bitConverter.ToUInt64(value.ToArray(), INDEX);

            var actual = _bitConverter.ToUInt64(value.Slice(INDEX, sizeof(ulong)));

            actual.Should().Be(expected);
        }

        [Test]
        public void ToChar_undersized_buffer() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> value = stackalloc byte[1];
                bc.ToChar(value);
            }
        );

        [Test]
        public void ToDouble_undersized_buffer() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> value = stackalloc byte[1];
                bc.ToDouble(value);
            }
        );

        [Test]
        public void ToSingle_undersized_buffer() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> value = stackalloc byte[1];
                bc.ToSingle(value);
            }
        );

        [Test]
        public void ToInt16_undersized_buffer() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> value = stackalloc byte[1];
                bc.ToInt16(value);
            }
        );

        [Test]
        public void ToInt32_undersized_buffer() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> value = stackalloc byte[1];
                bc.ToInt32(value);
            }
        );

        [Test]
        public void ToInt64_undersized_buffer() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> value = stackalloc byte[1];
                bc.ToInt64(value);
            }
        );

        [Test]
        public void ToUInt16_undersized_buffer() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> value = stackalloc byte[1];
                bc.ToUInt16(value);
            }
        );

        [Test]
        public void ToUInt32_undersized_buffer() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> value = stackalloc byte[1];
                bc.ToUInt32(value);
            }
        );

        [Test]
        public void ToUInt64_undersized_buffer() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> value = stackalloc byte[1];
                bc.ToUInt64(value);
            }
        );

        private void AssertUndersizedBufferThrows(Action<EndianBitConverter> action)
        {
            _bitConverter.Invoking(action)
                .Should()
                .Throw<ArgumentOutOfRangeException>();
        }

        private static class Sources
        {
            public static object[] BitConverters =
            {
                new object[] { new LittleEndianBitConverter() },
                new object[] { new BigEndianBitConverter() }
            };
        }
    }
}
