using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class Day21 : ASolution
    {
        private Intcode droid { get; set; }

        public Day21() : base(21, 2019, "Springdroid Adventure")
        {
            droid = new(Input);
        }

        private void WriteOutputRegister()
        {
            // Outside the standard ASCII range, skip this
            Console.WriteLine("Output:");
            while (droid.output_register.Count > 0)
            {
                if (droid.output_register.Peek() > 128)
                    return;
                    
                Console.Write((char)droid.output_register.Dequeue());
            }
            Console.WriteLine();
        }

        protected override string SolvePartOne()
        {
            // I have no idea what is going on here so I am using someone's SpringDroid answer
            // I had to work out issues with my IntCode to make it work
            // https://old.reddit.com/r/adventofcode/comments/edll5a/2019_day_21_solutions/fr1r0lt/
            var droidProgram = @"NOT B J 
NOT C T
OR T J
AND D J
NOT A T
OR T J 
WALK";

            // Program the code
            string.Join('\n', droidProgram.SplitByNewline(true))
                .Select(c => (long)c)
                .ToList()
                .ForEach(c => droid.SetInput(c));

            // Add another \n
            droid.SetInput((long)'\n');

            droid.Run();
            WriteOutputRegister();

            // If we still have anything in the output, then we have an answer maybe
            if (droid.output_register.Count > 0)
            {
                var output = droid.output_register.Dequeue();

                return output.ToString();
            }

            return string.Empty;
        }

        protected override string SolvePartTwo()
        {
            // I have no idea what is going on here so I am using someone's SpringDroid answer
            // I had to work out issues with my IntCode to make it work
            // https://old.reddit.com/r/adventofcode/comments/edll5a/2019_day_21_solutions/fr1r0lt/
            var droidProgram = @"NOT B J 
NOT C T
OR T J
AND D J
AND H J
NOT A T
OR T J 
RUN";

            // Reset
            droid = new(Input);

            // Program the code
            string.Join('\n', droidProgram.SplitByNewline(true))
                .Select(c => (long)c)
                .ToList()
                .ForEach(c => droid.SetInput(c));

            // Add another \n
            droid.SetInput((long)'\n');

            droid.Run();
            WriteOutputRegister();

            // If we still have anything in the output, then we have an answer maybe
            if (droid.output_register.Count > 0)
            {
                var output = droid.output_register.Dequeue();

                return output.ToString();
            }

            return string.Empty;
        }
    }
}
