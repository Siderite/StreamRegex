using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Siderite.StreamRegex
{
    /// <summary>
    /// Collection of <see cref="StreamGroup"/> objects
    /// </summary>
    [Serializable]
    public class StreamGroupCollection:IEnumerable<StreamGroup>
    {
        /// <summary>
        /// internal dictionary for the stream groups
        /// </summary>
        private Dictionary<string, StreamGroup> _dict;

        /// <summary>
        /// internal list of stream groups (only instantiated at first access by integer index)
        /// </summary>
        private List<StreamGroup> _list;

        /// <summary>
        /// This should be instantiated only in the Siderite.StreamRegex namespace
        /// </summary>
        /// <param name="groups">Collection of groups</param>
        /// <param name="textReaderPosition">The position of the match buffer into the text reader</param>
        /// <param name="names">The array of group names in the internal <see cref="Regex"/></param>
        internal StreamGroupCollection(GroupCollection groups, int textReaderPosition, string[] names)
        {
            _dict = new Dictionary<string, StreamGroup>();
            var index = 0;
            foreach (Group group in groups)
            {
                string name = names[index++];
                _dict[name] = new StreamGroup(group, textReaderPosition, name);
            }
        }

        /// <summary>
        /// Enables access to a member of the collection by int index.
        /// </summary>
        /// <returns></returns>
        public StreamGroup this[int index]
        {
            get
            {
                if (_list==null)
                {
                    _list = _dict.Values.ToList();
                }
                return _list[index];
            }
        }

        /// <summary>
        /// Enables access to a member of the collection by string index.
        /// </summary>
        /// <param name="groupName">group name</param>
        /// <returns></returns>
        public StreamGroup this[string groupName] => _dict[groupName];

        /// <summary>
        /// Count of group items
        /// </summary>
        public int Count => _dict.Count;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<StreamGroup> GetEnumerator()
        {
            return _dict.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dict.Values).GetEnumerator();
        }
    }
}