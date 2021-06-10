using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameState == GameManager.GameState.Running)
        {
            transform.position -= new Vector3(0, .003f, 0);
        }
    }
}
