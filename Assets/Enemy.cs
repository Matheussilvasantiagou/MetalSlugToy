using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;

    private Transform alvo;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        GameObject jogador = GameObject.FindGameObjectWithTag("Player");
        if (jogador != null) alvo = jogador.transform;
    }

    void Update()
    {
        if (alvo == null) return;

        float direcao = (alvo.position.x > transform.position.x) ? 1f : -1f;
        transform.position += new Vector3(direcao * speed * Time.deltaTime, 0f, 0f);

        if (sr != null) sr.flipX = (direcao < 0f);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        // Levou cuspe do zumbi: morre e dá ponto
        if (outro.GetComponent<SpitProjectile>() != null)
        {
            if (GameManager.Instance != null) GameManager.Instance.AdicionarPonto();
            Destroy(gameObject);
            return;
        }

        // Encostou no zumbi: o jogador morre
        if (outro.CompareTag("Player"))
        {
            if (GameManager.Instance != null) GameManager.Instance.JogadorMorreu();
        }
    }
}