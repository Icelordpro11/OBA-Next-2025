using UnityEngine;

namespace Artem_Library.Attribute_Scripts
{
    public class LineAttribute: PropertyAttribute
    {
        public int Thickness = 1;
        public float Padding = 0f;
        public colorType Color;

        public LineAttribute()
        {
        
        }
    }

    public enum colorType
    {
        Red,
        Blue,
        Gray,
        Black,
        Yellow,
        Cyan,
        Green,
        Normal,
        Magenta,
        White
    }
}