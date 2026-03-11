using UnityEngine;
using System.Collections.Generic;

public class BattleUnit : MonoBehaviour
{
    public CharacterData data;
    public int currentHP;
    public int currentMP;
    public int currentAttack;   // nuevo: stat de ataque en runtime
    public int currentDefense;  // nuevo: stat de defensa en runtime
    public int level = 1;       // nuevo: nivel actual del personaje

    private Dictionary<MoveData, int> cooldowns = new Dictionary<MoveData, int>();

    public bool isInvulnerable = false;
    public float damageMultiplier = 1f;
    public bool hasReloadBuff = false;

    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize()
    {
        currentHP = data.maxHP;
        currentMP = data.maxMP;
        currentAttack = data.attack;     // nuevo
        currentDefense = data.defense;   // nuevo
        cooldowns.Clear();
    }

    public bool CanUseMove(MoveData move)
    {
        if (currentMP < move.mpCost) return false;
        if (cooldowns.ContainsKey(move) && cooldowns[move] > 0) return false;
        return true;
    }

    public void RegisterCooldown(MoveData move)
    {
        if (move.cooldownTurns > 0)
            cooldowns[move] = move.cooldownTurns;
    }

    public void TickCooldowns()
    {
        List<MoveData> keys = new List<MoveData>(cooldowns.Keys);
        foreach (var key in keys)
        {
            if (cooldowns[key] > 0)
                cooldowns[key]--;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable)
        {
            Debug.Log(data.characterName + " es invulnerable, no recibe daño");
            return;
        }
        // nuevo: la defensa reduce el daño, mínimo 1 para que siempre duela algo
        int reduced = Mathf.Max(1, amount - currentDefense);
        currentHP = Mathf.Max(0, currentHP - reduced);
        Debug.Log(data.characterName + " recibe " + reduced + " daño (bloqueó " + (amount - reduced) + ")");
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(data.maxHP, currentHP + amount);
    }

    // nuevo: sube stats al subir de nivel
    public void LevelUp()
    {
        level++;
        // Cada nivel agrega 10% del stat base
        // RoundToInt redondea al entero más cercano
        currentAttack = Mathf.RoundToInt(data.attack * (1f + level * 0.1f));
        currentDefense = Mathf.RoundToInt(data.defense * (1f + level * 0.1f));
        int hpBonus = Mathf.RoundToInt(data.maxHP * 0.1f);
        currentHP = Mathf.Min(currentHP + hpBonus, data.maxHP + hpBonus * level);
        Debug.Log(data.characterName + " subió al nivel " + level);
    }

    public void ClearTurnBuffs()
    {
        isInvulnerable = false;
        damageMultiplier = 1f;
    }
}