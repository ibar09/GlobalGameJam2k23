using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmuneConsumable : Consumable
{
    public override void DoEffect(GameObject head)
    {
        var player = head.transform.parent.GetComponent<Player>();
        player.isImmune = true;
        player.immunityCD = 0;
        Debug.Log("Immunity given");
    }
}