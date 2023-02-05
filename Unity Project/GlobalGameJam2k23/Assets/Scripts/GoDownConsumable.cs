using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoDownConsumable : Consumable
{
    public int distance = 1;

    public override void DoEffect(GameObject other)
    {
        var player = other.gameObject.transform.parent.gameObject.GetComponent<Player>();
        player.SetPosition(transform.position);
        player.MoveBy(distance, Player.Direction.DOWN);
    }
}