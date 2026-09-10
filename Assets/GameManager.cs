using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int pontos = 0;
    private float tempo = 0f;

    private Transform jogador;
    private Vector3 posicaoInicialDoJogador;
    private EnemySpawner spawner;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            jogador = obj.transform;
            posicaoInicialDoJogador = jogador.position;
        }

        spawner = Object.FindFirstObjectByType<EnemySpawner>();
    }

    void Update()
    {
        tempo += Time.deltaTime;
    }

    public void AdicionarPonto()
    {
        pontos++;
    }

    public void JogadorMorreu()
    {
        // Tira todos os inimigos da tela
        Enemy[] inimigos = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Enemy inimigo in inimigos)
        {
            Destroy(inimigo.gameObject);
        }

        // Tira os cuspes que ainda estão voando
        SpitProjectile[] cuspes = Object.FindObjectsByType<SpitProjectile>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (SpitProjectile cuspe in cuspes)
        {
            Destroy(cuspe.gameObject);
        }

        // Devolve o zumbi para a posição inicial
        if (jogador != null)
        {
            jogador.position = posicaoInicialDoJogador;
            Rigidbody2D rb = jogador.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        // Zera placar e dificuldade
        pontos = 0;
        tempo = 0f;
        if (spawner != null) spawner.ReiniciarDificuldade();
    }

    void OnGUI()
    {
        GUIStyle estilo = new GUIStyle();
        estilo.fontSize = 24;
        estilo.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 20, 400, 40), "Humanos eliminados: " + pontos, estilo);
        GUI.Label(new Rect(20, 50, 400, 40), "Tempo: " + Mathf.FloorToInt(tempo) + "s", estilo);
    }
}