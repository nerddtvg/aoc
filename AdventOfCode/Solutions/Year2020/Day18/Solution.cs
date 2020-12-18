using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day18 : ASolution
    {

        public Day18() : base(18, 2020, "")
        {

        }

        private int SolvePuzzle(List<string> input) =>
            SolvePuzzle(string.Join(" ", input));

        private int SolvePuzzle(string input) {
            // This goes entry by character to determine what to do
            // When we hit a '(', we find its corresponding ')' and replace the expression with SolvePuzzle(child_expression)

            string[] parts = input.Split(" ", StringSplitOptions.TrimEntries);
            var finalExpression = new List<string>();

            for(int i=0; i<parts.Length; i++) {
                // Do this because we can't modify a foreach variable
                var part = parts[i];

                var tempExpression = new List<string>();

                // Does this have '('
                if (part.StartsWith('(')) {
                    // Trim it
                    part = part.Substring(1);

                    // Determine how many we have in this entry
                    int pCount = 0;
                    while(part.Substring(pCount, 1) == "(")
                        pCount++;
                    
                    // Add to the tempExpression
                    tempExpression.Add(part);

                    // Now we need to search forward and find the end of this expression
                    int q;
                    for(q=i+1; pCount > 0 && q<parts.Length; q++) {
                        // For each, find any new '(' and add it
                        // Then find any new ')' and remove the count
                        pCount += parts[q].Count(a => a == '(');
                        pCount -= parts[q].Count(a => a == ')');
                    
                        // Add to the tempExpression
                        tempExpression.Add(parts[q]);
                    }

                    // Once pCount is 0, we found the end
                    finalExpression.Add(SolvePuzzle(tempExpression).ToString());

                    // Skip ahead!
                    i = q;
                } else {
                    // This is just a number or operator (should have no parens)
                    finalExpression.Add(part);
                }
            }

            // Now we can parse through this list of items and operate on it
            // Set this to true so we know to expect a number, if we don't get a number, we have a problem
            bool lastOperator = true;
            string op = string.Empty;
            int value = 0;

            foreach(var exp in finalExpression) {
                if (lastOperator && (exp == "*" || exp == "+"))
                    throw new Exception("Received operator when none expected.");
                
                if (!lastOperator && (exp != "*" || exp != "+"))
                    throw new Exception("Received numeric value when operator expected.");
                
                if (lastOperator) {
                    if (op == string.Empty) {
                        // This is the first entry
                        value = Int32.Parse(exp);
                    } else {
                        // We have an operation to perform!
                        if (op == "*")
                            value *= Int32.Parse(exp);
                        else
                            value += Int32.Parse(exp);
                    }
                } else {
                    // Expecting an operator
                    // We've already checked above for a valid operator value
                    lastOperator = true;
                    op = exp;
                }
            }

            return value;
        }

        protected override string SolvePartOne()
        {
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
