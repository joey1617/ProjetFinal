using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HitFood : MonoBehaviour
{

    private CharacterController controller;

    public float speed = 600.0f;
    public float turnSpeed = 400.0f;
    private Vector3 moveDirection = Vector3.zero;
    public float gravity = 20.0f;
    private float verticalVelocity;
    public float jumpForce = 10f;
    public bool canJump;

    public Camera tps;
    Consumer food;
    private Animator anim;

    string foodName;
    private bool cake;
    private bool beer;
    private bool bigAir;

    private bool air;
    private bool actif;

    public bool floating;

    public ParticleSystem drinkB;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponentInChildren<Animator>();


        for (int i = 0; i < 2; i++)
        {
           GameObject.Find("Fox").gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;

        floating = false;
        cake = false;
        beer = false;
        actif = false;
        bigAir = false;

        air = false;
        canJump = false;
    }

    // Update is called once per frame
    void Update()
    {
      
        //Astronaute est sur le sol
        if (controller.isGrounded)
        {
            anim.SetBool("Floating", false);
            anim.SetInteger("Jumping", 0);
            canJump = true;
            air = false;
            floating = false;          
            moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;

        }

        //Dans les air
        else
        {           
            verticalVelocity -= gravity * Time.deltaTime;
        }

        //Comportement de l'astronaute
        Comportement();

        //Comportement du saut de l'astronaute lorsqu'il mange une part de gâteau
        if (Input.GetKey(KeyCode.Space) && canJump == true && bigAir == true)
        {
            anim.SetBool("BackFlip", true);
            anim.SetBool("Flip", false);
            verticalVelocity = jumpForce;
            canJump = false;
            air = true;          
        }

        //Comportement lié a l'astronaute lorsqu'il mange une part de gâteau, Enlève le comportement du BackFlip
        if (Input.GetKeyUp(KeyCode.Space))
        {
            anim.SetBool("BackFlip", false);
            
        }

        //Comportement du saut normal de l'astronaute
        if (Input.GetKeyDown(KeyCode.Space) && canJump == true && bigAir == false)
        {
            anim.SetBool("Flip", false);
            verticalVelocity = jumpForce;
            canJump = false;
            air = true;
        }

        //Comportement de sprint de l'astronaute
        if (Input.GetKey(KeyCode.F))
        {
            speed = 15f;
            anim.SetBool("Flip", false);
            anim.SetBool("Running", true);
        }


        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        //Comportement de marche de l'astronaute lorsqu'il ne sprint plus
        if (Input.GetKeyUp(KeyCode.F))
        {
            speed = 8f;
            anim.SetBool("Running", false);
        }


        //Movement de l'astronaute
        Vector3 moveVector = new Vector3(0, verticalVelocity, 0);
        controller.Move(moveVector * Time.deltaTime);

        float turn = Input.GetAxis("Horizontal");
        transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
        controller.Move(moveDirection * Time.deltaTime);
        moveDirection.y -= gravity * Time.deltaTime;


        //Clique Gauche qui permet d'interagir avec la nourriture.
        //actif permet de savoir si un effet est activé ou non
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
            Eat();

            if (cake == true && actif == false)
            {
               // eat.Play();
                StartCoroutine(Jump());
            }


            else if (beer == true && actif == false)
            {
                drinkB.Play();
                StartCoroutine(Beer());
            }
                    
        }
        
    }

    //Permet de déterminer sur qu'elle objet l'utilisateur clique dessus
    string Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(tps.transform.position, tps.transform.forward, out hit))
        {
            foodName = hit.transform.name;

            Debug.Log(hit.transform.name);
            return foodName;
        }

        else
        {
            return null;
        }
    }

    //Permet d'activer les effet selon la nourriture et de changer l'apparence de la nourriture
    void Eat()
    {
        if (Shoot() == "Cake")
        {         
            food = GameObject.Find("Cake").GetComponent<Consumer>();
            food.Consume();
            cake = true;
            food = null;
        }

        else if (Shoot() == "Beer")
        {
            food = GameObject.Find("Beer").GetComponent<Consumer>();
            food.Consume();
            beer = true;
            food = null;
            anim.SetBool("Flip", true);
        }

        else
        {
            food = null;
        }

    }


    //Effet lorsque que la gâteau est mangé et elle dure 9 seccondes
    IEnumerator Jump()
    {
        
        float time = 0;
        actif = true;
        //Turn towards the side.
        while (time < 9f)
        {
            bigAir = true;
            time += Time.deltaTime;
            gravity = 5f;
            jumpForce = 12f;
            yield return null;
        }

        bigAir = false;
        gravity = 8f;
        jumpForce = 10f;
        cake = false;
        actif = false;
    }

    //Effet lorsque que la bière est bue, elle dure 10 secondes
    IEnumerator Beer()
    {

        float time = 0;
        actif = true;

        while (time < 10f)
        {
            time += Time.deltaTime;
            for (int i = 0; i < 2; i++)
            {
                GameObject.Find("Zombie").gameObject.transform.GetChild(i).gameObject.SetActive(false);
                GameObject.Find("Fox").gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
            
            yield return null;
        }

        for (int i = 0; i < 2; i++)
        {
            GameObject.Find("Fox").gameObject.transform.GetChild(i).gameObject.SetActive(false);
            GameObject.Find("Zombie").gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
        actif = false;
    }

    //Comportement d'un saut et de la marche
    void Comportement()
    {

        //Comportement lors d'un saut
        if (air == true && floating == false)
        {
            anim.SetInteger("Jumping", 1);
        }

        if (controller.isGrounded && floating == false)
        {
            air = false;
            anim.SetInteger("Jumping", 0);
        }
        //---------------------------------------------

        //Comportement lorsque l'astronaute marche
        if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d") && canJump == true && air == false)
        {
            anim.SetBool("Flip", false);
            anim.SetInteger("Walk", 1);

        }

        else
        {
            anim.SetInteger("Walk", 0);
        }
        //----------------------------------------------
    }

    //Si l'astronaute tombe, la scène recommence
    private void OnTriggerEnter(Collider other)
    {

            Scene loadedLevel = SceneManager.GetActiveScene();
            SceneManager.LoadScene(loadedLevel.buildIndex);
            Debug.Log("ByeBye");

    }

    //Detection de la collision sur la boîte pour faire floater l'astronaute et faire un grand saut
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.name == "Saut")
        {
            Debug.Log("Saut!!");
            verticalVelocity = 50f;
            anim.SetBool("Floating", true);
            canJump = false;
        }
    }

}
