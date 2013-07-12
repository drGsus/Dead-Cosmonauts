using UnityEngine;[RequireComponent(typeof(CharacterController))]public class Player : MonoBehaviour{    private const string _axisNameHorizontal = "Horizontal";    private const string _axisNameVertical = "Vertical";    /// <summary>    /// Your player's current movement.    /// </summary>    public Vector3 currentMovement;    /// <summary>    /// Your character's move speed.    /// </summary>    public float moveSpeed = 5;    /// <summary>    /// A pointer to the character controller, initialized by default.    /// </summary>    public CharacterController controller;	// Use this for initialization	void Awake ()	{        if (controller == null)        {            controller = GetComponent<CharacterController>();        }	}		// Update is called once per frame	void Update ()    {	    currentMovement = new Vector3(Input.GetAxisRaw(_axisNameHorizontal), Input.GetAxisRaw(_axisNameVertical), 0);	    controller.Move(currentMovement*moveSpeed*Time.deltaTime);	}}