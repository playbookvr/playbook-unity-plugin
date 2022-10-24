using System;
using UnityEngine;
using TMPro;
using Playbook;

public class ColorModifier : Modifier
{
    public Color currentColor;

    protected override void Awake()
    {
        base.Awake();

        //Update color displays
        if (target.GetComponent<TextMeshPro>())
        {
            currentColor = target.GetComponent<TextMeshPro>().color;
        } else
        {
            currentColor = target.GetComponent<Renderer>().material.color;
        }
    }

    private void Start()
    {
        //ColorChooserOnValueChanged(currentColor);
    }

    public void UpdateThemeColors()
    {
        var elementType = GetComponent<Element>().elementType;

        //switch (elementType)
        //{
        //    case ElementType.Button:
        //        currentColor = ThemesManager.themeAccentColor;
        //        break;
        //    case ElementType.Panel:
        //        currentColor = ThemesManager.themeMainColor;
        //        break;
        //    case ElementType.Text:
        //        currentColor = ThemesManager.themeTextColor;
        //        break;
        //    case ElementType.Stepper:
        //        currentColor = ThemesManager.themeMainColor;
        //        break;
        //    case ElementType.Loader:
        //        currentColor = ThemesManager.themeMainColor;
        //        break;
        //    case ElementType.Toggle:
        //        currentColor = GetComponent<PlaybookToggle>().GetOn() ? ThemesManager.themeAccentColor : Color.white;
        //        GetComponent<PlaybookToggle>().UpdateColors(ThemesManager.themeAccentColor, Color.white);
        //        break;
        //    case ElementType.Slider:
        //        currentColor = ThemesManager.themeAccentColor;
        //        break;
        //    case ElementType.Dropdown:
        //        currentColor = ThemesManager.themeMainColor;
        //        GetComponent<PlaybookDropdown>().UpdateColors(ThemesManager.themeMainColor, ThemesManager.themeAccentColor, ThemesManager.themeTextColor);
        //        break;
        //}
        //UpdateColor();
    }

    private void ColorChooserOnValueSet()
    {
        //OnModify();
    }

    //private void ColorChooserOnValueChanged(Color color)
    //{
    //    currentColor = color;
    //    target.GetComponent<Renderer>().material.SetColor("Color_DB15D303", color);
    //    target.GetComponent<Renderer>().material.color = color;

    //    if (target.GetComponent<TextMeshPro>())
    //    {
    //        target.GetComponent<TextMeshPro>().color = currentColor;
    //        target.GetComponent<TextMeshPro>().outlineColor = currentColor;
    //    }
    //    else if (target.GetComponentInParent<PlaybookLoaders>())
    //    {
    //        foreach(GameObject t in target.GetComponentInParent<PlaybookLoaders>().targets)
    //        {
    //            t.GetComponent<Renderer>().material.color = color;
    //        }
    //    }
    //    else if (target.GetComponentInParent<PlaybookButton>())
    //    {
    //        target.GetComponentInParent<PlaybookButton>().UpdateDualColors();
    //    }

    //    if (GetComponent<PlaybookToggle>())
    //    {
    //        GetComponent<PlaybookToggle>().OverrideColor(currentColor);
    //    }

    //    // Update options panel visual
    //    OptionPageController.INSTANCE.GetCurrentPage().UpdatePanelColor(color);
    //}

    //public void UpdateColor()
    //{
    //    target.GetComponent<Renderer>().material.SetColor("Color_DB15D303", currentColor);
    //    target.GetComponent<Renderer>().material.color = currentColor;

    //    if (target.GetComponent<TextMeshPro>())
    //    {
    //        target.GetComponent<TextMeshPro>().color = currentColor;
    //        target.GetComponent<TextMeshPro>().outlineColor = currentColor;
    //    }
    //    else if (target.GetComponentInParent<PlaybookLoaders>())
    //    {
    //        foreach (GameObject t in target.GetComponentInParent<PlaybookLoaders>().targets)
    //        {
    //            t.GetComponent<Renderer>().material.color = currentColor;
    //        }

    //    } else if(target.GetComponentInParent<PlaybookButton>())
    //    {
    //        target.GetComponentInParent<PlaybookButton>().UpdateDualColors();
    //    }

    //    if (GetComponent<PlaybookToggle>())
    //    {
    //        GetComponent<PlaybookToggle>().OverrideColor(currentColor);
    //    }
    //    //Update color displays
    //    ColorChooserOnValueChanged(currentColor);
    //    ColorWheelManager.CurrColorWheel.SetColor(currentColor);
    //}

    private void OnDestroy()
    {
        //RemoveColorChooser();
    }

    //public void AddColorChooser()
    //{
    //    ColorWheelManager.CurrColorWheel.ValueChanged += ColorChooserOnValueChanged;
    //    ColorWheelManager.CurrColorWheel.OnValueSet += ColorChooserOnValueSet;
    //}

    //public void RemoveColorChooser()
    //{
    //    if (ColorWheelManager.CurrColorWheel == null) return;

    //    ColorWheelManager.CurrColorWheel.ValueChanged -= ColorChooserOnValueChanged;
    //    ColorWheelManager.CurrColorWheel.OnValueSet -= ColorChooserOnValueSet;
    //}
}