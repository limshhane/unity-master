using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LIM_TRAN_HOUACINE_NGUYEN;
using SDD.Events;

public class AudioManager : Manager<AudioManager>
{
    [Header("MusicList")]
    [SerializeField] AudioClip m_MenuClip;
    [SerializeField] List<AudioClip> m_ClipsSFX = new List<AudioClip>();
    [SerializeField] List<AudioClip> m_ClipsLevel = new List<AudioClip>();
    AudioSource []m_AudioSources; // audiosource 0 ==> musique, audiosource 1 ==> bruitage (win,lose)
    //AudioClip current_Song;
    int m_CurrClipIndex = 0;
    float volumeLevelBeforePause;

    #region Manager implementation
    protected override IEnumerator InitCoroutine()
    {
        m_AudioSources = GetComponents<AudioSource>();
        volumeLevelBeforePause = m_AudioSources[0].volume;
        yield break;
    }
    #endregion


    #region Events' subscription
    public override void SubscribeEvents()
    {
        base.SubscribeEvents();
        EventManager.Instance.AddListener<GoToNextLevelEvent>(GoToNextLevel);
        EventManager.Instance.AddListener<LevelButtonClickedEvent>(LevelButtonClicked);

    }

    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        EventManager.Instance.RemoveListener<GoToNextLevelEvent>(GoToNextLevel);
        EventManager.Instance.RemoveListener<LevelButtonClickedEvent>(LevelButtonClicked);

    }
    #endregion

    protected override void GameMenu(GameMenuEvent e)
    {
        StopAllAudioSources();
        m_AudioSources[0].clip = m_MenuClip;
        m_AudioSources[0].Play();
    }

    protected override void GamePlay(GamePlayEvent e)
    {
        StopAllAudioSources();
        m_AudioSources[0].clip = m_ClipsLevel[m_CurrClipIndex];
        m_AudioSources[0].Play();
    }

    protected override void GameResume(GameResumeEvent e)
    {
        m_AudioSources[0].volume = volumeLevelBeforePause ;
    }

    protected override void GamePause(GamePauseEvent e)
    {
        volumeLevelBeforePause = m_AudioSources[0].volume;
        m_AudioSources[0].volume = 0.05f;
    }

    protected override void GameVictory(GameVictoryEvent e)
    {
        volumeLevelBeforePause = m_AudioSources[0].volume;
        m_AudioSources[0].volume = 0.05f;
        m_AudioSources[1].clip = m_ClipsSFX[0];
        m_AudioSources[1].Play();
    }

    protected override void GameOver(GameOverEvent e)
    {
        volumeLevelBeforePause = m_AudioSources[0].volume;
        m_AudioSources[0].volume = 0.05f;
        m_AudioSources[1].clip = m_ClipsSFX[1];
        m_AudioSources[1].Play();
    }

    public void GoToNextLevel(GoToNextLevelEvent e)
    {
        StopAllAudioSources();
        if (m_CurrClipIndex < m_ClipsLevel.Count - 1) m_CurrClipIndex++;
        m_AudioSources[0].clip = m_ClipsLevel[m_CurrClipIndex];
        m_AudioSources[0].Play();
    }

    public void StopAllAudioSources()
    {
        m_AudioSources[0].Stop();
        m_AudioSources[1].Stop();
    }
    public void LevelButtonClicked(LevelButtonClickedEvent e)
    {
        //Debug.Log("OUI LEVEL BUTTON");
        m_CurrClipIndex = e.levelIndex;
    }

    

}
