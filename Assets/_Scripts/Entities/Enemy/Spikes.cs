using System.Collections;
using UnityEngine;

public class Spikes : BaseEnemy
{
    public float damage;

    float _baseMoveSpeed;

    Coroutine _attackRoutine;

    protected override void Awake()
    {
        base.Awake();

        _baseMoveSpeed = _moveSpeed;
    }

    protected override void PatrolingState()
    {
        _moveSpeed = _baseMoveSpeed;

        base.PatrolingState();
    }

    protected override void ChasingState()
    {
        _moveSpeed = _baseMoveSpeed * 2.5f;

        base.ChasingState();
    }

    protected override void DoAttack()
    {
        base.DoAttack();

        _rigidbody2D.velocity = Vector2.zero;

        if(_attackRoutine == null)
            _attackRoutine = StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        RaycastHit2D hit = Physics2D.CircleCast(_transform.position, 0.5f, _transform.right, 0.5f, 1 << LayerMask.NameToLayer("Player"));

        if(hit.collider != null)
        {
            hit.collider.GetComponent<Health>().DealDamage(damage);
        }

        yield return new WaitForSeconds(0.5f);

        _state = EnemyState.Chasing;
        _attackRoutine = null;
    }
}