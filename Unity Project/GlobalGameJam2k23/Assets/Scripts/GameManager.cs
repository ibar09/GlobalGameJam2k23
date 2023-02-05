using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using Cinemachine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Player currentPlayer;
    public GameObject focusedCamera, mapCamera;
    public List<Player> players;
    public CinemachineVirtualCamera cam;
    public float animationSpeed;
    public float repetitionTimes;
    public TextMeshProUGUI directionText;
    public GameObject winPopUp;
    public GameObject buttons;
    public int width, height;
    public Tilemap map;
    public GameObject playerPrefab;
    public TileBase dirtTile;
    public TileBase blockTile;
    public TileBase waterTile;
    private Vector2 center;
    public BoxCollider2D waterDetector;
    public SpriteShape[] playerSpriteShapes;
    public int consumableCount;
    public GameObject[] consumablePrefabs;
    public GameObject[] treePrefabs;
    public GameObject gameUI;
    public GameObject Menu;


    /* private void Awake()
     {
         GameManager[] objs = GameObject.FindObjectsOfType<GameManager>();
         if (objs.Length > 1)
         {
             Destroy(this.gameObject);
         }

         DontDestroyOnLoad(this.gameObject);
     }*/
    private void Start()
    {
        //generateMap(2);
    }
    public void setDimensionsByPlayerCount(int N)
    {
        switch (N)
        {
            case 2:
                width = 11;
                height = 11;
                consumableCount = 8;
                break;
            case 3:
                width = 15;
                height = 15;
                consumableCount = 14;
                break;
            case 4:
                width = 19;
                height = 19;
                consumableCount = 20;
                break;
        }
    }
    public void generateMap(int N)

    {
        // SceneManager.LoadScene(1);

        Debug.Log(N);
        gameUI.SetActive(true);
        Menu.SetActive(false);
        setDimensionsByPlayerCount(N);
        int blockWidth = 30;
        TileBase[] tileArray = new TileBase[(width + 2 * blockWidth) * (height + 10)];
        for (int index = 0; index < tileArray.Length; index++)
        {
            int i = index % (width + 2 * blockWidth);
            int j = index / (width + 2 * blockWidth);
            if (i < blockWidth || i >= width + blockWidth)
            {
                tileArray[index] = blockTile;
            }
            else if (j < 10)
            {
                tileArray[index] = waterTile;
            }
            else
            {
                tileArray[index] = dirtTile;
            }
        }
        map.SetTilesBlock(new BoundsInt(0, -(height + 10), 0, width + 2 * blockWidth, height + 10, 1), tileArray);
        center = new Vector3(blockWidth + ((float)width / 2), -(float)height / 2, 0);
        players = new List<Player>();
        var playerPosition = new Vector3(center.x - (float)width / 2, 0.5f, 0);
        for (int i = 0; i < N; i++)
        {
            players.Add(Instantiate(playerPrefab).GetComponent<Player>());
            playerPosition += new Vector3((float)(width - N) / (N + 1) + 0.5f, 0, 0);
            players[i].transform.position = playerPosition;
            var tree = Instantiate(treePrefabs[i % treePrefabs.Length]);
            tree.transform.position = playerPosition + new Vector3(0, 0.04f, 0);
            playerPosition += new Vector3(0.5f, 0, 0);
            players[i].gameManager = this;
            players[i].GetComponent<SpriteShapeController>().spriteShape = playerSpriteShapes[i % playerSpriteShapes.Length];
        }
        currentPlayer = players[0];
        for (int i = 0; i < consumableCount; i++)
        {
            var consumable = Instantiate(consumablePrefabs[UnityEngine.Random.Range(0, consumablePrefabs.Length)]);
            consumable.transform.position = new Vector3(MathF.Floor(UnityEngine.Random.Range(MathF.Round(center.x - (float)width / 2 + 1), MathF.Round(center.x + (float)width / 2 + 0.1f))) - 0.5f, MathF.Floor(UnityEngine.Random.Range(MathF.Round(center.y - (float)height / 2 - 0.1f), MathF.Round(center.y + (float)height / 2 - 3))) + 0.5f, 0);
        }
        waterDetector.transform.position = new Vector2(center.x, center.y - (float)height / 2 - 2.5f);
        waterDetector.size = new Vector2(width, 5);
        mapCamera.transform.position = new Vector3(center.x, center.y + 1, -20);
        mapCamera.GetComponent<Camera>().orthographicSize = (float)height / 2 + 3;
        cam.Follow = currentPlayer.headTransform;
    }
    public Vector2 GetCenter()
    {
        return center;
    }
    public void NextTurn()
    {
        currentPlayer = players[(players.IndexOf(currentPlayer) + 1) % players.Count];
        cam.Follow = currentPlayer.headTransform;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].isImmune)
            {
                players[i].immunityCD++;
                if (players[i].immunityCD == 3)
                {
                    players[i].immunityCD = 0;
                    players[i].isImmune = false;
                }
            }
            if (players[i].isDownBoosted)
            {
                players[i].downBoostCD++;
                if (players[i].downBoostCD == 6)
                {
                    players[i].downBoostCD = 0;
                    players[i].isDownBoosted = false;
                    players[i].x = 0;
                }
            }
        }
    }


    public void Move(int steps)
    {
        currentPlayer.Move(steps);

    }
    public void SwitchCamera()
    {
        focusedCamera.SetActive(!focusedCamera.activeSelf);
        mapCamera.SetActive(!mapCamera.activeSelf);
    }
    public void DisactivateControls()
    {
        foreach (Transform button in buttons.transform)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }
    public void ActivateControls()
    {
        foreach (Transform button in buttons.transform)
        {
            button.GetComponent<Button>().interactable = true;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }



}
