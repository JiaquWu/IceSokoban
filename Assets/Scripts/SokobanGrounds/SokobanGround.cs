using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanGround : MonoBehaviour {

    [SerializeField]
    protected bool isLevelGoal;
    [HideInInspector]
    public bool IsLevelGoal =>isLevelGoal;
    [HideInInspector]
    public bool IsReached;
    public virtual bool IsWalkable() {
        return false;
    }
    public virtual bool IsPlaceable() {
        return true;
    }
    private void OnEnable() {
        LevelManager.RegisterGround(this);
    }
    public void OnObjectEnter(SokobanObject obj) {
        if(IsLevelGoal) {
            IsReached = true;
        }
        LevelManager.OnGoalReached(this);
    }
    public void OnObjectLeave(SokobanObject obj) {
        if(IsLevelGoal) {
            IsReached = false;
        }
    }
}
