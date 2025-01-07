using System.Collections.Generic;
using UnityEngine;
public class BulletHellSpawner : MonoBehaviour
{
    //INSTRUCTIONS
    //Create parent objects to hold the spawn locations for the projectiles.
    //Create parent objects to hold the sprites you want to enable to telegraph to your players that an attack is incoming.
    //Add them to the corresponding fields in the attackPatternsArray.
    //The functions you will need to call are "AddToCue" and "RunQueue", in that order.
    //"AddToCue" when you wish to telegraph your attack.
    //"RunQueue" when you wish to execute your attack.

    //EXPLANATION
    //The reason im using this parent structure is because it helps keep the hierarchy more organized and the script more flexible.
    //Allowing you to simple switch a single parent object instead of dozens of individual transforms and spriteRenderers.

    [Header("Required References")]
    [Tooltip("The gameObject to spawn.")]
    [SerializeField] private GameObject projectile;
    [Tooltip("A list of where to spawn the projectiles")]
    [SerializeField] private AttackPattern[] attackPatterns;
    [Header("Optional Settings")]
    [Tooltip("The parent to spawn the projectiles under in the hierarchy.")]
    [SerializeField] private Transform parent;
    [Tooltip("If you don't want repeating attacks, leave as false.")]
    [SerializeField] private bool repeats = false;

    //Records the last Attack Pattern spawned in order to prevent repeats
    private int lastIndex;
    private List<AttackPattern> queue = new();

    #region Public Methods
    public void AddToQueue()
    {
        //Fixes a spawning error!!!
        //More testing is needed to add functionality for spawning multiple attack patterns at once.
        if (queue.Count >= 1)
        {
            return;
        }

        lastIndex = ReturnNewNumber(0, attackPatterns.Length, lastIndex);
        queue.Add(attackPatterns[lastIndex]);
        ShowTelegraphLinesUsingParents(queue[queue.Count - 1]);
    }

    public void RunQueue()
    {
        Spawn(queue[queue.Count - 1]);
        queue.RemoveAt(queue.Count - 1);
    }
    #endregion

    #region Private Methods (You can poke around if you like, but not neccesary ;) )
    private int ReturnNewNumber(int minValue, int maxValue, int lastValue)
    {
        int randomValue = Random.Range(minValue, maxValue);
        while (repeats == false && randomValue == lastValue)
        {
            randomValue = Random.Range(minValue, maxValue);
        }
        return randomValue;
    }

    private void Spawn(AttackPattern attackPattern)
    {
        foreach (GameObject parent in attackPattern.spawnParents)
        {
            List<Transform> spawnPoints = new(parent.GetComponentsInChildren<Transform>());
            //Removes the transform of the parent
            spawnPoints.RemoveAt(0);
            foreach (Transform transform in spawnPoints)
            {
                Instantiate(projectile, transform.position, transform.rotation, this.parent);
            }
        }
        HideTelegraph(attackPattern);
    }

    private void ShowTelegraph(AttackPattern attackPattern)
    {
        foreach (GameObject parent in attackPattern.telegraphParents)
        {
            SpriteRenderer[] spriteChildren = parent.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sprite in spriteChildren)
            {
                sprite.enabled = true;
            }
        }
    }


    private void HideTelegraph(AttackPattern attackPattern)
    {
        foreach (GameObject parent in attackPattern.telegraphParents)
        {
            SpriteRenderer[] spriteChildren = parent.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sprite in spriteChildren)
            {
                sprite.enabled = false;
            }
        }
    }
    #endregion
}

[System.Serializable]
public class AttackPattern
{
    [Tooltip("Name of the Attack Pattern (Used only for your own convenience, has no impact on the code)")]
    public string name;
    [Tooltip("A list of parent object(s) that have sprites as children (Used as telegraphers for the projectiles)")]
    public GameObject[] telegraphParents;
    [Tooltip("A list of parent object(s) that have transform components in the children (Used as spawn location for the projectiles)")]
    public GameObject[] spawnParents;
}
