using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager _instance;
    public static AudioManager instance { get { return _instance; } }

    public AudioClip pickSound;
    public AudioClip placeSound;

    public AudioClip talkingSound;
    public AudioClip stretchSound;
    public AudioClip goodAnswerSound;
    public AudioClip badAnswerSound;

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
}
