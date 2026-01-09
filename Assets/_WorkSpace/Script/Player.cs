using UnityEngine;

public class Player : MonoBehaviour
{
    Transform _tr;

    Vector2 _playerPosition;
    Vector2 _clampX;
    Vector2 _clampY;
    void Awake()
    {
        _tr = transform;
    }

    void Start()
    {
        
    }

    void Update()
    {
        _playerPosition.x = Mathf.Clamp(_playerPosition.x, _clampX.x, _clampX.y);
        _playerPosition.y = Mathf.Clamp(_playerPosition.y, _clampY.x, _clampY.y);

        _tr.position = new Vector2(_playerPosition.x, _playerPosition.y);
    }
}
