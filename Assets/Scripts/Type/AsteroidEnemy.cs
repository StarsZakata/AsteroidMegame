using System.Collections.Generic;
using UnityEngine;

public enum TypeAsteroid
{
    small = 1,
    medium = 2,
    big
}

public class AsteroidEnemy : MonoBehaviour
{
    private const float MAX_SPEED_ASTEROID = 3f;
    private const float MIN_SPEED_ASTEROID = 0.25f;

    private const int MAX_SCORE_ASTEROID = 150;
    private const int MIN_SCORE_ASTEROID = 10;

    [Header("Main Parametr")]
    [Range(MIN_SPEED_ASTEROID, MAX_SPEED_ASTEROID)]
    public float Speed;
    [Range(MIN_SCORE_ASTEROID, MAX_SCORE_ASTEROID)]
    public int Score;

    [Space(20)]
    public GameObject MainParent;
    public GameObject SecondParentContainer;
    public AudioClip DestroyAsteroid;
    public List<AsteroidEnemy> MyChildrensList = new List<AsteroidEnemy>();
    public TypeAsteroid typeAsteroid = TypeAsteroid.big;

    public bool IsLeftOrRight;

    private AsteroidSpawner _asteroidSpawner;
    private Vector2 _direction;
    private Transform _transform;
    private bool _isMove;
    private float verticalHalfSize;
    private float horizontalHalfSize;


    public void InitBigAsteroid(AsteroidSpawner asteroidSpawner, GameObject container)
    {
        _asteroidSpawner = asteroidSpawner;
        MainParent = container;
        SecondParentContainer = container;

        BaseInit();
        _transform.position = CalculateStartPointAsteroid();
        _direction = Random.insideUnitCircle.normalized;
    }

    public void InitChildrenAsteroid(GameObject myParent, Vector2 dirParent, AsteroidSpawner asteroidSpawner, GameObject container)
    {
        _direction = DirectionForNewAsteroids(dirParent);

        _asteroidSpawner = asteroidSpawner;
        MainParent = myParent;
        SecondParentContainer = container;

        BaseInit();
        _transform.SetParent(SecondParentContainer.transform);
        switch (typeAsteroid)
        {
            case TypeAsteroid.medium:
                _transform.localScale = new Vector3(1.7f, 1.7f, 0);
                break;
            case TypeAsteroid.small:
                _transform.localScale = new Vector3(0.5f, 0.5f, 0);
                break;
            default:
            case TypeAsteroid.big:
                break;
        }
        Move(true);
    }

    public void Reset()
    {
        gameObject.SetActive(false);
        _direction = Vector3.zero;
        _transform.SetParent(MainParent.transform);
        _transform.position = MainParent.transform.position;
        foreach (AsteroidEnemy aster in MyChildrensList)
            if (aster.transform.parent == SecondParentContainer)
                aster.Reset();
    }

    public void ResetBigChildren()
    {
        foreach (AsteroidEnemy aster in MyChildrensList)
            if (aster.transform.parent == SecondParentContainer)
                aster.Reset();
    }

    private void BaseInit()
    {
        _transform = GetComponent<Transform>();
        verticalHalfSize = Camera.main.orthographicSize;
        horizontalHalfSize = verticalHalfSize * Screen.width / Screen.height;

        Speed = Random.Range(MIN_SPEED_ASTEROID, MAX_SPEED_ASTEROID);
    }

    private Vector3 CalculateStartPointAsteroid()
    {
        Vector3 pos = new Vector3();
        int posX = Random.Range(-1, 1);
        int posY = Random.Range(-1, 1);
        float maxHeight = Camera.main.orthographicSize;
        float maxWidth = maxHeight * 2;
        pos.x = Random.Range(3, maxWidth) * posX;
        pos.y = Random.Range(3, maxHeight) * posY;
        if (pos.x < 2 && pos.x > -2)
            pos.x += 5;
        return pos;
    }

    public void Move(bool value)
    {
        gameObject.SetActive(value);
        _isMove = value;
    }

    private Vector2 DirectionForNewAsteroids(Vector3 parentDirection)
    {
        Vector2 dir = new Vector2();
        float cos;
        float sin;
        if (IsLeftOrRight == true)
        {
            cos = Mathf.Sqrt(2) / 2;
            sin = cos;
        }
        else
        {
            cos = Mathf.Sqrt(2) / 2;
            sin = -Mathf.Sqrt(2) / 2;
        }
        dir.x = parentDirection.x * cos - parentDirection.y * sin;
        dir.y = parentDirection.x * sin + parentDirection.y * cos;
        return dir;
    }

    private void Update()
    {
        if (_isMove)
        {
            MoveAsteroid();
            SafeAreaLimit();
        }
    }

    private void MoveAsteroid()
    {
        _transform.position = new Vector3(_transform.position.x + _direction.x * Speed * Time.deltaTime,
                                            _transform.position.y + _direction.y * Speed * Time.deltaTime,
                                            0);
    }

    private void SafeAreaLimit()
    {
        verticalHalfSize = Camera.main.orthographicSize;
        horizontalHalfSize = verticalHalfSize * Screen.width / Screen.height;
        if (transform.position.y > verticalHalfSize)
        {
            transform.position = new Vector2(transform.position.x, -verticalHalfSize);
        }
        if (transform.position.y < -verticalHalfSize)
        {
            transform.position = new Vector2(transform.position.x, verticalHalfSize);
        }
        if (transform.position.x > horizontalHalfSize)
        {
            transform.position = new Vector2(-horizontalHalfSize, transform.position.y);
        }
        if (transform.position.x < -horizontalHalfSize)
        {
            transform.position = new Vector2(horizontalHalfSize, transform.position.y);
        }
    }

    //TODO
    public void OnMouseDown()
    {
        RunMoveMyChildrenAsteroids();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out BulletEnemy bullet))
            RunMoveMyChildrenAsteroids();
    }

    private void RunMoveMyChildrenAsteroids()
    {
        AudioManager.instance.PlayAudio(DestroyAsteroid);
        gameObject.SetActive(false);
        switch (typeAsteroid)
        {
            case TypeAsteroid.big:
                InitMyChildrenList();
                break;
            case TypeAsteroid.medium:
                transform.SetParent(MainParent.transform);
                InitMyChildrenList();
                break;
            case TypeAsteroid.small:
                transform.SetParent(MainParent.transform);
                _asteroidSpawner.ClearInActiveAsteroid(this);
                break;
        }
        Reset();
    }

    private void InitMyChildrenList()
    {
        foreach (AsteroidEnemy asteroidChildren in MyChildrensList)
        {
            asteroidChildren.transform.position = _transform.position;
            asteroidChildren.InitChildrenAsteroid(this.gameObject,
                                                _direction,
                                                _asteroidSpawner,
                                                SecondParentContainer);
            _asteroidSpawner.NewAsteroidsInMap(asteroidChildren);
        }
        _asteroidSpawner.ClearInActiveAsteroid(this);
    }

}
