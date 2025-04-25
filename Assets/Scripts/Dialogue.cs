using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    [TextArea(3, 6)]
    public string dialogueText;
    public bool isPlayer = true;
}
