using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using TMPro;

public class PlaybookText : Element
{
    public enum TextSize { H1, H2, H3, Custom };
    public enum ParagraphAlignment { Left, Center, Right };
    public enum FontWeight { Bold, Normal, Italic, BoldItalic };

    [SerializeField] TextMeshPro textMeshPro;
    [SerializeField] TextModifier textModifier;

    public TextSize FontSize => _textSize;

    public List<string> fontFamilyList;
    public int fontFamily;
    public float customFontSize;
    public string labelTxt;
    public TextSize _textSize;
    public FontWeight fontWeight;
    public ParagraphAlignment alignment;

    BoxCollider boxCollider;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        labelTxt = "Hello Playbook";
        fontWeight = FontWeight.Normal;

        boxCollider = GetComponentInChildren<BoxCollider>();

        GetComponent<TextModifier>().OnTextModify += OnTextModify;
    }

    // Update is called once per frame
    void Update()
    {
        //Alert for legibility
        /*
        if (Vector3.Distance(Camera.main.transform.position, transform.position) > 2.2f || Vector3.Distance(Camera.main.transform.position, transform.position) < 0.22f)
        {
            print("ALERT: Text is not legible");
        }
        */

        textMeshPro.text = labelTxt;

        //ALIGNMENT
        textMeshPro.alignment = alignment switch
        {
            ParagraphAlignment.Right => TextAlignmentOptions.Right,
            ParagraphAlignment.Center => TextAlignmentOptions.Center,
            ParagraphAlignment.Left => TextAlignmentOptions.Left,
            _ => throw new NotImplementedException(),
        };

        //TEXT SIZE
        textMeshPro.fontSize = _textSize switch
        {
            TextSize.H1 => 1f,
            TextSize.H2 => 0.66f,
            TextSize.H3 => 0.33f,
            TextSize.Custom => customFontSize,
            _ => throw new NotImplementedException(),
        };

        SizeCollider();
    }

    void OnTextModify(string savedText)
    {
        labelTxt = savedText;
    }

    public void SetQuickSize(QuickSizeChooser.Size size)
    {
        var textSize = (TextSize)size;
        _textSize = textSize;
        textModifier.ChooserOnValueChanged(textSize);
    }

    void SizeCollider()
    {
        var renderer = GetComponentInChildren<MeshRenderer>();

        boxCollider.center = boxCollider.transform.InverseTransformPoint(renderer.bounds.center);
        boxCollider.size = 
            boxCollider.transform.InverseTransformDirection(renderer.bounds.size / (transform.lossyScale.magnitude / 1.9f));
    }
}
