using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.GameOver)
        {
            Destroy(gameObject);
        }
    }
}
