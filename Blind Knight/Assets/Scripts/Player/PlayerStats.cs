
public class PlayerStats : ValueBase, IDamageable
{
    private int _potions = 0;
    public int Potions { get { return _potions; } }
    private float _attackDamage = 50f;
    public float AttackDamage { get { return _attackDamage; } }

    private void Start()
    {
        AddHP(maxValue);

        float saved = GameManager.Instance.SavedPlayerHealth;
        if (saved > 0)
        {
            currentValue = saved;
            OnValueChanged?.Invoke(0, currentValue, maxValue);
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
