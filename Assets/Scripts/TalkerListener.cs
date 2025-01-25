using UnityEngine;

public class TalkerListener : MonoBehaviour
{
    [Header("Eyes")]
    public SkinnedMeshRenderer skinnedMesh;
    public float blink_time = 0.0f;

    [Header("Random")]
    public GameObject[] Hairs;
    public Material[] Eyes, Eyes_closed, Skin, Shirt, Hair, materials;
    public int rand_skin, rand_hair;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            RandomGenerate();
        }
    }

    //Update is called once per frame
    void FixedUpdate()
    {
        EyesAnim();
    }

    public void RandomGenerate()
    {
        materials = skinnedMesh.materials;

        rand_skin = Random.Range(0, 5);
        materials[0] = Skin[rand_skin];
        materials[1] = Shirt[Random.Range(0, 5)];
        materials[2] = Hair[Random.Range(0, 5)];
        materials[3] = Eyes[rand_skin];

        for (int i = 0; i < Hairs.Length; i++)
        {
            Hairs[i].SetActive(false);
        }

        rand_hair = Random.Range(0, 5);
        Hairs[rand_hair].SetActive(true);
        Hairs[rand_hair].GetComponent<SkinnedMeshRenderer>().material = materials[2];

        skinnedMesh.materials = materials;
    }

    public void EyesAnim()
    {
        blink_time += Random.Range(0.35f, 2.0f);
        if (blink_time >= 100.0f)
        {
            EyesOpenClose(Eyes_closed[rand_skin]);
            if (blink_time >= 130.0f)
            {
                blink_time = 0.0f;
            }
        }
        else
        {
            EyesOpenClose(Eyes[rand_skin]);
        }
    }

    public void EyesOpenClose(Material eyesMaterial)
    {
        //skinnedMesh.materials[3] = eyesMaterial;

        Material[] materials = skinnedMesh.materials;
        materials[3] = eyesMaterial;
        skinnedMesh.materials = materials;
    }
}
