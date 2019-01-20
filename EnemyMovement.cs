using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    public static EnemyMovement instance;

    public GameObject heart2;
    public GameObject heart3;

    private Rigidbody2D rb;
    public float moveSpeed;
    private bool up = true;
    private float timer = 0;
    private float timer2 = 0;
    public float timerMax;
    public static int health = 3;
    private float startY;
    private float startX;
    private Vector2 posFix;
    private bool beenFroze = false;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        startY = rb.position.y;
        startX = rb.position.x;
        posFix.y = startY;
        posFix.x = startX;
    }
	
	// Update is called once per frame
	void Update () {
        if (!beenFroze)
        {
            if (transform.gameObject.tag == "Frozen")
            {
                GetComponent<Animator>().SetBool("isFrozen", true);
                Destroy(rb);
                beenFroze = true;
                Debug.Log("Enemy is frozen, preventing movement.");
            }
            posFix.y = rb.position.y - 1;
            if (timer >= timerMax)
            {
                timer = 0;
                up = false;
            }
            if (timer2 >= timerMax)
            {
                timer2 = 0;
                up = true;
            }

            switch (health)
            {
                case 0:
                    Destroy(gameObject);
                    break;
                case 1:
                    Destroy(GameObject.Find("heart2"));
                    break;
                case 2:
                    Destroy(GameObject.Find("heart3"));
                    break;
                case 3:
                    //Nothing needed here
                    break;
                default:
                    Debug.Log("Something went wrong, check health switch in EnemyMovement.cs");
                    break;
            }

            if (up)
            {
                timer++;
                rb.velocity = Vector2.up * moveSpeed;
                GetComponent<Animator>().SetBool("goingUp", true);
            }
            else
            {
                timer2++;
                rb.velocity = Vector2.down * moveSpeed;
                GetComponent<Animator>().SetBool("goingUp", false);
            }
            if (rb.position.y >= 4 + startY)
            {
                transform.position = posFix;
                Debug.Log("Fixing!");
            }
        }
	}

    void OnTriggerEnter2D(Collider2D collision){
        if (!beenFroze) {
            if (collision.gameObject.CompareTag("player"))
            {
                if (rb.position.y >= 4 + startY)
                {
                    transform.position = posFix;
                    Debug.Log("Fixing!");
                }
            }
        }
    }
    }
