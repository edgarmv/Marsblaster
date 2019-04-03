using System;
using UnityEngine;

// In order to interact with objects in the scene
// this class casts a ray into the scene and if it finds
// a VRInteractiveItem it exposes it for other classes to use.
// This script should be generally be placed on the camera.
public class ControllerRaycaster : MonoBehaviour
{
    public event Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user's gaze is over a collider.


    [SerializeField] private Transform m_Camera;
    [SerializeField] private LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
    [SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.
    [SerializeField] private bool m_ShowDebugRay;                   // Optionally show the debug ray.
    [SerializeField] private float m_DebugRayLength = 5f;           // Debug ray length.
    [SerializeField] private float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
    [SerializeField] private float m_RayLength = 500f;              // How far into the scene the ray is cast.
    [SerializeField] private LineRenderer m_LineRenderer = null;
    public bool ShowLineRenderer = true;
    [SerializeField] private Transform m_TrackingSpace = null;

    
    private VRInteractiveItem m_CurrentInteractible;                //The current interactive item
    private VRInteractiveItem m_LastInteractible;                   //The last interactive item
    private VRInteractiveItem m_SelectedInteractible;
    private Vector3 m_SelectedInteractibleOriginalPosition;


    // Utility for other classes to get the current interactive item
    public VRInteractiveItem CurrentInteractible
    {
        get { return m_CurrentInteractible; }
    }

    public bool ControllerIsConnected {
        get {
            OVRInput.Controller controller = OVRInput.GetConnectedControllers() & (OVRInput.Controller.LTrackedRemote | OVRInput.Controller.RTrackedRemote);
    return controller == OVRInput.Controller.LTrackedRemote || controller == OVRInput.Controller.RTrackedRemote;
        }
    }

    public OVRInput.Controller Controller {
        get {
            OVRInput.Controller controller = OVRInput.GetConnectedControllers();
            if ((controller & OVRInput.Controller.LTrackedRemote) == OVRInput.Controller.LTrackedRemote) {
                return OVRInput.Controller.LTrackedRemote;
            } else if ((controller & OVRInput.Controller.RTrackedRemote) == OVRInput.Controller.RTrackedRemote) {
                return OVRInput.Controller.RTrackedRemote;
            }
            return OVRInput.GetActiveController ();
        }
    }

    
    private void OnEnable()
    {
        //OVRInput.OnClick += HandleClick;
        //OVRInput.OnDoubleClick += HandleDoubleClick;
        //OVRInput.OnUp += HandleUp;
        //OVRInput.OnDown += HandleDown;
    }


    private void OnDisable ()
    {
        //OVRInput.OnClick -= HandleClick;
        //OVRInput.OnDoubleClick -= HandleDoubleClick;
        //OVRInput.OnUp -= HandleUp;
        //OVRInput.OnDown -= HandleDown;
    }


    private void Update()
    {
        EyeRaycast();
    }

  
    private void EyeRaycast()
    {
        Debug.Log("eye1");
        // Show the debug ray if required
        if (m_ShowDebugRay)
        {
            Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
        }

        // Create a ray that points forwards from the camera.
        Ray ray = new Ray(m_Camera.position, m_Camera.forward);
        RaycastHit hit;

        Vector3 worldStartPoint = Vector3.zero;
        Vector3 worldEndPoint = Vector3.zero;

        if (m_LineRenderer != null) {
            m_LineRenderer.enabled = ControllerIsConnected && ShowLineRenderer;
        }
        Debug.Log("eye2");

        if (ControllerIsConnected && m_TrackingSpace != null) {
            Matrix4x4 localToWorld = m_TrackingSpace.localToWorldMatrix;
            Quaternion orientation = OVRInput.GetLocalControllerRotation (Controller);

            Vector3 localStartPoint = OVRInput.GetLocalControllerPosition (Controller);
            Vector3 localEndPoint = localStartPoint + ((orientation * Vector3.forward) * 500.0f);

            worldStartPoint = localToWorld.MultiplyPoint(localStartPoint);
            worldEndPoint = localToWorld.MultiplyPoint(localEndPoint);
            Debug.Log("Update worldEndPoint");

            // Create new ray
            ray = new Ray(worldStartPoint, worldEndPoint - worldStartPoint);
        }
        Debug.Log("eye3");

        // Update selected object
        if (m_SelectedInteractible && m_SelectedInteractible.menuItem == false) {
            m_SelectedInteractible.gameObject.transform.position = worldStartPoint + ray.direction;
            m_SelectedInteractible.gameObject.transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
        }

        // Put selected object back
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) && m_SelectedInteractible && m_SelectedInteractible.menuItem == false) {
            m_SelectedInteractible.transform.position = m_SelectedInteractibleOriginalPosition;
            m_SelectedInteractible = null;
        }
        Debug.Log("eye4");
        
        // Do the raycast forweards to see if we hit an interactive item
        if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
        {
            VRInteractiveItem interactible = hit.collider.GetComponent<VRInteractiveItem>(); //attempt to get the VRInteractiveItem on the hit object
            if (interactible) {
                if (m_SelectedInteractible == null && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)) { 
                    m_SelectedInteractible = interactible;
                    m_SelectedInteractibleOriginalPosition = m_SelectedInteractible.gameObject.transform.position;
                    Material material = new Material(Shader.Find("Diffuse"));
                    material.mainTexture = m_SelectedInteractible.TheoryTexture;
                    m_SelectedInteractible.TheoryScreen.GetComponent<Renderer>().material = material;
                }
                worldEndPoint = hit.point;
                Debug.Log("Update worldEndPoint hit");
            }
            m_CurrentInteractible = interactible;
            Debug.Log("eye5");


            // If we hit an interactive item and it's not the same as the last interactive item, then call Over
            if (interactible && interactible != m_LastInteractible) {
                interactible.Over(); 
            }

            // Deactive the last interactive item 
            if (interactible != m_LastInteractible)
                DeactiveLastInteractible();

            m_LastInteractible = interactible;

            // Something was hit, set at the hit position.
            if (m_Reticle)
                m_Reticle.SetPosition(hit);
            Debug.Log("eye6");

            if (OnRaycasthit != null)
                OnRaycasthit(hit);
        }
        else
        {
            // Nothing was hit, deactive the last interactive item.
            DeactiveLastInteractible();
            m_CurrentInteractible = null;

            // Position the reticle at default distance.
            if (m_Reticle)
                m_Reticle.SetPosition(ray.origin, ray.direction);
        }
        if (ControllerIsConnected && m_LineRenderer != null) {
            Debug.Log("eye" + worldEndPoint.x + " " + worldEndPoint.y + " " + worldEndPoint.z);
            m_LineRenderer.SetPosition(0, worldStartPoint);
            m_LineRenderer.SetPosition(1, worldEndPoint);
        }
        Debug.Log("eye7");

    }


    private void DeactiveLastInteractible()
    {
        if (m_LastInteractible == null)
            return;

        m_LastInteractible.Out();
        m_LastInteractible = null;
    }


    private void HandleUp()
    {
        if (m_CurrentInteractible != null)
            m_CurrentInteractible.Up();
    }


    private void HandleDown()
    {
        if (m_CurrentInteractible != null)
            m_CurrentInteractible.Down();
    }


    private void HandleClick()
    {
        if (m_CurrentInteractible != null)
            m_CurrentInteractible.Click();
    }


    private void HandleDoubleClick()
    {
        if (m_CurrentInteractible != null)
            m_CurrentInteractible.DoubleClick();

    }
}
