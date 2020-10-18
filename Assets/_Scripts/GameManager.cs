using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Transform player;
    public GameObject[] levelPrefabs;

    int _currentLevelIndex;
    Level _currentLevel;

    private void Start()
    {
        CreateNextLevel();
    }

    public void CreateNextLevel()
    {
        _currentLevel = Instantiate(levelPrefabs[_currentLevelIndex]).GetComponent<Level>();
        _currentLevelIndex++;

        player.position = _currentLevel.playerSpawn.position;
        FadeManager.Instance.Fade(false, 0f, () => Events.OnLevelStart?.Invoke());
    }

    public void TriggerLevelEnd()
    {
        Events.OnLevelEnd?.Invoke();
    }

    public void GoToNextLevel()
    {
        FadeManager.Instance.Fade(true, 1f, () =>
        {
            Destroy(_currentLevel.gameObject);
            CreateNextLevel();
        });
    }
}