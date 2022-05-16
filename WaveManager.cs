using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public int CurrentWave { get; private set; }
    [SerializeField][Range(0,1)] float enemiesIncreasedPerWave;
    public int EnemiesInThisWave { get; private set; }
    [SerializeField] private int enemiesInFirstWave = 15;

    [SerializeField] GameEvent startWave;
    [SerializeField] GameEvent endWave;

    EnemyManager enemyManager;
    [SerializeField] float nextWaveTimer;
    float timer;

    [SerializeField] GameEvent itemDropEvent;
    [SerializeField] int roundsBetweenItemDrops;



    public bool waveActive { get; private set; }

    bool selectingItem;

    bool dropItemAfterThisWave;

    bool endWaveCalled;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        timer = nextWaveTimer;
    }

    public void NextWave()
    {
        CurrentWave++;
        if (CurrentWave == 1) EnemiesInThisWave = enemiesInFirstWave;
        else EnemiesInThisWave += Mathf.RoundToInt(EnemiesInThisWave * enemiesIncreasedPerWave);
        waveActive = true;
        timer = nextWaveTimer;
    }


    public void Update()
    {
        if (enemyManager.AllEnemiesKilled()) 
        {
            if (!endWaveCalled) 
            {
                endWave.Raise();
                endWaveCalled = true;
            }
        }
        TimerForNextWave();
    }

    public void ActivateItemEvent()
    {
        if (dropItemAfterThisWave) 
        {
            itemDropEvent.Raise();
            selectingItem = true;
            dropItemAfterThisWave = false;
        }
    }

    public void ItemSelected()
    {
        selectingItem = false;
    }

    public void CheckForItemWave()
    {
        if (CurrentWave % roundsBetweenItemDrops == 0 && !enemyManager.maxDifficulty) dropItemAfterThisWave = true;
    }

    public void DeactiveWave()
    {
        waveActive = false;
    }

    public void TimerForNextWave()
    {
        if (!waveActive)
        {
            if (selectingItem) return;

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                startWave.Raise();
                endWaveCalled = false;
            }
        }
    }



}
