using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Head"))
        {
            Debug.Log("Check B");
            gameManager.DisactivateControls();
            foreach (Player player in gameManager.players)
            {
                player.enabled = false;
            }
            gameManager.winPopUp.SetActive(true);
        }
    }
}
