using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gameManager;
    public int id;
    private void Start()
    {
        gameManager.players.Add(this);
    }
    public void Move(int steps)
    {
        Debug.Log("Player " + id + " has moved " + steps);
    }
}
