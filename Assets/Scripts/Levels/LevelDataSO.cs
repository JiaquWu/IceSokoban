using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Level/Data",fileName ="LevelData", order = 0)]
public class LevelDataSO : ScriptableObject {
    [SerializeField]
    private string fileName;
    public string FileName => fileName;

    [SerializeField]
    private string levelName;
    public string LevelName => LevelName;

    [SerializeField]
    private Sprite levelThumbnail;
    public Sprite LevelThumbnail => levelThumbnail;
    [SerializeField]
    private Sprite levelIndicateImage;
    public Sprite LevelIndicateImage => levelIndicateImage;

    [SerializeField]
    private List<AdjustCubeContainer> adjustCubeContainers;
    public List<AdjustCubeContainer> AdjustCubeContainers => adjustCubeContainers;

    public bool isAdjustCubeAvailable(float ElapsedTime)
    {
        for (int i = 0; i < adjustCubeContainers.Count; i++) {
            if(adjustCubeContainers[i].deadline < ElapsedTime) {
                return true;
            }
        }
        return false;
    }

    public void TriggerAdjustCubes(float ElapsedTime) {
        for (int i = 0; i < adjustCubeContainers.Count; i++) {
            if(adjustCubeContainers[i].deadline < ElapsedTime) {
                if(adjustCubeContainers[i].isOnlyObject) {
                    foreach (var pos in adjustCubeContainers[i].adjustCubes) {
                        LevelManager.GetObjectOn(pos).DisableObject();
                    }
                }else {
                    foreach (var pos in adjustCubeContainers[i].adjustCubes) {
                        LevelManager.GetGroundOn(pos).DisableGround();
                    }
                }
                
            }
        }
    }
}

[System.Serializable]
public class AdjustCubeContainer {
    public float deadline;
    public List<Vector3> adjustCubes;
    public bool isOnlyObject;
}