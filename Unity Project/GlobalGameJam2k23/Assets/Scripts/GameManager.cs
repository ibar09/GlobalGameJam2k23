using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using Cinemachine;
using TMPro;

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
    private Player immunePlayer;
    public int immunityCD, downBoostCD;
    private bool isThereImmunity;
    private bool isThereDownBoost;
    public int width, height;
    public Tilemap map;
    public GameObject playerPrefab;
    public TileBase dirtTile;
    public TileBase blockTile;
    public TileBase waterTile;
    private Vector2 center;
    public BoxCollider2D waterDetector;





    public void Start()
    {
        generateMap(2);
    }

    public void generateMap(int N)

    {
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
            playerPosition += new Vector3(0.5f, 0, 0);
            players[i].gameManager = this;
        }
        currentPlayer = players[0];
        waterDetector.transform.position = new Vector2(center.x, center.y - (float)height / 2 - 2.5f);
        waterDetector.size = new Vector2(width, 5);
        mapCamera.transform.position = new Vector3(center.x, center.y, -20);
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
        if (isThereImmunity)
        {
            immunityCD++;
            if (immunityCD == 3)
            {
                immunityCD = 0;
                DisactivateImmuneStatus(immunePlayer);

            }
            if (isThereDownBoost)
            {
                downBoostCD++;
                if (downBoostCD == 6)
                {
                    immunityCD = 0;
                    isThereDownBoost = false;
                    Debug.Log("DownBoost given");
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

    public void GiveImmuneStatus(Player player)
    {
        Debug.Log(player.transform);
        player.GetComponent<EdgeCollider2D>().enabled = false;
        immunePlayer = player;
        isThereImmunity = true;

    }
    public void DisactivateImmuneStatus(Player player)
    {
        player.GetComponent<EdgeCollider2D>().enabled = true;
        immunePlayer = null;
        isThereImmunity = false;
        Debug.Log("immunity taken");
    }
    public void GiveDownBoostStatus(Player player)
    {
        isThereDownBoost = true;
        player.x = 0.45f;
    }




}
