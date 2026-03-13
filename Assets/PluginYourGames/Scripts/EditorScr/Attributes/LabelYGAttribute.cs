using UnityEngine;

namespace YG
{
    public class LabelYGAttribute : PropertyAttribute
    {
        public string label { get; private set; }
        public string color { get; private set; }

        public LabelYGAttribute(string label)
        {
            this.label = label;
        }

        public LabelYGAttribute(string label, string color)
        {
            this.label = label;
            this.color = color;
        }
    }
}
