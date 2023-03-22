using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyManager : MonoBehaviour
{
    public Cover[] cover;
    public List<Enemy_AI> enemys;
    public List<Zombie_AI> zombies;

    private bool spawnedEnemies = false;

    public void SpawnEnemys()
    {
        if (spawnedEnemies)
            return;

        for (int i = 0; i < enemys.Count; i++)
        {
            Enemy_AI tempEnemy = enemys[i];

            tempEnemy.gameObject.SetActive(true);

            if (i < cover.Length)
            {
                cover[i].occupant = enemys[i].gameObject;
                tempEnemy.m_cover = cover[i].transform;
                tempEnemy.SetBehaviorType(BehaviorType.DEFENDER);
            }
            else
            {
                tempEnemy.SetBehaviorType(BehaviorType.ATTACKER);
            }
        }

        for (int i = 0; i < zombies.Count; i++)
        {
            Zombie_AI tempEnemy = zombies[i];
            tempEnemy.gameObject.SetActive(true);
        }

        spawnedEnemies = true;
    }

    public void PlayerInsideTheRoom()
    {
        foreach (Enemy_AI enemy in enemys)
        {
            enemy.playerInRoon = true;
        }
    }  

    public void PlayerOutOfRoom()
    {
        foreach (Enemy_AI enemy in enemys)
        {
            enemy.playerInRoon = false;
        }
    }
}
