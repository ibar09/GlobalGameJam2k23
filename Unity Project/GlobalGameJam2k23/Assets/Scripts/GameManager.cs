using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player currentPlayer;
    public int playersNumber;
    public List<Player> players;

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
    }

    /*
    public bool isMyTurn(Player player)
    {
        return currentPlayer == player;
    }*/

    public void Move(int steps)
    {
        currentPlayer.Move(steps);
        NextTurn();
    }


}
