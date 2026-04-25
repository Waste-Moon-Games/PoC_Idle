using UnityEngine;

namespace Utils.Audio
{
    public static class AudioUtils
    {
        public static float LinearToDB(float linear)
        {
            if (linear <= 0f) 
                return -80f;

            return Mathf.Log10(linear) * 20f;
        }

        public static float DBToLinear(float db)
        {
            if (db <= -80f)
                return 0f;

            return Mathf.Pow(10f, db / 20f);
        }
    }
}