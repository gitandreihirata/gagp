using UnityEngine;

public class S_Movement : MonoBehaviour
{
    [Header("Configuração de Movimento")]
    [Tooltip("Velocidade da bola no eixo X (→)")]
    public float speed = 5f;

    void Update()
    {
        // Move a bola continuamente para a direita (→) no eixo X
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
