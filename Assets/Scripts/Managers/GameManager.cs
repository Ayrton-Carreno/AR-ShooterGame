using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Progresión")]
    public int nivelActual = 1;
    public int nivelMaximo = 4;

    [Header("Tiempo límite por nivel (segundos)")]
    public float[] tiemposPorNivel = { 90f, 75f, 60f, 45f };

    [Header("Estado actual")]
    public EstadoJuego estado = EstadoJuego.Explorando;

    private int enemigosVivos = 0;
    private float timerNivel = 0f;
    private bool timerActivo = false;

    private UIManager uiManager;
    private PlayerStats playerStats;
    private PlayerShooter playerShooter;

    public enum EstadoJuego
    {
        Explorando,
        EnCombate,
        Victoria,
        Derrota
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        uiManager = UIManager.Instance;
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerShooter = FindFirstObjectByType<PlayerShooter>();
    }

    void Update()
    {
        if (!timerActivo) return;

        timerNivel -= Time.deltaTime;

        if (timerNivel <= 0f)
        {
            timerNivel = 0f;
            timerActivo = false;
            Debug.Log("¡Tiempo agotado!");
            EstadoDerrota();
        }
    }

    public void IniciarEncuentro(int cantidadEnemigos)
    {
        if (estado != EstadoJuego.Explorando) return;

        estado = EstadoJuego.EnCombate;
        enemigosVivos = cantidadEnemigos;

        timerNivel = tiemposPorNivel[nivelActual - 1];
        timerActivo = true;

        playerShooter.ActivarDisparo();

        Debug.Log($"Encuentro nivel {nivelActual} iniciado — Enemigos: {cantidadEnemigos}, Tiempo: {timerNivel}s");
    }

    public void EnemigoEliminado()
    {
        if (estado != EstadoJuego.EnCombate) return;

        enemigosVivos--;
        Debug.Log($"Enemigo eliminado. Quedan: {enemigosVivos}");

        if (enemigosVivos <= 0)
        {
            EncuentroCompletado();
        }
    }

    private void EncuentroCompletado()
    {
        timerActivo = false;
        playerShooter.DesactivarDisparo();

        Debug.Log($"¡Nivel {nivelActual} completado!");

        if (nivelActual >= nivelMaximo)
        {
            EstadoVictoria();
        }
        else
        {
            uiManager.MostrarNivelCompletado();
            nivelActual++;
            estado = EstadoJuego.Explorando;
            Debug.Log($"Avanzando al nivel {nivelActual}. Busca la siguiente imagen.");
        }
    }

    public void EstadoVictoria()
    {
        uiManager.MostrarVictoria();
        estado = EstadoJuego.Victoria;
        timerActivo = false;
        playerShooter.DesactivarDisparo();
        Debug.Log("¡Victoria! Completaste todos los niveles.");
    }

    public void EstadoDerrota()
    {
        uiManager.MostrarDerrota();
        estado = EstadoJuego.Derrota;
        timerActivo = false;
        playerShooter.DesactivarDisparo();
        Debug.Log("¡Derrota!");
    }

    public string GetTiempoRestante()
    {
        int segundos = Mathf.CeilToInt(timerNivel);
        int min = segundos / 60;
        int seg = segundos % 60;
        return $"{min:00}:{seg:00}";
    }

    public bool NivelEsAccesible(int nivelImagen)
    {
        return nivelImagen == nivelActual;
    }
}