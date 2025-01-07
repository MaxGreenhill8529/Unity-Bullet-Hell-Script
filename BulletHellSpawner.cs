using System.Collections.Generic;
using UnityEngine;
public class BulletHellSpawner : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private AttackPattern[] attackPatterns;
    [Header("Optional Settings")]
    [SerializeField] private Transform parent;



    private List<AttackPattern> queue = new();
    private int lastIndex;
    public void AddToQueue()
    {
        if (queue.Count >= 1)
        {
            return;
        }
        lastIndex = ReturnNewNumber(0, attackPatterns.Length, lastIndex);
        queue.Add(attackPatterns[lastIndex]);
        ShowTelegraphLinesUsingParents(queue[queue.Count - 1]);
    }

    public int ReturnNewNumber(int minValue, int maxValue, int lastValue)
    {
        int randomValue = Random.Range(minValue, maxValue);
        while (randomValue == lastValue)
        {
            randomValue = Random.Range(minValue, maxValue);
        }
        return randomValue;
    }


    public void RunQueue()
    {
        Spawn(queue[queue.Count - 1]);
        queue.RemoveAt(queue.Count - 1);
    }

    public void Spawn(AttackPattern attackPattern)
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
        HideTelegraphLinesUsingParents(attackPattern);
    }

    public void ShowTelegraphLinesUsingParents(AttackPattern attackPattern)
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


    public void HideTelegraphLinesUsingParents(AttackPattern attackPattern)
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
}
[System.Serializable]
public class AttackPattern
{
    public string name;
    public GameObject[] telegraphParents;
    public GameObject[] spawnParents;
}
