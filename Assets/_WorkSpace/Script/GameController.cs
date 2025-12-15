using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Transform _tr;
    PlayerInput _pi;

    [SerializeField]
    GameObject _player;
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

    string _state;
    void Awake()
    {
        _tr = transform;
        _pi = _player.GetComponent<PlayerInput>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        switch(_state)
        {
            case "Title":
                
                break;
            case "InGame":
                
                break;
            case "Result":

                break ;
        }
    }
}
