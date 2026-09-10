using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum TipoEnemigo
    {
        Normal,
        Rapido,
        Resistente,
        Ofensivo
    }

    [Header("Tipo de enemigo")]
    public TipoEnemigo tipo = TipoEnemigo.Normal;

    [Header("Configuración de movimiento")]
    public float velocidad = 1f;
    public float distanciaAtaque = 1.5f;

    [Header("Configuración de ataque")]
    public float intervaloAtaque = 2f;

    private float timerAtaque = 0f;
    private Transform camara;
    private EnemyStats stats;
    private Animator animator;
    private bool enCombate = false;
    private PlayerStats jugador;

    void Start()
    {
        camara = Camera.main.transform;
        stats = GetComponent<EnemyStats>();
        animator = GetComponent<Animator>();
        jugador = FindFirstObjectByType<PlayerStats>();
        AplicarTipo();
        IniciarCombate(); // TEMPORAL para pruebas
    }

    private void AplicarTipo()
    {
        switch (tipo)
        {
            case TipoEnemigo.Rapido:
                velocidad *= 2f;
                intervaloAtaque *= 0.5f;
                break;

            case TipoEnemigo.Resistente:
                velocidad *= 0.5f;
                intervaloAtaque *= 1.5f;
                break;

            case TipoEnemigo.Ofensivo:
                stats.dañoAtaque = Mathf.RoundToInt(stats.dañoAtaque * 2f);
                intervaloAtaque *= 0.75f;
                break;
        }

        Debug.Log($"Enemigo tipo {tipo} — Velocidad: {velocidad}, Intervalo: {intervaloAtaque}");
    }

    public void IniciarCombate()
    {
        enCombate = true;
    }

    public void DetenerCombate()
    {
        enCombate = false;

        if (animator != null)
        {
            animator.SetBool("Mov", false);
            animator.SetBool("IsAttack", false);
        }
    }

    void Update()
    {
        if (!enCombate) return;

        float distancia = Vector3.Distance(transform.position, camara.position);

        if (distancia > distanciaAtaque)
        {
            MoverseHaciaJugador();
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("Mov", false);
            }
            Atacar();
        }
    }

    private void MoverseHaciaJugador()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            camara.position,
            velocidad * Time.deltaTime
        );

        transform.LookAt(camara);

        if (animator != null)
        {
            animator.SetBool("Mov", true);
        }
    }

    private void Atacar()
    {
        timerAtaque += Time.deltaTime;

        if (timerAtaque >= intervaloAtaque)
        {
            timerAtaque = 0f;

            if (animator != null)
            {
                animator.SetBool("IsAttack", true);
            }

            if (jugador != null)
            {
                jugador.RecibirDaño(stats.dañoAtaque);
                Debug.Log($"{tipo} atacó al jugador por {stats.dañoAtaque} de daño.");
            }
            else
            {
                Debug.Log("No se encontró PlayerStats en la escena.");
            }
        }
    }

    public void ReaccionarAlDaño()
    {
        if (animator != null)
        {
            animator.SetBool("IsAttack", false);
        }
    }

    public void ReaccionarMuerte()
    {
        if (animator != null)
        {
            animator.SetBool("Mov", false);
            animator.SetBool("IsAttack", false);
        }
    }
}