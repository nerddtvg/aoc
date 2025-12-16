using AdventOfCode.Solutions;

namespace AdventOfCode
{

    class Program
    {

        public static Config Config = Config.Get("config.json");
        static readonly SolutionCollector Solutions = new(Config.Year, Config.Days);

        static void Main(string[] args)
        {
            foreach(ASolution solution in Solutions)
            {
                solution.Solve();
            }
        }
    }
}
