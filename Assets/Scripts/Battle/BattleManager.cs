using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class BattleManager : MonoBehaviour
{
    [Header("Unidades")]
    public List<BattleUnit> heroes;   // Asignar en Inspector: SanMartin, Gaucho, Fusilero
    public List<BattleUnit> enemies;  // Asignar en Inspector: el enemigo de prueba

    // Lista unificada ordenada por speed
    private List<BattleUnit> turnOrder = new List<BattleUnit>();

    // Índice de quién actúa ahora en la lista
    private int currentTurnIndex = 0;

    // La unidad que está actuando ahora mismo
    public BattleUnit ActiveUnit => turnOrder[currentTurnIndex];

    void Start()
    {
        InitializeBattle();
    }

    void InitializeBattle()
    {
        // Inicializar HP y MP de todos
        foreach (var unit in heroes) unit.Initialize();
        foreach (var unit in enemies) unit.Initialize();

        // Unir héroes y enemigos en una sola lista
        turnOrder.AddRange(heroes);
        turnOrder.AddRange(enemies);

        // Ordenar por speed de mayor a menor
        // Sort compara dos elementos a y b:
        // si devuelve negativo, b va antes; positivo, a va antes
        turnOrder.Sort((a, b) => b.data.speed.CompareTo(a.data.speed));

        Debug.Log("Orden de turnos:");
        foreach (var unit in turnOrder)
            Debug.Log(unit.data.characterName + " - Speed: " + unit.data.speed);

        StartCoroutine(RunBattle());
    }

    IEnumerator RunBattle()
    {
        while (!IsBattleOver())
        {
            BattleUnit current = ActiveUnit;

            // Si la unidad está muerta, saltamos su turno
            if (current.currentHP <= 0)
            {
                NextTurn();
                continue;
            }

            if (heroes.Contains(current))
            {
                // Es un héroe: esperamos a que el jugador elija una acción
                // La UI activará ExecutePlayerAction() cuando el jugador decida
                yield return new WaitUntil(() => playerActionChosen);
                playerActionChosen = false;
            }
            else
            {
                // Es un enemigo: IA simple, ataca al héroe con menos HP
                yield return StartCoroutine(EnemyTurn(current));
            }

            NextTurn();
            yield return new WaitForSeconds(0.5f); // pausa entre turnos
        }

        Debug.Log("Batalla terminada");
    }

    // La UI llama a este método cuando el jugador elige un movimiento
    private bool playerActionChosen = false;
    private MoveData chosenMove;
    private BattleUnit chosenTarget;

    public void ExecutePlayerAction(MoveData move, BattleUnit target)
    {
        BattleUnit attacker = ActiveUnit;

        // Verificar que puede usar el move
        if (!attacker.CanUseMove(move)) return;

        // Descontar MP
        attacker.currentMP -= move.mpCost;

        // Registrar cooldown si tiene
        attacker.RegisterCooldown(move);

        // Resolver el efecto según el tipo
        switch (move.effectType)
        {
            case MoveEffectType.None:
                if (move.isHealing)
                {
                    // Mate Amargo: cura % de HP máximo en lugar de valor fijo
                    // Por eso en MoveData ponemos minDamage=30 para representar el 30%
                    int healAmount = Mathf.RoundToInt(target.data.maxHP * (move.minDamage / 100f));
                    target.Heal(healAmount);
                    Debug.Log("Cura " + healAmount + " HP");
                }
                else
                {
                    // Ataque normal con rango aleatorio
                    int baseDamage = move.GetRandomDamage();

                    // Aplicar buff de Recarga si corresponde
                    if (move.moveName == "Disparo de Carabina" && attacker.hasReloadBuff)
                    {
                        baseDamage = Mathf.RoundToInt(baseDamage * 1.5f);
                        attacker.hasReloadBuff = false;
                        Debug.Log("Recarga aplicada, daño aumentado");
                    }

                    // Aplicar multiplicador de Arenga si está activo
                    int finalDamage = Mathf.RoundToInt(baseDamage * attacker.damageMultiplier);
                    target.TakeDamage(finalDamage);
                    Debug.Log(attacker.data.characterName + " hace " + finalDamage + " daño");
                }
                break;

            case MoveEffectType.Invulnerable:
                // No se compara a los Andes
                attacker.isInvulnerable = true;
                Debug.Log(attacker.data.characterName + " es invulnerable este turno");
                break;

            case MoveEffectType.BuffDamage:
                // Arenga: sube el multiplicador de todos los héroes vivos
                foreach (var hero in heroes)
                {
                    if (hero.currentHP > 0)
                        hero.damageMultiplier = 1.5f;
                }
                Debug.Log("Arenga activa, todos hacen 50% más de daño");
                break;

            case MoveEffectType.Reload:
                // Recarga: activa el buff en el fusilero para su próximo Disparo
                attacker.hasReloadBuff = true;
                Debug.Log("Fusilero recargado");
                break;
        }

        // Limpiar buffs de turno único al terminar
        attacker.ClearTurnBuffs();

        playerActionChosen = true;
    }


    IEnumerator EnemyTurn(BattleUnit enemy)
    {
        // IA básica: ataca al héroe vivo con menos HP
        BattleUnit target = GetLowestHPHero();
        if (target != null)
        {
            Debug.Log(enemy.data.characterName + " ataca a " + target.data.characterName);
            target.TakeDamage(enemy.data.attack);
        }
        yield return new WaitForSeconds(1f);
    }

    BattleUnit GetLowestHPHero()
    {
        BattleUnit lowest = null;
        foreach (var hero in heroes)
        {
            if (hero.currentHP <= 0) continue;
            if (lowest == null || hero.currentHP < lowest.currentHP)
                lowest = hero;
        }
        return lowest;
    }

    void NextTurn()
    {
        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;
    }

    bool IsBattleOver()
    {
        bool allHeroesDead = heroes.TrueForAll(h => h.currentHP <= 0);
        bool allEnemiesDead = enemies.TrueForAll(e => e.currentHP <= 0);
        return allHeroesDead || allEnemiesDead;
    }
}