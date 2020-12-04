using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{

    class PlanetTile {
        public int x {get;set;}
        public int y {get;set;}
        public string id {get;set;}
        public int area {get;set;}
        public bool edge {get;set;}
    }

    class SpaceTile {
        public int x {get;set;}
        public int y {get;set;}
        public string id {get;set;}
        public bool planet {get;set;}
        public List<(string id, int distance)> distances {get;set;}
        public int totalDistance {get;set;}
    }

    class Day06 : ASolution
    {

        public List<PlanetTile> planets = new List<PlanetTile>();
        public List<SpaceTile> tiles = new List<SpaceTile>();

        public Day06() : base(06, 2018, "")
        {
            // Parse each
            char id = '(';
            foreach(string line in Input.SplitByNewline()) {
                string[] xy = line.Split(",");
                
                planets.Add(new PlanetTile() {
                    x = Int32.Parse(xy[0]),
                    y = Int32.Parse(xy[1].Trim()),
                    id = (id++).ToString(),
                    area = 0,
                    edge = false
                });
            }

            // Loop through all of the tiles possible and find the closest
            // Padding the width/height for Part 2 calculations
            int minX = planets.Select(a => a.x).Min() - 200;
            int maxX = planets.Select(a => a.x).Max() + 200;
            int minY = planets.Select(a => a.y).Min() - 200;
            int maxY = planets.Select(a => a.y).Max() + 200;

            Console.WriteLine($"Min XY: {minX}, {minY}");
            Console.WriteLine($"Max XY: {maxX}, {maxY}");

            bool draw = false;

            for(int y=minY; y<=maxY; y++) {
                for(int x=minX; x<=maxX; x++) {
                    // Is this location a planet?
                    PlanetTile? planet = planets.FirstOrDefault(a => a.x == x && a.y == y);
                    
                    // Loop through the planets to find the closest
                    // We do this for planets as well to account for part 2
                    var distances = planets.Select(a => (a.id, Distance(x, y, a))).OrderBy(a => a.Item2).ToList();
                    int totalDistance = distances.Sum(a => a.Item2);

                    if (planet != null) {
                        // Set the info
                        tiles.Add(new SpaceTile() { x = x, y = y, id = planet.id, planet = true, distances = distances, totalDistance = totalDistance });
                        if (draw) Console.Write(planet.id);
                    } else {
                        // At least two planets have the same distance
                        // Save the distances calculated for later
                        if (distances[0].Item2 == distances[1].Item2) {
                            tiles.Add(new SpaceTile() { x = x, y = y, id = ".", planet = false, distances = distances, totalDistance = totalDistance });
                            if (draw) Console.Write(".");
                        } else {
                            tiles.Add(new SpaceTile() { x = x, y = y, id = distances[0].id, planet = false, distances = distances, totalDistance = totalDistance });
                            if (draw) Console.Write(distances[0].id);

                            // Mark any planets on the edges
                            if (x == minX || y == minY || x == maxX || y == maxY)
                                planets.Where(a => a.id == distances[0].id).First().edge = true;
                        }
                    }
                }

                if (draw) Console.WriteLine();
            }

            // Calculate area sizes for planets
            // We include the planet
            planets.ForEach(a => a.area = tiles.Count(b => b.id == a.id));
        }

        protected int Distance(int x, int y, PlanetTile tile) => Math.Abs(x - tile.x) + Math.Abs(y - tile.y);

        protected override string SolvePartOne()
        {
            return planets.Where(a => a.edge == false).OrderByDescending(a => a.area).First().area.ToString();
        }

        protected override string SolvePartTwo()
        {
            // What is the size of the region containing all locations which have a total distance to all given coordinates of less than 10000?
            return tiles.Count(a => a.totalDistance < 10000).ToString();
        }
    }
}
