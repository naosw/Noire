using UnityEngine.SceneManagement;
using System;

public static class Loader {
    public enum Scene
    {
        MainMenuScene,
        DeathScene,
        ValleyofSolura,
        LoadingScene
    }
    
    public const Scene firstScene = Scene.ValleyofSolura;
    
    public static Scene targetScene;

    public static void Load(Scene scene)
    {
        targetScene = scene;
        DataPersistenceManager.Instance.SaveGame();
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    
    // overloading: load using string scene name
    public static void Load(string scene)
    {
        targetScene = Enum.Parse<Scene>(scene);
        DataPersistenceManager.Instance.SaveGame();
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
