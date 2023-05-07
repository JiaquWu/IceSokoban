using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum Direction {
    NONE,
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public enum InteractionType {
    NONE,
    PICK_UP_ANIMALS,
    PUT_DOWN_ANIMALS
}

public class GameManager : SingletonManager<GameManager> {
    [SerializeField]
    private LevelSequence levelSequence;
    public LevelSequence LevelSequence => levelSequence;

    [SerializeField]
    private Material invisbileGroundMaterial;
    public Material InvisibleGroundMaterial => invisbileGroundMaterial;
    private Dictionary<string,Timer> sceneTimers = new();
    public Dictionary<string,Timer> SceneTimers => sceneTimers;
    protected override void Init() {
        levelSequence = Resources.Load<LevelSequence>("LevelSequence_01");
        if(levelSequence == null) {
            Debug.LogError("no such levelSequence");
        }
        invisbileGroundMaterial = Resources.Load<Material>("InvisibleGround");
        if(invisbileGroundMaterial == null) {
            Debug.LogError("no such invisbileGroundMaterial");
        }
        DontDestroyOnLoad(gameObject);
        //这里播放一个入场渐变
        
    }
    private void OnEnable() {
        SceneManager.sceneLoaded += (scene,loadSceneMode)=> {
            StartCoroutine(FadeInAnOut(true,0.5f,null));
            if(GetCurrentLevelIndex() != -1) {
                AudioManager.Instance.PlaySeaWaveAudio();
                //start timer
                if(!sceneTimers.ContainsKey(scene.name)) {
                    sceneTimers.Add(scene.name,new Timer());
                }else {
                    sceneTimers[scene.name].Continue();
                }
                if(levelSequence.levels[GetCurrentLevelIndex()].isAdjustCubeAvailable(sceneTimers[scene.name].Elapsed)) {
                    levelSequence.levels[GetCurrentLevelIndex()].TriggerAdjustCubes(sceneTimers[scene.name].Elapsed);
                }
            }
        };
        SceneManager.sceneUnloaded += (scene) => {
            //pause the timer
            if(sceneTimers.ContainsKey(scene.name)) {
                sceneTimers[scene.name].Pause();
            }else {
                //menu stuff
            }
        };
        StartCoroutine(FadeInAnOut(true,0.5f,null));
        if(GetCurrentLevelIndex() != -1) {
            AudioManager.Instance.PlaySeaWaveAudio();
        }
        GameEventsManager.StartListening(GameEventTypeInt.FINISH_LEVEL,OnLevelFinish);
    }

    private void OnDisable() {
        GameEventsManager.StopListening(GameEventTypeInt.FINISH_LEVEL,OnLevelFinish);
    }

    // private void Update() {
    //     if(sceneTimers.ContainsKey(SceneManager.GetActiveScene().name)) {
    //         Debug.Log("level: " +SceneManager.GetActiveScene().name + "Time: " + sceneTimers[SceneManager.GetActiveScene().name].Elapsed); 
    //     }
    // }
    public int GetCurrentLevelIndex() {
        string currentLevel = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levelSequence.levels.Count; i++) {
            if(levelSequence.levels[i].FileName == currentLevel) {
                return i;
            }
        }
        return -1;
    }
    public void LoadLevel(int levelIndex) {
        if(levelSequence.levels.Count <= levelIndex) {
            Debug.LogError("no such level" + levelIndex);
            return;
        }
        LevelDataSO data = levelSequence.levels[levelIndex];
        SceneManager.LoadScene(data.FileName);
    }
    public void LoadLevel(string fileName) {
        for (int i = 0; i < levelSequence.levels.Count; i++) {
            if(levelSequence.levels[i].FileName == fileName)  {
                StartCoroutine(FadeInAnOut(false,0.5f,()=>SceneManager.LoadScene(fileName)));
            }
        }
    }
    public void LoadNextOrPrevLevel(bool isPrevLevel) {
        
        int currentLevelIndex = GetCurrentLevelIndex();
        if(currentLevelIndex != -1) {
            int targetLevelIndex = isPrevLevel? currentLevelIndex-1 : currentLevelIndex+1;
                StartCoroutine(FadeInAnOut(false,0.5f,()=>SceneManager.LoadScene(levelSequence.levels[targetLevelIndex].FileName)));
        }
    }
    public void LoadCurrentLevel() {
        StartCoroutine(FadeInAnOut(false,0.5f,()=>SceneManager.LoadScene(SceneManager.GetActiveScene().name)));
    }
    public void LoadSceneWithFade(string sceneName) {
        StartCoroutine(FadeInAnOut(false,0.5f,()=>SceneManager.LoadScene(sceneName)));
    }
    IEnumerator FadeInAnOut(bool isFadeIn,float duration,Action loadSceneAction) {
        float startTime = Time.time;
        while(Time.time - startTime < duration) {
            //
            float value = isFadeIn?  duration - (Time.time - startTime):Time.time - startTime ;
            
            UIManager.Instance.SetFadeInAndOutPanelAlpha(value/duration);
            yield return null;
        }
        loadSceneAction?.Invoke();
    }
    public bool isFirstLevel() {
        string currentLevel = SceneManager.GetActiveScene().name;
        if(levelSequence.levels[0].FileName == currentLevel) return true;
        return false;
    }
    public bool isLastLevel() {
        string currentLevel = SceneManager.GetActiveScene().name;
        if(levelSequence.levels[levelSequence.levels.Count-1].FileName == currentLevel) return true;
        return false;
    }

    void OnLevelFinish(GameEventTypeInt eventTypeInt,int levelIndex) {
        if(GetCurrentLevelIndex() != -1) {
            var name = levelSequence.levels[GetCurrentLevelIndex()].FileName;
            if(sceneTimers.ContainsKey(name)) {
                sceneTimers[name].Finish();
            }     
        }
    }
}
