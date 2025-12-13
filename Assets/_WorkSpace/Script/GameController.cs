using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Transform _tr;

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
