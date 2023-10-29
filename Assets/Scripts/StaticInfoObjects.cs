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
    [SerializeField] public AnimationCurve CA_OPEN_REALM_CURVE; // chromatic aberration curve
    [SerializeField] public AnimationCurve LD_OPEN_REALM_CURVE; // lens distortion curve
    
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
    
    // maps scene -> (load type, load mode, initial position)
    public readonly Dictionary<GameScene, SceneInfo> LOADING_INFO = new()
    {
        { GameScene.MainMenuScene, new SceneInfo(SceneLoadType.Fast, LoadSceneMode.Single, Vector3.zero) },
        { GameScene.DeathScene, new SceneInfo(SceneLoadType.Fast, LoadSceneMode.Single, Vector3.zero) },
        { GameScene.ValleyofSolura, new SceneInfo(SceneLoadType.Normal, LoadSceneMode.Single, new Vector3(47.2f, 5.4f, 40.2f)) },
        { GameScene.BedrockPlains, new SceneInfo(SceneLoadType.Normal, LoadSceneMode.Single, Vector3.zero) },
        { GameScene.TheShorelines, new SceneInfo(SceneLoadType.Normal, LoadSceneMode.Single, Vector3.zero) },
    };
    
    public readonly Dictionary<string, GameScene> GAMESCENES = new()
    {
        { "MainMenuScene", GameScene.MainMenuScene },
        { "DeathScene", GameScene.DeathScene },
        { "LoadingScene", GameScene.LoadingScene },
        { "BedrockPlains", GameScene.BedrockPlains },
        { "ValleyofSolura", GameScene.ValleyofSolura },
        { "TheShorelines", GameScene.TheShorelines },
    };

    public readonly Dictionary<DreamState, Color> VORONOI_INDICATOR = new()
    {
        { DreamState.Neutral, Color.black },
        { DreamState.Lucid, Color.cyan },
        { DreamState.Deep, Color.magenta },
    };
}