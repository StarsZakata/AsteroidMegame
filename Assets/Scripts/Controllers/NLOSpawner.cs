using UnityEngine;

public class NLOSpawner : MonoBehaviour
{
    private const int MAX_TIME_DELAY_SPAWN_NLO = 40;
    private const int MIN_TIME_DELAY_SPAWN_NLO = 20;
    private const float Y_OFFSET_NLO = 0.8F;
    private const float X_OFFSET_NLO = 1.3F;

    public NLOEnemy PrefabNLO;
    public GameObject ContainerNLO;
    public GameObject ContainerNLOBullets;

    private NLOEnemy _currentNLO;
    private Asteroid _asteroid;
    private Player _player;

    private float _verticalHalfSize;
    private float _horizontalHalfSize;

    public void ShowNLO(bool value)
    {
        if (_currentNLO != null)
            _currentNLO.gameObject.SetActive(value);
        for (int i = 0; i < ContainerNLOBullets.transform.childCount; i++)
            ContainerNLOBullets.transform.GetChild(i).gameObject.SetActive(value);
    }

    public void ClearAllNLOandBullets()
    {
        if (_currentNLO != null)
        {
            _currentNLO.DestroyBullets();
            Destroy(_currentNLO.gameObject);
        }
    }


    public void Init(Asteroid asteroid, Player player)
    {
        _asteroid = asteroid;
        _player = player;
        RunSpawnNLO();

        _verticalHalfSize = Camera.main.orthographicSize;
        _horizontalHalfSize = _verticalHalfSize * Screen.width / Screen.height;
    }

    private void RunSpawnNLO()
    {
        int time_delay = Random.Range(MIN_TIME_DELAY_SPAWN_NLO, MAX_TIME_DELAY_SPAWN_NLO);
        Invoke(nameof(SpawnNLO), time_delay);
    }

    private void SpawnNLO()
    {
        if (_currentNLO == null)
        {
            _currentNLO = Instantiate(PrefabNLO, ContainerNLO.transform);
        }
        Transform NLOTrans = _currentNLO.GetComponent<Transform>();
        int changeX = Random.Range(0, 100);
        Vector2 direction = new Vector2();
        if (changeX < 50)
        {
            NLOTrans.position = new Vector2(_horizontalHalfSize * X_OFFSET_NLO,
                                            Random.Range(-_verticalHalfSize * Y_OFFSET_NLO,
                                                        _verticalHalfSize * Y_OFFSET_NLO));
            direction.x = -NLOTrans.position.x;
            direction.y = NLOTrans.position.y;
            NLOTrans.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            NLOTrans.position = new Vector2(-_horizontalHalfSize * X_OFFSET_NLO,
                                            Random.Range(-_verticalHalfSize * Y_OFFSET_NLO,
                                                        _verticalHalfSize * Y_OFFSET_NLO));
            direction.x = NLOTrans.position.x;
            direction.y = NLOTrans.position.y;
            NLOTrans.rotation = Quaternion.Euler(0, 0, -90);
        }
        _currentNLO.Init(this, _player, ContainerNLOBullets, direction);
        _currentNLO.Move(true);
    }

    public void ResetAndSpawnNLO()
    {
        RunSpawnNLO();
    }

    public void AddPlayerScore(int score)
    {
        _asteroid.AddPlayerScore(score);
    }

}
