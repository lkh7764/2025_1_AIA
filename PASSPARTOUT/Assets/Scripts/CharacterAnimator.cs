using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public static CharacterAnimator Instance;

    [Header("ANIMATION CONTROLLER")]
    public Animator controller;



    // init singleton
    private void initiate()
    {
        if (Instance != null)
        {
            Debug.Log("GameManager is already instiated!");
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    private void Awake()
    {
        initiate();
    }
    private void Start()
    {

    }



    public void MakeTransition_draw(bool value)
    {
        controller.SetBool("draw", value);
    }
}
