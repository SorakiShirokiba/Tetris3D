using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Piezas
{
    private int ancho;
    private int alto;
    public Color[] coloresPiezas;
    public int RandomPiezas;
    public int RandomColor;
    public string forma;
    public Piezas(int ancho, int alto)
    {
        this.ancho = ancho;
        this.alto = alto;
        
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Colores()
    {
        this.coloresPiezas[0] = Color.blue;
        this.coloresPiezas[1] = Color.red;
        this.coloresPiezas[2] = Color.yellow;
        this.coloresPiezas[3] = Color.green;
    }
    public GameObject[] CrearPiezas()
    {
        RandomPiezas = Random.Range(0, 4);
        RandomColor = Random.Range(0, 4);
        switch (RandomPiezas)
        {
            case 0: forma = "cuadrado"; return Cuadrado(coloresPiezas[RandomColor]);
            case 1: forma = "ele"; return Ele(coloresPiezas[RandomColor]);
            case 2: forma = "te"; return Te(coloresPiezas[RandomColor]);
            case 3: forma = "linea"; return Linea(coloresPiezas[RandomColor]);
            case 4: forma = "escalera"; return Escalera(coloresPiezas[RandomColor]);
            default: return null;
        }



    }
    /// <summary>
    /// Creamos el Cuadrado que vamos a usar para nuestro juego
    /// </summary>
    /// <param name="color">Es el parametro elegido por el random que le dara un color a la pieza</param>
    /// <returns></returns>
    GameObject[] Cuadrado(Color color)
    {
        GameObject[] piezas = new GameObject[4];
        GameObject ParteInferiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2, alto, 0);
        ParteInferiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[0] = ParteInferiorIzquierda;
        GameObject ParteInferiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorDerecha.transform.position = new Vector3(ancho / 2 + 1, alto, 0);
        ParteInferiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[1] = ParteInferiorDerecha;
        GameObject ParteSuperiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorIzquierda.transform.position = new Vector3(ancho / 2, alto + 1, 0);
        ParteSuperiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[2] = ParteSuperiorIzquierda;
        GameObject ParteSuperiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2 + 1, alto + 1, 0);
        ParteSuperiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[3] = ParteSuperiorDerecha;
        return piezas;


    }
    /// <summary>
    /// Creamos la ele que vamos a usar para nuestro juego
    /// </summary>
    /// <param name="color">Es el parametro elegido por el random que le dara un color a la pieza</param>
    /// <returns></returns>
    GameObject[] Ele(Color color)
    {
        GameObject[] piezas = new GameObject[4];
        GameObject ParteInferiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2, alto, 0);
        ParteInferiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[0] = ParteInferiorIzquierda;
        GameObject ParteInferiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorDerecha.transform.position = new Vector3(ancho / 2 + 1, alto, 0);
        ParteInferiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[1] = ParteInferiorDerecha;
        GameObject ParteSuperiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorIzquierda.transform.position = new Vector3(ancho / 2, alto + 1, 0);
        ParteSuperiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[2] = ParteSuperiorIzquierda;
        GameObject ParteSuperiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2, alto + 2, 0);
        ParteSuperiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[3] = ParteSuperiorDerecha;
        return piezas;


    }
    /// <summary>
    ///  Creamos la te que vamos a usar para nuestro juego
    /// </summary>
    /// <param name="color">Es el parametro elegido por el random que le dara un color a la pieza</param>
    /// <returns></returns>
    GameObject[] Te(Color color)
    {
        GameObject[] piezas = new GameObject[4];
        GameObject ParteInferiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2 + 1, alto, 0);
        ParteInferiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[0] = ParteInferiorIzquierda;
        GameObject ParteInferiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorDerecha.transform.position = new Vector3(ancho / 2, alto + 1, 0);
        ParteInferiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[1] = ParteInferiorDerecha;
        GameObject ParteSuperiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorIzquierda.transform.position = new Vector3(ancho / 2 + 1, alto + 1, 0);
        ParteSuperiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[2] = ParteSuperiorIzquierda;
        GameObject ParteSuperiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2 + 2, alto + 1, 0);
        ParteSuperiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[3] = ParteSuperiorDerecha;
        return piezas;


    }
    GameObject[] Linea(Color color)
    {
        GameObject[] piezas = new GameObject[4];
        GameObject ParteInferiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2, alto, 0);
        ParteInferiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[0] = ParteInferiorIzquierda;
        GameObject ParteInferiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorDerecha.transform.position = new Vector3(ancho / 2, alto + 1, 0);
        ParteInferiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[1] = ParteInferiorDerecha;
        GameObject ParteSuperiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorIzquierda.transform.position = new Vector3(ancho / 2, alto + 2, 0);
        ParteSuperiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[2] = ParteSuperiorIzquierda;
        GameObject ParteSuperiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2, alto + 3, 0);
        ParteSuperiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[3] = ParteSuperiorDerecha;
        return piezas;


    }
    GameObject[] Escalera(Color color)
    {
        GameObject[] piezas = new GameObject[4];
        GameObject ParteInferiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2 + 1, alto, 0);
        ParteInferiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[0] = ParteInferiorIzquierda;
        GameObject ParteInferiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorDerecha.transform.position = new Vector3(ancho / 2 + 1, alto + 1, 0);
        ParteInferiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[1] = ParteInferiorDerecha;
        GameObject ParteSuperiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorIzquierda.transform.position = new Vector3(ancho / 2, alto + 1, 0);
        ParteSuperiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[2] = ParteSuperiorIzquierda;
        GameObject ParteSuperiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2, alto + 2, 0);
        ParteSuperiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[3] = ParteSuperiorDerecha;
        return piezas;


    }
}
