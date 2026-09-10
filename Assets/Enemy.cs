using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;

    [Tooltip("Acima dessa altura o zumbi passa por cima em segurança")]
    public float alturaSeguraDoPulo = 0.8f;

    [Tooltip("Altura mínima para o pisão contar")]
    public float alturaMinimaParaPisao = 0.4f;

    [Tooltip("Impulso que o zumbi ganha ao pisar na cabeça do soldado")]
    public float forcaDoQuique = 7f;

    [Tooltip("A menos que essa distância, o soldado não muda mais de direção")]
    public float distanciaParaComprometer = 2.5f;

    private Transform alvo;
    private SpriteRenderer sr;
    private float direcaoAtual = 1f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        GameObject jogador = GameObject.FindGameObjectWithTag("Player");
        if (jogador != null)
        {
            alvo = jogador.transform;
            direcaoAtual = (alvo.position.x > transform.position.x) ? 1f : -1f;
        }
    }

    void Update()
    {
        if (alvo == null) return;

        float distanciaX = alvo.position.x - transform.position.x;

        if (Mathf.Abs(distanciaX) > distanciaParaComprometer)
        {
            direcaoAtual = (distanciaX > 0f) ? 1f : -1f;
        }

        transform.position += new Vector3(direcaoAtual * speed * Time.deltaTime, 0f, 0f);

        if (sr != null) sr.flipX = (direcaoAtual < 0f);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.GetComponent<SpitProjectile>() != null)
        {
            if (GameManager.Instance != null) GameManager.Instance.AdicionarPonto();
            Destroy(gameObject);
            return;
        }

        VerificarJogador(outro);
    }

    void OnTriggerStay2D(Collider2D outro)
    {
        VerificarJogador(outro);
    }

    void VerificarJogador(Collider2D outro)
    {
        if (!outro.CompareTag("Player")) return;

        float diferencaDeAltura = outro.transform.position.y - transform.position.y;
        Rigidbody2D rbJogador = outro.GetComponent<Rigidbody2D>();

        bool estaCaindo = (rbJogador != null && rbJogador.linearVelocity.y < -0.1f);

        // Caiu em cima da cabeça: pisão
        if (estaCaindo && diferencaDeAltura > alturaMinimaParaPisao)
        {
            if (GameManager.Instance != null) GameManager.Instance.AdicionarPonto();

            // Quica pra cima, dando chance de encadear outro pisão
            if (rbJogador != null)
            {
                rbJogador.linearVelocity = new Vector2(rbJogador.linearVelocity.x, forcaDoQuique);
            }

            Destroy(gameObject);
            return;
        }

        // Está alto o suficiente: só passa por cima, não morre
        if (diferencaDeAltura >= alturaSeguraDoPulo) return;

        if (GameManager.Instance != null) GameManager.Instance.JogadorMorreu();
    }
}