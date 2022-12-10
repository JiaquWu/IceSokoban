using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCube : SokobanObject {
    public override bool IsPushable()
    {
        return true;
    }
    public override void IsPushed()
    {
        //要检测能不能推,然后开始推之后继续检测啥的
    }
}
