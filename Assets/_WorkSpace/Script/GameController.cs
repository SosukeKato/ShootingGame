using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class PoolData
{
    public GameObject prefab;
    public Transform muzzle;
    public int poolSize;
    public List<GameObject> objectList = new();
    public Queue<GameObject> pool = new();
}

public enum PoolType
{
    PlayerBullet = 0,
    EnemyBullet = 1
}

public class GameController : MonoBehaviour
{
    #region コンポーネント取得用の変数

    Transform _tr;
    Transform _pt;
    Transform _tt;
    Transform _et;
    PlayerInput _pi;

    #endregion

    #region エディタから変更できる変数

    [SerializeField]
    PoolData[] _pdArray;

    [SerializeField, Header("Poolの親オブジェクト")]
    Transform _parent;
    [SerializeField,Header("Player")]
    GameObject _player;
    [SerializeField,Header("照準")]
    GameObject _target;
    [SerializeField,Header("Enemy")]
    GameObject _enemy;
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
    [SerializeField, Header("弾の移動速度")]
    float _bulletSpeed;
    [SerializeField, Header("敵の弾とPlayerの衝突距離")]
    float _playerToEnemyBulletCol;
    [SerializeField, Header("Playerの弾と敵の衝突距離")]
    float _enemyToPlayerBulletCol;
    [SerializeField,Header("Enemyの状態変化1段階目が終わる時間")]
    int _firstFormEndTime;
    [SerializeField,Header("EnemyのHPの最大値")]
    int _enemyMaxHP;
    [SerializeField,Header("X軸の移動範囲制限(min,max)")]
    Vector2 _fieldClampX;
    [SerializeField,Header("Y軸の移動範囲制限(min,max)")]
    Vector2 _fieldClampY;

    #endregion

    #region 内部変数

    Vector2 _playerMoveInput;
    Vector2 _targetMoveInput;

    Vector2 _playerPosition;
    Vector2 _targetPosition;

    int _enemyHP;

    string _state;

    bool _isPause;
    bool _isLoading;

    #endregion

    void Awake()
    {
        
    }

    void Start()
    {
        #region コンポーネントの取得

        _tr = transform;
        _pt = _player.transform;
        _tt = _target.transform;
        _et = _enemy.transform;
        _pi = GetComponent<PlayerInput>();

        #endregion

        #region Poolの初期化処理

        for (int i = 0; i < _pdArray.Length; i++)
        {
            _pdArray[i].pool = new();
            for (int j = 0; j < _pdArray[i].poolSize; j++)
            {
                GameObject bullet = Instantiate(_pdArray[i].prefab);
                bullet.SetActive(false);
                bullet.transform.SetParent(_parent);
                _pdArray[i].pool.Enqueue(bullet);
            }
        }

        #endregion
    }

    void Update()
    {
        #region Playerの処理

        #region Playerの攻撃

        if (_pi.actions["PlayerAttack"].WasPressedThisFrame())
        {
            SpawnBullet(PoolType.PlayerBullet);
        }

        #region Playerの弾の移動

        for (int i = 0; i < _pdArray[(int)PoolType.PlayerBullet].objectList.Count; i++)
        {
            _pdArray[(int)PoolType.PlayerBullet].objectList[i].transform.position += _pdArray[(int)PoolType.PlayerBullet].objectList[i].transform.up * Time.deltaTime * _bulletSpeed;
        }

        #endregion

        #endregion

        #region Playerの移動

        #region Playerの移動範囲制限

        _playerPosition = _pt.position;

        _playerPosition.x = Mathf.Clamp(_playerPosition.x, _fieldClampX.x, _fieldClampX.y);
        _playerPosition.y = Mathf.Clamp(_playerPosition.y, _fieldClampY.x, _fieldClampY.y);

        _pt.position = _playerPosition;

        #endregion

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
            _playerMoveInput = Vector2.zero;
        }
        _pt.position += new Vector3(_playerMoveInput.x, _playerMoveInput.y, 0).normalized * Time.deltaTime * _moveSpeed;

