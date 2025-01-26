using UnityEngine;

public abstract class ModificatorBase : ScriptableObject
{
    public GameObject visualization;
    public Word word;
    public float rarity;
    public bool isNegative=true;

    public virtual void Setup(Word word)
    {
        this.word = word;
        if (visualization != null)
        {
            var viz = Instantiate(visualization, word.modificatorIcon.transform);
        }
    }

    public virtual void OnScore()
    {

    }

    public virtual void OnStartMovement()
    {

    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnPlace(WordAnchor anchor)
    {

    }
}
