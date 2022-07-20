
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AugustEngine.Collections
{
    /// <summary>
    /// A timeline is a special kind of list.
    /// It knows the number of Objects that occur before, at, or after any
    /// time on the timeline
    /// </summary>
    public class Timeline<K, T> where K : System.IComparable
    {
        private SortedList<K, TimelineNode<T>> _nodes;
        public Timeline()
        {
            _nodes = new SortedList<K, TimelineNode<T>>();
        }
        public int OccurancesUpTo(K time)
        {
            return _nodes[closestElementBinarySearch(_nodes.Keys,time)].occurancesInclusive;
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
                return _nodes[time].dataPoints[0];
            }
            return default(T);
        }

        /// <summary>
        /// Returns all elements
        /// At this time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<T> GetAll(K time)
        {
            if (_nodes.ContainsKey(time))
            {
                return _nodes[time].dataPoints;
            }
            return null;
        }

        public void Add(K time, T occurance)
        {
            //check if it contains something here already
            if (!_nodes.ContainsKey(time))
            {
                _nodes[time] = new TimelineNode<T>(time);
            }
            _nodes[time].Add(occurance);

            //work your way down and increase all the occurances at each node above this one by 1
            var _keys = _nodes.Keys;
            for (int i = _keys.Count -1; i >= 0; i--)
            {
                //increase this occurance
                _nodes[_keys[i]].occurancesInclusive++;
                //break if this is the one we just added
                if (_nodes[_keys[i]].time.Equals(time)) break;
            }
        }

        /// <summary>
        /// The node that occurs at a given time
        /// </summary>
        private class TimelineNode<T>
        {
            public K time;
            public int occurancesInclusive;
            public TimelineNode(K time)
            {
                this.time = time;
            }
            public List<T> dataPoints = new List<T>();
            public void Add(T occurance)
            {
                dataPoints.Add(occurance);
            }
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
            return inputArray[min];
        }
    }

}
