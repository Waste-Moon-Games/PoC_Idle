using UnityEngine;

namespace YG
{
    public class ColorYGAttribute : PropertyAttribute
    {
        public Color color { get; private set; }

        public ColorYGAttribute(float r, float g, float b)
        {
            color = new Color(r, g, b);
        }

        public ColorYGAttribute(float r, float g, float b, float a)
        {
            color = new Color(r, g, b, a);
        }

        public ColorYGAttribute()
        {
            color = new Color(1.3f, 1.3f, 1.0f);
        }
    }
}
