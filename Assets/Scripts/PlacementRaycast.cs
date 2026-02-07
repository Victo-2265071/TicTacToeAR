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
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        GameObject planche = Instantiate(objetAPlacer, hit.point, rotation);

        GameManager.Instance.InitialiserPartie(planche);
        DesactiverARPlaneManager();
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

        if (GameManager.Instance.plancheActuelle != null)
            Destroy(GameManager.Instance.plancheActuelle);

        GameManager.Instance.controlPanel.SetActive(false);

        GameManager.Instance.instructions.text = "Appuyez sur une surface scannée pour placer le plateau.";
        GameManager.Instance.joueurEnCours.text = "";
    }
}
