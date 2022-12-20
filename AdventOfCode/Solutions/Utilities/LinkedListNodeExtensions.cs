using System;
using System.Collections.Generic;
using System.Linq;


namespace AdventOfCode.Solutions
{
    public static class LinkedListNodeExtensions
    {
        /// <summary>
        /// Step forwards or backwards a given number of steps
        /// </summary>
        /// <param name="source">The node to start from</param>
        /// <param name="step">An integer representing steps to take. If negative, the Previous nodes will be used. If positive, the Next nodes will be used.</param>
        /// <returns>The found node</returns>
        public static LinkedListNode<TSource> GetNodeByStep<TSource>(this LinkedListNode<TSource> source, int step)
        {
            var temp = source;

            // If we have no List or Count, how are we here?
            if (source.List == default)
                throw new InvalidOperationException("Source node is not linked to a list.");

            if (source.List.Count == 0)
                throw new InvalidOperationException("The source node's list has length of zero.");

            // How many to go?
            int count = Math.Abs(step) % source.List.Count;

            if (step < 0)
                for (int i = 0; i < count; i++)
                    temp = temp.Previous ?? temp.List?.Last!;
            else
                for (int i = 0; i < count; i++)
                    temp = temp.Next ?? temp.List?.First!;

            return temp;
        }
    }
}

