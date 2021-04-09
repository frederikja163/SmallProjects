using OpenTK.Mathematics;
using SysRandom = System.Random;

namespace Engine
{
    public static class RandomF
    {
        private static readonly SysRandom Random = new SysRandom();

        public static float NextFloat(float min = 0, float max = 1)
        {
            return (float)Random.NextDouble() * (max - min) + min;
        }

        public static Vector2 NextVector2(float min = 0, float max = 1)
        {
            return new Vector2(NextFloat(min, max), NextFloat(min, max));
        }
    }
}