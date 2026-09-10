using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    public Slider barraVida;
    public TextMeshProUGUI textoTimer;

    [Header("Pantallas")]
    public GameObject winState;
    public GameObject lossState;
    public GameObject nextLevel;

    [Header("UI por Target")]
    public GameObject[] botonesCompatir;
    public GameObject[] mensajesNoDisponible;

    private GameManager gameManager;
    private PlayerStats playerStats;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        playerStats = FindFirstObjectByType<PlayerStats>();

        if (barraVida != null)
        {
            barraVida.maxValue = playerStats.vidaMaxima;
            barraVida.value = playerStats.vidaActual;
        }

        if (winState != null) winState.SetActive(false);
        if (lossState != null) lossState.SetActive(false);
        if (nextLevel != null) nextLevel.SetActive(false);
    }

    void Update()
    {
        ActualizarBarraVida();
        ActualizarTimer();
    }

    private void ActualizarBarraVida()
    {
        if (barraVida != null && playerStats != null)
        {
            barraVida.value = playerStats.vidaActual;
        }
    }

    private void ActualizarTimer()
    {
        if (textoTimer != null && gameManager.estado == GameManager.EstadoJuego.EnCombate)
        {
            textoTimer.text = gameManager.GetTiempoRestante();
        }
        else if (textoTimer != null)
        {
            textoTimer.text = "";
        }
    }

    public void MostrarVictoria()
    {
        if (winState != null) winState.SetActive(true);
    }

    public void MostrarDerrota()
    {
        if (lossState != null) lossState.SetActive(true);
    }

    public void MostrarNivelCompletado()
    {
        if (nextLevel != null)
        {
            nextLevel.SetActive(true);
            Invoke("OcultarNivelCompletado", 2f);
        }
    }

    private void OcultarNivelCompletado()
    {
        if (nextLevel != null) nextLevel.SetActive(false);
    }

    public void OcultarTodosLosBotones()
    {
        foreach (GameObject boton in botonesCompatir)
        {
            if (boton != null) boton.SetActive(false);
        }

        foreach (GameObject mensaje in mensajesNoDisponible)
        {
            if (mensaje != null) mensaje.SetActive(false);
        }
    }
}