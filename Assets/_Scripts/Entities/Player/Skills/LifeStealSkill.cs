using UnityEngine;

public class LifeStealSkill : BaseSkill
{
    protected override bool DoCast(SimpleEvent callback = null)
    {
        Debug.Log("Life Stolen");
        return true;
    }
}
