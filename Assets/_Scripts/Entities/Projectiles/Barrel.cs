using UnityEngine;

public class Barrel : Projectile
{
    [SerializeField] float _barrelStrenght;

    Rigidbody2D _rigidbody2D;

    protected override void Move(float direction) { }

    protected override void OnInit(float direction)
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.AddForce(new Vector2(1f * -direction, 0.1f) * _barrelStrenght);
        _rigidbody2D.AddTorque(_barrelStrenght * direction / 10f);
    }
}
