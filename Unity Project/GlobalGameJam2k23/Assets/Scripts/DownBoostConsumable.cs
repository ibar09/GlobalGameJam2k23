using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownBoostConsumable : Consumable
{
    public override void DoEffect(GameObject head)
    {
        var player = head.transform.parent.GetComponent<Player>();
        player.x = 0.45f;
        player.isDownBoosted = true;
        player.downBoostCD = 0;
        Debug.Log("DownBoost given");
    }
}
