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
    protected override void Init() {
        levelSequence = Resources.Load<LevelSequence>("LevelSequence_01");
        if(levelSequence == null) {
            Debug.LogError("no such levelSequence");
        }
        DontDestroyOnLoad(gameObject);
    }
  
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
            if(levelSequence.levels[i].FileName == fileName) SceneManager.LoadScene(fileName);
        }
    }
    public void LoadNextOrPrevLevel(bool isPrevLevel) {
        
        int currentLevelIndex = GetCurrentLevelIndex();
        Debug.Log("??????????????" + SceneManager.GetActiveScene().name);
        if(currentLevelIndex != -1) {
            int targetLevelIndex = isPrevLevel? currentLevelIndex-1 : currentLevelIndex+1;
                SceneManager.LoadScene(levelSequence.levels[targetLevelIndex].FileName);
        }
    }
    public void LoadCurrentLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
