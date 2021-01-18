using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private EntityManager m_manager;
    private Entity m_sceneLoader;

    public Button preloadButton, transitionButton, cleanUpButton;
    public Toggle sample;

    void Start()
    {
        m_manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        m_sceneLoader = m_manager.CreateEntity(typeof(LevelScene.Loader.DoPreload), typeof(LevelScene.Loader.DoTransition));
    }

    public void OnPreloadScene()
    {
        m_manager.AddComponent<LevelScene.Loader.DoPreload>(m_sceneLoader);
        preloadButton.interactable = !(transitionButton.interactable = true);
    }

    public void OnTransition()
    {
        m_manager.AddComponent<LevelScene.Loader.DoTransition>(m_sceneLoader);
        transitionButton.interactable = !(cleanUpButton.interactable = true);
    }

    public void OnCleanup()
    {
        m_manager.AddComponent<LevelScene.Loader.DoCleanup>(m_sceneLoader);
        cleanUpButton.interactable = !(preloadButton.interactable = true);
    }

    public void OnToggle()
    {
        if (sample.isOn)
        {
            m_manager.AddComponent<LevelScene.Loader.DoLoad>(m_sceneLoader);
        }
        else
        {
            m_manager.AddComponent<LevelScene.Loader.DoUnload>(m_sceneLoader);
        }
    }
}
