using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{

    class Day12 : ASolution
    {
        Dictionary<int, bool> plants = new Dictionary<int, bool>();
        Dictionary<string, bool> rules = new Dictionary<string, bool>();

        public Day12() : base(12, 2018, "")
        {

        }

        private void ParseInput() {
            plants = new Dictionary<int, bool>();
            rules = new Dictionary<string, bool>();

            // Initial state is a set of pots 0..end
            // We need to include -1, -2 as no-plants
            plants.Add(-2, false);
            plants.Add(-1, false);

            // Get the first line, second part
            string initial = Input.SplitByNewline()[0].Split(":")[1].Trim();

            // Set the plants
            for(int i=0; i<initial.Length; i++)
                plants.Add(i, initial.Substring(i, 1) == "#" ? true : false);
            
            // Now let's set the rules up (second half of the input)
            foreach(string rule in Input.SplitByBlankLine()[1]) {
                string[] parts = rule.Split("=>").Select(a => a.Trim()).ToArray();

                rules.Add(parts[0], parts[1] == "#" ? true : false);
            }

            Console.WriteLine($"Plant Count: {plants.Where(a => a.Value == true).Count()}");
            Console.WriteLine($"Rule Count: {rules.Count}");
        }

        private void RunGenerations(int count=1) {
            for(int i=0; i<count; i++) {
                RunGeneration();
            
                // Print the Generation
                printGeneration(i+1, -1);
            }
        }

        private void printGeneration(int generation, int start=-2) {
            int minKey = plants.Min(a => a.Key);
            int maxKey = plants.Max(a => a.Key);

            // If this is generation 0, print a header
            if (generation == 0) {
                // Padd for line start
                Console.Write("    ");
                
                // Pad for < 0
                Console.Write("".PadLeft(Math.Abs(start), ' '));

                for(int i=0; i<maxKey*2; i++) {
                    Console.Write(i % 2 == 0 ? (i%10).ToString() : " ");
                }
                Console.WriteLine();
            }

            // Print this generation
            Console.Write(generation.ToString("00: "));

            for(int i=start; i<=maxKey; i++) {
                if (i < minKey) Console.Write(getPlantString(false));
                else
                    Console.Write(getPlantString(plants[i]));
            }

            Console.WriteLine();
        }

        private void RunGeneration() {
            // Loop through the plants and run it!
            Dictionary<int, bool> newGeneration = new Dictionary<int, bool>();

            int minKey = plants.Min(a => a.Key);
            int maxKey = plants.Max(a => a.Key);

            for(int i=minKey-2; i<=maxKey+2; i++) {
                // Create a list of LLCRR plants where C=i
                // If any side plantss don't exist, they are not a plant
                string thisPlant = "";

                // Get before
                if (i-2 < minKey || i-2 > maxKey) thisPlant += getPlantString(false);
                else thisPlant += getPlantString(plants[i-2]);

                if (i-1 < minKey || i-1 > maxKey) thisPlant += getPlantString(false);
                else thisPlant += getPlantString(plants[i-1]);
                
                // Get this plant
                if (i < minKey || i > maxKey) thisPlant += getPlantString(false);
                else thisPlant += getPlantString(plants[i]);

                // Get after
                if (i+1 < minKey || i+1 > maxKey) thisPlant += getPlantString(false);
                else thisPlant += getPlantString(plants[i+1]);

                if (i+2 < minKey || i+2 > maxKey) thisPlant += getPlantString(false);
                else thisPlant += getPlantString(plants[i+2]);

                // New plant!
                newGeneration.Add(i, rules[thisPlant]);
            }

            // If we start or end with 5 non-plants, remove 3 to keep the strings shorter
            if (!newGeneration[minKey-2] && !newGeneration[minKey-2] && !newGeneration[minKey] && !newGeneration[minKey+1] && !newGeneration[minKey+2]) {
                newGeneration.Remove(minKey-2);
                newGeneration.Remove(minKey-1);
                newGeneration.Remove(minKey);
            }
            
            if (!newGeneration[maxKey-2] && !newGeneration[maxKey-2] && !newGeneration[maxKey] && !newGeneration[maxKey+1] && !newGeneration[maxKey+2]) {
                newGeneration.Remove(maxKey+2);
                newGeneration.Remove(maxKey+1);
                newGeneration.Remove(maxKey);
            }

            plants = newGeneration;
        }

        private string getPlantString(bool isPlant) => isPlant ? "#" : ".";

        protected override string SolvePartOne()
        {
            ParseInput();

            // Print the Initial Generation
            //printGeneration(0, -4);
            //RunGenerations(20);

            return GetSum().ToString();
        }

        protected int GetSum() => plants.Where(a => a.Value == true).Sum(a => a.Key);

        protected override string SolvePartTwo()
        {
            // There has to be a pattern. Perhaps it is a pattern in the counts
            List<int> sums = new List<int>();

            sums.Add(GetSum());

            for(int i=1; i<=10000; i++) {
                RunGeneration();
                
                // Now let's get the sum
                int sum = GetSum();

                // Print some help
                Console.WriteLine($"After {i}: {sum} [{sum-sums[sums.Count-1]}] " + (sums.Contains(sum) ? "*" : ""));

                sums.Add(sum);
            }

            // After doing that, we found this:
            /*
            After 89: 2115 [-68] 
            After 90: 2047 [-68] 
            After 91: 2062 [15] 
            After 92: 2077 [15] 
            After 93: 2092 [15] 
            After 94: 2107 [15] 
            After 95: 2122 [15] *
            After 96: 2137 [15] 
            After 97: 2152 [15] 
            After 98: 2167 [15] 
            After 99: 2182 [15] 
            */

            // So we just add 15 to every generation after 90 (2047)
            ulong genStart = 90;
            ulong genValue = 2047;
            ulong genEnd = 50000000000;
            ulong diff = 15;

            return (genValue + ((genEnd - genStart) * diff)).ToString();
        }
    }
}
