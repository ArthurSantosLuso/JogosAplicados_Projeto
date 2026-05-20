using System;
using UnityEngine;

public class EnemyHealth : ValueBase, IDamageable
{
    public Action<float, float> OnValueChangedEnemy;

    private void Start()
    {
        /// The way the system is now, the health UI value is just shown when the UI is notified that something changed.
        /// So in order to initialize the UI, I call this method add the health to the enemy.
        AddHP(maxValue);
        GameManager.Instance.CurrentEnemy = gameObject;
        //GameManager.Instance.
    }

    // Reduce enemy hp
    public void Damage(float damageValue)
    {
        base.ReduceValue(damageValue);
        OnValueChangedEnemy?.Invoke(currentValue, maxValue);
        VerifyLife();
    }

    // Check if enemy died
    private void VerifyLife()
    {
        if (currentValue <= 0)
        {
            KillEnemy();
        }
    }

    // Trigger enemy death 
    private void KillEnemy()
    {
        // Death logic
        GameManager.Instance.CurrentEnemy = null;
        gameObject.SetActive(false);
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
