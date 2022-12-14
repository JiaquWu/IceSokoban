using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IceCube : SokobanObject {
    bool isFired;
    private bool shouldStop = false;
    private IceCube UpAttachedIceCube;
    private IceCube DownAttachedIceCube;
    private IceCube LeftAttachedIceCube;
    private IceCube RightAttachedIceCube;
    public Coroutine moveCoroutine;
    public Action<bool> onFinishedMove;
    public override bool IsPushable()
    {
        return true;
    }
    public override bool IsPushed(Direction dir)
    {
        //要检测能不能推,然后开始推之后继续检测啥的
        List<IceCube> cubes = GetAllAttachedIceCubes();
        int walkableGroundCount = 0;
        bool isStillHereAfterMovement = false;
        bool shouldCheckAgain = true;
        shouldStop = false;
        
        
        if(cubes.Any(x=>!x.MoveCheck(dir,out SokobanGround ground,out SokobanObject obj))) {//就是发现往前走不了的时候,检测脚底下
            shouldCheckAgain = false;
            for (int i = 0; i < cubes.Count; i++) {
                SokobanGround ground = LevelManager.GetGroundOn(cubes[i].transform.position);
                if(ground != null) {
                    ground.OnObjectEnter(cubes[i]);
                }
            }
            AudioManager.Instance.StopIcePushAudio();
            return false;
        }
        
        //说明每一个冰块前面都能放
        onFinishedMove = (b)=> {
            if(shouldCheckAgain && !shouldStop) {
                IsPushed(dir);
                onFinishedMove = null;
            }else {
                //判断是否到终点了
                for (int i = 0; i < cubes.Count; i++) {
                    SokobanGround ground = LevelManager.GetGroundOn(cubes[i].transform.position);
                    if(ground != null) {
                        ground.OnObjectEnter(cubes[i]);
                    }
                }
                if(b) {
                    gameObject.SetActive(false);
                }
                AudioManager.Instance.StopIcePushAudio();
            }
        };
        for (int i = 0; i < cubes.Count; i++) {
            if(cubes[i].MoveCheck(dir,out SokobanGround ground,out SokobanObject obj)) {
                //如果ground是在外面的,要判断ground的数量
                if(ground.IsPlaceable() && ground.IsWalkable()) {
                    walkableGroundCount++;
                    if(walkableGroundCount >= (float)cubes.Count / 2) isStillHereAfterMovement = true;
                }
                //如果obj是燃烧着的火焰,那么这块冰要单独拿出来让它融化掉
                if(obj is Campfire && (obj as Campfire).IsFiring) {
                    Debug.Log(obj);
                    cubes[i].isFired = true;
                    (obj as Campfire).BurnOut();
                }
                //如果ground全是冰面,那移动完要再检测一次是不是能移动
                SokobanGround currentGround = LevelManager.GetGroundOn(cubes[i].transform.position);
                if((!(ground is IceGround || ground is InvisibleGround)) || !(currentGround is IceGround || currentGround is InvisibleGround)) {
                    //如果当前方块不是冰或者外面,或者下一格不是冰块或者外面,都不能继续
                    shouldCheckAgain = false;
                }
            }
            //说明能推,所以开始行动
            
        }
        // for (int i = 0; i < cubes.Count; i++) {
        //     if(cubes[i].gameObject.activeSelf) {
        //         SokobanGround ground = LevelManager.GetGroundOn(cubes[i].transform.position);
        //         if(ground != null) {
        //             Debug.Log("能到这里吗" + ground.gameObject.transform.position);
        //             ground.OnObjectEnter(cubes[i]);//要先判定
        //         }
        //     }
        // }
        
        for (int i = 0; i < cubes.Count; i++) {
            if(!cubes[i].gameObject.activeSelf) return false; 
            IceCube cube = cubes[i];
            cube.StartNewMovement(dir,isStillHereAfterMovement,cube.isFired,cube.onFinishedMove,()=>{
                this.shouldStop = true;
            });
        }
        
        return true;
    }
    public override bool CanBePushedByIceCube() {
        return true;
    }
    public override bool MoveCheck(Direction dir, out SokobanGround ground, out SokobanObject obj) {
        ground = LevelManager.GetGroundOn(transform.position + dir.DirectionToVector3());
        obj = LevelManager.GetObjectOn(transform.position + dir.DirectionToVector3());
        if(LevelManager.Player.transform.position.Vector3ToVector2() == (transform.position + dir.DirectionToVector3()).Vector3ToVector2()) {
            return false;
        }
        if(obj != null) {
            return obj.CanBePushedByIceCube();//能被推就能move
        }else if(ground != null) {
            return ground.IsPlaceable();
        }else {
            return false;
        }
    }
    public void StartNewMovement(Direction dir,bool isStillHereAfterMovement,bool isFired,Action<bool> onFinishMove,Action onStop) {
        if(moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(IceCubeMove(dir,isStillHereAfterMovement,isFired,onFinishMove,onStop));;
    }
    IEnumerator IceCubeMove(Direction dir,bool isStillHereAfterMovement,bool isFired,Action<bool> onFinishMove,Action onStop) {
        Vector3 target = transform.position + dir.DirectionToVector3();
        SokobanGround ground = LevelManager.GetGroundOn(transform.position);
        if(ground != null) ground.OnObjectLeave(this);
        while(Vector3.Distance(transform.position, target) > 0.001f) {
            transform.position = Vector3.MoveTowards(transform.position,target,Time.deltaTime * 5);
            yield return null;
        }
        transform.position = target;
        bool shouldDisableMyself = false;
        //先判断会不会掉下去 //再判断会不会融化
        if(!isStillHereAfterMovement || isFired) {
            //先这样
            Debug.Log("救命");
            onStop?.Invoke();
            DisconnectMySurroundingCubes();
            LevelManager.UnRegisterLevelObject(this);
            
            if(onFinishMove != null) {//说明哥们是被推的,待会销毁
                shouldDisableMyself = true;
            }else {//说明是别的方块,需要马上销毁!
                gameObject.SetActive(false);
            }
        }       
        //检测一下周围的iceCube,让它们加入,前提是我还没消失呢
        yield return null;
        if(gameObject.activeSelf && !(onFinishMove != null && shouldDisableMyself)) {//现在没有被销毁,并且待会也不会被销毁
        Debug.Log("onFinishMove != null" + (onFinishMove == null) + "shouldDisableMyself" + shouldDisableMyself);
            if(CheckNeighborAttachedIceCubes(dir)) {
                
                onStop?.Invoke();
            }else {
                
            }
        }
        yield return null;
        // ground = LevelManager.GetGroundOn(transform.position);
        // if(ground != null) ground.OnObjectEnter(this);
        
        //要知道所有的方块都到了这里,我才能说invoke
        onFinishMove?.Invoke(shouldDisableMyself);
    }
    public List<IceCube> GetAllAttachedIceCubes() {
        List<IceCube> result = new List<IceCube>();
        RecursiveAddIceCubes(this,ref result);
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
    public bool CheckNeighborAttachedIceCubes(Direction dir) {
        
        List<Vector3> targets = dir.DirectionToGet3DirectionVectors();
        bool result = false;
        for (int i = 0; i < targets.Count; i++) {
            SokobanObject obj = LevelManager.GetObjectOn(transform.position + targets[i]);
            if(obj is IceCube && obj.gameObject.activeSelf) {
                //
                if(SetIceCubeWithDirection(targets[i].Vector3ToDirection(),obj as IceCube) 
                && (obj as IceCube).SetIceCubeWithDirection(targets[i].Vector3ToDirection().ReverseDirection(),this)) {
                    result = true;
                }
            }
        }
        
        // foreach (var item in targets) {
        //     SokobanObject obj = LevelManager.GetObjectOn(transform.position + item);
        //     Debug.Log(obj);
        //     if(obj is IceCube) {
        //         if(SetIceCubeWithDirection(item.Vector3ToDirection(),obj as IceCube) 
        //         && (obj as IceCube).SetIceCubeWithDirection(item.Vector3ToDirection().ReverseDirection(),this)) {
        //             return true;
        //         }
        //     }
        // }
        return result;
    }
    public bool SetIceCubeWithDirection(Direction dir,IceCube cube) {
        switch (dir)
        {
            case Direction.UP:
            if(UpAttachedIceCube == null) {
                UpAttachedIceCube = cube;
                return true;//加入了新的才是set了
            }
            return false;
            
            case Direction.DOWN:
            if(DownAttachedIceCube == null) {
                DownAttachedIceCube = cube;
                return true;
            }
            return false;
            
            case Direction.LEFT:
            if(LeftAttachedIceCube == null) {
                LeftAttachedIceCube = cube;
                return true;
            }
            return false;
            
            case Direction.RIGHT:
            if(RightAttachedIceCube == null) {
                RightAttachedIceCube = cube;
                return true;
            }
            return false;
        }
        return false;
        //Debug.Log("那就连起来拉 " + dir + cube);

    }
    public void DisconnectMySurroundingCubes() {
        
        if(UpAttachedIceCube != null) {
            UpAttachedIceCube.DisconnectCubeWithDirection(Direction.DOWN);
            UpAttachedIceCube = null;
        }
        if(DownAttachedIceCube != null) {
            DownAttachedIceCube.DisconnectCubeWithDirection(Direction.UP);
            DownAttachedIceCube = null;
        }
        if(LeftAttachedIceCube != null){
            LeftAttachedIceCube.DisconnectCubeWithDirection(Direction.RIGHT);
            LeftAttachedIceCube = null;
        }
        if(RightAttachedIceCube != null) {
            RightAttachedIceCube.DisconnectCubeWithDirection(Direction.LEFT);
            RightAttachedIceCube = null;
        }
        Debug.Log("UpAttachedIceCube " + UpAttachedIceCube + "DownAttachedIceCube " + DownAttachedIceCube 
        +"LeftAttachedIceCube " + LeftAttachedIceCube + "RightAttachedIceCube " + RightAttachedIceCube);
    }
    public void DisconnectCubeWithDirection(Direction dir) {
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
