using UnityEngine;

public class MainMenu_GM : MonoBehaviour
{

    public GameObject Talker_Listener;
    public Character character;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = Talker_Listener.GetComponent<Character>();
        character.talkerListener.RandomGenerate();
        character.SetIdlePose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
