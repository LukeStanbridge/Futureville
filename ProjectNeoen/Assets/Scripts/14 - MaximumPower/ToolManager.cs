using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private List<DisplayLabel> _displayLabels;
    [field: SerializeField] public DisplayLabel CurrentTool { get; private set; }
    public void SetCurrentTool(DisplayLabel label) => CurrentTool = label;

    private void Awake()
    {
        foreach (DisplayLabel label in _displayLabels)
        {
            label.SetManager(this);
        }
    }

    public void DeactivateTool()
    {
        if (CurrentTool != null)
        {
            CurrentTool.HideLabel();
            CurrentTool = null;
        }
    }

    public void HideActiveTool()
    {
        if(CurrentTool != null) CurrentTool.HideLabel();
    }

    public void ShowHiddenTool()
    {
        if (CurrentTool != null) CurrentTool.ShowLabel();
    }
}
