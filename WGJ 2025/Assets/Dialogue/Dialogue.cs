using System.Collections; // ← Necesario para IEnumerator
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogoInicio : MonoBehaviour
{
    public GameObject panelDialogo;
    public TMP_Text textoDialogo;
    public string[] lineas;
    public float velocidadEscritura = 0.05f;

    private int indiceLinea = 0;

    void Start()
    {
        panelDialogo.SetActive(true);
        StartCoroutine(MostrarLinea());
    }


    IEnumerator MostrarLinea()
    {
        textoDialogo.text = "";
        foreach (char letra in lineas[indiceLinea].ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        // Espera antes de pasar a la siguiente línea
        yield return new WaitForSeconds(1.5f);

        if (indiceLinea < lineas.Length - 1)
        {
            indiceLinea++;
            StartCoroutine(MostrarLinea());
        }
        else
        {
            panelDialogo.SetActive(false); // Oculta el panel al terminar
        }
    }

    public void SiguienteLinea()
    {
        if (indiceLinea < lineas.Length - 1)
        {
            indiceLinea++;
            StartCoroutine(MostrarLinea());
        }
        else
        {
            panelDialogo.SetActive(false);
        }
    }
}
