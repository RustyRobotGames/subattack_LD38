using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    public static AudioManager Instance
    {
        get { return instance; }
    }

    public GameObject AudioPrefab;

    public List<AudioEffect> audios = new List<AudioEffect>();
    

    Dictionary<string, AudioClip> clips;

    private void Start()
    {
        if (instance != null)
            Destroy(instance);

        instance = this;

        clips = new Dictionary<string, AudioClip>();
        foreach (var audio in audios)
            clips[audio.key] = audio.clip;
    }

    public void PlayAudioClip(string key, Vector3 position)
    {
        if (clips.ContainsKey(key))
        {
            var go = Instantiate(AudioPrefab, position, Quaternion.identity);
            go.transform.SetParent(this.transform);

            AudioSource source = go.GetComponent<AudioSource>();
            AudioClip clipToPlay = clips[key];
            
            source.clip = clipToPlay;
            source.Play();

            Destroy(go, clipToPlay.length);
        }
        else
            Debug.LogError("Audio not found");
    }
}


[System.Serializable]
public class AudioEffect
{
    public string key;
    public AudioClip clip;
}
