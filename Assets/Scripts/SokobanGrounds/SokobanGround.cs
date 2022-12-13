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
    private ParticleSystem particle;
    public ParticleSystem Particle {
        get {
            if(particle == null) {
                particle = GetComponentInChildren<ParticleSystem>();
                if(particle == null) {
                    Debug.LogError("no particle here");
                }
            }
            return particle;
        }
    }
    public virtual bool IsWalkable() {
        return false;
    }
    public virtual bool IsPlaceable() {
        return true;
    }
    private void OnEnable() {
        LevelManager.RegisterGround(this);
        if(IsLevelGoal) {
            Particle.gameObject.SetActive(true);
        }else  {
            Particle.gameObject.SetActive(false);
        }
    }
    public void OnObjectEnter(SokobanObject obj) {
        if(IsLevelGoal) {
            IsReached = true;
            Particle.gameObject.SetActive(false);
        }
        LevelManager.OnGoalReached(this);
    }
    public void OnObjectLeave(SokobanObject obj) {
        if(IsLevelGoal) {
            IsReached = false;
            Particle.gameObject.SetActive(true);
        }
    }
}
