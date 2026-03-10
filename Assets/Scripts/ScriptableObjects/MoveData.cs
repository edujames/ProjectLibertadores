using UnityEngine;

[CreateAssetMenu(fileName = "NewMove", menuName = "Libertadores/Move")]
public class MoveData : ScriptableObject
{
    public string moveName;
    public int mpCost;
    public int damage;
    public bool isHealing;
    public bool targetAll;
}