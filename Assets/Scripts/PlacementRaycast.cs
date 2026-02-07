using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementRaycast : MonoBehaviour
{
    public GameObject objetAPlacer;
    public ARPlaneManager arPlaneManager;

    public ARAnchorManager arAnchorManager;
    private ARAnchor ancreActuelle;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return; // <-- Cette ligne est proposée par l'IA, elle empèche le raycast au travers des UI.

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<ARPlane>() != null) // Si c'est un plane
                {
                    PlacerPlanche(hit);
                }
                if (hit.collider.GetComponent<Case>() != null && GameManager.Instance.partieEnCours) // Si c'est une case
                {
                    GameManager.Instance.JouerCase(hit.collider.GetComponent<Case>());
                }
            }
        }
    }

    void PlacerPlanche(RaycastHit hit)
    {
        // Récupérer le plane AR touché
        ARPlane arPlane = hit.collider.GetComponent<ARPlane>();

        if (arPlane != null)
        {
            // Créer une ancre sur le plane
            Pose anchorPose = new Pose(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            ancreActuelle = arAnchorManager.AttachAnchor(arPlane, anchorPose);

            if (ancreActuelle != null)
            {
                // Instancier la planche comme enfant de l'ancre
                GameObject planche = Instantiate(objetAPlacer, ancreActuelle.transform);
                planche.transform.localPosition = Vector3.zero;
                planche.transform.localRotation = Quaternion.identity;

                GameManager.Instance.InitialiserPartie(planche);
                DesactiverARPlaneManager();
            }
        }
    }

    void DesactiverARPlaneManager()
    {
        arPlaneManager.requestedDetectionMode = PlaneDetectionMode.None; // Ligne proposée par l'IA

        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }

    public void ActiverARPlaneManager()
    {
        arPlaneManager.enabled = true;
        arPlaneManager.requestedDetectionMode = PlaneDetectionMode.Horizontal | PlaneDetectionMode.Vertical; // Ligne proposée par l'IA

        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }

        GameManager.Instance.partieEnCours = false;
        GameManager.Instance.RedemarrerPartieAvecPlancheExistante(); // Nettoie le stock comme il faut avant de déplacer

        // Détruire l'ancre 
        if (ancreActuelle != null)
        {
            Destroy(ancreActuelle.gameObject);
            ancreActuelle = null;
        }

        // Détruire la planche 
        if (GameManager.Instance.plancheActuelle != null)
            Destroy(GameManager.Instance.plancheActuelle);

        GameManager.Instance.controlPanel.SetActive(false);

        GameManager.Instance.instructions.text = "Appuyez sur une surface scannée pour placer le plateau.";
        GameManager.Instance.joueurEnCours.text = "";
    }
}
