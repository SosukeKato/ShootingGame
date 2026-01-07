using UnityEngine;

public class BulletMove : MonoBehaviour
{
    Transform _tr;

    [SerializeField]
    int _moveSpeed;
    void Start()
    {
        _tr = transform;
    }

    void Update()
    {
        _tr.position += _tr.forward * Time.deltaTime * _moveSpeed;
    }
}
