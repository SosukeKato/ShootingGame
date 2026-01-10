using UnityEngine;

public class Player : MonoBehaviour
{
    Transform _tr;

    [SerializeField]
    Transform _target;

    Vector2 _playerPosition;
    Vector2 _targetPosition;
    Vector2 _fieldClampX;
    Vector2 _fieldClampY;
    void Awake()
    {
        _tr = transform;
    }

    void Start()
    {
        
    }

    void Update()
    {
        #region à⁄ìÆîÕàÕêßå¿
        _playerPosition.x = Mathf.Clamp(_playerPosition.x, _fieldClampX.x, _fieldClampX.y);
        _playerPosition.y = Mathf.Clamp(_playerPosition.y, _fieldClampY.x, _fieldClampY.y);
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, _fieldClampX.x, _fieldClampX.y);
        _targetPosition.y = Mathf.Clamp(_targetPosition.y, _fieldClampY.x, _fieldClampY.y);

        _tr.position = _playerPosition;
        _target.position = _targetPosition;
        #endregion


    }
}
