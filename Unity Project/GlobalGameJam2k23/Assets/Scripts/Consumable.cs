using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : MonoBehaviour
{
    /*
    private Vector3 currentPos;
    public float animationSpeed = 0.1f;
    private void Start()
    {
        currentPos = transform.position;
    }
    private void Update()
    {

        transform.position += new Vector3(transform.position.x, animationSpeed);
        if (transform.position.y >= currentPos.y + 0.5f || transform.position.y <= currentPos.y - 0.5f)
            animationSpeed = -animationSpeed;
    }*/
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
