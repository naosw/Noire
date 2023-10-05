using UnityEngine.SceneManagement;
public struct SceneInfo
{
    public SceneLoadType LoadType {get; private set;}
    public LoadSceneMode LoadMode {get; private set;}

    public SceneInfo(SceneLoadType loadType, LoadSceneMode loadMode)
    {
        LoadType = loadType;
        LoadMode = loadMode;
    }
}