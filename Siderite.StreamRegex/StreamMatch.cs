using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Siderite.StreamRegex
{
    /// <summary>
    /// Represents the results from a single regular expression match. 
    /// </summary>
    public class StreamMatch
    {
        /// <summary>
        /// internal <see cref="Regex"/> instance
        /// </summary>
        private readonly Regex _regex;

        /// <summary>
        /// internal <see cref="TextReader"/> instance
        /// </summary>
        private readonly TextReader _reader;

        /// <summary>
        /// internal buffer
        /// </summary>
        private readonly char[] _buffer;

        /// <summary>
        /// maximum possible match size
        /// </summary>
        private readonly int _maxMatchSize;

        /// <summary>
        /// the position in the internal buffer
        /// </summary>
        private int _bufferPosition;

        /// <summary>
        /// the size of the data inside the buffer
        /// </summary>
        private int _bufferLength;

        /// <summary>
        /// the string contained in the buffer
        /// </summary>
        private string _stringValue;

        /// <summary>
        /// internal <see cref="Match"/> instance
        /// </summary>
        private Match _match;

        /// <summary>
        /// the position of the buffer inside the text reader
        /// </summary>
        private int _globalPosition;

        /// <summary>
        /// last successful match index relative to the text reader
        /// </summary>
        private int _lastSuccessfulIndex = -1;

        /// <summary>
        /// internal capture collection
        /// </summary>
        private StreamCaptureCollection _captures;

        /// <summary>
        /// internal group collection
        /// </summary>
        private StreamGroupCollection _groups;

        /// <summary>
        /// Gets a collection of all the captures matched by the capturing group, in innermost-leftmost-first order
        /// (or innermost-rightmost-first order if the regular expression is modified with the <see cref="RegexOptions.RightToLeft"/> option).
        /// The collection may have zero or more items.
        /// </summary>
        public StreamCaptureCollection Captures
        {
            get
            {
                if (_captures == null)
                {
                    _captures = new StreamCaptureCollection(_match.Captures, _match.Success ? _globalPosition : 0);
                }
                return _captures;
            }
        }

        /// <summary>
        /// Gets a collection of groups matched by the regular expression.
        /// </summary>
        public StreamGroupCollection Groups
        {
            get
            {
                if (_groups == null)
                {
                    _groups = new StreamGroupCollection(_match.Groups, _match.Success ? _globalPosition : 0, _regex.GetGroupNames());
                }
                return _groups;
            }
        }

        /// <summary>
        /// The position in the original string where the first character of the captured substring is found.
        /// </summary>
        public int Index => _match.Index + (_match.Success ? _globalPosition : 0);

        /// <summary>
        /// The length of the capture
        /// </summary>
        public int Length => _match.Length;

        /// <summary>
        /// The value of the capture
        /// </summary>
        public string Value => _match.Value;

        /// <summary>
        /// Gets a value indicating whether the match is successful.
        /// </summary>
        public bool Success => _match.Success;

        /// <summary>
        /// This should be instantiated only in the Siderite.StreamRegex namespace
        /// </summary>
        /// <param name="regex">The regular expression object</param>
        /// <param name="reader">The text reader</param>
        /// <param name="bufferSize">The buffer size</param>
        /// <param name="maxMatchSize">The maximum possible match size</param>
        internal StreamMatch(Regex regex, TextReader reader, int bufferSize, int maxMatchSize)
        {
            _regex = regex;
            _reader = reader;
            _buffer = new char[bufferSize];
            _bufferPosition = 0;
            _maxMatchSize = maxMatchSize;
            _globalPosition = 0;

            _bufferLength = _reader.Read(_buffer, 0, _buffer.Length);
            matchBuffer();
        }

        /// <summary>
        ///  Returns itself with the results for the next match, starting at the position at which the last match ended
        /// </summary>
        /// <returns></returns>
        public StreamMatch NextMatch()
        {
            _captures = null;
            _groups = null;
            if (_match.Success)
            {
                _bufferPosition = _match.Index + _match.Length;
                _lastSuccessfulIndex = _globalPosition + _match.Index;
                _match = _match.NextMatch();
                if (_match.Success)
                {
                    _bufferPosition = _match.Index + _match.Length;
                    _lastSuccessfulIndex = _globalPosition + _match.Index;
                    return this;
                }
            }
            if (_bufferLength < _buffer.Length)
            {
                return this;
            }
            _bufferPosition = Math.Max(_bufferPosition, _bufferLength - _maxMatchSize);
            moveBuffer();
            matchBuffer();
            return this;
        }

        /// <summary>
        /// get the first match in the internal buffer
        /// </summary>
        private void matchBuffer()
        {
            _stringValue = new string(_buffer, 0, _bufferLength);
            _match = _regex.Match(_stringValue);
            if (!_match.Success || _match.Index+_globalPosition==_lastSuccessfulIndex)
            {
                NextMatch();
            }
        }

        /// <summary>
        /// Read more of the buffer
        /// </summary>
        private void moveBuffer()
        {
            var length = _bufferLength - _bufferPosition;
            _globalPosition += _bufferPosition;
            Array.Copy(_buffer, _bufferPosition, _buffer, 0, length);
            _bufferLength = length + _reader.Read(_buffer, length, _buffer.Length-length);
            _bufferPosition = length;
        }
    }
}