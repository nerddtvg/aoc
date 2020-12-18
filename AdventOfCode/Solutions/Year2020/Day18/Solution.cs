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

        private long SolvePuzzle(List<string> input, int PuzzlePart=1) =>
            SolvePuzzle(string.Join(" ", input), PuzzlePart);

        private long SolvePuzzle(string input, int PuzzlePart=1) {
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
                    int pCount = 1;
                    while(part.Substring(pCount-1, 1) == "(")
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

                        // If this is the very last one (pCount is now 0), we need to remove the last paren
                        // Add to the tempExpression
                        tempExpression.Add(pCount == 0 ? parts[q].Substring(0, parts[q].Length-1) : parts[q]);
                    }

                    // Once pCount is 0, we found the end
                    finalExpression.Add(SolvePuzzle(tempExpression, PuzzlePart).ToString());

                    // Skip ahead!
                    i = q-1;
                } else {
                    // This is just a number or operator (should have no parens)
                    finalExpression.Add(part);
                }
            }

            // Now we can parse through this list of items and operate on it
            // Set this to true so we know to expect a number, if we don't get a number, we have a problem
            bool lastOperator = true;
            string op = string.Empty;
            long value = 0;

            if (PuzzlePart == 1) {
                foreach(var exp in finalExpression) {
                    if (lastOperator && (exp == "*" || exp == "+"))
                        throw new Exception("Received operator when none expected.");
                    
                    if (!lastOperator && (exp != "*" && exp != "+"))
                        throw new Exception("Received numeric value when operator expected.");
                    
                    if (lastOperator) {
                        lastOperator = false;

                        if (op == string.Empty) {
                            // This is the first entry
                            value = Int64.Parse(exp);
                        } else {
                            // We have an operation to perform!
                            if (op == "*")
                                value *= Int64.Parse(exp);
                            else
                                value += Int64.Parse(exp);
                        }
                    } else {
                        // Expecting an operator
                        // We've already checked above for a valid operator value
                        lastOperator = true;
                        op = exp;
                    }
                }
            } else {
                // For part 2, we will parse through and "solve" for the addition first, then simply go through this again with part=1
                if (!finalExpression.Contains("+")) return SolvePuzzle(finalExpression, 1);

                List<string> tempExpression = new List<string>();
                long lastValue = 0;
                bool lastOperatorWasAddition = false;
                
                // Need to find all of the "+" positions and "fix" them
                for(int i=1; i<finalExpression.Count-1; i+=2) {
                    // We hop forward to only look at operators
                    if (finalExpression[i] == "+") {
                        // If we have a length, we need to remove the last to replace it
                        if (tempExpression.Count > 0)
                            tempExpression.RemoveAt(tempExpression.Count-1);

                        lastValue = (lastOperatorWasAddition ? lastValue : Int64.Parse(finalExpression[i-1])) + Int64.Parse(finalExpression[i+1]);
                        tempExpression.Add(lastValue.ToString());

                        lastOperatorWasAddition = true;
                    } else {
                        // Not addition, just append these entries (if the last one was addition, we already have the entry)
                        if (tempExpression.Count == 0) tempExpression.Add(finalExpression[i-1]);
                        tempExpression.Add(finalExpression[i]);
                        tempExpression.Add(finalExpression[i+1]);

                        lastOperatorWasAddition = false;
                    }
                }

                return SolvePuzzle(tempExpression, 1);
            }

            return value;
        }

        protected override string SolvePartOne()
        {
            return Input.SplitByNewline(true, true).Sum(a => SolvePuzzle(a)).ToString();
        }

        protected override string SolvePartTwo()
        {
            return Input.SplitByNewline(true, true).Sum(a => SolvePuzzle(a, 2)).ToString();
        }
    }
}
