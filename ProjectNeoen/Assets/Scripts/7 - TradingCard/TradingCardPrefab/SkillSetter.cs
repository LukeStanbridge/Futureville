using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class SkillSetter : MonoBehaviour
{
    public List<GameObject> Skills;
    public GameObject SkillDescriptionPrefab;
    public RectTransform SkillsSection;
    public RectTransform HeadingSection;
    public RectTransform Content;

    private int _childIndex;
    private float _headingHeight;
    private float _contentHeight;

    private void Awake()
    {
        int i = 0;
        foreach(Transform t in SkillDescriptionPrefab.transform)
        {
            if (t.name == "SkillText") _childIndex = i;
            i++;
        }

        _headingHeight = HeadingSection.sizeDelta.y;
    }

    public void AssignCardSkills(List<string> skills)
    {
        foreach (string skill in skills)
        {
            GameObject skillDescription = Instantiate(SkillDescriptionPrefab, Content);
            skillDescription.transform.GetChild(_childIndex).GetComponent<TextMeshProUGUI>().text = skill;
            TextMeshProUGUI text = skillDescription.transform.GetChild(_childIndex).GetComponent<TextMeshProUGUI>();
            float skillDescriptionHeight = text.preferredHeight;
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(text.GetComponent<RectTransform>().sizeDelta.x, skillDescriptionHeight);
            skillDescription.GetComponent<RectTransform>().sizeDelta = new Vector2(skillDescription.GetComponent<RectTransform>().sizeDelta.x, skillDescriptionHeight);
            _contentHeight += skillDescriptionHeight;
        }
        Content.sizeDelta = new Vector2(Content.sizeDelta.x, _contentHeight);
        SkillsSection.sizeDelta = new Vector2(SkillsSection.sizeDelta.x, _contentHeight + _headingHeight);
    }
}


