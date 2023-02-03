using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Directon
    {
        UP, DOWN, LEFT, RIGHT
    }
    [SerializeField]
    private float x = 0f;
    Directon directon;
    public void DirectionRng()
    {
        float downWeight = 0.25f + x;
        float percentage = Random.Range(0, 1f);
        Debug.Log(percentage);
        if (percentage <= downWeight)
        {
            x = 0;
            directon = Directon.DOWN;
        }
        else
        {
            x += 0.05f;
            int newRng = Random.Range(1, 4);
            Debug.Log(newRng);
            switch (newRng)
            {
                case 1:
                    directon = Directon.UP;
                    break;
                case 2:
                    directon = Directon.LEFT;
                    break;
                case 3:
                    directon = Directon.RIGHT;
                    break;
            }
        }
    }

    public GameManager gameManager;
    public int id;
    private void Start()
    {
        gameManager.players.Add(this);
    }
    public void Move(int steps)
    {
        DirectionRng();
        Debug.Log("Player " + id + " has moved " + steps + " to " + directon);
    }


}
