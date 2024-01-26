using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "KeyColorLayouts", menuName = "ScriptableObjects/KeyColorLayouts", order = 1)]
public class KeyColorLayoutsScriptableObject : ScriptableObject
{
    public Key[] redKeys;
    public Key[] greenKeys;
    public Key[] blueKeys;
    public Key[] yellowKeys;
}
