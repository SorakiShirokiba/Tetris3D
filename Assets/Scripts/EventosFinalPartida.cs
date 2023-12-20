using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventosFinalPartida : MonoBehaviour
{
    private void OnEnable()
    {
        Tetris.ActualizarFinal += actualizarFinal;
    }
    private void OnDisable()
    {
        Tetris.ActualizarFinal -= actualizarFinal;
    }

    void actualizarFinal(int puntuacion)
    {
        GetComponent<TextMeshProUGUI>().text = $"FIN DE LA PARTIDA TU PUNTUACIÓN: {puntuacion}";
    }
}
