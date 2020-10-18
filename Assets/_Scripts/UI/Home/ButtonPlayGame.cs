using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonPlayGame : MonoBehaviour
{
    private void Start()
    {
        FadeManager.Instance.Fade(false);
    }

    public void PlayGame()
    {
        FadeManager.Instance.Fade(true, 0f, () => SceneManager.LoadScene("Main"));
    }
}