using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonManager<LevelManager> {
    //List<SokobanGround> currentLevelGrounds = new List<SokobanGround>();

    List<SokobanObject> currentLevelObjects = new List<SokobanObject>();

    Dictionary<string,SokobanGround> currentGroundsDict = new Dictionary<string, SokobanGround>();//ground是静态的,所以应该可以这么干
    public static void RegisterGround(SokobanGround ground) {
        if(!Instance.currentGroundsDict.ContainsValue(ground)) {
            Instance.currentGroundsDict.Add(ground.transform.position.Vector3ToVector3Int().Vector3IntToString(),ground);
        }
    }

    public static void RegisterLevelObject(SokobanObject sokobanObject) {
        if(!Instance.currentLevelObjects.Contains(sokobanObject)) {
            Instance.currentLevelObjects.Add(sokobanObject);
        }
    }

    public static SokobanGround GetGroundOn(Vector3 pos) {
        if(Instance.currentGroundsDict.ContainsKey(pos.Vector3ToString())) {
            return Instance.currentGroundsDict[pos.Vector3ToString()];
        }
        Debug.LogWarning("no ground was found");
        return null;
    }
}
