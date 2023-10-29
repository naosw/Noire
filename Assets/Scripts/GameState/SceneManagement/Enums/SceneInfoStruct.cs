using UnityEngine;
using UnityEngine.SceneManagement;
public struct SceneInfo
{
    public SceneLoadType LoadType {get; private set;}
    public LoadSceneMode LoadMode {get; private set;}
    
    public Vector3 InitialPosition {get; private set;}

    public SceneInfo(SceneLoadType loadType, LoadSceneMode loadMode, Vector3 initialPosition)
    {
        LoadType = loadType;
        LoadMode = loadMode;
        InitialPosition = initialPosition;
    }
}