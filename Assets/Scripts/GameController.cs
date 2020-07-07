using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //Controle de direções
    public enum Direction
    {
        LEFT, RIGHT, UP, DOWN
    }

    //Armazena a direção em que a cobra está se movendo
    public Direction moveDirection;
    public Transform head;          // Armazena a cabeça da cobra
    public List<Transform> tail;    // Armazena a lista de obj tails (Corpo da cobra)

    private Vector3 lastPosition;    //Salva a última posição de um obj 

    public Transform food;           // prefab da comida
    public GameObject tailPrefab;    // prefab da calda

    public int col;     // Número de colunas 
    public int rows;    // Número de linhas

    public float delayStep; // Tempo entre um passo e outro
    public float step;      // Quantidade de movimento a cada passo

    public Text textScore;
    public Text textHiScore;

    private int score;
    private int hiScore;

    public GameObject panelGameOver;
    public GameObject panelTitle;


    // Start is called before the first frame update
    void Start()
    {
        // Inicializa pausado
        Time.timeScale = 0;

        // Inicializa o HiScore
        hiScore = PlayerPrefs.GetInt("HiScore");
        textHiScore.text = "Hi-Score: " + hiScore; 

        // Chama o método que move a cobra a cada tempo definido pela variável delayStep
        StartCoroutine("MoveSnake");

        // Instancia a comida em uma posição aleatória
        SetFood();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            moveDirection = Direction.UP;
            head.rotation = Quaternion.Euler(0,0,-90);
        }

        if(Input.GetKeyDown(KeyCode.DownArrow)){
            moveDirection = Direction.DOWN;
            head.rotation = Quaternion.Euler(0,0,90);
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            moveDirection = Direction.LEFT;
            head.rotation = Quaternion.Euler(0,0,0);
        }

        if(Input.GetKeyDown(KeyCode.RightArrow)){
            moveDirection = Direction.RIGHT;
            head.rotation = Quaternion.Euler(0,0,180);
        }
    }

    IEnumerator MoveSnake()
    {
        yield return new WaitForSeconds(delayStep);
        Vector3 nexPos = Vector3.zero;

        //Escolhe qual a proxima direção da cobra
        switch(moveDirection)
        {
            case Direction.UP:
            nexPos = Vector3.up;
            break;

            case Direction.DOWN:
            nexPos = Vector3.down;
            break;

            case Direction.LEFT:
            nexPos = Vector3.left;
            break;

            case Direction.RIGHT:
            nexPos = Vector3.right;
            break;
        }

        nexPos *= step;                 // Adiciona o passo
        lastPosition = head.position;   //Salva a ultima posição da cabeça
        head.position += nexPos;        // Movimenta a cobra

        
        //Movimenta o corpo da cobra
        foreach(Transform t in tail)
        {
            Vector3 temp = t.position;  // Armazena a possição atual do obj
            t.position = lastPosition;  // Movimenta o obj para a possição do obj da frente
            lastPosition = temp;        // Atualiza a última posição
            t.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        
        StartCoroutine("MoveSnake");
    }

    public void Eat()
    {
        Vector3 tailPosition = head.position;

        if(tail.Count > 0)
        {
            tailPosition = tail[tail.Count - 1].position;
        }

        GameObject temp = Instantiate(tailPrefab, tailPosition, transform.localRotation);
        tail.Add(temp.transform);

        score += 10;
        textScore.text = "Score: " + score;

        SetFood();
    }

    public void SetFood()
    {
        int x = Random.Range((col -1)/2 * -1, (col - 1)/2);
        int y = Random.Range((rows -1)/2 * -1, (rows - 1)/2);

        food.position = new Vector2(x * step, y * step);
    }

    public void GameOver()
    {
        panelGameOver.SetActive(true);

        if(score > hiScore)
        {
            hiScore = score;
            PlayerPrefs.SetInt("HiScore", score);
            textHiScore.text = "New Hi-Score: " + hiScore;
        }

        Time.timeScale = 0;
    }

    public void PlayGame()
    {
        head.position = Vector3.zero;
        SetFood();
        score = 0;

        //Limpa a lista de caldas
        foreach (Transform t in tail)
        {
            Destroy(t.gameObject);
        }
        tail.Clear();

        textScore.text = "Score: " + score;
        textHiScore.text = "Hi-Score: " + hiScore;

        moveDirection = Direction.LEFT;
        head.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        panelGameOver.SetActive(false);
        panelTitle.SetActive(false);
        Time.timeScale = 1;
    }
}
