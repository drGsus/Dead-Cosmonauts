using UnityEngine;public class Player : MonoBehaviour{    private const string AxisNameHorizontal = "Horizontal";    private const string AxisNameVertical = "Vertical";	    public Vector3 currentMovement;    public float moveSpeed = 5;    public CharacterController controller;	public float weaponCooldown = 0.1f;	private float weaponTimer;	public float rotationSpeed = 10;	private float noShootTimer;	public string[] leftStickAxis = {"Horizontal", "Vertical"};	public string[] rightStickAxis = {"RSHorizontal", "RSVertical"};		void Awake ()	{        if (controller == null)        {            controller = GetComponent<CharacterController>();        }	}	void Update ()
	{
		currentMovement[0] = Mathf.Abs(Input.GetAxisRaw(AxisNameHorizontal)) > 0.1f ? Input.GetAxisRaw(AxisNameHorizontal) : 0;		currentMovement[1] = Mathf.Abs(Input.GetAxisRaw(AxisNameVertical)) > 0.1f ? Input.GetAxisRaw(AxisNameVertical) : 0;
        //currentMovement[1] = Input.GetAxisRaw(AxisNameVertical);	    controller.Move(currentMovement*moveSpeed*Time.deltaTime);		float y = Input.GetAxis("RSVertical");		float x = Input.GetAxis("RSHorizontal");		/*if (Mathf.Abs(Input.GetAxis(rightStickAxis[0])) <= 0.1f && Mathf.Abs(Input.GetAxis(rightStickAxis[1]) <= 0.1f))		{		}*/		if (Mathf.Abs(y) > 0.1f || Mathf.Abs(x) > 0.1f ){			Vector3 targetEuler = new Vector3(0,0, (Mathf.Atan2(y, x))*180/(Mathf.PI));			//transform.localEulerAngles = Vector3.Lerp(transform.eulerAngles, targetEuler, Time.deltaTime * 15);			transform.localEulerAngles = new Vector3(0,0,Mathf.LerpAngle(transform.localEulerAngles.z, targetEuler.z, Time.deltaTime * rotationSpeed));			if (Mathf.Abs(y) > 0.8f || Mathf.Abs(x) > 0.8f ){				if (weaponTimer <= 0)				{					PREFAB.SpawnPrefab(PREFAB.BULLET, transform.position, transform.localEulerAngles-new Vector3(0,0,90), "1");					weaponTimer = weaponCooldown;				}			}				}		weaponTimer -= Time.deltaTime;		//transform.LookAt(transform.position+new Vector3(Input.GetAxisRaw("RSVertical"), Input.GetAxisRaw("RSHorizontal"),0), Vector3.up);		if (Input.GetButtonDown("Fire1"))		{				}		//print (new Vector3(x, y,0));		//print (Input.GetButton("Fire1"));	}}