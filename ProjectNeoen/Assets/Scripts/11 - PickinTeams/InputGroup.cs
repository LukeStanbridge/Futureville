using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class InputGroup : MonoBehaviour
{
    public List<UserInput> userInputs;
    public UserInput selectedInput;
    public Color tabIdle;
    [SerializeField] private Color tabActive;

    public void Subscribe(UserInput input)
    {
        if (userInputs == null)
        {
            userInputs = new List<UserInput>();
        }

        userInputs.Add(input);
    }

    public void OnTabEnter(UserInput input)
    {
        ResetTabs();
        if (selectedInput == null || input != selectedInput)
        {
            input.background.effectColor = tabActive;
            input.background.effectDistance = new Vector2(5, 5);
            selectedInput = input;
        }
    }

    public void OnTabExit(UserInput input)
    {
        ResetTabs();
        if (selectedInput != input) input.background.effectDistance = new Vector2(2, 2);
        selectedInput = null;
    }

    public void ResetTabs()
    {
        foreach (UserInput button in userInputs)
        {
            button.background.effectColor = tabIdle;
            button.background.effectDistance = new Vector2(2, 2);
        }
    }
}
