using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
    public const int UNIT_DISTANCE = 1;
    public static Direction ReverseDirection(this Direction dir) {
        switch (dir)
        {
            case Direction.UP:
            return Direction.DOWN;
            case Direction.DOWN:
            return Direction.UP;
            case Direction.LEFT:
            return Direction.RIGHT;
            case Direction.RIGHT:
            return Direction.LEFT;
        }
        return Direction.NONE;
    }
    public static Direction Vector3ToDirection(this Vector3 vector) {
        vector = vector.normalized;
        if(vector.x == 0) {
            if(vector.z == 1) {
                return Direction.UP;
            }else if(vector.z == -1) {
                return Direction.DOWN;
            }
        }else if(vector.z == 0) {
            if(vector.x == -1) {
                return Direction.LEFT;
            }else if(vector.x == 1) {
                return Direction.RIGHT;
            }
        }
        return Direction.NONE;
    }
    public static Vector3 DirectionToVector3(this Direction dir) {
        //not sure about the distance,distance here is actually the unit distance
        switch (dir) {
            case Direction.UP:
            return new Vector3(0,0,UNIT_DISTANCE);
            case Direction.DOWN:
            return new Vector3(0,0,-UNIT_DISTANCE);
            case Direction.LEFT:
            return new Vector3(-UNIT_DISTANCE,0,0);
            case Direction.RIGHT:
            return new Vector3(UNIT_DISTANCE,0,0);
        }
        return Vector3.zero;
    }
    public static Vector2 DirectionToVector2(this Direction dir) {
        //not sure about the distance
        switch (dir) {
            case Direction.UP:
            return new Vector3(0,UNIT_DISTANCE);
            case Direction.DOWN:
            return new Vector3(0,-UNIT_DISTANCE);
            case Direction.LEFT:
            return new Vector3(-UNIT_DISTANCE,0);
            case Direction.RIGHT:
            return new Vector3(UNIT_DISTANCE,0);
        }
        return Vector3.zero;
    }
    public static List<Vector3> DirectionToGet3DirectionVectors(this Direction dir) {//比如往前,就返回前左右
        List<Vector3> result = new List<Vector3>();
        switch (dir) {
            case Direction.UP:
            result.Add(new Vector3(0,0,UNIT_DISTANCE));
            result.Add(new Vector3(-UNIT_DISTANCE,0,0));
            result.Add(new Vector3(UNIT_DISTANCE,0,0));
            break;
            case Direction.DOWN:
            result.Add(new Vector3(0,0,-UNIT_DISTANCE));
            result.Add(new Vector3(-UNIT_DISTANCE,0,0));
            result.Add(new Vector3(UNIT_DISTANCE,0,0));
            break;
            case Direction.LEFT:
            result.Add(new Vector3(0,0,UNIT_DISTANCE));
            result.Add(new Vector3(0,0,-UNIT_DISTANCE));
            result.Add(new Vector3(-UNIT_DISTANCE,0,0));
            break;
            case Direction.RIGHT:
            result.Add(new Vector3(0,0,UNIT_DISTANCE));
            result.Add(new Vector3(0,0,-UNIT_DISTANCE));
            result.Add(new Vector3(UNIT_DISTANCE,0,0));
            break;
        }
        return result;
    }
    public static Vector3 DirectionToWorldRotation(this Direction dir) {
        switch (dir)
        {
            case Direction.UP:
            return new Vector3(0,0,0);
            case Direction.DOWN:
            return new Vector3(0,180,0);
            case Direction.LEFT:
            return new Vector3(0,270,0);
            case Direction.RIGHT:
            return new Vector3(0,90,0);
        }
        return Vector3.zero;
    }
    public static bool IsPerpendicular(this Direction dir,Direction targetDir) {
        return Vector2.Dot(dir.DirectionToVector2(),targetDir.DirectionToVector2()) == 0;
    }
    // public static InteractionType ReverseInteractionType(this InteractionType type) {
    //     switch(type) {
    //         case InteractionType.PICK_UP_ANIMALS:
    //         return InteractionType.PUT_DOWN_ANIMALS;
    //         case InteractionType.PUT_DOWN_ANIMALS:
    //         return InteractionType.PICK_UP_ANIMALS;
    //     }
    //     return InteractionType.NONE;
    // }

    public static Vector3Int Vector3ToVector3Int(this Vector3 vec) {
        return new Vector3Int(Mathf.RoundToInt(vec.x),Mathf.RoundToInt(vec.y),Mathf.RoundToInt(vec.z));
    }

    public static string Vector3ToString(this Vector3 vector)
    {
        return string.Format("({0}, {1})", vector.x, vector.z);//y轴无所谓
    }
    public static string Vector3IntToString(this Vector3Int vector)
    {
        return string.Format("({0}, {1}, {2})", vector.x, vector.y, vector.z);
    }
    public static Vector2 Vector3ToVector2(this Vector3 vector) {
        return new Vector2(vector.x,vector.z);
    }
    public static bool IsAttachedWithTargetIceCube(this IceCube cube, IceCube target, out Direction dir) {//dir是对于我来说,目标的方向是什么
        if(Vector3.Distance(cube.transform.position,target.transform.position) == UNIT_DISTANCE) {
            dir = (target.transform.position - cube.transform.position).Vector3ToDirection();
            Debug.Log("方向是什么呢?" + dir);
            return dir != Direction.NONE;
        }
        dir = Direction.NONE;
        return false;
    }
    // public static Dictionary<TKey,TValue> MergeTwoDictionary<TKey,TValue>(this Dictionary<TKey,TValue> myDict,Dictionary<TKey,TValue> secondDict) {
    //     Dictionary<TKey,TValue> res = new Dictionary<TKey, TValue>();
    //     foreach (var item in myDict) {
    //         if(!res.ContainsKey(item.Key)) {
    //             res.Add(item.Key,item.Value);
    //         }
    //     }
    //     foreach (var item in secondDict) {
    //         if(!res.ContainsKey(item.Key)) {
    //             res.Add(item.Key,item.Value);
    //         }
    //     }
    //     return res;
    // }
    // public static bool IsInRange(this int myInt,int min,int max) {
    //     return myInt >= min && myInt < max;//will be at a new level if it's equal to max
    // }
    // public static int GetIndexInArray(this int count,int[] arr) {
    //     Debug.Log("arr.Length" + arr.Length );
    //     if(arr.Length == 0 || (arr.Length != 0 && count < arr[0])) return -1;
        
    //     for (int i = 0; i < arr.Length; i++) {
    //         if(i<arr.Length -1 ) {
    //             if(count.IsInRange(arr[i],arr[i+1])) return i;
    //         }else {
    //             if(count >= arr[i]) return i;
    //         }
    //     }
    //     return -1;
    // }
}
