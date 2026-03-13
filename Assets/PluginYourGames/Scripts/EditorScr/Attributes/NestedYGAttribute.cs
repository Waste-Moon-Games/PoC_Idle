using UnityEngine;

namespace YG
{
    public sealed class NestedYGAttribute : PropertyAttribute
    {
        public string[] propertyNames { get; }
        public bool drawLine { get; }
        public int offset { get; }
        public int offsetLine { get; }

        public NestedYGAttribute(params string[] propNames)
        {
            propertyNames = propNames;
            drawLine = true;
            offset = 20;
        }

        public NestedYGAttribute(bool drawLine, params string[] propNames)
        {
            propertyNames = propNames;
            this.drawLine = drawLine;
            offset = 20;
        }

        public NestedYGAttribute(int offset, params string[] propNames)
        {
            propertyNames = propNames;
            this.offset = offset;
            drawLine = offset != 0;
        }

        public NestedYGAttribute(bool drawLine, int offset, params string[] propNames)
        {
            propertyNames = propNames;
            this.drawLine = drawLine;
            this.offset = offset;
        }

        public NestedYGAttribute(int offsetLine, int offset, params string[] propNames)
        {
            propertyNames = propNames;
            drawLine = true;
            this.offsetLine = offsetLine;
            this.offset = offset;
        }
    }
}
