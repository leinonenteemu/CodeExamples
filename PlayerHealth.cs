using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    int maxHealth = 9;
    [SerializeField] public int currentHealth { get; private set; }

    public delegate void OnHealthChange();
    public OnHealthChange healthChange;

    public delegate void DamageTaken();
    public DamageTaken damageTaken;

    [HideInInspector] public bool invulnerable;
    [HideInInspector] public bool deflecting;

    private PlayerStatHandler statHandler;
    [SerializeField] GameEvent deathEvent;
    bool hasDied;
    [SerializeField] float tempShieldAfterDamageTaken = 0.5f;

    private void Start()
    {
        statHandler = GetComponent<PlayerStatHandler>();
        maxHealth = statHandler.maxHealth;
        currentHealth = maxHealth;
        hasDied = false;
        healthChange?.Invoke();
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void TakeDamage(int damage, GameObject damageGiver)
    {
        if (deflecting)
        {
            IDamageable damageable;
            if (damageGiver.TryGetComponent<IDamageable>(out damageable)) damageable.TakeDamage(statHandler.ReturnDamage());
        }

        if (invulnerable) return;

        currentHealth -= damage;
        damageTaken?.Invoke();
        StartCoroutine(TemporaryInvulnerability());
        if (currentHealth <= 0 && !hasDied) 
        {
            deathEvent.Raise();
            hasDied = true;
        }

    }
    private IEnumerator TemporaryInvulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(tempShieldAfterDamageTaken);
        invulnerable = false;
    }
}
