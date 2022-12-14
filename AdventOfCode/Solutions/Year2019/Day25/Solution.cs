using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class Day25 : ASolution
    {
        Intcode computer;

        public Day25() : base(25, 2019, "Cryostasis")
        {
            computer = new(Input);
        }

        protected override string SolvePartOne()
        {
            var inputs = new string[]
            {
                // Initial room has doors east, south, west
                "inv",
                // Room 2 has door south and west (room 1)
                "east",
                // Room 3 has doors south and north (room 2)
                "south",
                // "take giant electromagnet",
                // Taking the electromagnnet prevents movement
                // Room 4 has doors north (room 3)
                "south",
            // "take hologram",     //  Too heavy on its own
            #region
                // Back to room 3
                "north",
                // Back to room 2
                "north",
                // Back to room 1
                "west",
                // Room 1 to Room 5 has doors north (room 1), east, and west
                "south",
                #endregion
            "take mouse",
            #region
                // Room 6 has doors west (room 5)
                "east",
                #endregion
            // "take shell",
            #region
                // Back to room 5
                "west",
                // Room 6 has doors east (room 5)
                "west",
                #endregion
            // "take whirled peas",
            #region
                // Back to room 5
                "east",
                // Back to room 1
                "north",
                // Room 1 to Room 7 has doors north, east (room 1), and west
                "west",
                // Room 7 to Room 8 has doors east (room 7), south, and west
                "west",
                #endregion
            "take antenna",
            #region
                // Room 8 to Room 9 has doors east (room 8) and south
                "west",
                // "take photons", // Eaten by a Grue
                // Room 9 to Room 10 has doors north (Room 9) and south
                "south",
                // "take escape pod",
                // Room 10 to Room 11 has doors north (Room 10) and south
                "south",
                // "south" // "Droids on this ship are lighter than the detected value"
                // Back to room 10
                "north",
                // Back to room 9
                "north",
                // Back to room 8
                "east",
                // Room 8 to Room 12 has doors north (Room 8)
                "south",
                #endregion
            // "take spool of cat6",    //  Too heavy on its own
            #region
                // Back to room 8
                "north",
                // Back to room 7
                "east",
                // Room 7 to Room 13 has doors north, south (Room 7), and west
                "north",
                // "take molten lava",
                // Room 13 to Room 14 has rooms south (Room 13) and west
                "north",
                // "take infinite loop",
                // Room 14 to Room 15 has rooms east (Room 14)
                "west",
                #endregion
            "take semiconductor",
            #region
                // Back to room 14
                "east",
                // Back to room 13
                "south",
                // Room 13 to Room 16 has doors north, east (Room 13), and south
                "west",
                // Room 16 to Room 17 has doors south (Room 16)
                "north",
                // Back to Room 16
                "south",
                // Room 16 to Room 18 has doors north (Room 16)
                "south",
                #endregion
            "take hypercube",
            #region
                // Back to Room 16
                "north",
                // Back to Room 13
                "east",
                // Back to Room 7
                "south",
                // Back to Room 1
                "east",
                // Room 1 to 7 to 8 to 9 to 10
                "west", "west", "west", "south", "south",
                "inv",
                // Move into droid space
                "south"
                #endregion
            };

            foreach (var instruction in inputs)
            {
                instruction
                    .ToCharArray()
                    .Select(c => (long)c)
                    .ToList()
                    .ForEach(c => computer.SetInput(c));

                // Append a new line
                computer.SetInput(10);
            }

            computer.Run();

            // Print the output
            foreach(var c in computer.output_register)
            {
                Console.Write((char)c);
            }

            Console.WriteLine();

            return string.Empty;
        }

        protected override string SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
