using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanObject : MonoBehaviour {
    public virtual bool IsPushable() {//会不会出现推的动作
        return false;
    }
    public virtual bool IsPushed(Direction direction) {
        return false;
    }
    public virtual bool MoveCheck(Direction dir, out SokobanGround ground) {
        ground = LevelManager.GetGroundOn(transform.position + dir.DirectionToVector3());
        return ground != null;
    }
    private void OnEnable() {
        LevelManager.RegisterLevelObject(this);
    }
}
