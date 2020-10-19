
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public enum EnemyState { Patroling, Chasing, Attacking, Idle }

    [SerializeField] protected Transform[] _waypoints;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _moveSpeed;

    protected Transform _transform;
    protected Animator _animator;
    protected Rigidbody2D _rigidbody2D;
    protected Health _health;

    protected EnemyState _state = EnemyState.Patroling;
    protected int _currentWaypoint;
    protected Transform _playerTrans;

    protected virtual void Awake()
    {
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
        _health.OnDeath += (float f) => Destroy(gameObject);
    }

    private void Update()
    {
        switch(_state)
        {
            case EnemyState.Patroling:
                PatrolingState();
            break;
            case EnemyState.Chasing:
                ChasingState();
            break;
            //case EnemyState.Attacking:
            //    AttackingState();
            //break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _playerTrans = collision.GetComponent<Transform>();
            _state = EnemyState.Chasing;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerTrans = null;
            _state = EnemyState.Patroling;
        }
    }

    protected virtual void PatrolingState()
    {
        _animator.Play("Moving");

        Vector3 direction = _waypoints[_currentWaypoint % _waypoints.Length].position - _transform.position;
        float distance = direction.magnitude;
        direction /= distance;

        _rigidbody2D.velocity = new Vector2(direction.x * _moveSpeed * Time.deltaTime, _rigidbody2D.velocity.y);

        Flip();

        if (distance <= 1f)
            _currentWaypoint++;
    }

    protected virtual void ChasingState()
    {
        if(_playerTrans == null)
        {
            _state = EnemyState.Patroling;
            return;
        }

        _animator.Play("Moving");

        Vector3 direction = _playerTrans.position - _transform.position;
        direction.y = 0f;
        float distance = direction.magnitude;
        direction /= distance;

        _rigidbody2D.velocity = new Vector2(direction.x * _moveSpeed * Time.deltaTime, _rigidbody2D.velocity.y);

        Flip();

        if (distance <= _attackRange)
            DoAttack();
    }

    protected virtual void DoAttack() 
    {
        _state = EnemyState.Attacking;
        _animator.Play("Attacking");
    }

    protected virtual void AttackingState() { }

    protected void Flip()
    {
        if (Mathf.Abs(_rigidbody2D.velocity.x) <= 0f)
            return;

        Vector3 scale = _transform.localScale;
        scale.x = Mathf.Sign(_rigidbody2D.velocity.x);
        _transform.localScale = scale;
    }
}
