using System;
using System.Collections.Generic;
using FMOD;
using UnityEditorInternal;
using UnityEngine.SceneManagement;

// some useful aliases ...
using ANIM = SceneTransitionAnim;
using LOAD = SceneLoadType;

public static class Loader {
    // maps scene -> (in transition, out transition, load type, load mode)
    private static readonly Dictionary<GameScene, SceneInfo> INFO = new()
    {
        { GameScene.MainMenuScene, new SceneInfo(ANIM.Fade, ANIM.Circle, LOAD.Fast, LoadSceneMode.Single) },
        { GameScene.DeathScene, new SceneInfo(ANIM.Fade, ANIM.Fade, LOAD.Fast, LoadSceneMode.Single) },
        { GameScene.ValleyofSolura, new SceneInfo(ANIM.Fade, ANIM.Fade, LOAD.Normal, LoadSceneMode.Single) },
        { GameScene.LoadingScene, new SceneInfo(ANIM.Fade, ANIM.Fade, LOAD.Fast, LoadSceneMode.Single) },
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
    
    public static SceneInfo loadingSceneInfo = INFO[GameScene.LoadingScene];
    public static SceneInfo TargetSceneInfoObj;
    public static string TargetScene;
    
    // THE function to call to load any scene
    public static void Load(GameScene nextScene)
    {
        TargetScene = nextScene.ToString();
        TargetSceneInfoObj = INFO[nextScene];
        
        var currentScene = GAMESCENES[SceneManager.GetActiveScene().name];
        var currSceneInfoObj = INFO[currentScene];
        
        switch (TargetSceneInfoObj.LoadType)
        {
            case LOAD.Fast:
                SceneTransitioner.Instance.LoadScene(TargetScene, currSceneInfoObj.OutAnim, TargetSceneInfoObj.InAnim, TargetSceneInfoObj.LoadMode);
                break;
            case LOAD.Normal:
                SceneTransitioner.Instance.LoadScene(LoadScene, currSceneInfoObj.OutAnim, loadingSceneInfo.InAnim, TargetSceneInfoObj.LoadMode);
                break;
            case LOAD.None:
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
