using System;
using TMPro;
using UnityEngine;

public class TextModifier : Modifier
{
    [SerializeField] int h1Size, h2Size, h3Size;
    
    private TextMeshPro _text;
    private BoxCollider boxCollider;
    private Vector3 originalColliderSize;
    private string savedText;

    public Action<string> OnTextModify;

    public bool textHasPlaceholder;
    private float textSizeNumeric;
    public PlaybookText.TextSize textSize;

    private bool _finishedInitialization;
    
    protected override void Awake()
    {
        base.Awake();
        textHasPlaceholder = true;
        _text = target.GetComponent<TextMeshPro>();

        if(GetComponent<BoxCollider>() != null)
        {
            boxCollider = target.GetComponent<BoxCollider>();
            originalColliderSize = boxCollider.size;
        }
        
        if (textSizeNumeric > 0)
        {
            SetTextSize(textSizeNumeric);
        }
    }

    protected void Start()
    {
        _finishedInitialization = true;
        
        SetText(savedText);
    }

    public void ChooserOnValueChanged(PlaybookText.TextSize size)
    {
        //OnModify();
        SetTextSize(size);
    }

    //public void LinkTextToKeyboard()
    //{
    //    KeyboardManager.INSTANCE.ToggleKeyboard(true);
    //    KeyboardManager.INSTANCE.LinkKeyboardToText(this);
    //}

    //private void OnDestroy()
    //{
    //    KeyboardManager.INSTANCE.ToggleKeyboard(false);
    //}

    void SetTextSize(PlaybookText.TextSize size)
    {
        _text.fontSize = size switch
        {
            PlaybookText.TextSize.H1 => h1Size,
            PlaybookText.TextSize.H2 => h2Size,
            PlaybookText.TextSize.H3 => h3Size,
            _ => throw new NotImplementedException(),
        };

        textSize = size;
        textSizeNumeric = _text.fontSize;

        if (boxCollider != null)
        {
            boxCollider.size = originalColliderSize * (_text.fontSize / h1Size);
        } 
    }

    public void SetTextSize(float size)
    {
        textSizeNumeric = size;
        if (_text != null)
        {
            _text.fontSize = size;
        }

        if (boxCollider != null)
        {
            boxCollider.size = originalColliderSize * (_text.fontSize / h1Size);
        }
    }

    public void ResetText()
    {
        savedText = "";
        _text.text = savedText;

        textHasPlaceholder = false;

        OnTextModify?.Invoke(savedText);
    }

    public void AddToText(string c)
    {
        savedText += c;
        _text.text = savedText;

        textHasPlaceholder = true;

        OnTextModify?.Invoke(savedText);
    }

    public void RemoveFromText()
    {
        if (savedText.Length == 0) return;

        savedText = savedText.Remove(savedText.Length - 1);
        _text.text = savedText;

        if (savedText.Length == 0)
            textHasPlaceholder = false;

        OnTextModify?.Invoke(savedText);
    }

    public void SetText(string text)
    {
        savedText = text;
        if (savedText != null && _finishedInitialization)
            _text.text = savedText;
    }
    
    public string GetText()
    {
        return _text.text;
    }
    
}