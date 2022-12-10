using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonManager<LevelManager> {
    //List<SokobanGround> currentLevelGrounds = new List<SokobanGround>();

    List<SokobanObject> currentLevelObjects = new List<SokobanObject>();

    Dictionary<string,SokobanGround> currentGroundsDict = new Dictionary<string, SokobanGround>();//ground是静态的,所以应该可以这么干
    public static void RegisterGround(SokobanGround ground) {
        if(!Instance.currentGroundsDict.ContainsValue(ground)) {
            Debug.Log(ground.transform.position.Vector3ToString());
            Instance.currentGroundsDict.Add(ground.transform.position.Vector3ToString(),ground);
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
        Debug.Log(pos.Vector3ToString());
        Debug.LogWarning("no ground was found");
        return null;
    }

    public static SokobanObject GetObjectOn(Vector3 pos) {
        for (int i = 0; i < Instance.currentLevelObjects.Count; i++) {//因为关卡中不会有多少物体,所以这么干应该没问题
            if(Vector3.Distance(Instance.currentLevelObjects[i].transform.position,pos) < 0.01f) {
                return Instance.currentLevelObjects[i];
            }
        }
        return null;
    }
}
