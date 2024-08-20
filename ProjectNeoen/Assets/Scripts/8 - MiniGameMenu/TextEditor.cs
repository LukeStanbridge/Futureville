using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Text", menuName = "CustomTextEditor")]
public class TextEditor : ScriptableObject //text object for lengthy game descriptions
{
    [SerializeField] private TMP_SpriteAsset _number;

    [TextArea(10, 100)]
    public string textAreaString;
}