using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOSExt.EMP.Impl
{
    internal static class Random
    {
        // EEC.Utils.Rand
        internal static System.Random _rand { get; private set; } = new System.Random();

        internal static float GetRandomDelay(float min, float max) => min + GetRandom01() * (max - min);

        internal static float GetRandom01() => (float)_rand.NextDouble();

        internal static int GetRandomRange(int min, int maxPlusOne) => _rand.Next(min, maxPlusOne);

        internal static int Index(int length) => _rand.Next(0, length);

        internal static bool FlickerUtil(int oneInX = 2) => Index(oneInX) == 0;
    }
}
