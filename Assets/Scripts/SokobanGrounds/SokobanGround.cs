using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanGround : MonoBehaviour {

    [SerializeField]
    protected bool isLevelGoal;
    public virtual bool IsWalkable() {
        return false;
    }
    public virtual bool IsPlaceable() {
        return true;
    }
    private void OnEnable() {
        LevelManager.RegisterGround(this);
    }
}
