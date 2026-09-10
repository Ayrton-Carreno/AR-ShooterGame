using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Configuración")]
    public int vidaMaxima = 100;

    [Header("Estado actual")]
    public int vidaActual;

    [Header("Feedback visual")]
    public GameObject damageFlash;
    public float duracionFlash = 0.2f;

    private float timerFlash = 0f;
    private bool flashActivo = false;
    private GameManager gameManager;

    void Start()
    {
        vidaActual = vidaMaxima;
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        if (flashActivo)
        {
            timerFlash -= Time.deltaTime;
            if (timerFlash <= 0f)
            {
                flashActivo = false;
                if (damageFlash != null) damageFlash.SetActive(false);
            }
        }
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        Debug.Log("Jugador recibió daño. Vida actual: " + vidaActual);
        Debug.Log("DamageFlash asignado: " + (damageFlash != null));

        if (damageFlash != null)
        {
            damageFlash.SetActive(true);
            timerFlash = duracionFlash;
            flashActivo = true;
            Debug.Log("Flash activado por " + duracionFlash + " segundos");
        }

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void Curar(int cantidad)
    {
        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
    }

    private void Morir()
    {
        Debug.Log("Jugador muerto.");
        gameManager.EstadoDerrota();
    }
}