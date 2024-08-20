using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLocation : MonoBehaviour
{
    [SerializeField] private string _location;
    [SerializeField] private Color _backgroundColour;
    [SerializeField] private Vector2 _size;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.State != GameState.Futureville) return;
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<DisplayLocation>().DisplayLocationText(_location, _backgroundColour, _size);
        }
    }
}
