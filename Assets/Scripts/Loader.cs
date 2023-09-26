using UnityEngine.SceneManagement;

public static class Loader {
    public enum Scene
    {
        MainMenuScene,
        DeathScene,
        ValleyofSolura,
        LoadingScene
    }
    
    public static Scene targetScene;

    public static void Load(Scene scene)
    {
        targetScene = scene;
        DataPersistenceManager.Instance.SaveGame();
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
