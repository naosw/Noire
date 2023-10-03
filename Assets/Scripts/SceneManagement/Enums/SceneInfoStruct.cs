using UnityEngine.SceneManagement;
public struct SceneInfo
{
    public SceneTransitionAnim InAnim {get; private set;}
    public SceneTransitionAnim OutAnim {get; private set;}
    public SceneLoadType LoadType {get; private set;}
    public LoadSceneMode LoadMode {get; private set;}

    public SceneInfo(SceneTransitionAnim inAnim, SceneTransitionAnim outAnim, SceneLoadType loadType, LoadSceneMode loadMode)
    {
        InAnim = inAnim;
        OutAnim = outAnim;
        LoadType = loadType;
        LoadMode = loadMode;
    }
}