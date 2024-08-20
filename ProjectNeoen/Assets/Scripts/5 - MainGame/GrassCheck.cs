using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.State != GameState.Futureville) return;
        if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<ClickController>().OnGrass = true;
            collision.GetComponent <ClickController>().PlayFootstepAudio();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<ClickController>().OnGrass = false;
            collision.GetComponent<ClickController>().PlayFootstepAudio();

        }
    }
}
