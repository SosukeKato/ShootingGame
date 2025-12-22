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

    [SerializeField]
    GameObject _player;
    [SerializeField]
    GameObject _target;
    [SerializeField]
    GameObject[] _enemy;
    [SerializeField]
    GameObject _playerBullet;
    [SerializeField]
    GameObject[] _enemyBullet;
    [SerializeField]
    GameObject[] _stage;
    [SerializeField]
    Image _titleImage;
    [SerializeField]
    Image _gameOverImage;
    [SerializeField]
    Image _gameClearImage;
    [SerializeField]
    float _moveSpeed;
    [SerializeField]
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
        #region PlayerÇÃèàóù

        #region PlayerÇÃà⁄ìÆ

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

        #region è∆èÄÇÃà⁄ìÆ

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

        #region PlayerÇÃçUåÇ
        if (_pi.actions["PlayerAttack"].WasPressedThisFrame())
        {
            Debug.Log("Attack");
        }
        #endregion

        #endregion
    }
}
