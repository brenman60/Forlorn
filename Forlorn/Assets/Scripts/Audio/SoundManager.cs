using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private Sounds sounds;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitUI()
    {
        if (Instance == null)
        {
            GameObject managerObject = Instantiate(Resources.Load<GameObject>("Utils/SoundManager"));
            managerObject.name = "SoundManager";

            SoundManager manager = managerObject.GetComponent<SoundManager>();
            Instance = manager;

            DontDestroyOnLoad(managerObject);
        }
    }

    public void PlayAudio(string soundName, bool randomPitch, float initialVolume = 1)
    {
        Sound sound = sounds.GetSoundByName(soundName);
        AudioSource soundObject = CreateSoundObject(sound);
        soundObject.pitch = !randomPitch ? 1f : Random.Range(0.8f, 1.2f);
        soundObject.volume = initialVolume;
    }

    public void PlayAudio(Sound sound, bool randomPitch, float initialVolume = 1)
    {
        AudioSource soundObject = CreateSoundObject(sound);
        soundObject.pitch = !randomPitch ? 1f : Random.Range(0.8f, 1.2f);
        soundObject.volume = initialVolume;
    }

    private AudioSource CreateSoundObject(Sound sound)
    {
        GameObject soundObject = new GameObject(sound.name);
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        SoundConfigure configure = soundObject.AddComponent<SoundConfigure>();

        audioSource.clip = sound.soundClip;
        configure.soundType = sound.soundType;

        return audioSource;
    }
}
