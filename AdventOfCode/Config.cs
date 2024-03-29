using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    /// <summary>
    /// Application Configuration Class
    /// </summary>
    class Config
    {
        /// <summary>
        /// Backing Field: Cookie
        /// </summary>
        string _c = string.Empty;

        /// <summary>
        /// Backing Field: Year
        /// </summary>
        int _y = 2015;

        /// <summary>
        /// Backing Field: Days
        /// </summary>
        /// <value></value>
        int[] _d = new int[] { 0 };

        /// <summary>
        /// Advent of Code Session Cookie
        /// </summary>
        public string Cookie
        {
            get => _c;
            set
            {
                if (Regex.IsMatch(value, "^session=[a-z0-9]+$")) _c = value;
            }
        }

        /// <summary>
        /// Puzzle Year
        /// </summary>
        public int Year
        {
            get => _y;
            set
            {
                if (value >= 2015 && value <= DateTime.Now.Year) _y = value;
            }
        }

        /// <summary>
        /// Puzzle Day(s)
        /// </summary>
        /// <value></value>
        [JsonConverter(typeof(DaysConverter))]
        public int[] Days
        {
            get => _d;
            set
            {
                bool allDaysCovered = false;
                _d = value.Where(v =>
                {
                    if (v == 0) allDaysCovered = true;
                    return v > 0 && v < 26;
                }).ToArray();

                if (allDaysCovered)
                {
                    _d = new int[] { 0 };
                }
                else
                {
                    Array.Sort(_d);
                }
            }
        }

        void setDefaults()
        {
            //Make sure we're looking at EST, or it might break for most of the US
            DateTime CURRENT_EST = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Utc).AddHours(-5);
            if (Cookie == default(string)) Cookie = string.Empty;
            if (Year == default(int)) Year = CURRENT_EST.Year;
            if (Days == default(int[])) Days = (CURRENT_EST.Month == 12 && CURRENT_EST.Day <= 25) ? new int[] { CURRENT_EST.Day } : new int[] { 0 };
        }

        public static Config Get(string path)
        {
            var options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            Config config = new();
            if (File.Exists(path) && JsonSerializer.Deserialize<Config>(File.ReadAllText(path), options) is Config configCast)
            {
                config = configCast;
                config.setDefaults();
            }
            else
            {
                config.setDefaults();
                File.WriteAllText(path, JsonSerializer.Serialize<Config>(config, options));
            }

            return config;
        }
    }

    class DaysConverter : JsonConverter<int[]>
    {
        public override int[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number) return new int[] { reader.GetInt16() };
            var tokens = reader.TokenType == JsonTokenType.String
                ? new string[] { reader.GetString() ?? string.Empty }
                : (JsonSerializer.Deserialize<object[]>(ref reader)?.Select<object, string>(o => o.ToString() ?? string.Empty) ?? Array.Empty<string>());
            return tokens.SelectMany<string, int>(ParseString).ToArray();
        }

        private IEnumerable<int> ParseString(string str)
        {
            return str.Split(",").SelectMany<string, int>(str =>
            {
                if (str.Contains(".."))
                {
                    var split = str.Split("..");
                    int start = int.Parse(split[0]);
                    int stop = int.Parse(split[1]);
                    return Enumerable.Range(start, stop - start + 1);
                }
                else if (int.TryParse(str, out int day))
                {
                    return new int[] { day };
                }
                return new int[0];
            });
        }

        public override void Write(Utf8JsonWriter writer, int[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (int val in value) writer.WriteNumberValue(val);
            writer.WriteEndArray();
        }
    }
}
