using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
public class UIManager : SingletonManager<UIManager> {
    [SerializeField]
    private GameObject drownAnimObject;
    [SerializeField]
    private GameObject leftLevelButton;
    [SerializeField]
    private GameObject rightLevelButton;
    [SerializeField]
    private GameObject restartLevelButton;
    [SerializeField]
    private GameObject selectLevelMenuPanel;
    [SerializeField]
    private GameObject selectLevelLeftButton;
    [SerializeField]
    private GameObject selectLevelRightButton;
    [SerializeField]
    private TMP_Text levelText;

    [SerializeField]
    private TMP_Text timerText;
    [SerializeField]
    private GameObject timerPanel;

    [SerializeField]
    private Image fadeInOutImage;
    [SerializeField]
    private GameObject levelImage_01;
    [SerializeField]
    private GameObject levelImage_02;
    [SerializeField]
    private GameObject levelImage_03;
    [SerializeField]
    private GameObject levelImage_04;
    [SerializeField]
    private GameObject levelImage_05;
    [SerializeField]
    private GameObject levelImage_06;
    [SerializeField]
    private GameObject levelIndicateImage;
    private List<GameObject> levelButtonList;
    private int levelSelectPage;

    private IA_Main uiInputAction;
    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable() {
        
        // uiInputAction.UI.Esc.performed += OnEscButtonPerformed;

        

        SceneManager.sceneLoaded += (scene,loadSceneMode)=> {
            if(levelText != null) {
            int levelIndex = GameManager.Instance.GetCurrentLevelIndex();
            if(levelIndex < 0) {
                levelText.gameObject.SetActive(false);
            }else {
                levelText.gameObject.SetActive(true);
                levelText.text = "Level " + (levelIndex + 1).ToString();
            }
            
        }

        if(leftLevelButton != null) {
            if(GameManager.Instance.GetCurrentLevelIndex() == -1) {
                leftLevelButton.SetActive(false);
            }else {
                leftLevelButton.SetActive(!GameManager.Instance.isFirstLevel());
            }
        }
        if(rightLevelButton != null) {
            if(GameManager.Instance.GetCurrentLevelIndex() == -1) {
                rightLevelButton.SetActive(false);
            }else {
                rightLevelButton.SetActive(!GameManager.Instance.isLastLevel());
            }
        }
        if(restartLevelButton != null) {
            restartLevelButton.SetActive(GameManager.Instance.GetCurrentLevelIndex() != -1);
        }
        if(selectLevelMenuPanel != null) {
            selectLevelMenuPanel.SetActive(false);
        }
        if(levelIndicateImage != null) {         
            if(GameManager.Instance.GetCurrentLevelIndex() != -1) {
                Debug.Log(GameManager.Instance.LevelSequence.levels[GameManager.Instance.GetCurrentLevelIndex()].LevelIndicateImage);
                levelIndicateImage.GetComponent<Image>().sprite = 
                GameManager.Instance.LevelSequence.levels[GameManager.Instance.GetCurrentLevelIndex()].LevelIndicateImage;
            }
            levelIndicateImage.SetActive(levelIndicateImage.GetComponent<Image>().sprite != null);    
        }       
        if(drownAnimObject != null) {
            Vector3 temp = Camera.main.ViewportToWorldPoint(new Vector2(0.5f,1)) + Vector3.down;
            drownAnimObject.transform.position = new Vector3(temp.x,temp.y,0);
            drownAnimObject.SetActive(false);
        }

        if(levelImage_01 == null) return;
        levelButtonList = new List<GameObject>();
        levelButtonList.Add(levelImage_01.transform.GetChild(0).gameObject);
        levelButtonList.Add(levelImage_02.transform.GetChild(0).gameObject);
        levelButtonList.Add(levelImage_03.transform.GetChild(0).gameObject);
        levelButtonList.Add(levelImage_04.transform.GetChild(0).gameObject);
        levelButtonList.Add(levelImage_05.transform.GetChild(0).gameObject);
        levelButtonList.Add(levelImage_06.transform.GetChild(0).gameObject);

        
        };
        uiInputAction = new IA_Main();
        uiInputAction.Enable();
        uiInputAction.UI.ShowTimer.performed += ShowTimer;
       
        //GameEventsManager.StartListening(GameEventTypeVoid.SHOW_TIMER,ShowTimer);
    }
    private void OnDisable() {

        // uiInputAction.UI.Esc.performed -= OnEscButtonPerformed;
        uiInputAction.UI.ShowTimer.performed -= ShowTimer;
        uiInputAction.Disable();
        //GameEventsManager.StopListening(GameEventTypeVoid.SHOW_TIMER,ShowTimer);
    }
    private void Start() {

        

    }
    public void OnStartFirstLevelButtonClicked() {
        AudioManager.Instance.PlayUIClickAudio();
        GameManager.Instance.LoadLevel(0);
    }
    public void OnLeftLevelButtonClicked() {
        AudioManager.Instance.PlayUIClickAudio();
        GameManager.Instance.LoadNextOrPrevLevel(true);
    }
    public void OnRightLevelButtonClicked() {
        AudioManager.Instance.PlayUIClickAudio();
        GameManager.Instance.LoadNextOrPrevLevel(false);
    }
    public void OnRestartLevelButtonClicked() {
        AudioManager.Instance.PlayUIClickAudio();
        GameManager.Instance.LoadCurrentLevel();
    }
    public void OnEscButtonPerformed(InputAction.CallbackContext context) {
        OnSelectLevelMenuButtonClicked();
    }
    public void OnSelectLevelMenuButtonClicked() {
        //AudioManager.Instance.PlayUIClickAudio();
        selectLevelMenuPanel.SetActive(!selectLevelMenuPanel.activeSelf);
        if(selectLevelMenuPanel.activeSelf) {      
            levelSelectPage = 0;
            RefreshCurrentLevelPage();
        }
    }
    public void OnSelectLevelLeftAndRightButtonClicked(bool isRight) {
        //AudioManager.Instance.PlayUIClickAudio();
        levelSelectPage = isRight? levelSelectPage + 1 : levelSelectPage - 1;
        RefreshCurrentLevelPage();
    }
    private void RefreshCurrentLevelPage() {
       if(GameManager.Instance.LevelSequence == null || GameManager.Instance.LevelSequence.levels.Count == 0) Debug.LogError("no levelSequce or level");
       int maxPages = Mathf.CeilToInt(GameManager.Instance.LevelSequence.levels.Count / 6f) - 1;
       Debug.Log(GameManager.Instance.LevelSequence.levels.Count/6f);
       selectLevelLeftButton.SetActive(levelSelectPage != 0);
       selectLevelRightButton.SetActive(levelSelectPage != maxPages);
       int activeObjectCount = 0;
       if(levelSelectPage == maxPages && GameManager.Instance.LevelSequence.levels.Count % 6 != 0) {
          activeObjectCount = GameManager.Instance.LevelSequence.levels.Count % 6;
       }else {
           activeObjectCount = 6;
       }
       for (int i = 0; i < levelButtonList.Count; i++) {
           if(i<activeObjectCount) {
               levelButtonList[i].SetActive(true);
               LevelDataSO data = GameManager.Instance.LevelSequence.levels[levelSelectPage * 6 + i];
               levelButtonList[i].GetComponent<Image>().sprite = data.LevelThumbnail;
               levelButtonList[i].GetComponent<Button>().onClick.RemoveAllListeners();
               levelButtonList[i].GetComponent<Button>().onClick.AddListener(()=> {
                   //AudioManager.Instance.PlayUIClickAudio();
                   GameManager.Instance.LoadLevel(data.FileName);});
           }else {
               levelButtonList[i].SetActive(false);
           }
       }
    }
    public void SetFadeInAndOutPanelAlpha(float value) {
        if(fadeInOutImage.TryGetComponent<Image>(out Image image)) {
            Color color = image.color;
            image.color = new Color(color.r,color.g,color.b,value);
        }
    }

    void ShowTimer(InputAction.CallbackContext ctx)
    {
        timerPanel.SetActive(!timerPanel.activeSelf);
        timerText.text = "Timer" + System.Environment.NewLine;
        foreach (var item in GameManager.Instance.SceneTimers)
        {
            //item.Value.Continue();
            timerText.text += item.Key + ": " + item.Value.GetTime().ToString() + System.Environment.NewLine;
        }
    }
}