        #endregion

        #region 照準の移動

        #region 照準の移動範囲制限

        _targetPosition = _tt.position;

        _targetPosition.x = Mathf.Clamp(_targetPosition.x, _fieldClampX.x, _fieldClampX.y);
        _targetPosition.y = Mathf.Clamp(_targetPosition.y, _fieldClampY.x, _fieldClampY.y);

        _tt.position = _targetPosition;

        #endregion

        if (_pi.actions["TargetMove"].IsPressed())
        {
            _targetMoveInput = _pi.actions["TargetMove"].ReadValue<Vector2>();
        }
        else
        {
            _targetMoveInput = Vector2.zero;
        }
        _tt.position += new Vector3(_targetMoveInput.x, _targetMoveInput.y, 0).normalized * Time.deltaTime * _moveSpeed;

        #endregion

        #endregion

        #region Enemyの処理

        #region EnemyのHPの処理

        if (_enemyHP > _enemyMaxHP)
        {
            _enemyHP = _enemyMaxHP;
        }
        else if (_enemyHP <= 0)
        {
            //Debug.Log("敵が倒されたらしいね。お前の勝ち、何で勝ったか明日までに考えといてください");
        }

        #endregion

        #region Enemyの攻撃

        #endregion

        #endregion

        #region 当たり判定の処理

        #region Playerと敵の弾



        #endregion

        #region 敵とPlayerの弾

        for (int i = 0; i < _pdArray[(int)PoolType.PlayerBullet].objectList.Count; i++)
        {
            float EnemyX = _et.position.x;
            float EnemyY = _et.position.y;
            float PlayerBulletX = _pdArray[(int)PoolType.PlayerBullet].objectList[i].transform.position.x;
            float PlayerBulletY = _pdArray[(int)PoolType.PlayerBullet].objectList[i].transform.position.y;
            float distance = (EnemyX * EnemyX + EnemyY * EnemyY) - (PlayerBulletX * PlayerBulletX + PlayerBulletY * PlayerBulletY);

            if (distance < _enemyToPlayerBulletCol)
            {
                _pdArray[(int)PoolType.PlayerBullet].objectList.RemoveAt(i);
                i--;
                _state = "GameOver";
            }
        }

        #endregion

        #endregion
    }

    #region Poolからオブジェクトを取得する処理
    GameObject GetBullet(int index)
    {
        GameObject bullet;

        if (_pdArray[index].pool.Count > 0)
        {
            bullet = _pdArray[index].pool.Dequeue();
        }
        else
        {
            bullet = Instantiate(_pdArray[index].prefab);
            bullet.transform.SetParent(_parent);
            _pdArray[index].objectList.Add(bullet);
            Debug.LogWarning("足りなかったから仕方ないし作ってやるやで");
        }

        bullet.SetActive(true);

        return bullet;
    }
    #endregion

    #region Poolにオブジェクトを返却する処理
    void ReturnBullet(int index, GameObject bullet)
    {
        if (bullet == null)
        {
            return;
        }

        if (index < 0 || index >= _pdArray[index].pool.Count)
        {
            Debug.LogError($"インデックス{index}が範囲外までいっちゃったやでどうするやで？");
            return;
        }

        bullet.SetActive(false);
        bullet.transform.SetParent(_parent);
        _pdArray[index].pool.Enqueue(bullet);
    }
    #endregion

    #region Pool内のオブジェクトの生成処理
    void SpawnBullet(PoolType bulletType)
    {
        GameObject bullet = GetBullet((int) bulletType);

        _pdArray[(int)bulletType].objectList.Add(bullet);

        bullet.transform.SetPositionAndRotation(_pdArray[(int)bulletType].muzzle.position, Quaternion.identity);
    }
    #endregion
}
