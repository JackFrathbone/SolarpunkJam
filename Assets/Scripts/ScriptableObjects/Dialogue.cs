using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
public class DialogueObject : ScriptableObject
{
    public List<Dialogue> dialogues = new();
}
