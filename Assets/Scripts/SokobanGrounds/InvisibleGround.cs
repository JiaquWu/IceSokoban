using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleGround : SokobanGround {
    public override bool IsPlaceable() {
        return true;
    }
    public override bool IsWalkable() {
        return false;
    }
}
