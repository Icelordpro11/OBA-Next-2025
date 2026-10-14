using System;
using Artem_Library.Attribute_Scripts;
using UnityEditor;
using UnityEngine;

namespace Artem_Library.Editor
{
    [CustomPropertyDrawer(typeof(LineAttribute))]
    public class HorizontalLineDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            var attr = attribute as LineAttribute;
            return Mathf.Max(attr!.Padding, attr.Thickness);
        }

        public override void OnGUI(Rect position)
        {
            var attr = attribute as LineAttribute;
            position.height = attr!.Thickness * 0.5f;
            position.y += attr.Padding * .5f;
            
            EditorGUI.DrawRect(position, EditorGUIUtility.isProSkin?  Choose(attr.Color): new Color(.7f,.7f,.7f,.7f));
        }

        private static Color Choose(colorType type) =>
            type switch
            {
                colorType.Red => Color.red,
                colorType.Blue => Color.blue,
                colorType.Gray => Color.gray,
                colorType.Black => Color.black,
                colorType.Yellow => Color.yellow,
                colorType.Cyan => Color.cyan,
                colorType.Green => Color.green,
                colorType.Normal =>  new Color(.3f,.3f,.3f,.3f),
                colorType.Magenta => Color.magenta,
                colorType.White => Color.white,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
    }
}