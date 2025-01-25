using UnityEngine;

public class Eyeblink : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;
    public Material Eyes_normal, Eyes_closed;
    public float timer = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EyesAnim();

    }

    public void EyesAnim()
    {
        timer += Random.Range(0.35f, 2.0f);
        if (timer >= 100.0f)
        {
            EyesOpenClose(Eyes_closed);
            if (timer >= 130.0f)
            {
                timer = 0.0f;
            }
        }
        else
        {
            EyesOpenClose(Eyes_normal);
        }
    }

    public void EyesOpenClose(Material eyesMaterial)
    {
        Material[] materials = skinnedMesh.materials;
        materials[2] = eyesMaterial;
        skinnedMesh.materials = materials;
    }

}
