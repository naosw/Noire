using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class Loader {
    private static string LoadScene = GameScene.LoadingScene.ToString();
    public const GameScene FirstScene = GameScene.BedrockPlains;
    
    public static SceneInfo TargetSceneInfoObj;
    
    public static string TargetScene;
    
    // THE function to call to load any scene. Returns true upon successful loading.
    public static bool Load(GameScene nextScene)
    {
        TargetScene = nextScene.ToString();
        TargetSceneInfoObj = StaticInfoObjects.Instance.LOADING_INFO[nextScene];
        
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
        return Load(StaticInfoObjects.Instance.GAMESCENES[nextScene]);
    }
    
    public static bool Load(Scene nextScene)
    {
        return Load(StaticInfoObjects.Instance.GAMESCENES[nextScene.name]);
    }

    public static void Respawn()
    {
        Load(DataPersistenceManager.Instance.CurrentScene);
    }
}
