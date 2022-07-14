using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLOEnemy : MonoBehaviour
{
    private const float DO_NOT_SHOW_BETWEEN = 0.5f;
    private const int TIME_START_TO_FINISH = 10;

    private const int MAX_TIME_DELAY_BULLET = 5;
    private const int MIN_TIME_DELAY_BULLET = 2;

    public BulletEnemy PrefabBulletNLO;
    public AudioClip GunBullet;
    public int MaxNumberBullet = 10;
    public int NLOScore;

    private Player _player;
    private NLOSpawner _nloSpawner;
    private GameObject _containerBullet;

    private Transform _transform;
    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private List<BulletEnemy> _bulletNLOList = new List<BulletEnemy>();

    private bool _isMove;
    public bool GetMove()
    {
        return _isMove;
    }

    private bool _isDestroy;
    public bool IsDestroy()
    {
        return _isDestroy;
    }

    private float _timeDelay;


    public void DestroyBullets()
    {
        foreach (BulletEnemy bullet in _bulletNLOList)
            Destroy(bullet.gameObject);
        _bulletNLOList.Clear();
    }

    public void Init(NLOSpawner nloSpawner, Player player, GameObject containerBullet, Vector3 targetPoint)
    {
        StopAllCoroutines();
        _containerBullet = containerBullet;
        _nloSpawner = nloSpawner;
        _player = player;
        _transform = GetComponent<Transform>();
        _startPosition = _transform.position;
        _endPosition = targetPoint;
    }

    public void Move(bool value)
    {
        _isDestroy = false;
        _timeDelay = 0;
        gameObject.SetActive(value);
        _isMove = value;
        StartCoroutine(AttackPlayer());
    }

    private void Update()
    {
        if (_isMove)
        {
            LootToPlayer();
            _timeDelay += Time.deltaTime / TIME_START_TO_FINISH;
            MoveObject();
            if (Math.Abs(_transform.position.x - _endPosition.x) <= DO_NOT_SHOW_BETWEEN)
            {
                _isMove = false;
                gameObject.SetActive(false);
                _nloSpawner.ResetAndSpawnNLO();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out AsteroidEnemy asteroid))
            gameObject.SetActive(false);
        if (other.TryGetComponent(out BulletEnemy bullet))
            if (bullet.IsNLOBullet == false)
            {
                _isDestroy = true;
                _nloSpawner.AddPlayerScore(NLOScore);
                gameObject.SetActive(false);
            }
    }

    private void LootToPlayer()
    {
        var neededRotation = Quaternion.LookRotation(Vector3.forward, _player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, neededRotation, 100f * Time.deltaTime);
    }

    private void MoveObject()
    {
        _transform.position = Vector2.Lerp(_startPosition, _endPosition, _timeDelay);
    }

    IEnumerator AttackPlayer()
    {
        while (_isMove)
        {
            bool needNewBullet = false;
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(MIN_TIME_DELAY_BULLET, MAX_TIME_DELAY_BULLET));

            foreach (BulletEnemy bullet in _bulletNLOList)
            {
                if (bullet.gameObject.activeSelf == false)
                {
                    bullet.gameObject.SetActive(true);
                    bullet.RunMove(_transform.position, _transform.rotation);
                    AudioManager.instance.PlayAudio(GunBullet);
                    needNewBullet = false;
                    break;
                }
                else
                    needNewBullet = true;
            }
            if (_bulletNLOList.Count == 0) needNewBullet = true;
            if (needNewBullet && _bulletNLOList.Count < MaxNumberBullet)
            {
                BulletEnemy tmp = Instantiate(PrefabBulletNLO, _containerBullet.transform);
                tmp.IsNLOBullet = true;
                tmp.RunMove(_transform.position, _transform.rotation);
                AudioManager.instance.PlayAudio(GunBullet);
                _bulletNLOList.Add(tmp);
            }
        }
    }
}
