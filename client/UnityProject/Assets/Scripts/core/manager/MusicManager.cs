using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour 
{
    public AudioSource audioSource = null;

    private Hashtable sounds = new Hashtable();

    public void Initialize()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// ����һ����Ƶ
    /// </summary>
    public AudioClip LoadAudioClip(string path)
    {
        AudioClip ac = Get(path);
        if (ac == null)
        {
            ac = (AudioClip)Resources.Load(path, typeof(AudioClip));
            Add(path, ac);
        }
        return ac;
    }

    /// <summary>
    /// �Ƿ񲥷ű������֣�Ĭ����1������
    /// </summary>
    /// <returns></returns>
    public bool CanPlayBackSound()
    {
        string key = AppConst.AppPrefix + "BackSound";
        int i = PlayerPrefs.GetInt(key, 1);
        return i == 1;
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="canPlay"></param>
    public void PlayBacksound(string name, bool canPlay)
    {
        if (this.audioSource.clip != null)
        {
            if (name.IndexOf(this.audioSource.clip.name) > -1)
            {
                if (!canPlay)
                {
                    this.audioSource.Stop();
                    this.audioSource.clip = null;
                    //Util.ClearMemory();
                }
                return;
            }
        }

        if (canPlay)
        {
            this.audioSource.loop = true;
            this.audioSource.clip = LoadAudioClip(name);
            this.audioSource.Play();
        }
        else
        {
            this.audioSource.Stop();
            this.audioSource.clip = null;
            //Util.ClearMemory();
        }
    }

    /// <summary>
    /// �Ƿ񲥷���Ч,Ĭ����1������
    /// </summary>
    public bool CanPlaySoundEffect()
    {
        string key = AppConst.AppPrefix + "SoundEffect";
        int i = PlayerPrefs.GetInt(key, 1);
        return i == 1;
    }

    /// <summary>
    /// ������Ƶ����
    /// </summary>
    public void Play(AudioClip clip, Vector3 position,float strenth = 1)
    {
        if (!CanPlaySoundEffect())
            return;
        AudioSource.PlayClipAtPoint(clip, position, strenth);
    }

    /// <summary>
    /// ���һ������
    /// </summary>
    private void Add(string key, AudioClip value)
    {
        if (sounds[key] != null || value == null)
            return;
        sounds.Add(key, value);
    }

    /// <summary>
    /// ��ȡһ������
    /// </summary>
    private AudioClip Get(string key)
    {
        if (sounds[key] == null)
            return null;
        return sounds[key] as AudioClip;
    }
}
