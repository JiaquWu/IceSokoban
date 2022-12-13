using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonManager<LevelManager> {
    //List<SokobanGround> currentLevelGrounds = new List<SokobanGround>();

    List<SokobanObject> currentLevelObjects = new List<SokobanObject>();
    List<IceCube> currentLevelIceCubes = new List<IceCube>();
    Dictionary<string,SokobanGround> currentGroundsDict = new Dictionary<string, SokobanGround>();//ground是静态的,所以应该可以这么干
    public static void RegisterGround(SokobanGround ground) {
        if(!Instance.currentGroundsDict.ContainsValue(ground)) {
            Instance.currentGroundsDict.Add(ground.transform.position.Vector3ToString(),ground);
        }
        if(ground is InvisibleGround) {
            ground.gameObject.SetActive(false);
        }
    }
    public static void RegisterLevelObject(SokobanObject sokobanObject) {
        if(!Instance.currentLevelObjects.Contains(sokobanObject)) {
            Instance.currentLevelObjects.Add(sokobanObject);
            //如果是冰块,
            if(sokobanObject is IceCube) {
                //那么就检测一遍其他的冰块
                Debug.Log("有几个" + Instance.currentLevelIceCubes.Count);
                foreach (IceCube cube in Instance.currentLevelIceCubes) {
                    //如果其他的冰块位置上和这个冰块是相连的,那么就让它们连起来
                    if((sokobanObject as IceCube).IsAttachedWithTargetIceCube(cube,out Direction direction)) {
                        (sokobanObject as IceCube).SetIceCubeWithDirection(direction,cube);
                        cube.SetIceCubeWithDirection(direction.ReverseDirection(),sokobanObject as IceCube);
                    }
                }

                if(!Instance.currentLevelIceCubes.Contains(sokobanObject as IceCube)) {
                    Instance.currentLevelIceCubes.Add(sokobanObject as IceCube);
                }
                
            }
        }
    }

    public static void UnRegisterLevelObject(SokobanObject sokobanObject) {
        if(Instance.currentLevelObjects.Contains(sokobanObject)) {
            Instance.currentLevelObjects.Remove(sokobanObject);

            if(sokobanObject is IceCube) {
                if(Instance.currentLevelIceCubes.Contains(sokobanObject as IceCube)) {
                    Instance.currentLevelIceCubes.Remove(sokobanObject as IceCube);
                }
            }
        }
    }
    public static SokobanGround GetGroundOn(Vector3 pos) {
        if(Instance.currentGroundsDict.ContainsKey(pos.Vector3ToVector3Int().Vector3IntToString())) {
            return Instance.currentGroundsDict[pos.Vector3ToVector3Int().Vector3IntToString()];
        }
        Debug.Log(pos.Vector3ToVector3Int().Vector3IntToString());
        Debug.LogWarning("no ground was found");
        return null;
    }

    public static SokobanObject GetObjectOn(Vector3 pos) {
        for (int i = 0; i < Instance.currentLevelObjects.Count; i++) {//因为关卡中不会有多少物体,所以这么干应该没问题
        //Debug.Log(Vector3.Distance(Instance.currentLevelObjects[i].transform.position,pos) +"" + Instance.currentLevelObjects[i]);
            if(Vector3.Distance(Instance.currentLevelObjects[i].transform.position.Vector3ToVector2(),pos.Vector3ToVector2()) < 0.01f) {
                return Instance.currentLevelObjects[i];
            }
        }
        return null;
    }
}
