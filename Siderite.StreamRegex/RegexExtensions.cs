using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Siderite.StreamRegex
{
    /// <summary>
    /// Adds stream functionality to <see cref="Regex"/> objects
    /// </summary>
    public static class RegexExtensions
    {
        /// <summary>
        ///  Searches the specified <see cref="TextReader"/> for the first occurrence of the regular expression 
        ///  specified in the <see cref="Regex"/> constructor.
        /// </summary>
        /// <param name="regex">The regular expression object</param>
        /// <param name="reader">A TextReader</param>
        /// <param name="maxMatchSize">Important to performance, it represents the maximum length of a match. 
        /// If you only look for words of maximum 10 characters, you should set this to 10.
        /// Defaults to 10000</param>
        /// <param name="bufferSize">The size of the buffer used for matching. This should usually be left alone.
        /// Defaults to 65536</param>
        /// <returns></returns>
        public static StreamMatch Match(this Regex regex, TextReader reader, int maxMatchSize = 10000, int bufferSize=65536)
        {
            if (regex == null)
            {
                throw new ArgumentNullException(nameof(regex));
            }
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (maxMatchSize<=0)
            {
                throw new ArgumentException(nameof(maxMatchSize));
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentException(nameof(bufferSize));
            }
            if (bufferSize < maxMatchSize)
            {
                throw new ArgumentException($"{nameof(bufferSize)} is less than {nameof(maxMatchSize)}");
            }
            return new StreamMatch(regex, reader, bufferSize, maxMatchSize);
        }

        /// <summary>
        ///  Searches the specified <see cref="Stream"/> for the first occurrence of the regular expression 
        ///  specified in the <see cref="Regex"/> constructor.
        /// </summary>
        /// <param name="regex">The regular expression object</param>
        /// <param name="stream">A Stream</param>
        /// <param name="encoding">An optional encoding. Defaults to UTF8.</param>
        /// <param name="maxMatchSize">Important to performance, it represents the maximum length of a match. 
        /// If you only look for words of maximum 10 characters, you should set this to 10.
        /// Defaults to 10000</param>
        /// <param name="bufferSize">The size of the buffer used for matching. This should usually be left alone.
        /// Defaults to 65536</param>
        /// <returns></returns>
        public static StreamMatch Match(this Regex regex, Stream stream, Encoding encoding = null, int maxMatchSize = 10000, int bufferSize = 65536)
        {
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8, encoding == null, bufferSize, true))
            {
                return regex.Match(reader, maxMatchSize, bufferSize);
            }
        }
    }
}
