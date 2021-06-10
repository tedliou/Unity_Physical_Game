using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Line Renderer")] public GameObject lineRendrerPrefab;
    public float drawDistance;
    public EdgeCollider2D edgeColliderPrefab;
    
    [Header("Player")]
    public GameObject playerPrefab;
    public Transform spawnPosition;
    
    [Header("State")]
    public GameState gameState = GameState.Idle;
    public enum GameState
    {
        Idle = 0,
        Running = 1,
        Pause = 2,
        GameOver = 3
    }

    [Header("UI")] public GameObject gameOverPanel;
    public Text scoreText, hpText;
    public Text scoreResult, hightestScore;

    [HideInInspector] public GameObject playerObject;
    private LineRenderer _lineRenderer;
    private List<DrawData> _drawDatas;
    private int score, hp;
    private readonly string key = "Score";
    private struct DrawData
    {
        public LineRenderer lineRenderer;
        public EdgeCollider2D edgeCollider2D;

        public void DestroySelf(float duration = 0)
        {
            Destroy(lineRenderer.gameObject, duration);
            Destroy(edgeCollider2D.gameObject, duration);
        }
    }

    private void Awake()
    {
        Instance = this;
        ResetGame();
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, 0);
            PlayerPrefs.Save();
        }
    }

    public void StartGame()
    {
        gameState = GameState.Running;
        playerObject = Instantiate(playerPrefab, spawnPosition.position, Quaternion.identity);
        _drawDatas = new List<DrawData>();
        ResetScore();
        for (var i = 0; i < 10; i++)
        {
            TrapSpawner.Instance.SpawnTrap();
        }
    }

    public void StopGame()
    {
        gameState = GameState.GameOver;
        gameOverPanel.SetActive(true);

        if (PlayerPrefs.GetInt(key) < score)
        {
            PlayerPrefs.SetInt(key, score);
            PlayerPrefs.Save();
        }
        scoreResult.text = $"分數：{score}";
        hightestScore.text = $"歷史最高：{PlayerPrefs.GetInt(key)}";
    }

    public void ResetGame()
    {
        var main = Camera.main;
        var camPos = main.transform.position;
        camPos.y = 0;
        main.transform.position = camPos;
        ResetScore();
        foreach (Transform e in TrapSpawner.Instance.transform)
        {
            Destroy(e.gameObject);
        }
    }

    public void AddScore()
    {
        score += 1;
        UpdateScore();
    }

    public void ResetScore()
    {
        score = 0;
        hp = 3;
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreText.text = $"分數：{score}";
        hpText.text = $"生命：{hp}";
    }

    public void ReduceHP(Trap target = null)
    {
        hp -= 1;
        if (hp <= 0)
        {
            StopGame();
        }
        UpdateScore();
        //if (target) Destroy(target);
    }

    private void Update()
    {
        if (gameState == GameState.Running)
        {
            // Try Get Mouse Position
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
            // Spawn Object
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var obj = Instantiate(lineRendrerPrefab, mousePos, Quaternion.identity);
                var render = obj.GetComponent<LineRenderer>();
                render.positionCount++;
                render.SetPosition(0, mousePos);

                var collider = Instantiate(edgeColliderPrefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<EdgeCollider2D>();
                var pointList = new List<Vector2> {mousePos};
                collider.SetPoints(pointList);
                    
                _drawDatas.Add(new DrawData()
                {
                    lineRenderer = render,
                    edgeCollider2D = collider
                });
            }
            
            if (Input.GetKey(KeyCode.Mouse0) && _drawDatas.Count > 0)
            {
                // Set Line Renderer Position
                var data = _drawDatas[_drawDatas.Count - 1];
                var render = data.lineRenderer;
                var currentIndex = render.positionCount;
                var lastDistance = Vector2.Distance(mousePos, render.GetPosition(currentIndex - 1));
                if (lastDistance >= drawDistance)
                {
                    render.positionCount++;
                    render.SetPosition(currentIndex, mousePos);

                    var pointList = new Vector3[render.positionCount];
                    render.GetPositions(pointList);
                    var colliderPoints = new List<Vector2>();
                        
                    for (var i = 0; i < pointList.Length; i++)
                    {
                        Vector2 v2Point = pointList[i];
                        colliderPoints.Add(v2Point);
                    }

                    data.edgeCollider2D.SetPoints(colliderPoints);
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && _drawDatas.Count > 0)
            {
                _drawDatas[_drawDatas.Count - 1].DestroySelf(2);
                _drawDatas.RemoveAt(_drawDatas.Count - 1);
            }
        }
    }
}
