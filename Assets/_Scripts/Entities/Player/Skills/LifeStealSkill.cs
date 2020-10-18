using UnityEngine;

public class LifeStealSkill : BaseSkill
{
    public float healAmount = 0.2f;

    Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    protected override bool DoCast(SimpleEvent callback = null)
    {
        Debug.Log("Heeal");
        _health.Heal(healAmount);
        return true;
    }
}
