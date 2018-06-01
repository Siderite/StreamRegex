using System;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;
using Siderite.StreamRegex;

namespace Tests
{
    public class StreamRegexTests
    {
        [Fact]
        public void ShouldThrowIfReaderIsNull()
        {
            var reg = new Regex(@"test");
            TextReader reader = null;
            Assert.Throws<ArgumentNullException>(() => reg.Match(reader));
        }

        [Fact]
        public void ShouldThrowIfRegexIsNull()
        {
            Regex reg = null;
            TextReader reader = new StringReader("test");
            Assert.Throws<ArgumentNullException>(() => reg.Match(reader));
        }

        [Fact]
        public void ShouldThrowIfMaxMatchSizeIsInvalid()
        {
            Regex reg = new Regex("test");
            TextReader reader = new StringReader("test");
            Assert.Throws<ArgumentException>(() => reg.Match(reader, 0));
            Assert.Throws<ArgumentException>(() => reg.Match(reader, -1));
            Assert.Throws<ArgumentException>(() => reg.Match(reader, -100));
        }

        [Fact]
        public void ShouldThrowIfBufferSizeIsInvalid()
        {
            Regex reg = new Regex("test");
            TextReader reader = new StringReader("test");
            Assert.Throws<ArgumentException>(() => reg.Match(reader, 10000, 0));
            Assert.Throws<ArgumentException>(() => reg.Match(reader, 10000, -1));
            Assert.Throws<ArgumentException>(() => reg.Match(reader, 10000, -100));
        }

        [Fact]
        public void ShouldThrowIfBufferSizeInvalidToMaxMatchSize()
        {
            Regex reg = new Regex("test");
            TextReader reader = new StringReader("test");
            Assert.Throws<ArgumentException>(() => reg.Match(reader, 11, 10));
            Assert.Throws<ArgumentException>(() => reg.Match(reader, 100, 1));
            Assert.Throws<ArgumentException>(() => reg.Match(reader, 100001, 10000));
        }

        [Theory]
        [InlineData("", @"", RegexOptions.None)]
        [InlineData("abcd", @"a", RegexOptions.None)]
        [InlineData("abcd", @"x", RegexOptions.None)]
        [InlineData("abcdabcd", @"d", RegexOptions.None)]
        [InlineData("abcdabcd", @"(c)", RegexOptions.None)]
        [InlineData("aaaabbbbbccccdddddd", @"(?<test>b+)", RegexOptions.None)]
        [InlineData("abcdabcd", @"((?<test>a)|b)c?", RegexOptions.None)]
        [InlineData("abcdabcd", @"((?<test>a)|b)x?", RegexOptions.None)]
        [InlineData("abcdabcd", @"((?<test>a)|b)x", RegexOptions.None)]
        public void ShouldWorkTheSameAsForString(string input, string regexPattern, RegexOptions regexOptions)
        {
            var reader = new StringReader(input);
            var regex = new Regex(regexPattern, regexOptions);
            var streamMatch = regex.Match(reader);
            var match = regex.Match(input);
            while (true)
            {
                Assert.Equal(match.Success, streamMatch.Success);
                Assert.Equal(match.Value, streamMatch.Value);
                Assert.Equal(match.Index, streamMatch.Index);
                Assert.Equal(match.Length, streamMatch.Length);
                Assert.Equal(match.Captures.Count, streamMatch.Captures.Count);
                Assert.Equal(match.Groups.Count, streamMatch.Groups.Count);
                for (var i = 0; i < match.Captures.Count; i++)
                {
                    Assert.Equal(match.Captures[i].Index, streamMatch.Captures[i].Index);
                    Assert.Equal(match.Captures[i].Length, streamMatch.Captures[i].Length);
                    Assert.Equal(match.Captures[i].Value, streamMatch.Captures[i].Value);
                }
                for (var i = 0; i < match.Groups.Count; i++)
                {
                    Assert.Equal(match.Groups[i].Index, streamMatch.Groups[i].Index);
                    Assert.Equal(match.Groups[i].Length, streamMatch.Groups[i].Length);
                    Assert.Equal(match.Groups[i].Name, streamMatch.Groups[i].Name);
                    Assert.Equal(match.Groups[i].Success, streamMatch.Groups[i].Success);
                    Assert.Equal(match.Groups[i].Value, streamMatch.Groups[i].Value);
                    for (var j = 0; j < match.Groups[i].Captures.Count; j++)
                    {
                        Assert.Equal(match.Groups[i].Captures[j].Index, streamMatch.Groups[i].Captures[j].Index);
                        Assert.Equal(match.Groups[i].Captures[j].Length, streamMatch.Groups[i].Captures[j].Length);
                        Assert.Equal(match.Groups[i].Captures[j].Value, streamMatch.Groups[i].Captures[j].Value);
                    }
                }
                if (!match.Success && !streamMatch.Success)
                {
                    break;
                }
                if (match.Success)
                {
                    match = match.NextMatch();
                }
                if (streamMatch.Success)
                {
                    streamMatch = streamMatch.NextMatch();
                }

            }
        }

