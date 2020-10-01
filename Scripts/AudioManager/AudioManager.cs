using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Static instance for singleton
    public static AudioManager instance;
    [Tooltip("How many audio sources to pool on Start()")]
    public int poolingAmount = 10;
    [Tooltip("Enable this so, in the event more sources are requested than available, the pooler can take it upon itself to add more.")]
    public bool overflowProtection = true;

    //A queue of ready to use audio sources
    private Queue<AudioSource> audioSources;
    //A list of the active audio sources, this will be looped through to returned finished sources back to the queue
    private List<AudioSource> activeSources;

    //Garbage Reduction
    private AudioSource cacheSource;
    private GameObject cacheObj;

    //How often, in seconds, should the AudioManager check for finished audio sources
    private float checkTime = 0.5f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        BuildPool();
    }

    private void Update()
    {
        //Only check every half of a second, no need to check every single frame.
        checkTime -= Time.deltaTime;
        if (checkTime <= 0)
        {
            CheckForFinished();
            checkTime = 0.5f;
        }
    }

    //Build the initial pool
    private void BuildPool()
    {
        //Create new queue and list objects
        audioSources = new Queue<AudioSource>();
        activeSources = new List<AudioSource>();

        //Add the number of entries specified by the pooling amount
        for (int i = 0; i < poolingAmount; i++)
        {
            AddEntry();
        }
    }

    //Adds an entry to audio sources queue
    private void AddEntry()
    {
        cacheObj = new GameObject();
        cacheObj.transform.parent = transform;
        cacheSource = cacheObj.AddComponent<AudioSource>();
        cacheSource.playOnAwake = false;
        cacheObj.SetActive(false);
        audioSources.Enqueue(cacheSource);
    }

    
    private void CheckForFinished()
    {
        //Iterate through all the active audio sources and check if they are done playing
        for (int i = 0; i < activeSources.Count; i++)
        {
            //If they are done playing deactivate them and add them back into the queue
            if (!activeSources[i].isPlaying)
            {
                audioSources.Enqueue(activeSources[i]);
                activeSources[i].gameObject.SetActive(false);
                activeSources.RemoveAt(i);
                i--;
            }
        }
    }

    //Grabs an Audio Source from the queue if we have one available
    private AudioSource GetAudioSource()
    {
        if (audioSources.Count < 1)
        {
            if (overflowProtection)
                AddEntry();
            else
            {
                Debug.LogError("Audio Source Queue Empty, consider increasing pool size or turn on overflow protection.");
                return null;
            }
        }

        return audioSources.Dequeue();
    }

    /// <summary>
    /// Method for manually returning Audio Sources back into the pool
    /// </summary>
    /// <param name="source"></param>
    public static void ReturnToPool(AudioSource source)
    {
        source.gameObject.SetActive(false);
        instance.audioSources.Enqueue(source);
    }

    #region 2D Sound Methods
    /// <summary>
    /// Plays an AudioClip without spatial blending with default settings
    /// </summary>
    /// <param name="clip"></param>
    public static void PlaySound2D(AudioClip clip)
    {
        instance.cacheSource = instance.GetAudioSource();
        instance.cacheSource.clip = clip;
        instance.cacheSource.volume = 1f;
        instance.cacheSource.loop = false;
        instance.cacheSource.spatialBlend = 0.0f;
        instance.cacheSource.gameObject.SetActive(true);
        instance.activeSources.Add(instance.cacheSource);
        instance.cacheSource.Play();
    }
    /// <summary>
    /// Plays an AudioClip without spatial blending with adjustable volume
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    public static void PlaySound2D(AudioClip clip, float volume)
    {
        instance.cacheSource = instance.GetAudioSource();
        instance.cacheSource.clip = clip;
        instance.cacheSource.volume = volume;
        instance.cacheSource.loop = false;
        instance.cacheSource.spatialBlend = 0.0f;
        instance.cacheSource.gameObject.SetActive(true);
        instance.activeSources.Add(instance.cacheSource);
        instance.cacheSource.Play();
    }
    /// <summary>
    /// Plays an AudioClip without spatial blending with adjustable volume and pitch
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    public static void PlaySound2D(AudioClip clip, float volume, float pitch)
    {
        instance.cacheSource = instance.GetAudioSource();
        instance.cacheSource.clip = clip;
        instance.cacheSource.volume = volume;
        instance.cacheSource.pitch = pitch;
        instance.cacheSource.loop = false;
        instance.cacheSource.spatialBlend = 0.0f;
        instance.cacheSource.gameObject.SetActive(true);
        instance.activeSources.Add(instance.cacheSource);
        instance.cacheSource.Play();
    }
    /// <summary>
    /// Plays an AudioClip without spatial blending with adjustable volume and pitch that loops
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns>AudioSource</returns>
    public static AudioSource PlaySound2DLooped(AudioClip clip, float volume, float pitch)
    {
        instance.cacheSource = instance.GetAudioSource();
        instance.cacheSource.clip = clip;
        instance.cacheSource.volume = volume;
        instance.cacheSource.pitch = pitch;
        instance.cacheSource.loop = true;
        instance.cacheSource.spatialBlend = 0.0f;
        instance.cacheSource.gameObject.SetActive(true);
        instance.cacheSource.Play();
        return instance.cacheSource;
    }
    #endregion

    #region 3D Sound Methods
    /// <summary>
    /// Plays and AudioClip with spatial blending at a specific location
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    public static void PlaySound3D(AudioClip clip, Vector3 position)
    {
        instance.cacheSource = instance.GetAudioSource();
        instance.cacheSource.clip = clip;
        instance.cacheSource.volume = 1f;
        instance.cacheSource.loop = false;
        instance.cacheSource.spatialBlend = 1.0f;
        instance.cacheSource.gameObject.transform.position = position;
        instance.cacheSource.gameObject.SetActive(true);
        instance.activeSources.Add(instance.cacheSource);
        instance.cacheSource.Play();
    }

    /// <summary>
    /// Plays and AudioClip with spatial blending at a specific location with adjustable volume
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    public static void PlaySound3D(AudioClip clip, Vector3 position, float volume)
    {
        instance.cacheSource = instance.GetAudioSource();
        instance.cacheSource.clip = clip;
        instance.cacheSource.volume = 1f;
        instance.cacheSource.loop = false;
        instance.cacheSource.spatialBlend = 1.0f;
        instance.cacheSource.gameObject.transform.position = position;
        instance.cacheSource.gameObject.SetActive(true);
        instance.activeSources.Add(instance.cacheSource);
        instance.cacheSource.Play();
    }
    /// <summary>
    /// Plays and AudioClip with spatial blending at a specific location with adjustable volume and pitch
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    public static void PlaySound3D(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        instance.cacheSource = instance.GetAudioSource();
        instance.cacheSource.clip = clip;
        instance.cacheSource.volume = 1f;
        instance.cacheSource.pitch = pitch;
        instance.cacheSource.loop = false;
        instance.cacheSource.spatialBlend = 1.0f;
        instance.cacheSource.gameObject.transform.position = position;
        instance.cacheSource.gameObject.SetActive(true);
        instance.activeSources.Add(instance.cacheSource);
        instance.cacheSource.Play();
    }
    /// <summary>
    /// Plays and AudioClip with spatial blending at a specific location with adjustable volume and pitch that loops
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    public static AudioSource PlaySound3DLooped(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        instance.cacheSource = instance.GetAudioSource();
        instance.cacheSource.clip = clip;
        instance.cacheSource.volume = 1f;
        instance.cacheSource.pitch = pitch;
        instance.cacheSource.loop = false;
        instance.cacheSource.spatialBlend = 1.0f;
        instance.cacheSource.gameObject.transform.position = position;
        instance.cacheSource.gameObject.SetActive(true);
        instance.activeSources.Add(instance.cacheSource);
        instance.cacheSource.Play();
        return instance.cacheSource;
    }
    #endregion
}
