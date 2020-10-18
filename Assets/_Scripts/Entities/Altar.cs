using System.Collections;
using UnityEngine;

public class Altar : MonoBehaviour
{
    static Coroutine _endLevelCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_endLevelCoroutine == null)
                _endLevelCoroutine = StartCoroutine(EndLevelCoroutine());
        }
    }

    IEnumerator EndLevelCoroutine()
    {
        GameManager.Instance.TriggerLevelEnd();
        yield return null;

        _endLevelCoroutine = null;
    }
}
