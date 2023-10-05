using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class Loader {
    // maps scene -> (in transition, out transition, load type, load mode)
    private static readonly Dictionary<GameScene, SceneInfo> INFO = new()
    {
        { GameScene.MainMenuScene, new SceneInfo(SceneLoadType.Fast, LoadSceneMode.Single) },
        { GameScene.DeathScene, new SceneInfo(SceneLoadType.Fast, LoadSceneMode.Single) },
        { GameScene.ValleyofSolura, new SceneInfo(SceneLoadType.Normal, LoadSceneMode.Single) },
    };
    
    private static readonly Dictionary<string, GameScene> GAMESCENES = new()
    {
        {"MainMenuScene", GameScene.MainMenuScene},
        {"DeathScene", GameScene.DeathScene},
        {"ValleyofSolura", GameScene.ValleyofSolura},
        {"LoadingScene", GameScene.LoadingScene}
    };
    
    private static string LoadScene = GameScene.LoadingScene.ToString();
    public const GameScene FirstScene = GameScene.ValleyofSolura;
    
    public static SceneInfo TargetSceneInfoObj;
    
    public static string TargetScene;
    
    // THE function to call to load any scene
    public static void Load(GameScene nextScene)
    {
        TargetScene = nextScene.ToString();
        TargetSceneInfoObj = INFO[nextScene];
        
        switch (TargetSceneInfoObj.LoadType)
        {
            case SceneLoadType.Fast:
                SceneTransitioner.Instance.LoadScene(TargetScene, TargetSceneInfoObj.LoadMode);
                break;
            case SceneLoadType.Normal:
                SceneTransitioner.Instance.LoadScene(LoadScene);
                break;
            case SceneLoadType.None:
                SceneTransitioner.Instance.LoadScene(TargetScene);
                break;
            default:
                SceneTransitioner.Instance.LoadScene(TargetScene);
                break;
        }
    }
    
    // overloading: load using string scene name
    public static void Load(string nextScene)
    {
        Load(GAMESCENES[nextScene]);
    }
    
    public static void Load(Scene nextScene)
    {
        Load(GAMESCENES[nextScene.name]);
    }
}
