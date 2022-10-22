using System;
using UnityEngine;
using Playbook;

public class QuickSizeChooser : MonoBehaviour
{
    public enum Size { S, M, L };

    [SerializeField] Material deselectedMat;
    [SerializeField] Material selectedMat;
    [SerializeField] GameObject[] buttons;
    [SerializeField] bool notForSize = false;

    public Size Value;


    private void Start()
    {
        UpdateButtonColor();
    }

    public void ChooseQuickSize(int index)
    {
        Value = (Size)index;
        UpdateButtonColor();

    }

    public void UpdateButtonColor()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if ((int)Value == i)
            {
                buttons[i].GetComponent<Renderer>().material = selectedMat;
            }
            else
            {
                buttons[i].GetComponent<Renderer>().material = deselectedMat;
            }
        }
    }
}
