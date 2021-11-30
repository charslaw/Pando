using System;
using FluentAssertions;
using MiscUtil.Conversion;
using NUnit.Framework;

namespace EndianBitConverterTests
{
    [TestFixtureSource(typeof(Sources), nameof(Sources.BitConverters))]
    public class CopyBytesSpanTests
    {
        private readonly EndianBitConverter _bitConverter;

        public CopyBytesSpanTests(EndianBitConverter bitConverter) { _bitConverter = bitConverter; }

        [Test]
        public void Bool([Values] bool value)
        {
            var expected = _bitConverter.GetBytes(value);
            Span<byte> buffer = stackalloc byte[sizeof(bool)];
            _bitConverter.CopyBytes(value, buffer);
            var actual = buffer.ToArray();
            actual.Should().Equal(expected);
        }

        [TestCase(' '), TestCase('a'), TestCase('7'), TestCase(char.MaxValue), TestCase(char.MinValue)]
        public void Char(char value)
        {
            var expected = _bitConverter.GetBytes(value);

            Span<byte> buffer = stackalloc byte[sizeof(char)];
            _bitConverter.CopyBytes(value, buffer);
            var actual = buffer.ToArray();

            actual.Should().Equal(expected);
        }

        [TestCase(0), TestCase(Math.PI), TestCase(-2.5), TestCase(double.MaxValue), TestCase(double.MinValue)]
        public void Double(double value)
        {
            var expected = _bitConverter.GetBytes(value);

            Span<byte> buffer = stackalloc byte[sizeof(double)];
            _bitConverter.CopyBytes(value, buffer);
            var actual = buffer.ToArray();

            actual.Should().Equal(expected);
        }

        [TestCase(0), TestCase(1337), TestCase(short.MaxValue), TestCase(short.MinValue)]
        public void Short(short value)
        {
            var expected = _bitConverter.GetBytes(value);

            Span<byte> buffer = stackalloc byte[sizeof(short)];
            _bitConverter.CopyBytes(value, buffer);
            var actual = buffer.ToArray();

            actual.Should().Equal(expected);
        }

        [TestCase(0), TestCase(1337), TestCase(int.MaxValue), TestCase(int.MinValue)]
        public void Int(int value)
        {
            var expected = _bitConverter.GetBytes(value);

            Span<byte> buffer = stackalloc byte[sizeof(int)];
            _bitConverter.CopyBytes(value, buffer);
            var actual = buffer.ToArray();

            actual.Should().Equal(expected);
        }

        [TestCase(0), TestCase(1337), TestCase(long.MaxValue), TestCase(long.MinValue)]
        public void Long(long value)
        {
            var expected = _bitConverter.GetBytes(value);

            Span<byte> buffer = stackalloc byte[sizeof(long)];
            _bitConverter.CopyBytes(value, buffer);
            var actual = buffer.ToArray();

            actual.Should().Equal(expected);
        }

        [TestCase(0), TestCase((float) Math.PI), TestCase(-2.5f), TestCase(float.MaxValue), TestCase(float.MinValue)]
        public void Float(float value)
        {
            var expected = _bitConverter.GetBytes(value);

            Span<byte> buffer = stackalloc byte[sizeof(float)];
            _bitConverter.CopyBytes(value, buffer);
            var actual = buffer.ToArray();

            actual.Should().Equal(expected);
        }

        [TestCase((ushort) 0), TestCase((ushort) 1337), TestCase(ushort.MaxValue), TestCase(ushort.MinValue)]
        public void UShort(ushort value)
        {
            var expected = _bitConverter.GetBytes(value);

            Span<byte> buffer = stackalloc byte[sizeof(ushort)];
            _bitConverter.CopyBytes(value, buffer);
            var actual = buffer.ToArray();

            actual.Should().Equal(expected);
        }

        [TestCase(0U), TestCase(1337U), TestCase(uint.MaxValue), TestCase(uint.MinValue)]
        public void UInt(uint value)
        {
            var expected = _bitConverter.GetBytes(value);

            Span<byte> buffer = stackalloc byte[sizeof(uint)];
            _bitConverter.CopyBytes(value, buffer);
            var actual = buffer.ToArray();

            actual.Should().Equal(expected);
        }

        [TestCase(0U), TestCase(1337U), TestCase(ulong.MaxValue), TestCase(ulong.MinValue)]
        public void ULong(ulong value)
        {
            var expected = _bitConverter.GetBytes(value);

            Span<byte> buffer = stackalloc byte[sizeof(ulong)];
            _bitConverter.CopyBytes(value, buffer);
            var actual = buffer.ToArray();

            actual.Should().Equal(expected);
        }

        [Test]
        public void Char_undersized_buffer_throws() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> buffer = stackalloc byte[1];
                bc.CopyBytes(' ', buffer);
            }
        );

        [Test]
        public void Double_undersized_buffer_throws() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> buffer = stackalloc byte[1];
                bc.CopyBytes(Math.PI, buffer);
            }
        );

        [Test]
        public void Short_undersized_buffer_throws() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> buffer = stackalloc byte[1];
                bc.CopyBytes((short) 1337, buffer);
            }
        );

        [Test]
        public void Int_undersized_buffer_throws() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> buffer = stackalloc byte[1];
                bc.CopyBytes(1337, buffer);
            }
        );

        [Test]
        public void Long_undersized_buffer_throws() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> buffer = stackalloc byte[1];
                bc.CopyBytes((long) 1337, buffer);
            }
        );

        [Test]
        public void Float_undersized_buffer_throws() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> buffer = stackalloc byte[1];
                bc.CopyBytes((float) Math.PI, buffer);
            }
        );

        [Test]
        public void UShort_undersized_buffer_throws() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> buffer = stackalloc byte[1];
                bc.CopyBytes((ushort) 1337, buffer);
            }
        );

        [Test]
        public void UInt_undersized_buffer_throws() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> buffer = stackalloc byte[1];
                bc.CopyBytes((uint) 1337, buffer);
            }
        );

        [Test]
        public void ULong_undersized_buffer_throws() => AssertUndersizedBufferThrows(bc =>
            {
                Span<byte> buffer = stackalloc byte[1];
                bc.CopyBytes((ulong) 1337, buffer);
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
