
using UnityEngine;

public class MeleeSkill : BaseSkill
{
    [SerializeField] float _damage;

    Transform _transform;
    CapsuleCollider2D _collider;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _collider = GetComponent<CapsuleCollider2D>();
    }

    protected override bool DoCast(SimpleEvent callback = null)
    {
        Vector2 origin = _transform.position + Vector3.right * _collider.size.x;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, Vector2.one / 2f, 0f, Vector2.right * Mathf.Sign(_transform.localScale.x), 2f, 1 << LayerMask.NameToLayer("Enemy"));

        for(int i = 0; i < hits.Length; i++)
        {
            hits[i].collider.GetComponent<Health>().DealDamage(_damage);
            callback?.Invoke();
        }

        return true;
    }
}
