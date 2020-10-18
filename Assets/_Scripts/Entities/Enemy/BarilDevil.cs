using System.Collections;
using UnityEngine;

public class BarilDevil : BaseEnemy
{
    [SerializeField] Transform _barrelOrigin;
    [SerializeField] GameObject _barrelPrefab;

    Coroutine _attackRoutine;

    protected override void PatrolingState() { _state = EnemyState.Chasing; }
    protected override void ChasingState() 
    {
        if (_playerTrans == null)
            return;

        _animator.Play("Idle");

        Vector3 direction = _playerTrans.position - _transform.position;
        float distance = direction.magnitude;
        direction /= distance;

        Vector3 scale = _transform.localScale;
        scale.x = -Mathf.Sign(direction.x);
        _transform.localScale = scale;

        if (distance <= _attackRange)
            DoAttack();
    }

    protected override void DoAttack()
    {
        base.DoAttack();

        if(_attackRoutine == null)
            _attackRoutine = StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        _animator.Play("Idle");
        Instantiate(_barrelPrefab, _barrelOrigin.position, Quaternion.identity).GetComponent<Projectile>().Init(_transform.localScale.x);

        yield return new WaitForSeconds(2f);

        _state = EnemyState.Chasing;
        _attackRoutine = null;
    }
}
