using System;
using UnityEngine;

public class EnemyHealth : ValueBase, IDamageable
{
    public Action<float, float> OnValueChangedEnemy;

    private void Start()
    {
        AddHP(maxValue);
        GameManager.Instance.CurrentEnemy = gameObject;
    }

    /// <summary>
    /// Reduce enemy hp
    /// </summary>
    /// <param name="damageValue">Amout of HP to be reduced.</param>
    public void Damage(float damageValue)
    {
        base.ReduceValue(damageValue);
        OnValueChangedEnemy?.Invoke(currentValue, maxValue);
        VerifyLife();
    }

    /// <summary>
    /// Check if enemy died.
    /// </summary>
    private void VerifyLife()
    {
        if (currentValue <= 0)
        {
            KillEnemy();
        }
    }

    /// <summary>
    /// Trigger enemy death.
    /// </summary>
    private void KillEnemy()
    {
        // Death logic
        GameManager.Instance.CurrentEnemy = null;
        gameObject.SetActive(false);
        CombatManager.Instance.ReturnToMap();
    }

    public void AddHP(float value)
    {
        base.IncreaseValue(value);
        OnValueChanged?.Invoke(0, currentValue, maxValue);
    }

    public bool CanDamage()
    {
        return currentValue > 0;
    }
}
