using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerController player;
    public GameObject[] levelPrefabs;
    public CinemachineConfiner cameraConfiner;

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
        player.SetPlayerState(PlayerController.PlayerState.Teleporting);
        player.Transform.DOMoveY(player.Transform.position.y + 2f, 1f).Play();

        FadeManager.Instance.Fade(true, 1f, () =>
        {
            Destroy(_currentLevel.gameObject);
            CreateNextLevel();
        });
    }
}