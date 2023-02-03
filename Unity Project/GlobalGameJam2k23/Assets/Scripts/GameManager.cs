using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public Player currentPlayer;
    public GameObject focusedCamera, mapCmaera;
    public int playersNumber;
    public List<Player> players;
    public CinemachineVirtualCamera cam;




    public void generateMap(int N)

    {
        //generate l map hasb aadad l players w yhot kol player f blastou
        playersNumber = N;
        currentPlayer = players[0];
    }
    public void NextTurn()
    {
        if (players.IndexOf(currentPlayer) != playersNumber - 1)
            currentPlayer = players[players.IndexOf(currentPlayer) + 1];
        else
            currentPlayer = players[0];
        cam.Follow = currentPlayer.transform;
    }


    public void Move(int steps)
    {
        currentPlayer.Move(steps);
        NextTurn();
    }
    public void SwitchCamera()
    {
        focusedCamera.SetActive(!focusedCamera.activeSelf);
        mapCmaera.SetActive(!mapCmaera.activeSelf);
    }


}
