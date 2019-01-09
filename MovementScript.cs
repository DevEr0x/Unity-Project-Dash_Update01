using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MovementScript : MonoBehaviour {

    private GameObject dashEffect;
    public GameObject damageTaken;
    public GameObject dashEffectNormal;
    public GameObject dashEffectLightning;
    public GameObject dashEffectFire;

    public Text healthDisplay;
    private Rigidbody2D rb;
    public float moveSpeed;
    public float jumpHeight;
    public bool isGrounded;
    public float dashSpeed;
    public float upwardDashSpeed;
    public float startDashTime;
    public float dashTime;
    private bool dashActive;
    private bool dashRestored;
    public int playerHealth = 100;
    private Vector2 upRight = (Vector2.right + Vector2.up);
    private Vector2 upLeft = (Vector2.left + Vector2.up);
    private int powerUp = 0;
    /*
     Power up numbers:
     0: No Power Up
     1: Lighting Power Up
     2: Fire Power Up
         */

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        dashTime = startDashTime;
        healthDisplay.text = "Health: "+playerHealth.ToString();
    }

    // Update is called once per frame
    void Update () {
        healthDisplay.text = "Health: " + playerHealth.ToString();
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);

        if(playerHealth <= 0){
            Debug.Log("Player Dead!");
            SceneManager.LoadScene("GameOver");
        }

        switch (powerUp){
            case 0:
                dashEffect = dashEffectNormal;
                break;
            case 1:
                dashEffect = dashEffectLightning;
                break;
            case 2:
                dashEffect = dashEffectFire;
                break;
            default:
                dashEffect = dashEffectNormal;
                break;
        }

        if (Input.GetKeyDown(KeyCode.Q)){
            playerHealth--;
        }

        Destroy(GameObject.Find("DashBurst(Clone)"), 0.35f);
        Destroy(GameObject.Find("ElectricityDashBurst(Clone)"), 0.35f);
        Destroy(GameObject.Find("FireDashBurst(Clone)"), 0.35f);
        Destroy(GameObject.Find("DamageTaken(Clone)"), 0.35f);

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashRestored) {
            //transform.position += move * dashSpeed * Time.deltaTime;
            if(move.x > 0 && dashActive == false && Input.GetKey(KeyCode.UpArrow)){ //Upright dash
                GetComponent<Animator>().SetBool("isDashing", true);
                rb.velocity = upRight * upwardDashSpeed;
                Instantiate(dashEffect, transform.position, Quaternion.identity);
                dashActive = true;
                dashRestored = false;
            }
            if(move.x < 0 && dashActive == false && Input.GetKey(KeyCode.UpArrow)){ //Upleft dash
                GetComponent<Animator>().SetBool("isDashing", true);
                rb.velocity = upLeft * upwardDashSpeed;
                Instantiate(dashEffect, transform.position, Quaternion.identity);
                dashActive = true;
                dashRestored = false;
            }
            if (move.x > 0 && dashActive == false){ //Right dash
                GetComponent<Animator>().SetBool("isDashing", true);
                rb.velocity = Vector2.right * dashSpeed;
                Instantiate(dashEffect, transform.position, Quaternion.identity);
                dashActive = true;
                dashRestored = false;
            }
            if(move.x < 0 && dashActive == false){ //Left dash
                GetComponent<Animator>().SetBool("isDashing", true);
                rb.velocity = Vector2.left * dashSpeed;
                Instantiate(dashEffect, transform.position, Quaternion.identity);
                dashActive = true;
                dashRestored = false;
            }
        } else {
            transform.position += move * moveSpeed * Time.deltaTime;
        }
        if(dashTime <= 0 && dashActive){
            dashActive = false;
            GetComponent<Animator>().SetBool("isDashing", false);
            rb.velocity = Vector2.zero;
            dashTime = startDashTime;
        }else if(dashActive && dashTime > 0){
            dashTime -= Time.deltaTime;
        }
        if(dashTime > 0 && isGrounded && dashActive == false){
            dashRestored = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded){
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpHeight), ForceMode2D.Force);
            GetComponent<Animator>().SetBool("isJumping", true);
            StartCoroutine(JumpAnim());
        }
        if(move.x > 0 && isGrounded){
            GetComponent<Animator>().SetBool("isWalking", true);
            GetComponent<Animator>().SetBool("facingRight", true);
        }
        if (move.x < 0 && isGrounded){
            GetComponent<Animator>().SetBool("isWalking", true);
            GetComponent<Animator>().SetBool("facingRight", false);
        }
        if(move.x == 0 || isGrounded == false && GetComponent<Animator>().GetBool("isJumping") == false){
            GetComponent<Animator>().SetBool("isWalking", false);
        }
    }

    IEnumerator JumpAnim(){
        yield return new WaitForSeconds(0.333f);
        GetComponent<Animator>().SetBool("isJumping", false);
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.CompareTag("Thunder")){
            Debug.Log("Thunder!");
            powerUp = 1;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("None")){
            Debug.Log("No powers!");
            powerUp = 0;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Fire")){
            Debug.Log("Fire!");
            powerUp = 2;
            Destroy(collision.gameObject);
        }

        //Enemy Detection
        if (collision.gameObject.CompareTag("enemy")){
            if(powerUp == 1 && dashActive){
                Debug.Log("Enemy Dead! " + dashActive);
                Destroy(collision.gameObject);
                Instantiate(dashEffect, transform.position, Quaternion.identity);
            } else if(powerUp == 2 && dashActive){
                Debug.Log("Enemy hit!" + dashActive);
                EnemyMovement.health--;
                Debug.Log(EnemyMovement.health);
            }
            else{
                Debug.Log("Player Hit!");
                playerHealth-= 20;
                Instantiate(damageTaken, transform.position, Quaternion.identity);
            }
         }
        }
    void OnTriggerEnter2D(){
        isGrounded = true;
    }
    void OnTriggerStay2D(){
        isGrounded = true;
    }
    void OnTriggerExit2D(){
        isGrounded = false;
    }
}

