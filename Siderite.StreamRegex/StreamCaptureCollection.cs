using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Siderite.StreamRegex
{
   /// <summary>
   /// Collection of <see cref="StreamCapture"/> objects
   /// </summary>
    [Serializable]
    public class StreamCaptureCollection : IEnumerable<StreamCapture>
    {
        /// <summary>
        /// internal capture list
        /// </summary>
        private List<StreamCapture> _list;

        /// <summary>
        /// This should be instantiated only in the Siderite.StreamRegex namespace
        /// </summary>
        /// <param name="captures">Collection of regex captures</param>
        /// <param name="streamPosition"></param>
        internal StreamCaptureCollection(CaptureCollection captures, int streamPosition)
        {
            _list = new List<StreamCapture>(captures
                .Cast<Capture>()
                .Select(capture => new StreamCapture(capture, streamPosition)));
        }

        /// <summary>
        /// Enables access to a member of the collection by int index.
        /// </summary>
        /// <returns></returns>
        public StreamCapture this[int index] => _list[index];

        /// <summary>
        /// Count of capture items
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<StreamCapture> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
}