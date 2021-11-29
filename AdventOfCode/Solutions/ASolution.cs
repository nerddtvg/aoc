using System;
using System.IO;
using System.Net;
using System.Net.Http;

#nullable enable

namespace AdventOfCode.Solutions
{

    abstract class ASolution
    {

        Lazy<string?> _input, _part1, _part2;

        public int Day { get; }
        public int Year { get; }
        public string Title { get; }
        public string DebugInput { get; set; } = string.Empty;
        public string Input => !string.IsNullOrEmpty(DebugInput) ? DebugInput : (string.IsNullOrEmpty(_input.Value) ? string.Empty : _input.Value);
        public string Part1 => string.IsNullOrEmpty(_part1.Value) ? string.Empty : _part1.Value;
        public string Part2 => string.IsNullOrEmpty(_part2.Value) ? string.Empty : _part2.Value;

        private protected ASolution(int day, int year, string title)
        {
            Day = day;
            Year = year;
            Title = title;
            _input = new Lazy<string?>(() => LoadInput());
            _part1 = new Lazy<string?>(() => SolvePartOne());
            _part2 = new Lazy<string?>(() => SolvePartTwo());
        }

        public void Solve(int part = 0)
        {
            if(string.IsNullOrEmpty(Input)) return;

            string output = $"--- Day {Day}: {Title} --- \n";
            if(!string.IsNullOrEmpty(DebugInput))
            {
                output += $"!!! DebugInput used: {DebugInput}\n";
            }

            try
            {
                if (part != 2)
                {
                    if (!string.IsNullOrEmpty(Part1))
                    {
                        output += $"Part 1: {Part1}\n";
                    }
                    else
                    {
                        output += "Part 1: Unsolved\n";
                    }
                }
                if (part != 1)
                {
                    if (!string.IsNullOrEmpty(Part2))
                    {
                        output += $"Part 2: {Part2}\n";
                    }
                    else
                    {
                        output += "Part 2: Unsolved\n";
                    }
                }
            }
            catch (Exception ex)
            {
                // Catching exceptions from the solution code
                Console.WriteLine("Exception caught:");
                Console.WriteLine(ex.Message);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("--- Inner Exception ---");
                    Console.WriteLine(ex.InnerException.Message);
                    if (!string.IsNullOrEmpty(ex.InnerException.StackTrace))
                        Console.WriteLine(ex.InnerException.StackTrace);
                    Console.WriteLine("--- End Inner Exception ---");
                }

                if (!string.IsNullOrEmpty(ex.StackTrace))
                    Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine(output);
        }

        string LoadInput()
        {
            string INPUT_FILEPATH = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, $"../../../Solutions/Year{Year}/Day{Day.ToString("D2")}/input"));
            string INPUT_URL = $"https://adventofcode.com/{Year}/day/{Day}/input";
            string input = "";

            if(File.Exists(INPUT_FILEPATH) && new FileInfo(INPUT_FILEPATH).Length > 0)
            {
                input = File.ReadAllText(INPUT_FILEPATH);
            }
            else
            {
                try
                {
                    TimeZoneInfo estZone;

                    // Avoiding hard-coding in the timezone offset from UTC
                    try {
                        estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    } catch(TimeZoneNotFoundException) {
                        estZone = TimeZoneInfo.FindSystemTimeZoneById("US/Eastern");
                    }
                    
                    DateTimeOffset CURRENT_EST = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, estZone);
                    if(CURRENT_EST < new DateTimeOffset(Year, 12, Day, 0, 0, 0, estZone.GetUtcOffset(DateTimeOffset.UtcNow))) throw new InvalidOperationException();

                    var cookieContainer = new CookieContainer();
                    using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                    using(var client = new HttpClient(handler))
                    {
                        // Get the base URI for this cookie
                        var uri = new Uri(INPUT_URL);

                        // Add the cookie
                        var cookie = Program.Config.Cookie.Split('=', 2, StringSplitOptions.TrimEntries);
                        cookieContainer.Add(uri, new Cookie(cookie[0], cookie.Length > 1 ? cookie[1] : string.Empty));

                        input = client.GetStringAsync(INPUT_URL).Result.TrimEnd();
                        File.WriteAllText(INPUT_FILEPATH, input);
                    }
                }
                catch(WebException e)
                {
                    if (e.Response == null)
                    {
                        Console.WriteLine($"Day {Day}: An invalid response was provided in the WebException.");
                    }
                    else
                    {
                        var statusCode = ((HttpWebResponse)e.Response).StatusCode;
                        if (statusCode == HttpStatusCode.BadRequest)
                        {
                            Console.WriteLine($"Day {Day}: Error code 400 when attempting to retrieve puzzle input through the web client. Your session cookie is probably not recognized.");
                        }
                        else if (statusCode == HttpStatusCode.NotFound)
                        {
                            Console.WriteLine($"Day {Day}: Error code 404 when attempting to retrieve puzzle input through the web client. The puzzle is probably not available yet.");
                        }
                        else
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
                catch(InvalidOperationException)
                {
                    Console.WriteLine($"Day {Day}: Cannot fetch puzzle input before given date (Eastern Standard Time).");
                }
                catch(TimeZoneNotFoundException)
                {
                    Console.WriteLine($"Day {Day}: Unable to find the time zone (Eastern Time Zone or US/Eastern) in the system definitions.");
                }
            }
            return input;
        }

        protected abstract string? SolvePartOne();
        protected abstract string? SolvePartTwo();
    }
}

#nullable restore
