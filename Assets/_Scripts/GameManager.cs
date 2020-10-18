using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public PlayerController player;
    public GameObject[] levelPrefabs;
    public CinemachineConfiner cameraConfiner;
    public CanvasGroup gameOverCanvasGroup;

    int _currentLevelIndex;
    Level _currentLevel;

    private void Start()
    {
        CreateNextLevel();
    }

    public void CreateNextLevel()
    {
        player.SetPlayerState(PlayerController.PlayerState.Idle);
        _currentLevel = Instantiate(levelPrefabs[_currentLevelIndex]).GetComponent<Level>();
        _currentLevelIndex++;

        cameraConfiner.m_BoundingShape2D = _currentLevel.GetComponent<PolygonCollider2D>();

        player.Transform.position = _currentLevel.playerSpawn.position;
        FadeManager.Instance.Fade(false, 0f, () => Events.OnLevelStart?.Invoke());
    }

    public void TriggerLevelEnd()
    {
        Events.OnLevelEnd?.Invoke();
    }

    public void GoToNextLevel()
    {
        player.soundController.PlayTeleport();
        player.SetPlayerState(PlayerController.PlayerState.Teleporting);
        player.Transform.DOMoveY(player.Transform.position.y + 2f, 1f).Play();

        FadeManager.Instance.Fade(true, 1f, () =>
        {
            if (_currentLevelIndex >= levelPrefabs.Length - 1)
            {
                FadeManager.Instance.Fade(true, 0f, () => SceneManager.LoadScene("End"));
            }
            else
            {
                Destroy(_currentLevel.gameObject);
                CreateNextLevel();
            }
        });
    }

    public void GameOver()
    {
        gameOverCanvasGroup.blocksRaycasts = true;
        gameOverCanvasGroup.interactable = true;
        gameOverCanvasGroup.DOFade(1f, 1f).Play();
    }

    public void Reload()
    {
        FadeManager.Instance.Fade(true, 0f, () => SceneManager.LoadScene("Main"));
    }
}