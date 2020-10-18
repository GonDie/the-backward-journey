using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    bool _triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_triggered)
            return;

        if (collision.CompareTag("Player"))
        {
            _triggered = true;
            GameManager.Instance.TriggerLevelEnd();
        }
    }
}
