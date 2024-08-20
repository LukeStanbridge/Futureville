using UnityEngine;

[System.Serializable]
public class Instruction
{
    [TextArea(6, 20)]
    public string Description;

    public Sprite InstructionObjectImage;

    public Vector2 ImageDimensions;

    public float Scale = 1f;

    public bool TextWithImage;
}
