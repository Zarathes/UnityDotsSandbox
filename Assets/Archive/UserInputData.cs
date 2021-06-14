using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct UserInputData : IComponentData
{
    public KeyCode upAction;
    public KeyCode downAction;
    public KeyCode rightAction;
    public KeyCode leftAction;
    public KeyCode jumpAction;
}