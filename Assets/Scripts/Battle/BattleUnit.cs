using UnityEngine;
using System.Collections.Generic;

public class BattleUnit : MonoBehaviour
{
    public CharacterData data;
    public int currentHP;
    public int currentMP;

    // Rastrea cuántos turnos le quedan de cooldown a cada move
    // La clave es el MoveData, el valor es turnos restantes
    private Dictionary<MoveData, int> cooldowns = new Dictionary<MoveData, int>();

    // Buffs activos
    public bool isInvulnerable = false;
    public float damageMultiplier = 1f;  // Arenga lo sube a 1.5f por un turno
    public bool hasReloadBuff = false;   // Recarga del fusilero

    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize()
    {
        currentHP = data.maxHP;
        currentMP = data.maxMP;
        cooldowns.Clear();
    }

    // Verifica si un move está disponible (cooldown y MP)
    public bool CanUseMove(MoveData move)
    {
        if (currentMP < move.mpCost) return false;
        if (cooldowns.ContainsKey(move) && cooldowns[move] > 0) return false;
        return true;
    }

    // Registra el cooldown cuando se usa un move
    public void RegisterCooldown(MoveData move)
    {
        if (move.cooldownTurns > 0)
            cooldowns[move] = move.cooldownTurns;
    }

    // Se llama al final de cada turno de esta unidad para reducir cooldowns
    public void TickCooldowns()
    {
        // Copiamos las keys porque no podemos modificar el Dictionary mientras lo iteramos
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
        currentHP = Mathf.Max(0, currentHP - amount);
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(data.maxHP, currentHP + amount);
    }

    // Limpia buffs de un solo turno al final del turno
    public void ClearTurnBuffs()
    {
        isInvulnerable = false;
        damageMultiplier = 1f;
        // hasReloadBuff NO se limpia aquí, solo se limpia cuando se usa Disparo
    }
}