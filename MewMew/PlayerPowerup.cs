using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPowerup : MonoBehaviour
{
    [SerializeField] WaveManager waveManager;
    public PowerupBaseSO currentPowerup { get; private set; }
    [SerializeField] List<PowerupBaseSO> activePowerups = new List<PowerupBaseSO>();

    [SerializeField] Combo comboSystem;

    public delegate void UpdatePowerup();
    public UpdatePowerup updatePowerup;
    [SerializeField] GameObject powerupEffect;

    

    #region InputSystem
    PlayerInputActions playerInputActions;
    InputAction usePowerup;
    #endregion

    #region Event Functions
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        usePowerup = playerInputActions.Gameplay.Powerup;
        usePowerup.Enable();
        playerInputActions.Gameplay.Powerup.performed += UsePowerup;
        comboSystem.fullCombo += UpgradePowerup;
    }

    private void OnDisable()
    {
        usePowerup.Disable();
        playerInputActions.Gameplay.Powerup.Disable();
        comboSystem.fullCombo -= UpgradePowerup;
    }

    // Update is called once per frame
    void Update()
    {
        PowerupTimer();
        
    }


    #endregion

    private void UsePowerup(InputAction.CallbackContext obj)
    {
        if (currentPowerup != null) 
        {
            activePowerups.Add(currentPowerup);
            currentPowerup = null;
            updatePowerup?.Invoke();
        }
    }


    public void GivePowerup(PowerupBaseSO powerup)
    {
        powerup.SetNormalState();
        currentPowerup = powerup;
        currentPowerup.Initialize(gameObject);
        if (comboSystem.isFullCombo()) UpgradePowerup();
        updatePowerup?.Invoke();
    }

    public void UpgradePowerup()
    {
        if (currentPowerup != null && currentPowerup.currentState == PowerupState.Normal) 
        {
            currentPowerup.Buff();
            updatePowerup?.Invoke();
        } 
    }

    void PowerupTimer()
    {
        if (activePowerups.Count > 0)
        {
            comboSystem.FreezeCounter();
            UsePowerups();
            powerupEffect.SetActive(true);
        }

        else 
        {
            powerupEffect.SetActive(false);
            if (waveManager.waveActive) 
            {  
                comboSystem.ContinueCounter();
            }
        } 
    }

    void UsePowerups()
    {
        for (int i = 0; i < activePowerups.Count; i++)
        {
            activePowerups[i].UsePowerup();
        }
    }

    public void RemovePowerup(PowerupBaseSO powerup)
    {
        activePowerups.Remove(powerup);
    }
}
