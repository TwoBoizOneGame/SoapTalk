using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager _instance;
    public static AudioManager instance { get { return _instance; } }

    public AudioClip pickSound;
    public AudioClip placeSound;

    void Awake()
    {
        _instance=this;
    }

    public async UniTask PlayOneShot(AudioClip clip)
    {
        var sfx = new GameObject();
        var ac = sfx.AddComponent<AudioSource>();
        ac.clip = clip;
        ac.spatialBlend=0;
        ac.Play();
        await UniTask.WaitForSeconds(clip.length);
        Destroy(sfx);
    }
}
