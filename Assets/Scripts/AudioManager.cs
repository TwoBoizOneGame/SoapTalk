using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager _instance;
    public static AudioManager instance { get { return _instance; } }

    public AudioClip pickSound;
    public AudioClip placeSound;

    public AudioClip talkingSound;
    public AudioClip[] stretchSounds;
    public AudioClip[] goodAnswerSound;
    public AudioClip[] badAnswerSound;

    public AudioClip[] popSounds;

    public AudioClip[] spawnAnchorSounds;
    public AudioClip[] whooshSounds;

    public AudioClip[] explosionSounds;

    public AudioClip[] jumpSounds;

    public AudioClip[] coinSounds;

    void Awake()
    {
        _instance=this;
    }

    public async UniTask PlayOneShotAsync(AudioClip clip)
    {
        var sfx = PlayOneShot(clip,false);     
        await UniTask.WaitForSeconds(clip.length);
        Destroy(sfx);
    }

    public async UniTask PlayOneShotAsync(AudioClip[] audioClips)
    {
        await PlayOneShotAsync(audioClips[Random.Range(0, audioClips.Length)]);
    }

    public AudioSource PlayOneShot(AudioClip clip, bool loop)
    {
        var sfx = new GameObject();
        var ac = sfx.AddComponent<AudioSource>();
        ac.clip = clip;
        ac.loop=loop;
        ac.spatialBlend=0;
        ac.Play();
        return ac;
    }

    public AudioSource PlayOneShot(AudioClip[] audioClips, bool loop)
    {
        return PlayOneShot(audioClips[Random.Range(0, audioClips.Length)], loop);
    }
}
