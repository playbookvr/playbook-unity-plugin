using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playbook;
using UnityEditor;
using System.IO;
using UnityEngine.Serialization;

public enum ElementType
{
    Button,
    Panel,
    Text,
    Image,
    Toggle,
    Slider,
    Stepper,
    Loader,
    GesturePrompt,
    Dropdown,
    Asset
}

public enum ElementInteraction
{
    Interactive,
    NonInteractive
}

public class Element : MonoBehaviour
{
    [SerializeField] public ElementType elementType;
    [SerializeField] public ElementInteraction elementInteraction;
    [SerializeField] protected bool isElement = true;

    public event Action Selected, Deselected;

    public Vector3 _originalScale;
    
    public bool IsSelected { get; private set; }
    public string Name { get; private set; }

    protected virtual void Awake()
    {
        _originalScale = transform.localScale;

        if (!isElement) return;

        //var count = ElementManager.IncreaseElementCount(elementType);
        //SetName(count);
    }

    protected virtual void Start()
    {
        // Object is used as Playbook UI
        if (!isElement)
        {
            // Make object not exportable
            gameObject.tag = "Untagged";
        }
        
        //UpdateTheme();
        //if(optionTooltipPanel) optionTooltipPanel.SetActive(false);
    }

    private void OnDestroy()
    {
    }

    public void OnSelect()
    {
        //if (IsSelected) return;

        //if (ModeManager.GameMode == ModeManager.Mode.Preview) return;

        //// Mark object as selected
        //IsSelected = true;
        //ElementManager.SelectElement(this);

        //Selected?.Invoke();

        //// Only do the rest if in design mode
        //if (ModeManager.GameMode == ModeManager.Mode.Prototype) return;

        //// tooltip tutorial
        //if (optionTooltipPanel)
        //{
        //    optionTooltipPanel.SetActive(false);
        //    ToolTipsManager.UpdateTooltipTarget?.Invoke(ToolTipsManager.TooltipTarget.OptionPanel, optionTooltipPanel);
        //    ToolTipsManager.ShowTooltip?.Invoke("Tutorial_ComponentOptions");
        //    if (hasColorOption && ToolTipsManager.GetTooltipShownStatus("Tutorial_ComponentOptions"))
        //    {
        //        ToolTipsManager.ShowTooltip?.Invoke("Tutorial_ColorWheel");
        //    }
        //}

        ////Set this selected object to current text object
        //if (TryGetComponent(out TextModifier t))
        //{
        //    KeyboardManager.INSTANCE.ToggleKeyboard(true);
        //    t.LinkTextToKeyboard();
        //} 
        //else
        //{
        //    KeyboardManager.INSTANCE.ToggleKeyboard(false);
        //}

        //// Sound
        //PlaybookAudioManager.INSTANCE.Play("click");
        
        //// Haptics
        //HapticsManager.singleton.TriggerVibration(1);
    }

    //public void OnDeselect()
    //{
    //    //IsSelected = false;

    //    //if (ModeManager.GameMode != ModeManager.Mode.Edit)
    //    //    return;

    //    //Deselected?.Invoke();
    //}
   
    void SetName(int count)
    {
        Name = $"{elementType}#{count}";
        gameObject.name = Name;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    //public void UpdateTheme()
    //{
    //    if (TryGetComponent(out ColorModifier c))
    //        c.UpdateThemeColors();
    //    if (elementType == ElementType.Image)
    //        GetComponentInChildren<PlaybookImage>().UpdateTexture();
    //}

    //public virtual void OnSpawn()
    //{
    //    ElementManager.elements.Add(gameObject);

    //    OnSelect();
    //    GetComponent<PositionModifier>().OnSpawnDown();

    //    if (TryGetComponent(out ColorModifier cm))
    //        cm.UpdateThemeColors();
    //}

    //public void OnGrabSpawn()
    //{
    //    ElementManager.elements.Add(gameObject);
    //    OnSelect();

    //    if (TryGetComponent(out ColorModifier cm))
    //        cm.UpdateThemeColors();
    //}

    //public void OnSpawnEnd()
    //{
    //    GetComponent<PositionModifier>().OnClickUp();
    //}

    //public Color GetElementColor()
    //{
    //    if (TryGetComponent(out ColorModifier cm))
    //        return cm.currentColor;

    //    return Color.black;
    //}

    //public void SetElementColor(Color c)
    //{
    //    if (TryGetComponent(out ColorModifier cm))
    //        cm.currentColor = c;
    //}
}