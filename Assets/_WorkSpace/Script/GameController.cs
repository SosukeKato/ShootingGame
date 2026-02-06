using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class PoolData
{
    public GameObject prefab;
    public Transform[] muzzle;
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
    [SerializeField, Header("Player")]
    GameObject _player;
    [SerializeField, Header("照準")]
    GameObject _target;
    [SerializeField, Header("Enemy")]
    GameObject _enemy;
    [SerializeField, Header("Stageの背景になるImage")]
    Image[] _stage;
    [SerializeField, Header("Titleの背景になるImage")]
    Image _titleImage;
    [SerializeField, Header("GameOverの背景になるImage")]
    Image _gameOverImage;
    [SerializeField, Header("GameClearの背景になるImage")]
    Image _gameClearImage;
    [SerializeField, Header("Playerと照準の移動速度")]
    float _moveSpeed;
    [SerializeField, Header("Enemyの横移動速度")]
    float _enemyBesideMoveSpeed = 1;
    [SerializeField, Header("Playerの弾の移動速度")]
    float _playerBulletSpeed;
    [SerializeField, Header("敵の弾の移動速度")]
    float _enemyBulletSpeed;
    [SerializeField, Header("敵のmuzzleの回転速度")]
    float _enemyMuzzleRotateSpeed;
    [SerializeField, Header("敵の弾とPlayerの衝突距離")]
    float _playerToEnemyBulletCol;
    [SerializeField, Header("Playerの弾と敵の衝突距離")]
    float _enemyToPlayerBulletCol;
    [SerializeField, Header("EnemyのMuzzleが回転する攻撃のInterval")]
    float _enemyMuzzleRotateAttackInterval;
    [SerializeField, Header("Enemyが左右に移動しながら波状にばらまく攻撃のInterval")]
    float _enemyWaveAttackInterval;
    [SerializeField, Header("Enemyの状態変化1段階目が終わる時間")]
    int _firstFormEndTime;
    [SerializeField, Header("EnemyのHPの最大値")]
    int _enemyMaxHP;
    [SerializeField, Header("画面範囲(+の方向)")]
    Vector2 _screenSize;

    #endregion

    #region 内部変数

    Vector3 _trash = new Vector3(1000, 1000, 1000);

    Vector2 _playerMoveInput;
    Vector2 _targetMoveInput;
    Vector2 _playerPosition;
    Vector2 _targetPosition;

    int _enemyCurrentHP;
    int _enemyControlNumber;

    float _enemyMuzzleRotateAttackControlTimer;
    float _enemyWaveAttackControlTimer;
    float _anglePlus;
    float _angleMinus;

    string _activeScene;

    bool _isPause;
    bool _isLoading;

    #endregion

    void Awake()
    {

    }

    void Start()
    {
        #region 変数の初期化

        _enemyCurrentHP = _enemyMaxHP;
        _activeScene = "Title";

        #endregion

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
            SpawnBullet(PoolType.PlayerBullet, Quaternion.identity, 0);
        }

        #region Playerの弾の移動

        for (int i = 0; i < _pdArray[(int)PoolType.PlayerBullet].objectList.Count; i++)
        {
            Transform bulletTransform = _pdArray[(int)PoolType.PlayerBullet].objectList[i].transform;
            bulletTransform.position += bulletTransform.up * Time.deltaTime * _playerBulletSpeed;
            if (bulletTransform.position.x < -_screenSize.x || _screenSize.x < bulletTransform.position.x || bulletTransform.position.y < -_screenSize.y || _screenSize.y < bulletTransform.position.y)
            {
                bulletTransform.position = _trash;
                ReturnBullet((int)PoolType.PlayerBullet, _pdArray[(int)PoolType.PlayerBullet].objectList[i]);
                _pdArray[(int)PoolType.PlayerBullet].objectList.RemoveAt(i);
                i--;
            }
        }

        #endregion

        #endregion

        #region Playerの移動

        #region Playerの移動範囲制限

        _playerPosition = _pt.position;

        _playerPosition.x = Mathf.Clamp(_playerPosition.x, -_screenSize.x, _screenSize.x);
        _playerPosition.y = Mathf.Clamp(_playerPosition.y, -_screenSize.y, _screenSize.y);

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

        _targetPosition.x = Mathf.Clamp(_targetPosition.x, -_screenSize.x, _screenSize.x);
        _targetPosition.y = Mathf.Clamp(_targetPosition.y, -_screenSize.y, _screenSize.y);

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

        if (_enemyCurrentHP > _enemyMaxHP)
        {
            _enemyCurrentHP = _enemyMaxHP;
        }
        else if (_enemyCurrentHP <= 0)
        {
            Debug.Log("敵が倒されたらしいね。お前の勝ち、何で勝ったか明日までに考えといてください");
        }

        #endregion

        #region Enemyの攻撃

        #region Enemyを中心に回転しながら弾をばらまく攻撃

        _enemyMuzzleRotateAttackControlTimer += Time.deltaTime;
        _anglePlus += Time.deltaTime * _enemyMuzzleRotateSpeed;
        _pdArray[(int)PoolType.EnemyBullet].muzzle[0].transform.rotation = Quaternion.Euler(0, 0, _anglePlus);
        if (_enemyMuzzleRotateAttackControlTimer >= _enemyMuzzleRotateAttackInterval)
        {
            SpawnBullet(PoolType.EnemyBullet, _pdArray[(int)PoolType.EnemyBullet].muzzle[0].transform.rotation, 0);
            _enemyMuzzleRotateAttackControlTimer = 0;
        }

        #endregion

        #region Enemyが左右に移動しながら波状に行われる攻撃

        _enemyWaveAttackControlTimer += Time.deltaTime;

        _et.position += new Vector3(_enemyBesideMoveSpeed * Time.deltaTime, 0, 0);
        if (_et.position.x >= _screenSize.x || _et.position.x <= -_screenSize.x)
        {
            _enemyBesideMoveSpeed *= -1;
        }

        if (_enemyWaveAttackControlTimer >= _enemyWaveAttackInterval)
        {
            SpawnBullet(PoolType.EnemyBullet, Quaternion.identity, 0);
            _enemyWaveAttackControlTimer = 0;
        }

        #endregion

        #region 敵の弾の移動

        for (int i = 0; i < _pdArray[(int)PoolType.EnemyBullet].objectList.Count; i++)
        {
            Transform bulletTransform = _pdArray[(int)PoolType.EnemyBullet].objectList[i].transform;
            bulletTransform.position -= bulletTransform.up * Time.deltaTime * _enemyBulletSpeed;
            if (bulletTransform.position.x < -_screenSize.x || _screenSize.x < bulletTransform.position.x || bulletTransform.position.y < -_screenSize.y || _screenSize.y < bulletTransform.position.y)
            {
                bulletTransform.position = _trash;
                ReturnBullet((int)PoolType.EnemyBullet, _pdArray[(int)PoolType.EnemyBullet].objectList[i]);
                _pdArray[(int)PoolType.EnemyBullet].objectList.RemoveAt(i);
                i--;
            }
        }

        #endregion

        #endregion

        #endregion

        #region 当たり判定の処理

        #region Playerと敵の弾

        for (int i = 0; i < _pdArray[(int)PoolType.EnemyBullet].objectList.Count; i++)
        {
            float dx = _pt.position.x - _pdArray[(int)PoolType.EnemyBullet].objectList[i].transform.position.x;
            float dy = _pt.position.y - _pdArray[(int)PoolType.EnemyBullet].objectList[i].transform.position.y;
            float distance = dx * dx + dy * dy;

            if (distance < _playerToEnemyBulletCol)
            {
                ReturnBullet((int)PoolType.EnemyBullet, _pdArray[(int)PoolType.EnemyBullet].objectList[i]);
                _pdArray[(int)PoolType.EnemyBullet].objectList.RemoveAt(i);
                i--;
                _activeScene = "GameOver";
                Debug.Log("グエェェェ！死んだンゴ！");
            }
        }

        #endregion

        #region 敵とPlayerの弾

        for (int i = 0; i < _pdArray[(int)PoolType.PlayerBullet].objectList.Count; i++)
        {
            float dx = _et.position.x - _pdArray[(int)PoolType.PlayerBullet].objectList[i].transform.position.x;
            float dy = _et.position.y - _pdArray[(int)PoolType.PlayerBullet].objectList[i].transform.position.y;
            float distance = dx * dx + dy * dy;

            if (distance < _enemyToPlayerBulletCol)
            {
                ReturnBullet((int)PoolType.PlayerBullet, _pdArray[(int)PoolType.PlayerBullet].objectList[i]);
                _pdArray[(int)PoolType.PlayerBullet].objectList.RemoveAt(i);
                i--;
                _enemyCurrentHP -= 1;
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
    void SpawnBullet(PoolType bulletType, Quaternion Rotate, int index)
    {
        GameObject bullet = GetBullet((int)bulletType);

        _pdArray[(int)bulletType].objectList.Add(bullet);

        bullet.transform.SetPositionAndRotation(_pdArray[(int)bulletType].muzzle[index].position, Rotate);
    }
    #endregion
}
