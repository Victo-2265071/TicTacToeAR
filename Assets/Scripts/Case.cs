using UnityEngine;

public class Case : MonoBehaviour
{
    public char etat = 'n';
    public GameObject symbole;

    public void PlacerSymbole()
    {
        GameObject prefab = (etat == 'x')
            ? GameManager.Instance.symboleX
            : GameManager.Instance.symboleO;

        GameObject obj = Instantiate(prefab, transform.position, transform.rotation);
        symbole = obj;

        GetComponent<Animator>().SetTrigger("caseSelectionnee");
    }
}
