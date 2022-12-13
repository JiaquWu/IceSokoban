using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : SokobanObject {
    [SerializeField]
    private bool isFiring;
    public bool IsFiring => isFiring;
    public override bool CanBePushedByIceCube() {
        return isFiring;
    }
    public override bool IsPushable() {
        return false;
    }
    public override bool IsPushed(Direction dir) {
        return false;
    }

    public void BurnOut() {
        isFiring = false;
        //关闭火焰特效 如果有的话
    }
}
