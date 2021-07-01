using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : TriggableObject
{

    public GameObject model;
    public float speed = 3f;
    public float openedHeight;

    private Vector3 _targetPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        _targetPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        model.transform.localPosition = Vector3.Lerp(model.transform.localPosition, _targetPosition, speed * Time.deltaTime);
    }

    public override void OnTrigger()
    {
        base.OnTrigger();

        _targetPosition = new Vector3(0, openedHeight, 0);
    }

    public override void OnUntrigger()
    {
        base.OnUntrigger();

        _targetPosition = Vector3.zero;
    }
}
