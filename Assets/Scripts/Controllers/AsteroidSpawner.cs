using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    private const int MAX_BIG_ASTEROID = 15;
    private const int MIN_BIG_ASTEROID = 2;

    [Header("Main Spawn Parameters")]
    [Range(MIN_BIG_ASTEROID, MAX_BIG_ASTEROID)]
    public int MaxActiveAsteroid = 10;

    [Space(20)]
    public GameObject ContainerAsteroids;
    public AsteroidEnemy PrefabBigAsteroid;

    [SerializeField] private int _countAsteroidForFinish;
    [SerializeField] private int _currentCountAsteroidToFinish;

    private List<AsteroidEnemy> _activeBigAsteroids = new List<AsteroidEnemy>();
    private List<AsteroidEnemy> _allAsteroidsInMap = new List<AsteroidEnemy>();
    private Asteroid _asteroid;

    public void Init(Asteroid asteroid, int countAsteroid)
    {
        _asteroid = asteroid;
        _allAsteroidsInMap.Clear();
        _currentCountAsteroidToFinish = 0;
        // TODO bad
        ResetAsteroids();
        ResetAsteroids();
        ResetAsteroids();

        CheckPullBigAsteroids(countAsteroid);
    }

    private void CheckPullBigAsteroids(int countAsteroid)
    {
        if (countAsteroid > MaxActiveAsteroid) return;
        if (countAsteroid > _activeBigAsteroids.Count)
            CreateBigAsteroid(countAsteroid - _activeBigAsteroids.Count);
    }

    public void RunMoveAsteroidsInGame(int countAsteroid)
    {
        if (countAsteroid > MaxActiveAsteroid)
            countAsteroid = _activeBigAsteroids.Count;
        for (int i = 0; i < countAsteroid; i++)
        {
            _activeBigAsteroids[i].Move(true);
            _activeBigAsteroids[i].InitBigAsteroid(this, ContainerAsteroids);
            _allAsteroidsInMap.Add(_activeBigAsteroids[i]);
        }
        _countAsteroidForFinish = _activeBigAsteroids.Count * 7;
    }

    public void ContinueMoveAsteroids()
    {
        foreach (AsteroidEnemy asteroid in _allAsteroidsInMap)
            asteroid.Move(true);
    }

    public void StopMoveAsteroidsInGame()
    {
        foreach (AsteroidEnemy asteroid in _allAsteroidsInMap)
            asteroid.Move(false);
    }

    private void CreateBigAsteroid(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AsteroidEnemy asteroid = Instantiate(PrefabBigAsteroid,
                                                ContainerAsteroids.transform);
            asteroid.InitBigAsteroid(this, ContainerAsteroids);
            _activeBigAsteroids.Add(asteroid);
        }
    }

    public void NewAsteroidsInMap(AsteroidEnemy asteroid)
    {
        _allAsteroidsInMap.Add(asteroid);
    }

    public void ClearInActiveAsteroid(AsteroidEnemy asteroid)
    {
        _allAsteroidsInMap.Remove(asteroid);
        _asteroid.AddPlayerScore(asteroid.Score);
        _currentCountAsteroidToFinish++;
        if (_currentCountAsteroidToFinish >= _countAsteroidForFinish)
        {
            _currentCountAsteroidToFinish = 0;
            _asteroid.RunNewWave();
        }
    }

    // TODO need better Pull Don Show Asteroids
    private void ResetAsteroids()
    {
        for (int i = 0; i < ContainerAsteroids.transform.childCount; i++)
        {
            if (ContainerAsteroids.transform.GetChild(i).GetComponent<AsteroidEnemy>().typeAsteroid == TypeAsteroid.big)
            {
                ContainerAsteroids.transform.GetChild(i).GetComponent<AsteroidEnemy>().ResetBigChildren();
            }
            if (ContainerAsteroids.transform.GetChild(i).gameObject.activeSelf == false)
            {
                ContainerAsteroids.transform.GetChild(i).GetComponent<AsteroidEnemy>().Reset();
            }
        }
    }

    public void ClearAllAsteroids()
    {
        foreach (AsteroidEnemy aster in _activeBigAsteroids)
            Destroy(aster.gameObject);
        foreach (AsteroidEnemy aster in _allAsteroidsInMap)
            Destroy(aster.gameObject);
        _activeBigAsteroids.Clear();
        _allAsteroidsInMap.Clear();
        for (int i = 0; i < ContainerAsteroids.transform.childCount; i++)
            Destroy(ContainerAsteroids.transform.GetChild(i).gameObject);
    }

}
