using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Tetris : MonoBehaviour
{
    // Ampliaciones tener las 7 piezas, La bomba, y que si la fila es del mismo color puntue mas
    [Range(4, 20)]
    public int ancho = 10;
    [Range(10, 22)]
    public int alto = 10;
    private GameObject[] piezaActual = new GameObject[4];
    private GameObject[,] piezasTablero;
    private Color[] coloresPiezas = new Color[4];
    private int RandomPiezas = 0;
    private int RandomColor = 0;
    [SerializeField]
    private float intervalo;
    public KeyCode arriba = KeyCode.UpArrow;
    public KeyCode abajo = KeyCode.DownArrow;
    public KeyCode derecha = KeyCode.RightArrow;
    public KeyCode izquierda = KeyCode.LeftArrow;
    public KeyCode rotar = KeyCode.Space;
    private bool anclado = false;
    private string forma = "";
    private GameObject bombaActual;
    private float probabilidad = 1f;
    private bool esBomba = false;
    private Coroutine bajar;
    [SerializeField]
    private Material materialBomba;
    private int puntuacion = 0;
    public delegate void EventoPuntuacion(int puntuacion);
    public static event EventoPuntuacion ActualizarPuntuacion;
    public delegate void EventoFinal(int puntuacion);
    public static event EventoFinal ActualizarFinal;
    [SerializeField]
    private AudioSource efectoSonidoGirar;
    [SerializeField]
    private AudioSource efectoSonidoLinea;
    [SerializeField]
    private AudioSource efectoSonidoFinal;
    [SerializeField]
    private AudioSource melodia;
    [Range(0, 1)]
    public float profundidad = 0;
    private float profundidadRandom = 0;

    // Start is called before the first frame update
    void Start()
    {
        intervalo = PlayerPrefs.GetFloat("velocidad");
        CrearTablero();
        Colores();
        probabilidad = Random.value;
        if (probabilidad <= 0.1)
        {
            bombaActual = CrearBomba();
            esBomba = true;
        }
        else
        {
            piezaActual = CrearPiezas();
            esBomba = false;
        }
        piezasTablero = new GameObject[alto, ancho + 1];
        bajar = StartCoroutine(BajarAutomatico());
        ActualizarPuntuacion?.Invoke(puntuacion);


    }

    // Update is called once per frame
    void Update()
    {
        ajustarCamara();

        if (Input.GetKeyDown(abajo))
        {
            MoverAbajo();
        }
        if (Input.GetKeyDown(derecha))
        {
            MoverDerecha();
        }
        if (Input.GetKeyDown(izquierda))
        {
            MoverIzquierda();
        }
        if (Input.GetKeyDown(arriba))
        {
            StopCoroutine(bajar);
            BajarInmediatamente();
        }
        if (anclado) anclado = false;
        if (Input.GetKeyDown(rotar))
        {
            if (!esBomba && PuedeRotar(RotarPieza()))
            {
                Vector3[] comprobar = RotarPieza();
                for (int i = 0; i < piezaActual.Length; i++)
                {
                    piezaActual[i].transform.position = comprobar[i];
                }
                efectoSonidoGirar.Play();
            }
        }

    }
    /// <summary>
    /// Este Metodo se gasta para crear el tablero, a traves de los parametros establecidos por el usuario
    /// </summary>
    void CrearTablero()
    {
        float AnadirX;
        float AnadirY;
        AnadirX = ancho % 2 == 0 ? 1f : 1.5f;
        AnadirY = alto % 2 == 0 ? 1f : 0f;
        if (ancho % 2 == 0)
        {
            GameObject suelo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            suelo.transform.position = new Vector3(ancho / 2, -1f, 0);
            suelo.transform.localScale = new Vector3(ancho + AnadirX, 1, 1);
        }
        else
        {
            GameObject suelo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            suelo.transform.position = new Vector3(ancho / 2 + 0.5f, -1f, 0);
            suelo.transform.localScale = new Vector3(ancho + AnadirX, 1, 1);
        }
        GameObject paredDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        paredDerecha.transform.position = new Vector3(ancho + 1f, alto / 2 - 1f, 0);
        paredDerecha.transform.localScale = new Vector3(1, alto + AnadirY, 1);
        GameObject paredIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        paredIzquierda.transform.position = new Vector3(-1f, alto / 2 - 1f, 0);
        paredIzquierda.transform.localScale = new Vector3(1, alto + AnadirY, 1);
    }
    /// <summary>
    /// Creamos el array que usaremos para darle color a las piezasTablero.
    /// </summary>
    void Colores()
    {
        coloresPiezas[0] = Color.blue;
        coloresPiezas[1] = Color.red;
        coloresPiezas[2] = Color.yellow;
        coloresPiezas[3] = Color.green;
    }
    GameObject[] CrearPiezas()
    {
        RandomPiezas = Random.Range(0, 5);
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

    GameObject CrearBomba()
    {
        GameObject bomba;
        bomba = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bomba.transform.GetComponent<Renderer>().material = materialBomba;
        bomba.transform.GetComponent<Renderer>().material.color = Color.red;
        bomba.transform.position = new(ancho / 2, alto, 0);
        return bomba;
    }
    /// <summary>
    /// Creamos el Cuadrado que vamos a usar para nuestro juego
    /// </summary>
    /// <param name="color">Es el parametro elegido por el random que le dara un color a la pieza</param>
    /// <returns></returns>
    GameObject[] Cuadrado(Color color)
    {
        profundidadRandom = Random.Range(0, profundidad);
        GameObject[] piezas = new GameObject[4];
        GameObject ParteInferiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2, alto, profundidadRandom);
        ParteInferiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[0] = ParteInferiorIzquierda;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteInferiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorDerecha.transform.position = new Vector3(ancho / 2 + 1, alto, profundidadRandom);
        ParteInferiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[1] = ParteInferiorDerecha;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteSuperiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorIzquierda.transform.position = new Vector3(ancho / 2, alto + 1, profundidadRandom);
        ParteSuperiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[2] = ParteSuperiorIzquierda;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteSuperiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2 + 1, alto + 1, profundidadRandom);
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
        int girada;
        girada = Random.Range(1, 3);
        profundidadRandom = Random.Range(0, profundidad);
        GameObject[] piezas = new GameObject[4];
        GameObject ParteInferiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2, alto, profundidadRandom);
        ParteInferiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[0] = ParteInferiorIzquierda;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteInferiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (girada == 1)
        {
            ParteInferiorDerecha.transform.position = new Vector3(ancho / 2 + 1, alto, profundidadRandom);
        }
        else
        {
            ParteInferiorDerecha.transform.position = new Vector3(ancho / 2 - 1, alto, profundidadRandom);
        }
        ParteInferiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[1] = ParteInferiorDerecha;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteSuperiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorIzquierda.transform.position = new Vector3(ancho / 2, alto + 1, profundidadRandom);
        ParteSuperiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[2] = ParteSuperiorIzquierda;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteSuperiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2, alto + 2, profundidadRandom);
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
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteInferiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2 + 1, alto, profundidadRandom);
        ParteInferiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[0] = ParteInferiorIzquierda;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteInferiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorDerecha.transform.position = new Vector3(ancho / 2, alto + 1, profundidadRandom);
        ParteInferiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[1] = ParteInferiorDerecha;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteSuperiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorIzquierda.transform.position = new Vector3(ancho / 2 + 1, alto + 1, profundidadRandom);
        ParteSuperiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[2] = ParteSuperiorIzquierda;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteSuperiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2 + 2, alto + 1, profundidadRandom);
        ParteSuperiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[3] = ParteSuperiorDerecha;
        return piezas;


    }
    GameObject[] Linea(Color color)
    {
        GameObject[] piezas = new GameObject[4];
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteInferiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2, alto, profundidadRandom);
        ParteInferiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[0] = ParteInferiorIzquierda;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteInferiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorDerecha.transform.position = new Vector3(ancho / 2, alto + 1, profundidadRandom);
        ParteInferiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[1] = ParteInferiorDerecha;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteSuperiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorIzquierda.transform.position = new Vector3(ancho / 2, alto + 2, profundidadRandom);
        ParteSuperiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[2] = ParteSuperiorIzquierda;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteSuperiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2, alto + 3, profundidadRandom);
        ParteSuperiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[3] = ParteSuperiorDerecha;
        return piezas;


    }
    GameObject[] Escalera(Color color)
    {
        int girada;
        girada = Random.Range(1, 3);
        GameObject[] piezas = new GameObject[4];
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteInferiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (girada == 1)
        {
            ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2 + 1, alto, profundidadRandom);
        }
        else
        {
            ParteInferiorIzquierda.transform.position = new Vector3(ancho / 2, alto, profundidadRandom);
        }
        ParteInferiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[0] = ParteInferiorIzquierda;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteInferiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteInferiorDerecha.transform.position = new Vector3(ancho / 2 + 1, alto + 1, profundidadRandom);
        ParteInferiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[1] = ParteInferiorDerecha;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteSuperiorIzquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ParteSuperiorIzquierda.transform.position = new Vector3(ancho / 2, alto + 1, profundidadRandom);
        ParteSuperiorIzquierda.transform.GetComponent<Renderer>().material.color = color;
        piezas[2] = ParteSuperiorIzquierda;
        profundidadRandom = Random.Range(0, profundidad);
        GameObject ParteSuperiorDerecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (girada == 1)
        {
            ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2, alto + 2, profundidadRandom);
        }
        else
        {
            ParteSuperiorDerecha.transform.position = new Vector3(ancho / 2 + 1, alto + 2, profundidadRandom);
        }
        ParteSuperiorDerecha.transform.GetComponent<Renderer>().material.color = color;
        piezas[3] = ParteSuperiorDerecha;
        return piezas;


    }
    /// <summary>
    /// Este metodo nos centra la camara en el centro geometrico de el tablero
    /// </summary>
    private void ajustarCamara()
    {
        Vector3 posicionCamara = new(ancho / 2, alto / 2, -ancho/2);
        float distanciaCamara = alto + (alto / Camera.main.aspect);
        Camera.main.transform.position = posicionCamara - Vector3.forward * distanciaCamara;
    }
    /// <summary>
    /// Condicion con la cual nos dira si puede  moverse hacia abajo o tiene que pararse dicha pieza
    /// </summary>
    /// <param name="x">Es el position x de cada parte de la pieza</param>
    /// <returns></returns>
    bool MoverAbajo()
    {
        if (!esBomba)
        {
            if (ComprobacionesAbajo())
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector3 Movimiento = new(piezaActual[i].transform.position.x, piezaActual[i].transform.position.y - 1, piezaActual[i].transform.position.z);
                    piezaActual[i].transform.position = Movimiento;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (ComprobacionesAbajo())
            {
                Vector3 Movimiento = new(bombaActual.transform.position.x, bombaActual.transform.position.y - 1, bombaActual.transform.position.z);
                bombaActual.transform.position = Movimiento;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool MoverDerecha()
    {
        if (!esBomba)
        {
            if (ComprobacionesDerecha())
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector3 Movimiento = new(piezaActual[i].transform.position.x + 1, piezaActual[i].transform.position.y, piezaActual[i].transform.position.z);
                    piezaActual[i].transform.position = Movimiento;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (ComprobacionesDerecha())
            {
                Vector3 Movimiento = new(bombaActual.transform.position.x + 1, bombaActual.transform.position.y, bombaActual.transform.position.z);
                bombaActual.transform.position = Movimiento;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool MoverIzquierda()
    {
        if (!esBomba)
        {
            if (ComprobacionesIzquierda())
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector3 Movimiento = new(piezaActual[i].transform.position.x - 1, piezaActual[i].transform.position.y, piezaActual[i].transform.position.z);
                    piezaActual[i].transform.position = Movimiento;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (ComprobacionesIzquierda())
            {
                Vector3 Movimiento = new(bombaActual.transform.position.x - 1, bombaActual.transform.position.y, bombaActual.transform.position.z);
                bombaActual.transform.position = Movimiento;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool ComprobacionesDerecha()
    {
        if (!esBomba)
        {
            if (piezaActual[3].transform.position.y < alto - 1)
            {

                if (piezaActual[0].transform.position.x < ancho && piezaActual[1].transform.position.x < ancho && piezaActual[2].transform.position.x < ancho && piezaActual[3].transform.position.x < ancho)
                {
                    if (ComprobarCeroDerecha(piezaActual[0]) && ComprobarCeroDerecha(piezaActual[1]) && ComprobarCeroDerecha(piezaActual[2]) && ComprobarCeroDerecha(piezaActual[3]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (piezaActual[0].transform.position.x < ancho && piezaActual[1].transform.position.x < ancho && piezaActual[2].transform.position.x < ancho && piezaActual[3].transform.position.x < ancho)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (bombaActual.transform.position.y < alto - 1)
            {
                if (bombaActual.transform.position.x < ancho)
                {
                    if (ComprobarCeroDerecha(bombaActual))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (bombaActual.transform.position.x < ancho)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool ComprobacionesIzquierda()
    {
        if (!esBomba)
        {
            if (piezaActual[3].transform.position.y < alto - 1)
            {

                if (piezaActual[0].transform.position.x > 0 && piezaActual[1].transform.position.x > 0 && piezaActual[2].transform.position.x > 0 && piezaActual[3].transform.position.x > 0)
                {
                    if (ComprobarCeroIzquierda(piezaActual[0]) && ComprobarCeroIzquierda(piezaActual[1]) && ComprobarCeroIzquierda(piezaActual[2]) && ComprobarCeroIzquierda(piezaActual[3]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (piezaActual[0].transform.position.x > 0 && piezaActual[1].transform.position.x > 0 && piezaActual[2].transform.position.x > 0 && piezaActual[3].transform.position.x > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (bombaActual.transform.position.y < alto - 1)
            {
                if (bombaActual.transform.position.x > 0)
                {
                    if (ComprobarCeroIzquierda(bombaActual))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (bombaActual.transform.position.x > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    bool ComprobacionesAbajo()
    {
        if (!esBomba)
        {
            if (piezaActual[3].transform.position.y < alto && piezaActual[2].transform.position.y < alto && piezaActual[1].transform.position.y < alto && piezaActual[0].transform.position.y < alto)
            {

                if (piezaActual[0].transform.position.y > 0 && piezaActual[1].transform.position.y > 0 && piezaActual[2].transform.position.y > 0 && piezaActual[3].transform.position.y > 0)
                {
                    if (ComprobarCeroAbajo(piezaActual[0]) && ComprobarCeroAbajo(piezaActual[1]) && ComprobarCeroAbajo(piezaActual[2]) && ComprobarCeroAbajo(piezaActual[3]))
                    {
                        Debug.Log("Entro");
                        return true;
                    }

                    else
                    {
                        AnclarPieza();
                        anclado = true;
                        return false;
                    }
                }
                else
                {
                    AnclarPieza();
                    anclado = true;
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (bombaActual.transform.position.y < alto)
            {
                if (bombaActual.transform.position.y > 0)
                {
                    if (ComprobarCeroAbajo(bombaActual))
                    {
                        return true;
                    }
                    else
                    {
                        AnclarPieza();
                        anclado = true;
                        return false;
                    }
                }
                else
                {
                    AnclarPieza();
                    anclado = true;
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }


    void BajarInmediatamente()
    {
        while (!anclado && !esBomba)
        {
            if (ComprobacionesAbajo())
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector3 Movimiento = new(piezaActual[i].transform.position.x, piezaActual[i].transform.position.y - 1, piezaActual[i].transform.position.z);
                    piezaActual[i].transform.position = Movimiento;
                }

            }
        }
        while (!anclado && esBomba)
        {
            if (ComprobacionesAbajo())
            {

                Vector3 Movimiento = new(bombaActual.transform.position.x, bombaActual.transform.position.y - 1, bombaActual.transform.position.z);
                bombaActual.transform.position = Movimiento;


            }
        }
    }

    void AnclarPieza()
    {
        if (!esBomba)
        {
            if (piezaActual[0].transform.position.y >= alto - 1 || piezaActual[1].transform.position.y >= alto - 1 || piezaActual[2].transform.position.y >= alto - 1 || piezaActual[3].transform.position.y >= alto - 1)
            {
                Destroy(piezaActual[0]);
                Destroy(piezaActual[1]);
                Destroy(piezaActual[2]);
                Destroy(piezaActual[3]);
                StopCoroutine(bajar);
                ActualizarFinal?.Invoke(puntuacion);
                efectoSonidoFinal.Play();
                melodia.Stop();

            }
            else
            {
                int x = (int)piezaActual[0].transform.position.x;
                int y = (int)piezaActual[0].transform.position.y;
                piezasTablero[y, x] = piezaActual[0];
                x = (int)piezaActual[1].transform.position.x;
                y = (int)piezaActual[1].transform.position.y;
                piezasTablero[y, x] = piezaActual[1];
                x = (int)piezaActual[2].transform.position.x;
                y = (int)piezaActual[2].transform.position.y;
                piezasTablero[y, x] = piezaActual[2];
                x = (int)piezaActual[3].transform.position.x;
                y = (int)piezaActual[3].transform.position.y;
                piezasTablero[y, x] = piezaActual[3];
                ComprobarLinea(0);
                probabilidad = Random.value;
                if (probabilidad <= 0.1)
                {
                    bombaActual = CrearBomba();
                    esBomba = true;
                }
                else
                {
                    piezaActual = CrearPiezas();
                    esBomba = false;
                }
                StopCoroutine(bajar);
                bajar = StartCoroutine(BajarAutomatico());
            }
        }
        else
        {
            AnclarBomba();
            probabilidad = Random.value;
            if (probabilidad <= 0.1)
            {
                bombaActual = CrearBomba();
                esBomba = true;
            }
            else
            {
                piezaActual = CrearPiezas();
                esBomba = false;
            }
            StopCoroutine(bajar);
            bajar = StartCoroutine(BajarAutomatico());
        }

    }

    bool ComprobarCeroAbajo(GameObject pieza)
    {if(pieza.transform.position.y == 0)
        {
            return piezasTablero[(int)pieza.transform.position.y, (int)pieza.transform.position.x] == null;
        }
        else
        {
            return piezasTablero[(int)pieza.transform.position.y - 1, (int)pieza.transform.position.x] == null;
        }

    }
    bool ComprobarCeroDerecha(GameObject pieza)
    {
        if(pieza.transform.position.x == ancho)
        {
            return piezasTablero[(int)pieza.transform.position.y, (int)pieza.transform.position.x] == null;
        }
        else
        {
            return piezasTablero[(int)pieza.transform.position.y, (int)pieza.transform.position.x + 1] == null;
        }
    }
    bool ComprobarCeroIzquierda(GameObject pieza)
    {
        if(pieza.transform.position.x == 0)
        {
            return piezasTablero[(int)pieza.transform.position.y, (int)pieza.transform.position.x] == null;
        }
        else
        {
            return piezasTablero[(int)pieza.transform.position.y, (int)pieza.transform.position.x - 1] == null;
        }
    }
    void ComprobarLinea(int y)
    {
        int cont = 0;
        bool repetir = false;
        if (y == alto - 1)
        {
            return;
        }
        else
        {
            for (int i = 0; i < piezasTablero.GetLength(1); i++)
            {
                if (piezasTablero[y, i] != null)
                {

                    cont++;
                }
            }
            if (cont == ancho + 1)
            {
                repetir = true;
                DestruirLinea(y);
                BajarPiezas(y + 1);
            }
            if (repetir)
            {
                ComprobarLinea(y);
            }
            else
            {
                ComprobarLinea(y + 1);
            }
        }
    }
    void DestruirLinea(int y)
    {

        bool mismoColor = LineaDelMismoColor(y);

        for (int i = 0; i < piezasTablero.GetLength(1); i++)
        {
            Destroy(piezasTablero[y, i]);
            piezasTablero[y, i] = null;
        }
        Puntuacion(mismoColor);
        ActualizarPuntuacion?.Invoke(puntuacion);
        efectoSonidoLinea.Play();
    }
    void BajarPiezas(int y)
    {

        if (y == alto)
        {
            return;
        }
        else
        {
            for (int i = 0; i < piezasTablero.GetLength(1); i++)
            {
                if (y > 0)
                {
                    if (piezasTablero[y, i] != null)
                    {
                        Vector3 Movimiento = new(piezasTablero[y, i].transform.position.x, piezasTablero[y, i].transform.position.y - 1, piezasTablero[y, i].transform.position.z);
                        piezasTablero[y, i].transform.position = Movimiento;
                        piezasTablero[y - 1, i] = piezasTablero[y, i];
                        piezasTablero[y, i] = null;

                    }
                }
            }
            BajarPiezas(++y);
        }
    }
    IEnumerator BajarAutomatico()
    {
        if (!esBomba)
        {

            while (ComprobacionesAbajo())
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector3 Movimiento = new(piezaActual[i].transform.position.x, piezaActual[i].transform.position.y - 1, piezaActual[i].transform.position.z);
                    piezaActual[i].transform.position = Movimiento;

                }
                yield return new WaitForSeconds(intervalo);
            }
            if (!ComprobacionesAbajo())
            {
                AnclarPieza();
            }
        }
        else
        {
            while (ComprobacionesAbajo())
            {
                Vector3 Movimiento = new(bombaActual.transform.position.x, bombaActual.transform.position.y - 1, bombaActual.transform.position.z);
                bombaActual.transform.position = Movimiento;
                yield return new WaitForSeconds(intervalo);
            }
            if (!ComprobacionesAbajo())
            {
                AnclarPieza();
            }
        }


    }
    Vector3[] RotarPieza()
    {
        Vector3 centro = piezaActual[2].transform.position;
        Vector3[] rotacion = new Vector3[4];
        for (int i = 0; i < piezaActual.Length; i++)
        {
            Vector3 posicion = piezaActual[i].transform.position - centro;
            rotacion[i] = new Vector3(-posicion.y + centro.x, posicion.x + centro.y, piezaActual[i].transform.position.z);
        }
        return rotacion;

    }
    bool PuedeRotar(Vector3[] rotacion)
    {
        bool rotar = true;
        for (int i = 0; i < rotacion.Length; i++)
        {
            if (rotacion[i].x <= 0 || rotacion[i].y <= 0 || rotacion[i].x >= ancho && (piezasTablero[(int)rotacion[i].x - 1 , (int)rotacion[i].y -1] != null || piezasTablero[(int)rotacion[i].x + 1, (int)rotacion[i].y + 1] != null))

            {
                rotar = false;
            }
        }
        return rotar;
    }
    void AnclarBomba()
    {
        Vector3 Posicion = bombaActual.transform.position;
        int y = (int)Posicion.y;
        if (y - 1 >= 0) DestruirLinea(y - 1);
        Destroy(bombaActual);
        BajarPiezas(y);

    }
    void Puntuacion(bool color)
    {
        switch (intervalo)
        {
            case 1f:
                if (color)
                {
                    puntuacion += 100 * 2;
                }
                else { puntuacion += 100; }
                break;
            case 0.75f:
                if (color)
                {
                    puntuacion += 100 * 4;
                }
                else { puntuacion += 100 * 2; }
                break;
            case 0.5f:
                if (color)
                {
                    puntuacion += 100 * 8;
                }
                else { puntuacion += 100 * 4; }
                break;
            case 0.25f:
                if (color)
                {
                    puntuacion += 100 * 12;
                }
                else { puntuacion += 100 * 6; }
                break;
        }
    }
    bool LineaDelMismoColor(int y)
    {
        bool mismoColor = true;
        if (!esBomba)
        {
            Color colorFila = piezasTablero[y, 0].transform.GetComponent<Renderer>().material.color;
            for (int i = 0; i < piezasTablero.GetLength(1); i++)
            {
                if (piezasTablero[y, i].transform.GetComponent<Renderer>().material.color != colorFila && mismoColor)
                {
                    mismoColor = false;
                }

            }
        }
        else if (esBomba && y - 1 < 0)
        {
            return false;
        }
        else if (esBomba && y - 1 >= 0)
        {
            Color colorFila = piezasTablero[y - 1, (int)bombaActual.transform.position.x].transform.GetComponent<Renderer>().material.color;
            for (int i = 0; i < piezasTablero.GetLength(1); i++)
            {
                if (piezasTablero[y - 1, i] != null && piezasTablero[y - 1, i].transform.GetComponent<Renderer>().material.color != colorFila && mismoColor)
                {
                    mismoColor = false;
                }

            }
        }
        return mismoColor;
    }
}

