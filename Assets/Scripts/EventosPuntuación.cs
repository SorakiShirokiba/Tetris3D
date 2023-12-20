using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventosPuntuación : MonoBehaviour
{
    private void OnEnable()
    {
        Tetris.ActualizarPuntuacion += actualizarPuntuacion;
    }
    private void OnDisable()
    {
        Tetris.ActualizarPuntuacion -= actualizarPuntuacion;
    }

    void actualizarPuntuacion(int puntuacion)
    {
        GetComponent<TextMeshProUGUI>().text = $"Puntuación: {puntuacion}";
    }
}
