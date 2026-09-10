using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ImageTargetHandler : MonoBehaviour
{
    [Header("Configuración de este ImageTarget")]
    public int nivelDeEstaImagen = 1;
    public int cantidadEnemigos = 1;

    [Header("Enemigos de este nivel")]
    public GameObject[] prefabsEnemigos;

    [Header("UI")]
    public GameObject botonCombatir;
    public GameObject mensajeNoDisponible;

    private ObserverBehaviour observerBehaviour;
    private GameManager gameManager;
    private bool encuentroIniciado = false;
    private GameObject[] enemigosSpawneados;

    void Start()
    {
        gameManager = GameManager.Instance;

        observerBehaviour = GetComponent<ObserverBehaviour>();

        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }

        if (botonCombatir != null) botonCombatir.SetActive(false);
        if (mensajeNoDisponible != null) mensajeNoDisponible.SetActive(false);
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }

    private void OnTrackingFound()
    {
        Debug.Log($"Imagen {nivelDeEstaImagen} detectada.");

        if (encuentroIniciado) return;

        if (gameManager.NivelEsAccesible(nivelDeEstaImagen))
        {
            if (botonCombatir != null) botonCombatir.SetActive(true);
            if (mensajeNoDisponible != null) mensajeNoDisponible.SetActive(false);
        }
        else
        {
            if (botonCombatir != null) botonCombatir.SetActive(false);
            if (mensajeNoDisponible != null) mensajeNoDisponible.SetActive(true);
        }
    }

    private void OnTrackingLost()
    {
        Debug.Log($"Imagen {nivelDeEstaImagen} perdida.");

        if (botonCombatir != null) botonCombatir.SetActive(false);
        if (mensajeNoDisponible != null) mensajeNoDisponible.SetActive(false);

        if (encuentroIniciado && gameManager.estado == GameManager.EstadoJuego.EnCombate)
        {
            PausarCombate();
        }
    }

    public void AlPresionarCombatir()
    {
        if (encuentroIniciado) return;
        if (!gameManager.NivelEsAccesible(nivelDeEstaImagen)) return;

        encuentroIniciado = true;
        if (botonCombatir != null) botonCombatir.SetActive(false);

        SpawnearEnemigos();
        gameManager.IniciarEncuentro(cantidadEnemigos);
    }

    private void SpawnearEnemigos()
    {
        enemigosSpawneados = new GameObject[prefabsEnemigos.Length];

        for (int i = 0; i < prefabsEnemigos.Length; i++)
        {
            if (prefabsEnemigos[i] == null) continue;

            Vector3 offset = new Vector3(0f, 0.1f, 0f);

            GameObject enemigo = Instantiate(
                prefabsEnemigos[i],
                transform.position + offset,
                Quaternion.identity,
                transform
            );

            enemigo.transform.forward = transform.up;

            enemigosSpawneados[i] = enemigo;

            EnemyAI ai = enemigo.GetComponent<EnemyAI>();
            if (ai != null) ai.IniciarCombate();
        }

        Debug.Log($"Spawneados {prefabsEnemigos.Length} enemigos en nivel {nivelDeEstaImagen}.");
    }

    private void PausarCombate()
    {
        Debug.Log("Tracking perdido — pausando combate.");

        if (enemigosSpawneados == null) return;

        foreach (GameObject enemigo in enemigosSpawneados)
        {
            if (enemigo == null) continue;
            EnemyAI ai = enemigo.GetComponent<EnemyAI>();
            if (ai != null) ai.DetenerCombate();
        }
    }

    private void ReanudarCombate()
    {
        Debug.Log("Tracking recuperado — reanudando combate.");

        if (enemigosSpawneados == null) return;

        foreach (GameObject enemigo in enemigosSpawneados)
        {
            if (enemigo == null) continue;
            EnemyAI ai = enemigo.GetComponent<EnemyAI>();
            if (ai != null) ai.IniciarCombate();
        }
    }

    void OnDestroy()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }
}