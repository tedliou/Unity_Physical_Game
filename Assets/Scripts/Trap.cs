using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isDeadNow)
        {
            GameManager.Instance.StopGame();
        }
        else
        {
            GameManager.Instance.ReduceHP(this);
        }
    }

    public GameObject player;
    public bool isCounted;
    public bool isDeadNow;

    private void Update()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (!player)
            {
                return;
            }
        }
        if (!isCounted && player.transform.position.y < transform.position.y)
        {
            GameManager.Instance.AddScore();
            isCounted = true;
        }

        if (Camera.main.transform.position.y - transform.position.y < -6)
        {
            TrapSpawner.Instance.traps.Remove(gameObject);
            TrapSpawner.Instance.SpawnTrap();
            Destroy(gameObject);
        }
    }
}
