using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanObject : MonoBehaviour {
    public virtual bool CanBePushedByIceCube() {
        return false;
    }
    public virtual bool IsPushable() {//会不会出现推的动作
        return false;
    }
    public virtual bool IsPushed(Direction direction) {
        return false;
    }
    public virtual bool MoveCheck(Direction dir, out SokobanGround ground, out SokobanObject obj) {
        ground = LevelManager.GetGroundOn(transform.position + dir.DirectionToVector3());
        obj = LevelManager.GetObjectOn(transform.position + dir.DirectionToVector3());
        if(obj != null) {
            return obj.IsPushable();//能被推就能move
        }else if(ground != null) {
            return ground.IsPlaceable();
        }else {
            return false;
        }

    }
    public virtual void OnEnable() {
        LevelManager.RegisterLevelObject(this);
    }

    public virtual void OnDisable() {
        LevelManager.UnRegisterLevelObject(this);
    }

    public void DisableObject() {
        gameObject.SetActive(false);
    }
}
