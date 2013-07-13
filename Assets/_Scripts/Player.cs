using UnityEngine;public class Player : MonoBehaviour{    private const string AxisNameHorizontal = "Horizontal";    private const string AxisNameVertical = "Vertical";	    public Vector3 currentMovement;    public float moveSpeed = 5;    public CharacterController controller;	public float weaponCooldown = 0.1f;	private float weaponTimer;	public float rotationSpeed = 10;	private float noShootTimer;	public string[] leftStickAxis = {"Horizontal", "Vertical"};	public string[] rightStickAxis = {"RSHorizontal", "RSVertical"};	public int weaponId;	void Awake ()	{        if (controller == null)        {            controller = GetComponent<CharacterController>();        }	}	void Update ()
	{
		currentMovement[0] = Mathf.Abs(Input.GetAxisRaw(AxisNameHorizontal)) > 0.1f ? Input.GetAxisRaw(AxisNameHorizontal) : 0;		currentMovement[1] = Mathf.Abs(Input.GetAxisRaw(AxisNameVertical)) > 0.1f ? Input.GetAxisRaw(AxisNameVertical) : 0;
        //currentMovement[1] = Input.GetAxisRaw(AxisNameVertical);	    controller.Move(currentMovement*moveSpeed*Time.deltaTime);		float y = Input.GetAxis(rightStickAxis[1]);		float x = Input.GetAxis(rightStickAxis[0]);		if (Mathf.Abs(Input.GetAxis(rightStickAxis[0])) <= 0.1f && Mathf.Abs(Input.GetAxis(rightStickAxis[1])) <= 0.1f)		{			//print ("DEAD ZONE");			noShootTimer -= Time.deltaTime;		}else		{			noShootTimer = 1.5f;			print ("NO DEAD ZONE");		}		if (noShootTimer > 0){			if (Mathf.Abs(y) > 0.1f || Mathf.Abs(x) > 0.1f ){				Vector3 targetEuler = new Vector3(0,0, (Mathf.Atan2(y, x))*180/(Mathf.PI));				//transform.localEulerAngles = Vector3.Lerp(transform.eulerAngles, targetEuler, Time.deltaTime * 15);				transform.localEulerAngles = new Vector3(0,0,Mathf.LerpAngle(transform.localEulerAngles.z, targetEuler.z, Time.deltaTime * rotationSpeed));				if (Mathf.Abs(y) > 0.6f || Mathf.Abs(x) > 0.6f ){					WeaponShoot();				}			}		}else		{			y = Input.GetAxis(leftStickAxis[1]);			x = Input.GetAxis(leftStickAxis[0]);			if (Mathf.Abs(y) > 0.1f || Mathf.Abs(x) > 0.1f ){				Vector3 targetEuler = new Vector3(0,0, (Mathf.Atan2(y, x))*180/(Mathf.PI));				transform.localEulerAngles = new Vector3(0,0,Mathf.LerpAngle(transform.localEulerAngles.z, targetEuler.z, Time.deltaTime * 7));			}		}		weaponTimer -= Time.deltaTime;	}	void WeaponShoot()	{		if (weaponId == 0)		{			if (weaponTimer <= 0)			{				PREFAB.SpawnPrefab(PREFAB.BULLET, transform.position, transform.localEulerAngles-new Vector3(0,0,90), "1");				weaponTimer = weaponCooldown;			}		}else if (weaponId == 1)		{			if (weaponTimer <= 0)			{				PREFAB.SpawnPrefab(PREFAB.BULLET, transform.position, transform.localEulerAngles-new Vector3(0,0,90), "1");				weaponTimer = weaponCooldown*0.8f;			}		}		else if (weaponId == 2)		{			if (weaponTimer <= 0)			{				for (int i = 0; i < 5; i++)				{					PREFAB.SpawnPrefab(PREFAB.BULLET, transform.position, transform.localEulerAngles-new Vector3(0,0,76+(i*7)), "1");				}				weaponTimer = weaponCooldown*4.5f;			}		}	}	public void ChangeWeapon (int id)	{		weaponId = id;	}}