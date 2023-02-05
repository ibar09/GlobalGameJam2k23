using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmuneConsumable : Consumable
{
    public override void DoEffect(GameObject player)
    {
        FindObjectOfType<GameManager>().GiveImmuneStatus(player.transform.parent.GetComponent<Player>());
        Debug.Log("Immunity given");
    }
}
