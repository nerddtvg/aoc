using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions
{
    // Cloning a struct by simply passing a copy
    // Based on: https://stackoverflow.com/a/60007528
    public static class StructExtension
    {
        public static T Clone<T>(this T inStruct) where T : struct => inStruct;
    }
}
