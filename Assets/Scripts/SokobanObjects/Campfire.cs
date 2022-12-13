using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : SokobanObject {
    [SerializeField]
    AnimationCurve fireDisableAnimCurve;
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
        StartCoroutine(DisableFireAnim());
    }
    IEnumerator DisableFireAnim() {
        ParticleSystem particle = GetComponentInChildren<ParticleSystem>();
        ParticleSystem.EmissionModule emission = particle.emission;
        float startTime = Time.time;
        Debug.Log(emission);
        while(Time.time - startTime < fireDisableAnimCurve.keys[fireDisableAnimCurve.keys.Length - 1].time) {
            if(particle != null) {
                emission.rateOverTime = fireDisableAnimCurve.Evaluate(Time.time - startTime) * 90;
            }
            yield return null;
        }
        particle.gameObject.SetActive(false);
        
    }
}
