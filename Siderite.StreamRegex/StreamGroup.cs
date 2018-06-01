using System.Text.RegularExpressions;

namespace Siderite.StreamRegex
{
    /// <summary>
    /// Represents the results from a single capturing group.
    /// </summary>
    public class StreamGroup:StreamCapture
    {
        /// <summary>
        /// the internal <see cref="Group"/> instance
        /// </summary>
        private readonly Group _group;

        /// <summary>
        /// internal capture collection
        /// </summary>
        private StreamCaptureCollection _captures;

        /// <summary>
        /// the name of the group
        /// </summary>
        public string Name { get; }

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
                    _captures = new StreamCaptureCollection(_group.Captures, _testReaderPosition);
                }
                return _captures;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the match is successful.
        /// </summary>
        public bool Success => _group.Success;

        /// <summary>
        /// The position in the original string where the first character of the captured substring is found.
        /// </summary>
        public override int Index => _group.Success ? base.Index : 0;

        /// <summary>
        /// This should be instantiated only in the Siderite.StreamRegex namespace
        /// </summary>
        /// <param name="group">The internal <see cref="Group"/> instance</param>
        /// <param name="textReaderPosition">The position of the match buffer in the text reader</param>
        /// <param name="name">The name of the group</param>
        internal StreamGroup(Group group, int textReaderPosition, string name) :base(group, textReaderPosition)
        {
            _group = group;
            Name = name;
        }
    }
}