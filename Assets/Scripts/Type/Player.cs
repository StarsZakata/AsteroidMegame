using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float MAX_SPEED = 1f;
    private const float MIN_SPEED = 0.5f;
    private const float MAX_ACCELERATION = 0.25f;
    private const float MIN_ACCELERATION = 0.01f;
    private const float MAX_LOOK_SPEED = 1f;
    private const float MIN_LOOK_SPEED = 0.05f;
    private const int MAX_BULLET = 100;
    private const int MIN_BULLET = 3;
    private const float MAX_DELAY_SHOOT = 2f;
    private const float MIN_DELAY_SHOOT = 0.2f;

    [Header("Main Player Parameters")]
    [Range(MIN_SPEED, MAX_SPEED)]
    public float Speed;
    [Range(MIN_ACCELERATION, MAX_ACCELERATION)]
    public float Acceleration;
    [Range(MIN_LOOK_SPEED, MAX_LOOK_SPEED)]
    public float LookSpeed;
    private float MaxLookSpeed = 5f;
    [Range(MIN_BULLET, MAX_BULLET)]
    public int MaxNumberBullet;
    [Range(MIN_DELAY_SHOOT, MAX_DELAY_SHOOT)]
    public float ShootDelay;

    [Space(20)]
    public GameObject Weapon;
    public GameObject ParentForBullets;
    public BulletEnemy BulletPrefab;
    public AudioClip MoveSpeed;
    public AudioClip GunBullet;
    public AnimationClip _invulnerableAnimClip;

    private Asteroid _asteroid;
    private Transform _transform;
    private Animation _animation;
    private Vector2 _direction;
    private List<BulletEnemy> _bulletsList = new List<BulletEnemy>();

    private float _rotateAmount;
    private bool _isMove;
    private float _verticalHalfSize;
    private float _horizontalHalfSize;
    private float _shootDelayCounter;
    private bool _invulnerable;
    public void ResetInvulnerable()
    {
        _invulnerable = false;
        _animation.clip = null;
    }

    private int _variantController;


    public void Init(Asteroid asteroid)
    {
        _asteroid = asteroid;
        _transform = GetComponent<Transform>();
        _transform.position = Vector3.zero;
        _transform.rotation = Quaternion.identity;
        _direction = Vector2.zero;

        _verticalHalfSize = Camera.main.orthographicSize;
        _horizontalHalfSize = _verticalHalfSize * Screen.width / Screen.height;
        RunAnimInvulnerable();
        Move(true);
    }

    private void RunAnimInvulnerable()
    {
        _invulnerable = true;
        _animation = GetComponent<Animation>();
        _animation.clip = _invulnerableAnimClip;
        _animation.Play();
    }

    public void SetController(int varControls)
    {
        _variantController = varControls;
    }

    private void Update()
    {
        if (_isMove)
        {
            if (_variantController == 1)
                MoveKeyboard();
            else if (_variantController == 2)
                MoveKeyboardMouse();
            SafeAreaLimit();
        }
    }

    private void MoveKeyboardMouse()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _direction = Vector2.Lerp(_direction, _direction + (Vector2)transform.up * Acceleration, 0.9f);
            AudioManager.instance.PlayAudio(MoveSpeed);
        }
        _transform.position = _transform.position + (Vector3)_direction * Time.deltaTime;

        Vector2 mousePoition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePoition.y, mousePoition.x) * Mathf.Rad2Deg - 90;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _transform.rotation = Quaternion.Slerp(transform.rotation, rotation, MAX_LOOK_SPEED * 5 * Time.deltaTime);

        _shootDelayCounter += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(1) && _shootDelayCounter >= ShootDelay)
        {
            _shootDelayCounter = 0;
            CreateBullet();
        }
    }

    private void MoveKeyboard()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _direction = Vector2.Lerp(_direction, _direction + (Vector2)transform.up * Acceleration, 0.9f);
            AudioManager.instance.PlayAudio(MoveSpeed);
        }
        _transform.position = _transform.position + (Vector3)_direction * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            if (_rotateAmount <= MaxLookSpeed)
                _rotateAmount += LookSpeed;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            if (_rotateAmount >= -MaxLookSpeed)
                _rotateAmount -= LookSpeed;
        _transform.Rotate(0, 0, _rotateAmount);


        if (!Input.GetKey(KeyCode.A) &&
            !Input.GetKey(KeyCode.D) &&
            !Input.GetKey(KeyCode.RightArrow) &&
            !Input.GetKey(KeyCode.LeftArrow))
            if (_rotateAmount >= -0.0001f && _rotateAmount <= 0.0001f)
                _rotateAmount = 0;
            else
                _rotateAmount = Mathf.Lerp(_rotateAmount, 0, 0.2f);

        _shootDelayCounter += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && _shootDelayCounter >= ShootDelay)
        {
            _shootDelayCounter = 0;
            CreateBullet();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_invulnerable)
        {
            if (other.TryGetComponent(out BulletEnemy bullet) || other.TryGetComponent(out AsteroidEnemy asteroid))
            {
                _asteroid.ContinueWave();
                Move(false);
            }
        }
    }

    private void CreateBullet()
    {
        foreach (BulletEnemy bullet in _bulletsList)
            if (bullet.gameObject.activeSelf == false)
            {
                bullet.gameObject.SetActive(true);
                bullet.RunMove(Weapon.transform.position, transform.rotation);
                AudioManager.instance.PlayAudio(GunBullet);
                return;
            }
        if (_bulletsList.Count >= MaxNumberBullet) return;
        BulletEnemy tmp = Instantiate(BulletPrefab,
                                    Weapon.transform.position,
                                    Quaternion.identity,
                                    ParentForBullets.transform);
        tmp.RunMove(Weapon.transform.position, transform.rotation);
        AudioManager.instance.PlayAudio(GunBullet);
        _bulletsList.Add(tmp);
    }

    private void SafeAreaLimit()
    {
        _verticalHalfSize = Camera.main.orthographicSize;
        _horizontalHalfSize = _verticalHalfSize * Screen.width / Screen.height;
        if (transform.position.y > _verticalHalfSize)
        {
            transform.position = new Vector2(transform.position.x, -_verticalHalfSize);
        }
        if (transform.position.y < -_verticalHalfSize)
        {
            transform.position = new Vector2(transform.position.x, _verticalHalfSize);
        }
        if (transform.position.x > _horizontalHalfSize)
        {
            transform.position = new Vector2(-_horizontalHalfSize, transform.position.y);
        }
        if (transform.position.x < -_horizontalHalfSize)
        {
            transform.position = new Vector2(_horizontalHalfSize, transform.position.y);
        }
    }

    public void Move(bool value)
    {
        gameObject.SetActive(value);
        _isMove = value;
        for (int i = 0; i < ParentForBullets.transform.childCount; i++)
            ParentForBullets.transform.GetChild(i).gameObject.SetActive(value);
    }

    public void ClearAllBullets()
    {
        foreach (BulletEnemy bullet in _bulletsList)
            Destroy(bullet.gameObject);
        _bulletsList.Clear();
    }
}
