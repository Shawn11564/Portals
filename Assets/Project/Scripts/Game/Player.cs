using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Portal")]
    public GameObject portalPrefab;
    public RenderTexture[] renderTextures;
    
    [Header("Grabbable Objects")]
    public float grabbingDistance = 2f;
    public float throwingForce = 150f;

    public Action OnCollectOrb;
    
    private GrabbableObject _grabbableObject;
    private Camera _playerCamera;
    private List<Portal> _portals;
    private bool _shouldUseFirstPortal = true;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerCamera = transform.GetComponentInChildren<Camera>();
        _portals = new List<Portal>();
    }

    // Update is called once per frame
    void Update()
    {
        // Clicking logic for grabbing objects and shooting portals
        bool interactedWithObject = false;
        if (Input.GetMouseButtonDown(0))
        {
            // Release grabbable object if holding
            if (_grabbableObject != null)
            {
                Release();
                interactedWithObject = true;
            }
            else
            {
                // Short raycast
                RaycastHit hit;
                if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, grabbingDistance))
                {
                    // Check if looking at grabbable object
                    if (hit.transform.GetComponent<GrabbableObject>() != null)
                    {
                        GrabbableObject targetObject = hit.transform.GetComponent<GrabbableObject>();
                        // Hold the object
                        if (_grabbableObject == null)
                        {
                            Hold(targetObject);
                            interactedWithObject = true;
                        }
                    }
                }
            }
        }
        
        // Logic for spawning portals
        if (Input.GetMouseButtonDown(0) && interactedWithObject == false)
        {
            // Perform the raycast
            RaycastHit hit;
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit))
            {
                if (hit.transform.GetComponent<PortalArea>() != null)
                {
                    // Can spawn portal
                    Vector3 spawnPoint = hit.point;
                    SpawnPortal(hit.point, hit.normal, hit.transform.GetComponent<PortalArea>());
                }
            }
        }
        
        // Logic for holding the grabbable object
        if (_grabbableObject != null)
        {
            _grabbableObject.transform.position = _playerCamera.transform.position + _playerCamera.transform.forward * grabbingDistance;
        }
        
    }

    void Hold(GrabbableObject targetObject)
    {
        _grabbableObject = targetObject;
        _grabbableObject.GetComponent<Collider>().enabled = false;
        _grabbableObject.GetComponent<Rigidbody>().useGravity = false;
    }

    void Release()
    {
        _grabbableObject.GetComponent<Collider>().enabled = true;
        _grabbableObject.GetComponent<Rigidbody>().useGravity = true;
        _grabbableObject.GetComponent<Rigidbody>().AddForce(_playerCamera.transform.forward * throwingForce);
        _grabbableObject = null;
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.GetComponent<Orb>() != null)
        {
            if (OnCollectOrb != null)
            {
                OnCollectOrb();
            }
        }

        if (otherCollider.GetComponent<Portal>() != null && _portals.Count > 1)
        {
            // We've entered a portal
            Portal enterPortal = otherCollider.GetComponent<Portal>();
            Portal exitPortal = enterPortal == _portals[0] ? _portals[1] : _portals[0];
            
            gameObject.GetComponent<CharacterController>().enabled = false;
            transform.position = exitPortal.transform.position + exitPortal.transform.forward;
            transform.forward = exitPortal.transform.forward;
            gameObject.GetComponent<CharacterController>().enabled = true;
            transform.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().CopyRotation(exitPortal.transform);
        }
    }

    void SpawnPortal(Vector3 spawnPoint, Vector3 normal, PortalArea area)
    {
        Portal currentPortal;
        
        // Get which portal should be spawned and set it to currentPortal variable
        if (_portals.Count < 2)
        {
            GameObject portalObject = Instantiate(portalPrefab);

            currentPortal = portalObject.GetComponent<Portal>();
            // Dont use a camera the first time a portal is created
            currentPortal.GetComponentInChildren<Camera>().enabled = false;
            
            _portals.Add(currentPortal);

            if (_portals.Count == 2)
            {
                // Enable the cameras
                _portals[0].GetComponentInChildren<Camera>().enabled = true;
                _portals[1].GetComponentInChildren<Camera>().enabled = true;

                // Set the render textures to each portal
                _portals[0].GetComponentInChildren<Camera>().targetTexture = renderTextures[0];
                _portals[1].GetComponentInChildren<Camera>().targetTexture = renderTextures[1];
                
                // Set texture of renderers to the opposite portal textures
                // Allows seeing what the other portal's camera sees through original portal
                _portals[0].GetComponent<Renderer>().material.SetTexture("_MainTex", renderTextures[1]);
                _portals[1].GetComponent<Renderer>().material.SetTexture("_MainTex", renderTextures[0]);
            }
            
        }
        else
        {
            currentPortal = _portals[_shouldUseFirstPortal ? 0 : 1];
            _shouldUseFirstPortal = !_shouldUseFirstPortal;
        }
        
        currentPortal.transform.position = spawnPoint;
        currentPortal.transform.forward = normal;
    }
}
