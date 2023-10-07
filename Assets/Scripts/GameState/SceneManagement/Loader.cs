using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class Loader {
    // maps scene -> (in transition, out transition, load type, load mode)
    private static readonly Dictionary<GameScene, SceneInfo> INFO = new()
    {
        { GameScene.MainMenuScene, new SceneInfo(SceneLoadType.Fast, LoadSceneMode.Single) },
        { GameScene.DeathScene, new SceneInfo(SceneLoadType.Fast, LoadSceneMode.Single) },
        { GameScene.ValleyofSolura, new SceneInfo(SceneLoadType.Normal, LoadSceneMode.Single) },
        { GameScene.BedRockPlains, new SceneInfo(SceneLoadType.Normal, LoadSceneMode.Single) },
    };
    
    private static readonly Dictionary<string, GameScene> GAMESCENES = new()
    {
        {"MainMenuScene", GameScene.MainMenuScene},
        {"DeathScene", GameScene.DeathScene},
        {"ValleyofSolura", GameScene.ValleyofSolura},
        {"BedRockPlains", GameScene.BedRockPlains},
        {"LoadingScene", GameScene.LoadingScene}
    };
    
    private static string LoadScene = GameScene.LoadingScene.ToString();
    public const GameScene FirstScene = GameScene.BedRockPlains;
    
    public static SceneInfo TargetSceneInfoObj;
    
    public static string TargetScene;
    
    // THE function to call to load any scene. Returns true upon successful loading.
    public static bool Load(GameScene nextScene)
    {
        TargetScene = nextScene.ToString();
        TargetSceneInfoObj = INFO[nextScene];
        
        switch (TargetSceneInfoObj.LoadType)
        {
            case SceneLoadType.Fast:
                return SceneTransitioner.Instance.LoadScene(TargetScene, TargetSceneInfoObj.LoadMode);
            case SceneLoadType.Normal:
                return SceneTransitioner.Instance.LoadScene(LoadScene);
            case SceneLoadType.None:
                return SceneTransitioner.Instance.LoadScene(TargetScene);
            default:
                return SceneTransitioner.Instance.LoadScene(TargetScene);
        }
    }
    
    // overloading: load using string scene name
    public static bool Load(string nextScene)
    {
        return Load(GAMESCENES[nextScene]);
    }
    
    public static bool Load(Scene nextScene)
    {
        return Load(GAMESCENES[nextScene.name]);
    }
}
