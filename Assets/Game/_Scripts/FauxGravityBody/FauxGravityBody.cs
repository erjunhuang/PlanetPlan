using System;
using UnityEngine;

public class FauxGravityBody : Targetable
{
    public FauxGravityBodyLevel[] levels;
    public int currentLevel { get; protected set; }
    public FauxGravityBodyLevel currentFauxGravityBodyLevel { get; protected set; }
    public bool isAtMaxLevel
    {
        get { return currentLevel == levels.Length - 1; }
    }
    public FauxGravityBodyGhost towerGhostPrefab
    {
        get { return levels[currentLevel].towerGhostPrefab; }
    }

    public int purchaseCost
    {
        get { return levels[0].cost; }
    }

    public string upgradeDescription
    {
        get { return levels[0].upgradeDescription; }
    }

    public Action<FauxGravityBody> removed;

    protected override void Start()
	{
        base.Start();
        Initialize();
    }

    public virtual void Initialize()
    {
        SetLevel(0);
    }

    public virtual bool UpgradeTower() {
        if (isAtMaxLevel)
        {
            return false;
        }
        SetLevel(currentLevel + 1);
        return true;
    }

    public void Sell()
    {
        Remove();
    }

    public void Remove()
    {
        if (removed != null)
        {
            removed(this);
        }
        Destroy(gameObject);
    }


    protected void SetLevel(int level)
    {
        if (level < 0 || level >= levels.Length)
        {
            return;
        }
        currentLevel = level;
        if (currentFauxGravityBodyLevel != null)
        {
            Destroy(currentFauxGravityBodyLevel.gameObject);
        }
        currentFauxGravityBodyLevel = Instantiate(levels[currentLevel], transform);
    }



}
