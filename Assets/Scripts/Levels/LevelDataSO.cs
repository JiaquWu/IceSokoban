using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Level/Data",fileName ="LevelData", order = 0)]
public class LevelDataSO : ScriptableObject {
    [SerializeField]
    private string fileName;
    public string FileName => fileName;

    [SerializeField]
    private string levelName;
    public string LevelName => LevelName;

    [SerializeField]
    private Sprite levelThumbnail;
    public Sprite LevelThumbnail => levelThumbnail;
    [SerializeField]
    private Sprite levelIndicateImage;
    public Sprite LevelIndicateImage => levelIndicateImage;
    
}
