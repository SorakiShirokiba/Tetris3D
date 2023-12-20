using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PantallaDeInicio : MonoBehaviour
{
    [SerializeField]
    private Slider dificultad;
    [SerializeField]
    private TextMeshProUGUI textoDificultad;
    [SerializeField]
    private Button inicio;
    private float velocidad = 1f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        dificultad.onValueChanged.AddListener(CambiarTextoDificultad);
    }

    public void CambiarEscena()
    {
        DontDestroyOnLoad(dificultad);
        SceneManager.LoadScene(1);
    }
    void CambiarTextoDificultad(float actual)
    {
        int valor = (int)actual;

        switch (valor)
        {
            case 1: textoDificultad.SetText("Facil"); velocidad = 1f; break;
            case 2: textoDificultad.SetText("Medio"); velocidad = 0.75f; break;
            case 3: textoDificultad.SetText("Dificil"); velocidad = 0.5f; break;
            case 4: textoDificultad.SetText("Borracho"); velocidad = 0.25f; break;
        }
        PlayerPrefs.SetFloat("velocidad", velocidad);
    }
}
