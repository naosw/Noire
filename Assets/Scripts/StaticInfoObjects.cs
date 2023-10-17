using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// class for storing all CONST fields such as static dictionaries,
/// animation curves, etc
/// </summary>


public class StaticInfoObjects : MonoBehaviour
{
    public static StaticInfoObjects Instance { get; private set; }

    [SerializeField] public AnimationCurve FADE_ANIM_CURVE;
    [SerializeField] public AnimationCurve OPEN_REALM_CURVE;
    [SerializeField] public AnimationCurve CLOSE_REALM_CURVE;
    
    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // maps scene -> (in transition, out transition, load type, load mode)
    public readonly Dictionary<GameScene, SceneInfo> LOADING_INFO = new()
    {
        { GameScene.MainMenuScene, new SceneInfo(SceneLoadType.Fast, LoadSceneMode.Single) },
        { GameScene.DeathScene, new SceneInfo(SceneLoadType.Fast, LoadSceneMode.Single) },
        { GameScene.ValleyofSolura, new SceneInfo(SceneLoadType.Normal, LoadSceneMode.Single) },
        { GameScene.BedrockPlains, new SceneInfo(SceneLoadType.Normal, LoadSceneMode.Single) },
    };
    
    public readonly Dictionary<string, GameScene> GAMESCENES = new()
    {
        { "MainMenuScene", GameScene.MainMenuScene },
        { "DeathScene", GameScene.DeathScene },
        { "ValleyofSolura", GameScene.ValleyofSolura },
        { "BedrockPlains", GameScene.BedrockPlains },
        { "LoadingScene", GameScene.LoadingScene }
    };

    public readonly Dictionary<DreamState, Color> VORONOI_INDICATOR = new()
    {
        { DreamState.Neutral, Color.black },
        { DreamState.Lucid, Color.cyan },
        { DreamState.Deep, Color.magenta },
    };
}