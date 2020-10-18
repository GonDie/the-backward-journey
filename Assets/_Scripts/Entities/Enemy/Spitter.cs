using System.Collections;
using UnityEngine;

public class Spitter : BaseEnemy
{
    public GameObject spitPrefab;
    public Transform spitOrigin;

    Coroutine _attackRoutine;

    protected override void DoAttack()
    {
        base.DoAttack();

        if (_attackRoutine == null)
            _attackRoutine = StartCoroutine(SpitCoroutine());
    }

    IEnumerator SpitCoroutine()
    {
        yield return new WaitForSeconds(0.3f);

        Instantiate(spitPrefab, spitOrigin.position, Quaternion.identity).GetComponent<Projectile>().Init(Mathf.Sign(_transform.localScale.x));

        yield return new WaitForSeconds(2f);

        _state = EnemyState.Chasing;
        _attackRoutine = null;
    }
}