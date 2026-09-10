using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("Configuración de disparo")]
    public float alcanceDisparo = 10f;
    public int dañoPorDisparo = 25;
    public LayerMask capaEnemigos;

    [Header("Estado")]
    public bool puedeDisparar = false;

    private Camera camaraAR;

    void Start()
    {
        camaraAR = Camera.main;
    }

    void Update()
    {
        if (!puedeDisparar) return;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 posicion = Touchscreen.current.primaryTouch.position.ReadValue();
            Debug.Log("Tap detectado en: " + posicion);
            Disparar(posicion);
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 posicion = Mouse.current.position.ReadValue();
            Debug.Log("Click detectado en: " + posicion);
            Disparar(posicion);
        }
    }

    private void Disparar(Vector2 posicionEnPantalla)
    {
        Ray rayo = camaraAR.ScreenPointToRay(posicionEnPantalla);
        RaycastHit golpe;

        Debug.DrawRay(rayo.origin, rayo.direction * alcanceDisparo, Color.red, 1f);
        Debug.Log("Disparando rayo desde: " + rayo.origin + " dirección: " + rayo.direction);

        if (Physics.Raycast(rayo, out golpe, alcanceDisparo, capaEnemigos))
        {
            Debug.Log("Raycast impactó: " + golpe.collider.name + " | Layer: " + LayerMask.LayerToName(golpe.collider.gameObject.layer));

            EnemyStats enemigo = golpe.collider.GetComponent<EnemyStats>();

            if (enemigo != null)
            {
                enemigo.RecibirDaño(dañoPorDisparo);
                Debug.Log("Daño aplicado: " + dañoPorDisparo + " | Vida restante: " + enemigo.vidaActual);
            }
            else
            {
                Debug.Log("Objeto golpeado no tiene EnemyStats: " + golpe.collider.name);
            }
        }
        else
        {
            Debug.Log("Raycast no impactó nada. ¿El enemigo tiene layer Enemy asignado?");
        }
    }

    public void ActivarDisparo()
    {
        puedeDisparar = true;
        Debug.Log("Disparo activado.");
    }

    public void DesactivarDisparo()
    {
        puedeDisparar = false;
        Debug.Log("Disparo desactivado.");
    }
}