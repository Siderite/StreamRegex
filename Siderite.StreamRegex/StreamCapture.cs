using System.Text.RegularExpressions;

namespace Siderite.StreamRegex
{
    /// <summary>
    /// Represents the results from a single successful subexpression capture.
    /// It's a more accessible form of the <see cref="Capture"/> class.
    /// </summary>
    public class StreamCapture
    {
        /// <summary>
        /// the internal <see cref="Capture"/> instance
        /// </summary>
        private readonly Capture _capture;

        /// <summary>
        /// The position in the text reader of the match buffer
        /// </summary>
        protected readonly int _testReaderPosition;

        /// <summary>
        /// The index in the text reader of this capture
        /// </summary>
        public virtual int Index => _capture.Index + _testReaderPosition;

        /// <summary>
        /// The length of the capture
        /// </summary>
        public int Length => _capture.Length;

        /// <summary>
        /// The value of the capture
        /// </summary>
        public string Value => _capture.Value;

        /// <summary>
        /// This should be instantiated only in the Siderite.StreamRegex namespace
        /// </summary>
        /// <param name="capture">The original capture</param>
        /// <param name="textReaderPosition">The position in the text reader of the match buffer</param>
        internal StreamCapture(Capture capture, int textReaderPosition)
        {
            _capture = capture;
            _testReaderPosition = textReaderPosition;
        }
    }
}