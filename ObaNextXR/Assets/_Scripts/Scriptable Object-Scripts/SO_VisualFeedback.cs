using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Scriptable_Object_Scripts
{
    [CreateAssetMenu(fileName = "New Visual Feedback", menuName = "Visual Feedback Data ")]
    public class SO_VisualFeedback : ScriptableObject
    {
            public Color backgroundColor;
            public Sprite sprite;
            
            public void Apply(Image backgroundImage, Image targetSprite)
            {
                if (backgroundImage is not null)
                    backgroundImage.color = backgroundColor;

                if (targetSprite is not null && sprite is not null)
                    targetSprite.sprite = sprite;
            }

            public void Reset(Image targetSprite) => targetSprite?.gameObject.SetActive(false);
    }
}

