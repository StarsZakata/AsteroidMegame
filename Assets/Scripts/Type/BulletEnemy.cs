using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    private const float MAX_SPEED = 3f;
    private const float MIN_SPEED = 0.25f;

    [Range(MIN_SPEED, MAX_SPEED)]
    public float speed;
    public bool IsNLOBullet = false;

    private Transform _transform;

    private bool _isMove;
    private float verticalHalfSize;
    private float horizontalHalfSize;
    private float _distanceForMove;
    private float _currentDistance;

    public void RunMove(Vector3 startPosition, Quaternion startRotation)
    {
        _transform = GetComponent<Transform>();
        _transform.rotation = startRotation;
        _transform.position = startPosition;
        _isMove = true;
        verticalHalfSize = Camera.main.orthographicSize;
        _distanceForMove = 2 * verticalHalfSize * Screen.width / Screen.height;
        _currentDistance = 0;
    }

    private void Update()
    {
        if (_isMove)
        {
            _transform.Translate(Vector3.up * speed);
            _currentDistance += 1 * speed;
        }
        if (_currentDistance >= _distanceForMove)
        {
            _isMove = false;
            gameObject.SetActive(false);
        }
        SafeAreaLimit();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out AsteroidEnemy asteroid))
            gameObject.SetActive(false);
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
}
