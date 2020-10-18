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

    public bool Cast(SimpleEvent callback = null)
    {
        return _isActive && DoCast(callback);
    }

    protected abstract bool DoCast(SimpleEvent callback = null);
    
    protected void StartCoolingDown()
    {
        _inCooldown = true;
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(_cooldownDuration);

        _inCooldown = false;
    }
}
