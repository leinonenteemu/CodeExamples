using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupState { Normal, Buffed }
public abstract class PowerupBaseSO : ScriptableObject
{
    public GameObject player { get; private set; }
    public PlayerStatHandler statHandler { get; private set; }
    public PlayerHealth playerHealth { get; private set; }
    public Combo combo { get; private set; }
    [SerializeField] private float duration = 10;
    private float timer;
    
    public Sprite ActiveIcon { get; private set; }
    [field: SerializeField] public Sprite NormalIcon { get; private set; }
    [SerializeField] private Sprite BuffedIcon;
    private PlayerPowerup playerPowerup;
    
    public PowerupState currentState { get; private set; }

    protected bool isActivated;

    public void Initialize(GameObject player)
    {
        this.player = player;
        statHandler = player.GetComponent<PlayerStatHandler>();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerPowerup = player.GetComponent<PlayerPowerup>();
        combo = FindObjectOfType<Combo>();
        timer = duration;
        isActivated = false;
    }

    private void OnEnable()
    {
        currentState = PowerupState.Normal;
        ActiveIcon = NormalIcon;
        timer = duration;
    }

    public void Buff()
    {
        if (currentState == PowerupState.Normal) 
        {
            currentState = PowerupState.Buffed;
            ActiveIcon = BuffedIcon;
        }
    }

    public void SetNormalState()
    {
        currentState = PowerupState.Normal;
        ActiveIcon = NormalIcon;
    }

    public void UsePowerup()
    {
        switch (currentState)
        {
            case PowerupState.Normal: NormalPowerup();
                break;

            case PowerupState.Buffed: BuffedPowerup();
                break;
        }

        timer -= Time.deltaTime;

        if (timer <= 0) 
            { 
                EndPowerup();
                playerPowerup.RemovePowerup(this);
            }
    }

    protected abstract void NormalPowerup();
    protected abstract void BuffedPowerup();

    public abstract void EndPowerup();


}
