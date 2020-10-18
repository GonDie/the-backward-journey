using System.Collections;
using UnityEngine;

public abstract class BaseSkill : MonoBehaviour
{
    [SerializeField] bool _isActive = true;
    [SerializeField] string _skillName;
    public string SkillName { get => SkillName; }
    [SerializeField] float _cooldownDuration;
    float _cooldown;

    protected bool _inCooldown;

    public void Deactivate()
    {
        _isActive = false;
    }

    public bool Cast()
    {
        StartCoroutine(Cooldown());
        return _isActive && DoCast();
    }

    protected abstract bool DoCast();
    
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(_cooldownDuration);

        _inCooldown = false;
    }
}
