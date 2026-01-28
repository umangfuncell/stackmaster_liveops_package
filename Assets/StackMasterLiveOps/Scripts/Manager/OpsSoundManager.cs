using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpsSoundManager : MonoBehaviour
{
    [SerializeField] AudioSource _sfxAudioSource;
    [SerializeField] AudioSource _musicAudioSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip _startPlay;
    [SerializeField] private AudioClip _gameOver;
    [SerializeField] private AudioClip _mergeSfx;
    [SerializeField] private AudioClip _gotoGridNodeSfx;
    [SerializeField] private AudioClip _buttonClickSfx;
    [SerializeField] private AudioClip _achievementSfx;
    public static OpsSoundManager Instance;

    public void Init()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //handle music and sfx mute and unmute
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    public void PlaySfx(AudioClip audioClip)
    {
        _sfxAudioSource.PlayOneShot(audioClip);
    }
    public void PlayMusic()
    {
        _musicAudioSource.Play();
    }
    public void PlayStarSfx()
    {
        PlaySfx(_startPlay);
    }
    public void PlayGameover()
    {
        PlaySfx(_gameOver);
    }
    public void PlayMergeSfx()
    {
        PlaySfx(_mergeSfx);
    }
    public void PlayGoToNodeSfx()
    {
        PlaySfx(_gotoGridNodeSfx);
    }
    public void PlayBtnClickSound()
    {
        PlaySfx(_buttonClickSfx);
    }
    public void PlayAchievementSfx()
    {
        PlaySfx(_achievementSfx);
    }
}
