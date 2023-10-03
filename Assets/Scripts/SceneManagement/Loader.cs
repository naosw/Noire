using System;

public static class Loader {
    public const GameScene firstScene = GameScene.ValleyofSolura;

    public static string targetScene;
    
    public static void Load(GameScene scene)
    {
        targetScene = scene.ToString();
        SceneTransitioner.Instance.LoadScene(GameScene.LoadingScene.ToString(), SceneTransitionMode.Fade);
    }
    
    // overloading: load using string scene name
    public static void Load(string scene)
    {
        targetScene = scene;
        SceneTransitioner.Instance.LoadScene(GameScene.LoadingScene.ToString(), SceneTransitionMode.Fade);
    }
}
