using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public char joueur = 'x';
    public bool partieEnCours = true;

    public Case[] cases;

    public GameObject symboleX;
    public GameObject symboleO;

    public TextMeshProUGUI instructions;
    public TextMeshProUGUI joueurEnCours;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    public GameObject controlPanel;

    public GameObject plancheActuelle;

    void Awake()
    {
        // Crée un singleton du GameManager.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RedemarrerPartieAvecPlancheExistante()
    {
        InitialiserPartie(null);
    }

    public void InitialiserPartie(GameObject planche)
    {
        if (!plancheActuelle)
        {
            plancheActuelle = planche;
        }

        cases = null;
        cases = plancheActuelle.GetComponentsInChildren<Case>();

        foreach (Case c in cases)
        {
            c.etat = 'n';
            if (c.symbole != null)
            {
                Destroy(c.symbole);
                c.symbole = null;
            }
        }

        joueur = 'x';
        partieEnCours = true;
        instructions.text = "À vous de jouer! Appuyez sur une case pour placer votre symbole.";
        joueurEnCours.text = "Au tour de " + joueur;
        controlPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void JouerCase(Case caseJoue)
    {
        if (caseJoue.etat == 'n')
        {
            caseJoue.etat = joueur;
            caseJoue.PlacerSymbole();

            if (VerifierVictoire(joueur))
            {
                PartieTerminee($"Le joueur {joueur} a gagné!");
            }
            else if (VerifierEgalite())
            {
                PartieTerminee("Égalité!");
            }
            else
            {
                joueur = (joueur == 'x') ? 'o' : 'x';
                joueurEnCours.text = "Au tour de " + joueur;
            }
        }
    }

    void PartieTerminee(string message)
    {
        gameOverPanel.SetActive(true);
        partieEnCours = false;
        joueurEnCours.text = "";
        gameOverText.text = message;
        instructions.text = "Appuyez sur 'Redémarrer la partie' pour recommencer";
        Debug.Log(message);
    }

    bool VerifierVictoire(char symbole)
    {
        int[][] combinaisons = new int[][]
        {
        new int[] {0, 1, 2}, // Ligne 1
        new int[] {3, 4, 5}, // Ligne 2
        new int[] {6, 7, 8}, // Ligne 3
        new int[] {0, 3, 6}, // Colonne 1
        new int[] {1, 4, 7}, // Colonne 2
        new int[] {2, 5, 8}, // Colonne 3
        new int[] {0, 4, 8}, // Diagonale \
        new int[] {2, 4, 6}  // Diagonale /
        };

        foreach (int[] combo in combinaisons)
        {
            if (cases[combo[0]].etat == symbole &&
                cases[combo[1]].etat == symbole &&
                cases[combo[2]].etat == symbole)
            {
                // Tagguer les cases gagnantes
                foreach (int index in combo)
                {
                    cases[index].symbole.GetComponent<Animator>().SetTrigger("gagnante");
                }
                return true;
            }
        }

        return false;
    }

    bool VerifierEgalite()
    {
        // Il est primordial de tester si un joueur a gagné AVANT de tester l'égalité!
        foreach (Case c in cases)
        {
            if (c.etat == 'n') return false; // Il reste des cases vides
        }
        return true; // Toutes les cases sont remplies
    }

}
