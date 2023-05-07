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
                    Debug.LogWarning("no particle here or it's disabled");
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
        if(Particle == null) return;
        if(IsLevelGoal) {
            Particle.gameObject.SetActive(true);
        }else  {
            Particle.gameObject.SetActive(false);
        }
    }
    public void OnObjectEnter(SokobanObject obj) {
        if(IsLevelGoal && obj.gameObject.activeSelf) {
            IsReached = true;
            Particle.gameObject.SetActive(false);
            LevelManager.OnGoalReached(this);
        }
        
    }
    public void OnObjectLeave(SokobanObject obj) {
        if(IsLevelGoal) {
            IsReached = false;
            Particle.gameObject.SetActive(true);
        }
    }

    public void DisableGround()
    {
        //make this ground to invisible ground
        
        if(gameObject.TryGetComponent<IceGround>(out IceGround iceGround)) {
            iceGround.enabled = false;
            LevelManager.UnRegisterGround(iceGround);
        }
        if(gameObject.TryGetComponent<SandGround>(out SandGround sandGround)) {
            sandGround.enabled = false;
            LevelManager.UnRegisterGround(sandGround);
        }
        //如果这里有东西,要禁用
        if(LevelManager.GetObjectOn(transform.position) != null) {
            LevelManager.GetObjectOn(transform.position).gameObject.SetActive(false);
        }
        
        if(gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer renderer)) {
            renderer.material = GameManager.Instance.InvisibleGroundMaterial;
        }
        gameObject.AddComponent<InvisibleGround>();
        gameObject.SetActive(false);
    }
}
