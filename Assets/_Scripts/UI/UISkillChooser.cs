using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UISkillChooser : MonoBehaviour
{
    [SerializeField] GameObject _skillPrefab;
    [SerializeField] Transform _skillsContainer;
    [SerializeField] Button _confirmButton;
    [SerializeField] PlayerController _playerController;

    CanvasGroup _cg;
    ToggleGroup _toggleGroup;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        _toggleGroup = GetComponentInChildren<ToggleGroup>();

        Events.OnLevelEnd += OpenSkillSelection;
    }

    private void Update()
    {
        ToggleConfirm(_toggleGroup.AnyTogglesOn());
    }

    private void OnDestroy()
    {
        Events.OnLevelEnd -= OpenSkillSelection;
    }

    void OpenSkillSelection()
    {
        _cg.interactable = true;
        _cg.blocksRaycasts = true;

        //for (int i = 0; i < _skillsContainer.childCount; i++)
        //    Destroy(_skillsContainer.GetChild(i));

        UISkillContainer skillContainer;
        List<BaseSkill> skills = _playerController.Skills.Values.ToList();
        BaseSkill skill;
        for(int i = 0; i < skills.Count; i++)
        {
            skill = skills[i];
            skillContainer = Instantiate(_skillPrefab, _skillsContainer).GetComponent<UISkillContainer>();
            
            if(skill.IsActive)
            {
                skillContainer.canvasGroup.interactable = true;
                skillContainer.canvasGroup.blocksRaycasts = true;
                skillContainer.canvasGroup.alpha = 1f;
            }
            else
            {
                skillContainer.canvasGroup.interactable = false;
                skillContainer.canvasGroup.blocksRaycasts = false;
                skillContainer.canvasGroup.alpha = 0.3f;
            }

            skillContainer.skillId = skill.SkillId;
            skillContainer.toggle.group = _toggleGroup;
            skillContainer.skillName.text = skill.SkillName;
        }

        _cg.DOFade(1f, 1f).Play();
    }

    public void OnConfirm()
    {
        Toggle toggle = _toggleGroup.ActiveToggles().ToArray()[0];
        UISkillContainer skill = toggle.GetComponentInParent<UISkillContainer>();

        _playerController.Skills.Values.First(x => x.SkillId == skill.skillId).Deactivate();

        _cg.interactable = false;
        _cg.blocksRaycasts = false;
        _cg.DOFade(0f, 1f).OnComplete(() =>
        {
            for (int i = 0; i < _skillsContainer.childCount; i++)
                Destroy(_skillsContainer.GetChild(i).gameObject);
        }).Play();

        GameManager.Instance.GoToNextLevel();
    }

    void ToggleConfirm(bool toggle)
    {
        _confirmButton.interactable = toggle;
    }
}
