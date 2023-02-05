using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownBoostConsumable : Consumable
{
    public override void DoEffect(GameObject player)
    {
        FindObjectOfType<GameManager>().GiveDownBoostStatus(player.transform.parent.GetComponent<Player>());
        Debug.Log("DownBoost given");
    }
}
