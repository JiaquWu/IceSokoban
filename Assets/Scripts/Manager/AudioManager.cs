using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonManager<AudioManager> {
    [SerializeField]
    private AudioClip[] playerMoveAudioClips;
    [SerializeField]
    private AudioClip[] pushIceShortAudioClips;
    [SerializeField]
    private AudioClip[] pushIceLongAudioClips;
    
    [SerializeField]
    private AudioSource playerMoveAudio;
    [SerializeField]
    private AudioSource icePushAudio;
    [SerializeField]
    private AudioSource seaWaveAudio;
    [SerializeField]
    private AudioSource BGM;
    [SerializeField]
    private AudioSource playerFinishLevelAudio;
    // [SerializeField]
    // private AudioSource playerPutDownAnimalOnBoatAudio;
    [SerializeField]
    private AudioSource uiClickAudio;
    protected override void Init() {
        DontDestroyOnLoad(gameObject);
        PlayBGM();
    }
    public void PlayPlayerMoveAudio() {
        if(playerMoveAudio != null) {
            if(playerMoveAudioClips.Length > 0) {
                playerMoveAudio.clip = playerMoveAudioClips[Random.Range(0,playerMoveAudioClips.Length)];
            }
            playerMoveAudio.PlayOneShot(playerMoveAudio.clip);
        }
    }
    public void PlayicePushAudio() {
        
        if(icePushAudio != null) { 
            if(pushIceShortAudioClips.Length > 0) {
                icePushAudio.clip = pushIceShortAudioClips[Random.Range(0,pushIceShortAudioClips.Length)];
            }
            
            icePushAudio.PlayOneShot(icePushAudio.clip);
        }
    }
    public void StopIcePushAudio() {
        if(icePushAudio != null) icePushAudio.Stop();
    }
    public void PlaySeaWaveAudio() {
        if(seaWaveAudio != null) seaWaveAudio.Play();
    }
    public void PlayBGM() {
        if(BGM != null) BGM.Play();
    }
    public void PlayplayerFinishLevelAudio() {
        if(playerFinishLevelAudio != null) playerFinishLevelAudio.Play();
    }
    // public void PlayplayerPutDownAnimalOnBoatAudio() {
    //     if(playerPutDownAnimalOnBoatAudio != null) playerPutDownAnimalOnBoatAudio.Play();
    // }
    public void PlayUIClickAudio() {
        if(uiClickAudio != null) uiClickAudio.Play();
    }

} 
