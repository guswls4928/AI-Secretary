using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Growth Data", menuName = "Scriptable Object/Growth Data", order = int.MaxValue)]

public class NewBehaviourScript : ScriptableObject
{
    [SerializeField]
    private string characterName;
    public string CharacterName { get { return characterName; } }

    [SerializeField, Range(0f, 5f)]
    private Vector3 scale;
    private Vector3 Scale { get { return scale; } }

}