        [Theory]
        [InlineData("", @"", RegexOptions.None)]
        [InlineData("abcd", @"a", RegexOptions.None)]
        [InlineData("abcd", @"x", RegexOptions.None)]
        [InlineData("abcdabcd", @"d", RegexOptions.None)]
        [InlineData("abcdabcd", @"(c)", RegexOptions.None)]
        [InlineData("abcdabcd", @"((?<test>a)|b)c?", RegexOptions.None)]
        [InlineData("abcdabcd", @"((?<test>a)|b)x?", RegexOptions.None)]
        [InlineData("abcdabcd", @"((?<test>a)|b)x", RegexOptions.None)]
        public void ShouldWorkTheSameAsForStringWhenStringLargerThanBuffer(string input, string regexPattern, RegexOptions regexOptions)
        {
            input = new string(' ', 100000) + input + new string(' ', 100000);
            var reader = new StringReader(input);
            var regex = new Regex(regexPattern, regexOptions);
            var streamMatch = regex.Match(reader);
            var match = regex.Match(input);
            while (true)
            {
                assertEqual(streamMatch, match);
                if (!match.Success && !streamMatch.Success)
                {
                    break;
                }
                if (match.Success)
                {
                    match = match.NextMatch();
                }
                if (streamMatch.Success)
                {
                    streamMatch = streamMatch.NextMatch();
                }
            }
        }

        [Theory]
        [InlineData(0,0)]
        [InlineData(1,5)]
        [InlineData(10000,1000)]
        [InlineData(100000,100000)]
        [InlineData(1000000, 10000)]
        [InlineData(1000000, 0)]
        public void ShouldFindMatchAtCorrectIndex(int padding1, int padding2)
        {
            var val = "FoundValue";
            var str = new string(' ', padding1) + val + new string(' ', padding2) + val + new string('x', 1000);
            var reader = new StringReader(str);
            var regex = new Regex(val.ToLower(), RegexOptions.IgnoreCase);
            var streamMatch = regex.Match(reader);
            var matches = regex.Matches(str);
            foreach (Match match in matches)
            {
                assertEqual(streamMatch, match);
                streamMatch=streamMatch.NextMatch();
            }
        }

        [Fact]
        public void SpecialCaseEmptyPattern()
        {
            string str = new string('x', 2);
            var reader = new StringReader(str);
            var regex = new Regex("");
            var streamMatch = regex.Match(reader,1,1);
            var matches = regex.Matches(str);
            foreach (Match match in matches)
            {
                assertEqual(streamMatch, match);
                streamMatch = streamMatch.NextMatch();
            }
        }

        private static void assertEqual(StreamMatch streamMatch, Match match)
        {
            Assert.Equal(match.Success, streamMatch.Success);
            Assert.Equal(match.Value, streamMatch.Value);
            Assert.Equal(match.Index, streamMatch.Index);
            Assert.Equal(match.Length, streamMatch.Length);
            Assert.Equal(match.Captures.Count, streamMatch.Captures.Count);
            Assert.Equal(match.Groups.Count, streamMatch.Groups.Count);
            for (var i = 0; i < match.Captures.Count; i++)
            {
                Assert.Equal(match.Captures[i].Index, streamMatch.Captures[i].Index);
                Assert.Equal(match.Captures[i].Length, streamMatch.Captures[i].Length);
                Assert.Equal(match.Captures[i].Value, streamMatch.Captures[i].Value);
            }
            for (var i = 0; i < match.Groups.Count; i++)
            {
                Assert.Equal(match.Groups[i].Index, streamMatch.Groups[i].Index);
                Assert.Equal(match.Groups[i].Length, streamMatch.Groups[i].Length);
                Assert.Equal(match.Groups[i].Name, streamMatch.Groups[i].Name);
                Assert.Equal(match.Groups[i].Success, streamMatch.Groups[i].Success);
                Assert.Equal(match.Groups[i].Value, streamMatch.Groups[i].Value);
                for (var j = 0; j < match.Groups[i].Captures.Count; j++)
                {
                    Assert.Equal(match.Groups[i].Captures[j].Index, streamMatch.Groups[i].Captures[j].Index);
                    Assert.Equal(match.Groups[i].Captures[j].Length, streamMatch.Groups[i].Captures[j].Length);
                    Assert.Equal(match.Groups[i].Captures[j].Value, streamMatch.Groups[i].Captures[j].Value);

                }
            }
        }
    }
}
