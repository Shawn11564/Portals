using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PressureButton : MonoBehaviour
{

    public TriggableObject targetObject;
    public GameObject model;
    public float pressedHeight = 0f;
    public float pressedDuration = 1f;
    public float pressedSpeed = 3f;

    private Vector3 _targetPosition;
    private float _pressedTimer;
    private bool _pressed;

    // Start is called before the first frame update
    void Start()
    {
        _targetPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Hold button down for a few seconds
        _pressedTimer -= Time.deltaTime;
        if (_pressedTimer > 0)
        {
            _targetPosition = new Vector3(0, pressedHeight, 0);

            if (_pressed == false)
            {
                _pressed = true;
                OnPress();
            }
        }
        else
        {
            _targetPosition = Vector3.zero;
            if (_pressed == true)
            {
                _pressed = false;
                OnUnpress();
            }
        }
        
        // Preform movement of button model
        model.transform.localPosition = Vector3.Lerp(model.transform.localPosition, _targetPosition, pressedSpeed * Time.deltaTime);
    }

    void OnTriggerStay(Collider otherCollider)
    {
        if (otherCollider.GetComponent<Player>() != null || otherCollider.GetComponent<GrabbableObject>() != null)
        {
            _pressedTimer = pressedDuration;
        }
    }

    void OnPress()
    {
        model.GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.green);
        targetObject.OnTrigger();
    }

    void OnUnpress()
    {
        model.GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.red);
        targetObject.OnUntrigger();
    }
}
