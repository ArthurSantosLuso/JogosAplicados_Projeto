using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class PlayerStats : ValueBase, IDamageable
{
    private int _potions = 0;
    public int Potions { get { return _potions; } }
    private float _attackDamage = 100f;
    public float AttackDamage { get { return _attackDamage; } }
    private void Start()
    {
        /// The way the system is now, the health UI value is just shown when the UI is notified that something changed.
        /// So in order to initialize the UI, I call this method add the health to the player.
        AddHP(maxValue);

        float saved = GameManager.Instance.SavedPlayerHealth;
        if (saved > 0)
        {
            currentValue = saved;
            
        }
    }

    // Reduce player hp
    public void Damage(float damageValue)
    {
        base.ReduceValue(damageValue);
        OnValueChanged?.Invoke(0, currentValue, maxValue);
        VerifyLife();
    }

    // Check if player died
    private void VerifyLife()
    {
        if (currentValue <= 0)
        {
            KillPlayer();
        }
    }

    // Trigger player death 
    private void KillPlayer()
    {
        // Death logic
        //GameManager.Instance.DisplayDeathScreen();
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
    public void AddPotions(int amount)
    {
        _potions += amount;
    }
    public void AddDamage(float amount)
    {
        _attackDamage += amount;
    }
}
