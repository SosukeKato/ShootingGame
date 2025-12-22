using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Transform _tr;
    Transform _pt;
    Transform _tt;

    [SerializeField]
    PlayerInput _pi;

    [SerializeField,Header("Player")]
    GameObject _player;
    [SerializeField,Header("Æ€")]
    GameObject _target;
    [SerializeField,Header("Enemy")]
    GameObject[] _enemy;
    [SerializeField,Header("Player‚ª”­Ë‚·‚éBullet‚ÌPrefab")]
    GameObject _playerBullet;
    [SerializeField,Header("Enemy‚ª”­Ë‚·‚éBullet‚ÌPrefab")]
    GameObject[] _enemyBullet;
    [SerializeField,Header("Stage‚Ì”wŒi‚É‚È‚éImage")]
    Image[] _stage;
    [SerializeField,Header("Title‚Ì”wŒi‚É‚È‚éImage")]
    Image _titleImage;
    [SerializeField,Header("GameOver‚Ì”wŒi‚É‚È‚éImage")]
    Image _gameOverImage;
    [SerializeField,Header("GameClear‚Ì”wŒi‚É‚È‚éImage")]
    Image _gameClearImage;
    [SerializeField,Header("Player‚ÆÆ€‚ÌˆÚ“®‘¬“x")]
    float _moveSpeed;
    [SerializeField,Header("Enemy‚Ìó‘Ô•Ï‰»1’iŠK–Ú‚ªI‚í‚éŠÔ")]
    int _firstFormEndTime;

    Vector2 _playerMoveInput;
    Vector2 _targetMoveInput;

    string _state;

    bool _isPause;

    void Awake()
    {
        _tr = transform;
        _pt = _player.transform;
        _tt = _target.transform;
        _pi = GetComponent<PlayerInput>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        #region Player‚Ìˆ—

        #region Player‚ÌˆÚ“®

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

        #region Æ€‚ÌˆÚ“®

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

        #region Player‚ÌUŒ‚
        if (_pi.actions["PlayerAttack"].WasPressedThisFrame())
        {
            Debug.Log("Attack");
        }
        #endregion

        #endregion
    }
}
