using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Quem nasce")]
    public GameObject inimigoPrefab;

    [Header("Onde nasce")]
    public float distanciaDoCentro = 12f;
    public float alturaDoChao = 0f;

    [Header("Dificuldade inicial")]
    public float intervaloInicial = 3f;
    public float velocidadeInicial = 2f;

    [Header("Aumento gradativo da dificuldade")]
    public float reducaoDoIntervalo = 0.08f;
    public float intervaloMinimo = 0.7f;
    public float aumentoDaVelocidade = 0.15f;
    public float velocidadeMaxima = 6f;

    private float intervaloAtual;
    private float velocidadeAtual;
    private float proximoSpawn;

    void Start()
    {
        ReiniciarDificuldade();
    }

    void Update()
    {
        if (Time.time >= proximoSpawn)
        {
            Nascer();

            // A cada inimigo, o jogo fica um pouco mais difícil
            intervaloAtual = Mathf.Max(intervaloMinimo, intervaloAtual - reducaoDoIntervalo);
            velocidadeAtual = Mathf.Min(velocidadeMaxima, velocidadeAtual + aumentoDaVelocidade);

            proximoSpawn = Time.time + intervaloAtual;
        }
    }

    void Nascer()
    {
        if (inimigoPrefab == null) return;

        float lado = (Random.value < 0.5f) ? -1f : 1f;
        Vector3 posicao = new Vector3(lado * distanciaDoCentro, alturaDoChao, 0f);

        GameObject inimigo = Instantiate(inimigoPrefab, posicao, Quaternion.identity);

        Enemy script = inimigo.GetComponent<Enemy>();
        if (script != null) script.speed = velocidadeAtual;
    }

    public void ReiniciarDificuldade()
    {
        intervaloAtual = intervaloInicial;
        velocidadeAtual = velocidadeInicial;
        proximoSpawn = Time.time + intervaloAtual;
    }
}