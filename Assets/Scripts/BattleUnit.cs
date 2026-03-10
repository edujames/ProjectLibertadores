using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public CharacterData data;
    public int currentHP;
    public int currentMP;

    public void Initialize()
    {
        currentHP = data.maxHP;
        currentMP = data.maxMP;
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(data.maxHP, currentHP + amount);
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
    }
}