using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats base")]
    public int vidaMaxima = 100;
    public int dañoAtaque = 10;

    [Header("Multiplicador por nivel")]
    public float multiplicadorVida = 1.5f;
    public float multiplicadorDaño = 1.2f;

    [Header("Estado actual")]
    public int vidaActual;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        AplicarEscalado(gameManager.nivelActual);

        vidaActual = vidaMaxima;
    }

    private void AplicarEscalado(int nivel)
    {
        // Nivel 1 no modifica nada, nivel 2 en adelante escala
        float factor = Mathf.Pow(multiplicadorVida, nivel - 1);
        vidaMaxima = Mathf.RoundToInt(vidaMaxima * factor);

        float factorDaño = Mathf.Pow(multiplicadorDaño, nivel - 1);
        dañoAtaque = Mathf.RoundToInt(dañoAtaque * factorDaño);

        Debug.Log($"Enemigo escalado a nivel {nivel} — HP: {vidaMaxima}, Daño: {dañoAtaque}");
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        Debug.Log(gameObject.name + " recibió daño. Vida: " + vidaActual);

        GetComponent<EnemyAI>()?.ReaccionarAlDaño();

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log(gameObject.name + " eliminado.");
        gameManager.EnemigoEliminado();
        GetComponent<EnemyAI>()?.ReaccionarMuerte();
        Destroy(gameObject);
    }
}