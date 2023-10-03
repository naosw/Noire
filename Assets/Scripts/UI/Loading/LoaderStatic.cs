using System;

public static class LoaderStatic {
    public const GameScene firstScene = GameScene.ValleyofSolura;

    public static GameScene targetScene;
    
    public static void Load(GameScene scene)
    {
        targetScene = scene;
        SceneTransitioner.Instance.LoadScene(GameScene.LoadingScene.ToString(), SceneTransitionMode.Fade);
        // SceneTransitioner.Instance.LoadScene(scene.ToString(), SceneTransitionMode.Fade);
    }
    
    // overloading: load using string scene name
    public static void Load(string scene)
    {
        targetScene = Enum.Parse<GameScene>(scene);;
        SceneTransitioner.Instance.LoadScene(GameScene.LoadingScene.ToString(), SceneTransitionMode.Fade);
        // SceneTransitioner.Instance.LoadScene(scene, SceneTransitionMode.Fade);
    }
}
