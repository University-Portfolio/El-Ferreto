﻿using UnityEngine;
using System.Collections;
using System;

public class movement : MonoBehaviour
{

    private Rigidbody2D body;
    private Animator anim;
    //Stores last y accelerometer states where 0 is the current state
    private float[] jump_state = new float[10];

    public float jump_threshold = 0.2f;
    public float speed = 20f;
    public float jump_height = 10f;

    public bool keyboard_controlled = false;
    private bool touching_ground = false;


    void Start()
    {
        GetComponent<Rigidbody2D>().freezeRotation = true;
        body = GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        Vector2 movement;

        if (keyboard_controlled)
        {
            movement = new Vector2(Input.GetAxis("Horizontal"), 0);
        }
        else
        {
            movement = new Vector2(Input.acceleration.x, 0);
            pollJumpStates();
        }

        //Jump, if the player has tried to jump and is touching the ground
        if (hasJumped() && touching_ground)
        {
            jump(body);
        }

        body.velocity += (movement * Time.deltaTime * speed);

        animationUpdate();
    }

    void animationUpdate()
    {
        // setting ground condition for Animator
        anim.SetBool("ground", touching_ground || Math.Round(body.velocity.x * 100) == 0);

        // setting speed condition for Animator
        anim.SetFloat("speed", Mathf.Abs(body.velocity.x));

        //Flip sprite to face correct direction
        if (body.velocity.x < 0f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (body.velocity.x > 0f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    //Shift every value in the array and add current state
    void pollJumpStates()
    {
        float current_state = Input.acceleration.y;

        for (int i = jump_state.Length - 1; i > 0; i--)
        {
            jump_state[i] = jump_state[i - 1];
        }

        jump_state[0] = current_state;
    }

    void jump(Rigidbody2D body)
    {
        //If the player is on the ground and they jump
        body.velocity += new Vector2(0f, jump_height);
        touching_ground = false;
    }

    //Has the player tried to jump
    bool hasJumped()
    {
        if (keyboard_controlled) return Input.GetKeyDown(KeyCode.W);

        if (getJumpMagnitude() >= jump_threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    float getJumpMagnitude()
    {
        return Math.Abs(jump_state[0] - jump_state[jump_state.Length - 1]);
    }

    //Stores last object the player stood on
    private GameObject ground_object;

    void OnCollisionEnter2D(Collision2D collision_object)
    {

        Vector2 contant_normal = collision_object.contacts[0].normal;

        //If the contact normal is pointing up, the player must be on a flat surface
        if (contant_normal == new Vector2(0,1))
        {
            touching_ground = true;
            ground_object = collision_object.gameObject;
        }

        last_normal = contant_normal;
    }

    void OnCollisionExit2D(Collision2D collision_object)
    {
        if(collision_object.gameObject == ground_object) touching_ground = false;
    }
        

    private Vector2 last_normal;

    //For debugging purposes only
    void OnGUI()
    {
        GUI.Label(new Rect(100, 10, 1000, 100), "Debug:\n" + body.position + "\n" + Input.acceleration + "\n" + (int)(getJumpMagnitude() * 100) / 100f + "\n" + touching_ground + " " + last_normal);
    }
}