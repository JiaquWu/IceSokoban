using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
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

    public static Vector3 DirectionToVector(this Direction dir) {
        //not sure about the distance
        switch (dir) {
            case Direction.UP:
            return Vector3.up;
            case Direction.DOWN:
            return Vector3.down;
            case Direction.LEFT:
            return Vector3.left;
            case Direction.RIGHT:
            return Vector3.right;
        }
        return Vector3.zero;
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
