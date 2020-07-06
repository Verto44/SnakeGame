using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    
    public GameController gameController;
    
    //Define o que acontece quando a cabeça colide com outro obj
    private void OnTriggerEnter2D(Collider2D col) 
    {
        switch(col.gameObject.tag)
        {
            case "food":
            gameController.Eat();
            break;
            case "tail":
                gameController.GameOver();
            break;
        }
    }
}
