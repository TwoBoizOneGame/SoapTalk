using UnityEngine;

public abstract class CardBase : ScriptableObject
{
    public string title;
    public string description;
    public int cost;
    public Sprite symbol;

    public virtual void OnBought()
    {

    }
}
