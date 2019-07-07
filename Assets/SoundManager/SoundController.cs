using UnityEngine;
using System.Collections;
//using PathologicalGames;
using System.Collections.Generic;
using DG.Tweening;
/*
 * 2D音乐播放全局控制类
 * 如果有音乐已经在播放了,改变声音大小,或者停止播放 
 * 可控制已经在播放的音乐马上暂停,或改变当前的音量
 */
public class SoundController : MonoBehaviour
{

    private float fxVolume = 1;              //音效声音大小
    private float musicVolume = 1;           //音乐声音大小

    private bool isPlayFxVolume = true;      //是否播放音效声音
    private bool isPlayMusicVolume = true;   //是否播放音乐声音

    public AudioClip[] fxClip;
    public AudioClip[] musicClip;

    private SpawnPool audioSpwan;               //音乐游戏物体池
    private List<AudioSource> fxAudioList;      //为了控制已经播放的音乐,需要记录他们的引用
    private List<AudioSource> musicAudioList;
    public static SoundController instance;
    private int audioCount;                     //音乐下标值计时器

    private AudioSource audio;
    public void Awake()
    {
        audioSpwan = new SpawnPool();
        fxAudioList = new List<AudioSource>();
        musicAudioList = new List<AudioSource>();
        instance = this;
        audio = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 播放音效不可重叠播放
    /// </summary>
    /// <param name="name"></param>
    public void PlayFxSound(string name)
    {
        if (isPlayFxVolume)
        {
            for (int i = 0; i < fxClip.Length; i++)
            {
                if ((audio != null) && (name == fxClip[i].name))
                {
                    audio.PlayOneShot(fxClip[i], fxVolume);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 播放音效可重叠播放(NGUI播放方式,只能一次播放完整才能销毁,最好不要使用)
    /// </summary>
    /// <param name="name"></param>
    public void PlayFxSoundOverlap(string name,bool isLoop = false)
    {
        if (fxClip == null) return;

        if (isPlayFxVolume)
        {
            for (int i = 0; i < fxClip.Length; i++)
            {
                if (name == fxClip[i].name)
                {
                    //NGUITools.PlaySound(fxClipStatic[i], fxVolume);
                    var gameObject = audioSpwan.Spawn("AudioShotGame",transform);
                    var _audio = gameObject.GetComponent<AudioSource>();
                    _audio.clip = fxClip[i];
                    _audio.volume = fxVolume;
                    _audio.loop = isLoop;
                    _audio.Play();

                    AudioShot model = gameObject.GetComponent<AudioShot>();

                    if (string.IsNullOrEmpty(model.audioName))
                    {
                        model.name = audioCount + "";
                        audioCount++;
                        fxAudioList.Add(_audio);      //添加到集合中
                    }

                    return;
                }
            }
        }
    }

    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayMusicSound(string name)
    {
        if (isPlayMusicVolume)
        {
            for (int i = 0; i < musicClip.Length; i++)
            {
                if (name == musicClip[i].name)
                {
                    //if (audio.isPlaying) {
                        //audio.volume.
                        //audio.volume.do
                        audio.Stop();
                        //StartCoroutine(StopMusic(name,musicClip[i]));
                    //}
                    //else {

                        audio.PlayOneShot(musicClip[i], musicVolume);
                    //}
                    return;
                }
            }
        }
    }
    public void StopCurPlayingFx(string name) {
        for (int i = 0; i < fxAudioList.Count; i++)
        {
            if (fxAudioList[i].clip != null && fxAudioList[i].clip.name == name)
            {   
                fxAudioList[i].Stop();
                fxAudioList.Remove(fxAudioList[i]);
            }
        }
    }
    public IEnumerator StopMusic(string name,AudioClip audioClip) {
        float volum = 1;
        while(volum >= 0.01f) {
            volum = Mathf.Lerp(1,0,0.5f);
            audio.volume = volum;
            yield return new WaitForEndOfFrame();
        }
        audio.Stop();
        audio.PlayOneShot(audioClip, musicVolume);
    }
    public void PlayMusicSoundOverlap(string name)
    {
        if (musicClip == null) return;

        if (isPlayMusicVolume)
        {
            for (int i = 0; i < musicClip.Length; i++)
            {
                if (name == musicClip[i].name)
                {
                    var gameObject = audioSpwan.Spawn("AudioShotGame",this.transform);
                    var _audio = gameObject.GetComponent<AudioSource>();
                    _audio.clip = fxClip[i];
                    _audio.volume = fxVolume;
                    _audio.Play();

                    AudioShot model = gameObject.GetComponent<AudioShot>();
                    if (string.IsNullOrEmpty(model.audioName))
                    {
                        model.audioName = audioCount + "";
                        audioCount++;
                        //添加到集合中
                        musicAudioList.Add(_audio);
                    }

                    return;
                }
            }
        }
    }
    public void UpdateFxVolume(float volume)
    {
        this.fxVolume = volume;

        foreach (var i in fxAudioList)
        {
            i.volume = volume;
        }
    }
    public void UpdateMusicVolume(float volume)
    {
        this.musicVolume = volume;

        foreach (var i in musicAudioList)
        {
            i.volume = volume;
        }
    }
    public void IsPlayFxVolume(bool isPlay)
    {
        isPlayFxVolume = isPlay;

        if (isPlay == false)
        {
            foreach (var i in fxAudioList)
            {
                i.volume = 0;
            }
        }
        else
        {
            foreach (var i in fxAudioList)
            {
                i.volume = fxVolume;
            }
        }
    }
    public void IsPlayMusicVolume(bool isPlay)
    {
        isPlayMusicVolume = isPlay;

        if (isPlay == false)
        {
            foreach (var i in musicAudioList)
            {
                i.volume = 0;
            }
        }
        else
        {
            foreach (var i in musicAudioList)
            {
                i.volume = musicVolume;
            }
        }
    }
    public void ClearAudioSource()
    {
        for (int i = 0; i < musicAudioList.Count; i++)
        {
            if (musicAudioList[i].isPlaying == false)
            {
                musicAudioList.Remove(musicAudioList[i]);
            }
        }

        for (int i = 0; i < fxAudioList.Count; i++)
        {
            if (fxAudioList[i].isPlaying == false)
            {
                fxAudioList.Remove(fxAudioList[i]);
            }
        }
    }
    public bool IsFxPlaying(string name) {
        for (int i = 0; i < fxAudioList.Count; i++)
        {
            if (fxAudioList[i].clip.name == name)
            {
                return fxAudioList[i].isPlaying;
            }
        }
        return false;
    }
}