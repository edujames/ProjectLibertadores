using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Libertadores/Character")]
public class CharacterData : ScriptableObject
{
    [Header("Identidad")]
    public string characterName;
    public Sprite portrait;

    [Header("Stats base")]
    public int maxHP;
    public int maxMP;
    public int attack;
    public int defense;
    public int speed; // determina orden de turnos

    [Header("Movimientos")]
    public MoveData[] moves; // otro ScriptableObject
}