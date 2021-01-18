using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameModeData", menuName = "Assets/GameModeData")]
public class GameModeData : ScriptableObject
{
    [Header("Levels Manager")]
    public List<SceneInfo> SceneList = new List<SceneInfo>();

    [Serializable]
    public class SceneInfo
    {
        [SerializeField]
        public UnityEngine.Object m_Scene;
        [SerializeField]
        public SceneDefinition sceneDefinition;
    }
}