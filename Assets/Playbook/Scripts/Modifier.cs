using System;
using UnityEngine;
using Playbook;

[RequireComponent(typeof(Element))]
public class Modifier : MonoBehaviour
{
    [SerializeField] public GameObject target;
    protected Element parentElement;

    protected virtual void Awake()
    {
        parentElement = target.GetComponentInParent<Element>();
    }

}