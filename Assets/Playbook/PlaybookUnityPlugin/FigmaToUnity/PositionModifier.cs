using System;
using UnityEngine;
using Playbook;

public class PositionModifier : Modifier
{
    //TODO move this to the obj itself
    [SerializeField] private float sensitivity = 1;
    [SerializeField] private GameObject deleteFx;

    const float dragBuffer = 0.2f;
    float timeElapsed;
    private Vector3 posOffset;
    bool isClicked = false;
    bool _isSpawn = false;

    protected override void Awake()
    {
        base.Awake();

    }

    public void OnSpawnDown()
    {
        _isSpawn = true;
    }

}