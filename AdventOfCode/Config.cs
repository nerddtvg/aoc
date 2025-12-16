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
    partial class Config
    {

        [GeneratedRegex("^session=[a-z0-9]+$")]
        private static partial Regex SessionRegex();

        /// <summary>
        /// Advent of Code Session Cookie
        /// </summary>
        public string Cookie
        {
            get;
            set
            {
                if (!SessionRegex().IsMatch(value))
                    throw new ArgumentException("Cookie must be in the format 'session=[a-z0-9]+'");

                field = value;
            }
        } = string.Empty;

        /// <summary>
        /// Puzzle Year
        /// </summary>
        public int Year
        {
            get;
            set
            {
                if (value >= 2015 && value <= DateTime.Now.Year) field = value;
            }
        }

        /// <summary>
        /// Puzzle Day(s)
        /// </summary>
        /// <value></value>
        [JsonConverter(typeof(DaysConverter))]
        public int[] Days
        {
            get;
            set
            {
                bool allDaysCovered = false;
                field = [.. value.Where(v =>
                {
                    if (v == 0) allDaysCovered = true;
                    return v > 0 && v < 26;
                })];

                if (allDaysCovered)
                {
                    field = [0];
                }
                else
                {
                    Array.Sort(field);
                }
            }
        } = [0];

        void SetDefaults()
        {
            //Make sure we're looking at EST, or it might break for most of the US
            DateTime CURRENT_EST = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Utc).AddHours(-5);
            if (Cookie == default) Cookie = string.Empty;
            if (Year == default) Year = CURRENT_EST.Year;
            if (Days == default(int[])) Days = (CURRENT_EST.Month == 12 && CURRENT_EST.Day <= 25) ? [CURRENT_EST.Day] : [0];
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
                config.SetDefaults();
            }
            else
            {
                config.SetDefaults();
                File.WriteAllText(path, JsonSerializer.Serialize(config, options));
            }

            return config;
        }
    }

    class DaysConverter : JsonConverter<int[]>
    {
        public override int[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number) return [reader.GetInt16()];
            var tokens = reader.TokenType == JsonTokenType.String
                ? [reader.GetString() ?? string.Empty]
                : (JsonSerializer.Deserialize<object[]>(ref reader)?.Select(o => o.ToString() ?? string.Empty) ?? []);
            return [.. tokens.SelectMany(ParseString)];
        }

        private IEnumerable<int> ParseString(string str)
        {
            return str.Split(",").SelectMany(str =>
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
                    return [day];
                }
                return [];
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
