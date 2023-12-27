
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AugustEngine.Collections
{
    /// <summary>
    /// A timeline is a special kind of list.
    /// It knows the number of Objects that occur before, at, or after any
    /// time on the timeline
    /// K is the "time"
    /// T is the "occurance type"
    /// </summary>
    public class Timeline<K, T> where K : System.IComparable
    {
        private Dictionary<K, TimelineNode> _nodes;
        private List<K> sortedKeys = new List<K>();
        /// <summary>
        /// K is the "time"
        /// T is the "occurance type"
        /// </summary>
        public Timeline()
        {
            _nodes = new Dictionary<K, TimelineNode>();
        }
        public int OccurancesUpTo(K time)
        {
            if (sortedKeys.Count == 0) return 0;
            return _nodes[closestElementBinarySearch(sortedKeys, time)].occurancesInclusive;
        }
        public int OccurancesAfter(K time) {
            return _nodes.Count - OccurancesUpTo(time);
        }


        /// <summary>
        /// Returns the first element
        /// At this time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public T GetFirst(K time)
        {
            if (_nodes.ContainsKey(time))
            {
                return _nodes[time].Data;
            }
            return default(T);
        }



        public void Add(K time, T occurance)
        {
            _nodes[time] = new TimelineNode(time, occurance);
            sortedKeys.Clear();
            foreach (K k in _nodes.Keys)
            {
                sortedKeys.Add(k);
            }
            sortedKeys.Sort();

            //work your way down and increase all the occurances at each node above this one by 1
            for (int i = sortedKeys.Count -1; i >= 0; i--)
            {   
                
                //increase this occurance
                _nodes[sortedKeys[i]].occurancesInclusive = _nodes[sortedKeys[i]].occurancesInclusive + 1;
                //break if this is the one we just added
                if (_nodes[sortedKeys[i]].Time.Equals(time)) {
                    if (i-1 > 0)
                    {
                        _nodes[sortedKeys[i]].occurancesInclusive += _nodes[sortedKeys[i-1]].occurancesInclusive;
                    }
                    
                    break;
                }
            }
        }

        /// <summary>
        /// The node that occurs at a given time
        /// </summary>
        private class TimelineNode
        {
            private K time;
            private T data;
            public int occurancesInclusive;
            public TimelineNode(K time, T data)
            {
                this.time = time;
                this.data = data;
            }
            public T Data { get => data; }
            public K Time { get => time; }
        }

        private static K closestElementBinarySearch(IList<K> inputArray, K key)
        {
            
            int min = 0;
            int max = inputArray.Count - 1;
            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (key.Equals(inputArray[mid]))
                {
                    return inputArray[++mid];
                }
                else if (key.CompareTo(inputArray[mid]) < 0)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
            return inputArray[Mathf.Clamp(min,0,inputArray.Count-1)];
        }
    }

}
