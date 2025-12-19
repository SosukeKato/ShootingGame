using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Transform _tr;
    Transform _pt;
    Keyboard _corrent;

    [SerializeField]
    PlayerInput _pi;

    [SerializeField]
    GameObject _player;
    [SerializeField]
    GameObject _bulletTarget;
    [SerializeField]
    GameObject[] _enemy;
    [SerializeField]
    GameObject _playerBullet;
    [SerializeField]
    GameObject[] _enemyBullet;
    [SerializeField]
    GameObject[] _stage;
    [SerializeField]
    Image _tileImage;
    [SerializeField]
    Image _gameOverImage;
    [SerializeField]
    Image _gameClearImage;
    [SerializeField]
    float _moveSpeed;

    Vector2 _moveInput;

    string _state;

    bool _isPause;

    void Awake()
    {
        _tr = transform;
        _pt = _player.transform;
        _pi = _player.GetComponent<PlayerInput>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        _corrent = Keyboard.current;
        if (_corrent == null)
        {
            return;
        }
        if (_pi.actions["PlayerMove"].IsPressed())
        {
            _moveInput = _pi.actions["PlayerMove"].ReadValue<Vector2>();
        }
        else
        {
            _moveInput = new Vector2(0, 0);
        }
        _pt.position += new Vector3(_moveInput.x, _moveInput.y, 0).normalized * Time.deltaTime * _moveSpeed;
    }
}
