using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : MonoBehaviour
{
    public abstract void DoEffect(GameObject player);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Head"))
        {
            DoEffect(other.gameObject);
            Destroy(gameObject);
        }
    }
}
