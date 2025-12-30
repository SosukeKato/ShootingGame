using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class PoolData
{
    public GameObject prefab;
    public Transform muzzle;
    public int _poolSize;
}

public class GameController : MonoBehaviour
{
    Transform _tr;
    Transform _pt;
    Transform _tt;

    [SerializeField]
    PoolData[] _pdArray;

    [SerializeField]
    PlayerInput _pi;

    [SerializeField, Header("Poolの親オブジェクト")]
    Transform _parent;
    [SerializeField,Header("Player")]
    GameObject _player;
    [SerializeField,Header("照準")]
    GameObject _target;
    [SerializeField,Header("Enemy")]
    GameObject[] _enemy;
    [SerializeField,Header("Playerが発射するBulletのPrefab")]
    GameObject _playerBullet;
    [SerializeField,Header("Enemyが発射するBulletのPrefab")]
    GameObject[] _enemyBullet;
    [SerializeField,Header("Stageの背景になるImage")]
    Image[] _stage;
    [SerializeField,Header("Titleの背景になるImage")]
    Image _titleImage;
    [SerializeField,Header("GameOverの背景になるImage")]
    Image _gameOverImage;
    [SerializeField,Header("GameClearの背景になるImage")]
    Image _gameClearImage;
    [SerializeField,Header("Playerと照準の移動速度")]
    float _moveSpeed;
    [SerializeField,Header("Enemyの状態変化1段階目が終わる時間")]
    int _firstFormEndTime;

    Vector2 _playerMoveInput;
    Vector2 _targetMoveInput;

    string _state;

    bool _isPause;

    Queue<GameObject>[] _bulletPoolArray;

    void Awake()
    {
        _tr = transform;
        _pt = _player.transform;
        _tt = _target.transform;
        _pi = GetComponent<PlayerInput>();

        #region Poolの初期化処理
        _bulletPoolArray = new Queue<GameObject>[_pdArray.Length];

        for (int i= 0; i < _pdArray.Length; i++)
        {
            _bulletPoolArray[i] = new Queue<GameObject>();
            for (int j = 0; j < _pdArray[i]._poolSize;j++)
            {
                GameObject bullet = Instantiate(_pdArray[i].prefab);
                bullet.SetActive(false);
                bullet.transform.SetParent(_parent);
                _bulletPoolArray[i].Enqueue(bullet);
            }
        }
        #endregion
    }

    void Start()
    {
        
    }

    void Update()
    {
        #region Playerの処理

        #region Playerの移動

        if (_pi == null)
        {
            return;
        }
        if (_pi.actions["PlayerMove"].IsPressed())
        {
            _playerMoveInput = _pi.actions["PlayerMove"].ReadValue<Vector2>();
        }
        else
        {
            _playerMoveInput = new Vector3(0, 0, 0);
        }
        _pt.position += new Vector3(_playerMoveInput.x, _playerMoveInput.y, 0).normalized * Time.deltaTime * _moveSpeed;

        #endregion

        #region 照準の移動

        if (_pi.actions["TargetMove"].IsPressed())
        {
            _targetMoveInput = _pi.actions["TargetMove"].ReadValue<Vector2>();
        }
        else
        {
            _targetMoveInput = new Vector3(0, 0, 0);
        }
        _tt.position += new Vector3(_targetMoveInput.x, _targetMoveInput.y, 0).normalized * Time.deltaTime * _moveSpeed;

        #endregion

        #region Playerの攻撃
        if (_pi.actions["PlayerAttack"].WasPressedThisFrame())
        {
            Debug.Log("Attack");
        }
        #endregion

        #endregion
    }

    #region Poolからオブジェクトを取得する処理
    GameObject GetBullet(int index)
    {
        if (index < 0 || index >= _bulletPoolArray.Length)
        {
            Debug.LogError($"インデックス{index}が範囲外までいっちゃったやでどうするやで？");
            return null;
        }

        GameObject bullet;

        if (_bulletPoolArray[index].Count > 0)
        {
            bullet = _bulletPoolArray[index].Dequeue();
        }
        else
        {
            bullet = Instantiate(_pdArray[index].prefab);
            bullet.transform.SetParent(_parent);
            Debug.LogWarning("足りなかったから仕方ないし作ってやるやで");
        }

        bullet.SetActive(true);

        return null;
    }
    #endregion

    #region Poolにオブジェクトを返却する処理
    void ReturnBullet(int index, GameObject bullet)
    {
        if (bullet == null)
        {
            return;
        }

        if (index < 0 || index >= _bulletPoolArray.Length)
        {
            Debug.LogError($"インデックス{index}が範囲外までいっちゃったやでどうするやで？");
            return;
        }

        bullet.SetActive(false);
        bullet.transform.SetParent(_parent);
        _bulletPoolArray[index].Enqueue(bullet);
    }
    #endregion

    void SpawnBullet(int index)
    {
        GameObject bullet = GetBullet(index);

        bullet.transform.SetPositionAndRotation(_pdArray[1].muzzle.position, Quaternion.identity);
    }
}
