using System;
using System.Collections.Generic;
using System.Linq;


namespace AdventOfCode.Solutions
{
    public static class LinkedListExtensions
    {
        /// <summary>
        /// Returns the first <see cref="System.Collections.Generic.LinkedListNode{T}"/> in a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">An <see cref="System.Collections.Generic.LinkedList{T}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <returns>The first <see cref="System.Collections.Generic.LinkedListNode{T}"/> in the sequence that passes the test in the specified predicate function.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is <see langword="null" />.</exception>
        /// <exception cref="System.InvalidOperationException">No element satisfies the condition in the predicate. -or- The source sequence is empty.</exception>
        public static LinkedListNode<TSource> FirstNode<TSource>(this LinkedList<TSource> source, Func<TSource, bool> predicate)
        {
            // Make sure we have a source
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // Make sure we have a function
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (source.First == null)
                throw new InvalidOperationException("The source is empty.");

            var node = source.First;

            while (node != null)
            {
                if (predicate(node.Value))
                    return node;

                // Move on
                node = node.Next;
            }

            // Nothing found
            throw new InvalidOperationException("No result found.");
        }

        /// <summary>
        /// Returns the last <see cref="System.Collections.Generic.LinkedListNode{T}"/> in a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">An <see cref="System.Collections.Generic.LinkedList{T}"/> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <returns>The last <see cref="System.Collections.Generic.LinkedListNode{T}"/> in the sequence that passes the test in the specified predicate function.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is <see langword="null" />.</exception>
        /// <exception cref="System.InvalidOperationException">No element satisfies the condition in the predicate. -or- The source sequence is empty.</exception>
        public static LinkedListNode<TSource> LastNode<TSource>(this LinkedList<TSource> source, Func<TSource, bool> predicate)
        {
            // Make sure we have a source
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // Make sure we have a function
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (source.Last == null)
                throw new InvalidOperationException("The source is empty.");

            var node = source.Last;

            while (node != null)
            {
                if (predicate(node.Value))
                    return node;

                // Move on
                node = node.Previous;
            }

            // Nothing found
            throw new InvalidOperationException("No result found.");
        }
    }
}

