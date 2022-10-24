using System;
using UnityEngine;

public class RoundedCornersModifier : Modifier
{
    [SerializeField] RoundedQuadMesh _mesh;

    public float RoundedEdgeVal { get; set; }

    //public void SetStepper(PlaybookStepper stepper)
    //{
    //    _stepper = stepper;
    //    _stepper.OnValueChanged += StepperOnValueChanged;
    //    _stepper.OnValueSet += OnModify;
    //}

    //public void UnsetStepper()
    //{
    //    if (_stepper == null) return;

    //    _stepper.OnValueChanged -= StepperOnValueChanged;
    //    _stepper.OnValueSet -= OnModify;
    //    _stepper = null;
    //}

    public void StepperOnValueChanged(int val)
    {
        var tempVal = val / 20f;
        var newVal = Mathf.Clamp(tempVal, 0, 0.5f);
        _mesh.RoundEdges = newVal;
        RoundedEdgeVal = newVal;
    }

    //private void OnDestroy()
    //{
    //    UnsetStepper();
    //}
}
