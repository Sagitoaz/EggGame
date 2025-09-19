using UnityEngine;

[CreateAssetMenu(fileName = "EggData", menuName = "ScriptableObjects/EggData", order = 1)]
public class EggData : ScriptableObject
{
    public int Level;
    public Sprite EggSprite;
}
