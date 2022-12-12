using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IceCube : SokobanObject {
    public IceCube UpAttachedIceCube;
    public IceCube DownAttachedIceCube;
    public IceCube LeftAttachedIceCube;
    public IceCube RightAttachedIceCube;
    public Coroutine moveCoroutine;
    public override bool IsPushable()
    {
        return true;
    }
    public override bool IsPushed(Direction dir)
    {
        //要检测能不能推,然后开始推之后继续检测啥的
        List<IceCube> cubes = GetAllAttachedIceCubes();
        for (int i = 0; i < cubes.Count; i++) {
            if(!cubes[i].MoveCheck(dir,out SokobanGround ground,out SokobanObject obj)) {
                return false;
            }
        }
        for (int i = 0; i < cubes.Count; i++) {
            if(cubes[i].MoveCheck(dir,out SokobanGround ground,out SokobanObject obj)) {
                
            }
            cubes[i].StartNewMovement(dir);
        }
        
        return true;
    }
    public void StartNewMovement(Direction dir) {
        if(moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(IceCubeMove(dir));
    }
    IEnumerator IceCubeMove(Direction dir) {
        bool canContinue = true;
        Vector3 target = transform.position + dir.DirectionToVector3();
        
        Debug.Log("该移动了" + target);
        while(Vector3.Distance(transform.position, target) > 0.001f) {
            transform.position = Vector3.MoveTowards(transform.position,target,Time.deltaTime * 2);
            yield return null;
        }
        transform.position = target;
        //检测一下周围的iceCube,让它们加入
        CheckNeighborAttachedIceCubes(dir);
        //

        // List<IceCube> cubes = GetAllAttachedIceCubes();
        // for (int i = 0; i < cubes.Count; i++) {
        //     if(cubes[i].MoveCheck(dir,out SokobanGround ground)) {
        //         if(ground is IceGround) {
        //         }else {
        //             canContinue = false;
        //         }
        //     }
        // }
        // if(canContinue) {
        //     StopCoroutine(IceCubeMove(dir));
        //     StartCoroutine(IceCubeMove(dir));
        // }
    }
    public List<IceCube> GetAllAttachedIceCubes() {
        List<IceCube> result = new List<IceCube>();
        RecursiveAddIceCubes(this,ref result);
        Debug.Log(result.Count + "有这么多个");
        return result;
    }
    public void RecursiveAddIceCubes(IceCube cube,ref List<IceCube> result) {
        result.Add(cube);
        List<IceCube> neighbors = cube.GetMyAttachedIceCubes();
        if(neighbors.Count > 0) {
            foreach (IceCube item in neighbors) {
                if(!result.Contains(item)) {
                    RecursiveAddIceCubes(item, ref result);
                }
            }
        }
    }
    public List<IceCube> GetMyAttachedIceCubes() {
        List<IceCube> cubes = new List<IceCube>();
        if(UpAttachedIceCube != null) {
            cubes.Add(UpAttachedIceCube);
        }
        if(DownAttachedIceCube != null) {
            cubes.Add(DownAttachedIceCube);
        }
        if(LeftAttachedIceCube != null) {
            cubes.Add(LeftAttachedIceCube);
        }
        if(RightAttachedIceCube != null) {
            cubes.Add(RightAttachedIceCube);
        }

        return cubes;
    }
    public void CheckNeighborAttachedIceCubes(Direction dir) {
        List<Vector3> targets = dir.DirectionToGet3DirectionVectors();
        
        foreach (var item in targets) {
            Debug.Log(item);
            SokobanObject obj = LevelManager.GetObjectOn(transform.position + item);
            if(obj is IceCube) {
                
                SetIceCubeWithDirection(item.Vector3ToDirection(),obj as IceCube);
                (obj as IceCube).SetIceCubeWithDirection(item.Vector3ToDirection().ReverseDirection(),this);
            }
        }
    }
    public void SetIceCubeWithDirection(Direction dir,IceCube cube) {
        switch (dir)
        {
            case Direction.UP:
            if(UpAttachedIceCube == null)
            UpAttachedIceCube = cube;
            break;
            case Direction.DOWN:
            if(DownAttachedIceCube == null)
            DownAttachedIceCube = cube;
            break;
            case Direction.LEFT:
            if(LeftAttachedIceCube == null)
            LeftAttachedIceCube = cube;
            break;
            case Direction.RIGHT:
            if(RightAttachedIceCube == null)
            RightAttachedIceCube = cube;
            break;
        }
        //Debug.Log("那就连起来拉 " + dir + cube);

    }
    public void DisconnectCubeWithDirection(Direction dir,IceCube cube) {
        switch (dir)
        {
            case Direction.UP:
            UpAttachedIceCube = null;
            break;
            case Direction.DOWN:
            DownAttachedIceCube = null;
            break;
            case Direction.LEFT:
            LeftAttachedIceCube = null;
            break;
            case Direction.RIGHT:
            RightAttachedIceCube = null;
            break;
        }
    }
}
