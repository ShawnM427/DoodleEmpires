/*! 
@file PriorityQueue.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date September 27, 2013
@brief PriorityQueue Interface
@version 2.0

@section LICENSE

The MIT License (MIT)

Copyright (c) 2013 Woong Gyu La <juhgiyo@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

@section DESCRIPTION

An Interface for the PriorityQueue Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DoodleEmpires.Engine.Entities.PathFinder.Utils
{
    /// <summary>
    /// Represents a queue where items are sorted into a "priority" list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T> where T : IComparable
    {
        private List<T> m_data;
                
        /// <summary>
        /// Creates a new priority queue
        /// </summary>
        public PriorityQueue()
        {
            this.m_data = new List<T>();
        }

        /// <summary>
        /// Adds an item to the queue
        /// </summary>
        /// <param name="queueItem">The item to add</param>
        public void Enqueue(T queueItem)
        {
            m_data.Add(queueItem);
            m_data.Sort();
        }

        /// <summary>
        /// Clears this queue
        /// </summary>
        public void Clear()
        {
            m_data.Clear();
        }

        /// <summary>
        /// Removes and returns the first item in the queue
        /// </summary>
        /// <returns>The first item in the queue</returns>
        public T Dequeue()
        {
            T frontItem = m_data[0];
            m_data.RemoveAt(0);
            return frontItem;
        }

        /// <summary>
        /// Returns the first item in the queue without removing it
        /// </summary>
        /// <returns>The first item in the queue</returns>
        public T Peek()
        {
            T frontItem = m_data[0];
            return frontItem;
        }

        /// <summary>
        /// Checks if this queue contans an item
        /// </summary>
        /// <param name="queueItem">The item to search for</param>
        /// <returns>True if the queue contains th item</returns>
        public bool Contains(T queueItem)
        {
            return m_data.Contains(queueItem);
        }

        /// <summary>
        /// Gets the number of instances in this queue
        /// </summary>
        public int Count
        {
            get
            {
                return m_data.Count;
            }
        }
    }

}