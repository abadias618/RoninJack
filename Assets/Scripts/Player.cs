﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float jumpHight = 3.0f;
    [SerializeField] private float jumpSpeed = 0.25f;
    [SerializeField] private bool invariable = false;

    private int Score = 0;
    private Rigidbody RB;
    private Animation ANIM;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> atctions = new Dictionary<string, Action>();

    void Start()
    {
        RB = gameObject.GetComponent<Rigidbody>();
        ANIM = gameObject.GetComponent<Animation>();

        atctions.Add("slide", Slide);
        atctions.Add("attack ", Attack);
        atctions.Add("jump", Jump);

        keywordRecognizer = new KeywordRecognizer(atctions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedKeyword;
        keywordRecognizer.Start();
    }

    //Movement should be done in FixedUpdate to look proper.
    private void FixedUpdate()
    {
        transform.parent.Translate(new Vector3(0, 0, moveSpeed) * Time.deltaTime, Space.World);
        KeyboardCommands();
        Score = (int)transform.parent.localPosition.z;
        UI.SUI.UpdateScore(Score);
    }

    private void KeyboardCommands()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) Slide();
        if (Input.GetKeyDown(KeyCode.Z)) Attack();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetAxis("Horizontal") != 0) transform.position += new Vector3(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, 0);
        if (Input.GetAxis("Vertical") != 0) transform.position += new Vector3(0, 0, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
    }

    private void Slide()
    {
        if(invariable == false)
        {
            StartCoroutine(SlideCoroutine());
        }
    }

    private void Attack()
    {
        //Nothing here for now;
    }

    private void Jump()
    {
        if(transform.position.y <= 0)
        {
            StartCoroutine(JumpCoroutine());
        }
    }

    private void RecognizedKeyword(PhraseRecognizedEventArgs speach)
    {
        atctions[speach.text].Invoke();
    }

    private IEnumerator SlideCoroutine()
    {
        invariable = true;
        yield return new WaitForSeconds(5);
        invariable = false;
    }

    private IEnumerator JumpCoroutine()
    {
        ANIM.Play("Jump");
        yield return new WaitForFixedUpdate();
        RB.useGravity = false;
        float playersY = transform.position.y;
        while (transform.position.y < (playersY + jumpHight))
        {
            transform.Translate(new Vector3(0, jumpSpeed, 0));
            yield return null;
        }
        RB.useGravity = true;
        ANIM.Blend("Run");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            UI.SUI.EndLevel(false);
            gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "End")
        {
            UI.SUI.EndLevel(true);
            gameObject.SetActive(false);
        }
    }
}