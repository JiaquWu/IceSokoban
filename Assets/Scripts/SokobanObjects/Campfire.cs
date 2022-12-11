using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : SokobanObject {
    public override bool IsPushable() {
        return false;
    }
    public override bool IsPushed(Direction dir) {
        return false;
    }
}
