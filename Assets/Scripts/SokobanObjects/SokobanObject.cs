using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanObject : MonoBehaviour {
    public virtual bool IsPushable() {//会不会出现推的动作
        return false;
    }
    public virtual void IsPushed() {

    }
    private void OnEnable() {
        LevelManager.RegisterLevelObject(this);
    }
}
