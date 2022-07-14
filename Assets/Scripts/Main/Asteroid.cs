using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public const string PlayerKeyboard = "Управление: клавиатура";
    public const string PlayerKeyboardMouse = "Управление: клавиатура + мышь";
    private const float TIME_DELAY_FOR_NEW_WAVE = 0.3f;
    private const int MAX_PLAYER_LIFE = 10;
    private const int MIN_PLAYER_LIFE = 1;
    private const int MAX_BASE_BIG_ASTEROIDS = 5;
    private const int MIN_BASE_BIG_ASTEROIDS = 1;

    [Space(10)]
    [Range(MIN_PLAYER_LIFE, MAX_PLAYER_LIFE)]
    public int BaseLife;
    [Range(MIN_BASE_BIG_ASTEROIDS, MAX_BASE_BIG_ASTEROIDS)]
    public int CountBaseBigAsteroid;
    public int BaseScore;

    [Header("Main Game Objects")]
    public AsteroidSpawner asteroidSpawner;
    public NLOSpawner nLOSpawner;
    public UIAsteroid uiAsteroid;
    public Player player;

    private int _currentScore;
    private int _currentLife;
    private int _currentCountBigAsteroids;

    private void Start()
    {
        uiAsteroid.Init(this);
        uiAsteroid.ChangeGameData(_currentLife, _currentScore);
    }

    public void StartNewGame()
    {
        _currentScore = BaseScore;
        _currentLife = BaseLife;
        _currentCountBigAsteroids = CountBaseBigAsteroid;

        uiAsteroid.ChangeGameData(BaseLife, BaseScore);
        asteroidSpawner.ClearAllAsteroids();
        asteroidSpawner.Init(this, _currentCountBigAsteroids);
        asteroidSpawner.RunMoveAsteroidsInGame(_currentCountBigAsteroids);

        nLOSpawner.ClearAllNLOandBullets();
        nLOSpawner.Init(this, player);

        player.ClearAllBullets();
        player.Init(this);
        player.SetController(uiAsteroid.GetController());
    }

    public void StartNewLevel()
    {
        uiAsteroid.ChangeGameData(_currentLife, _currentScore);
        asteroidSpawner.Init(this, _currentCountBigAsteroids);
        asteroidSpawner.RunMoveAsteroidsInGame(_currentCountBigAsteroids);

        nLOSpawner.Init(this, player);

        player.Init(this);
        player.SetController(uiAsteroid.GetController());
    }

    public void PauseGame()
    {
        nLOSpawner.ShowNLO(false);
        nLOSpawner.StopAllCoroutines();
        player.Move(false);
        asteroidSpawner.StopMoveAsteroidsInGame();
    }

    public void ContinueThisGame()
    {
        nLOSpawner.ShowNLO(true);
        player.SetController(uiAsteroid.GetController());
        player.Move(true);
        asteroidSpawner.ContinueMoveAsteroids();
    }

    public void AddPlayerScore(int score)
    {
        _currentScore += score;
        uiAsteroid.ChangeGameData(_currentLife, _currentScore);
    }

    public void RunNewWave()
    {
        _currentCountBigAsteroids++;
        Invoke(nameof(StartNewLevel), TIME_DELAY_FOR_NEW_WAVE);
    }

    public void ContinueWave()
    {
        _currentLife--;
        uiAsteroid.ChangeGameData(_currentLife, _currentScore);
        if (_currentLife > 0)
        {
            Invoke(nameof(ContinueItsWave), TIME_DELAY_FOR_NEW_WAVE);
        }
        else
            Invoke(nameof(FinishGame), TIME_DELAY_FOR_NEW_WAVE);
    }

    private void ContinueItsWave()
    {
        player.Init(this);
        player.SetController(uiAsteroid.GetController());
    }

    private void FinishGame()
    {
        _currentLife = BaseLife;
        _currentScore = BaseScore;
        uiAsteroid.Init(this);
        player.Move(false);
        asteroidSpawner.StopMoveAsteroidsInGame();
        nLOSpawner.StopAllCoroutines();
    }

}
