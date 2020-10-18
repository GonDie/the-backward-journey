using System.Collections;
using UnityEngine;

public class RangedSkill : BaseSkill
{
    [SerializeField] Transform _arrowOrigin;
    [SerializeField] GameObject _arrowPrefab;

    Transform _transform;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    protected override bool DoCast(SimpleEvent callback = null)
    {
        if (_inCooldown)
            return false;

        StartCoolingDown();
        StartCoroutine(KnockArrow(callback));

        return true;
    }

    IEnumerator KnockArrow(SimpleEvent callback = null)
    {
        yield return new WaitForSeconds(0.3f);

        Instantiate(_arrowPrefab, _arrowOrigin.position, Quaternion.identity).GetComponent<Projectile>().Init(Mathf.Sign(_transform.localScale.x), callback);
    }
}