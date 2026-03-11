using UnityEngine;

public enum MoveEffectType
{
    None,           // ataque o cura normal
    Invulnerable,   // No se compara a los Andes
    BuffDamage,     // Arenga
    Reload          // Recarga del fusilero
}

[CreateAssetMenu(fileName = "NewMove", menuName = "Libertadores/Move")]
public class MoveData : ScriptableObject
{
    [Header("Identidad")]
    public string moveName;

    [Header("Costo")]
    public int mpCost;

    [Header("Daño (aleatorio entre min y max)")]
    public int minDamage;
    public int maxDamage;

    [Header("Tipo")]
    public bool isHealing;
    public bool isBasicAttack;   // los básicos no consumen MP y siempre están disponibles

    [Header("Cooldown")]
    public int cooldownTurns;    // 0 = sin cooldown, 2 = cada dos turnos

    [Header("Efecto especial")]
    public MoveEffectType effectType;

    // Calcula el daño aleatorio en el momento de usar el move
    public int GetRandomDamage()
    {
        return Random.Range(minDamage, maxDamage + 1);
    }
}