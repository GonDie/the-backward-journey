using UnityEngine;

public class EndScreen : MonoBehaviour
{
    void Start()
    {
        FadeManager.Instance.Fade(false);
    }
}
