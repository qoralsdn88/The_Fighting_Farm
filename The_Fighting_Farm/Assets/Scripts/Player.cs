using UnityEngine;

public partial class Player : MonoBehaviour
{
	[SerializeField]
	private float walkSpeed = 2.0f;

	[SerializeField]
	private float runSpeed = 4.0f;

	[SerializeField]
	private GameObject particlePrefab;

	[SerializeField]
	private GameObject swordPrefab;
	private GameObject swordDestination;
	private GameObject treeDestination;
    [SerializeField]
    private GameObject treePrefab;

	private Sword sword;
	private Tree tree;


	private Transform holsterTransform;
	private Transform handTransform;
	private Transform fistTransform;

	private Animator animator;

    private void Awake()
    {
		animator = GetComponent<Animator>();
    }

	private void Start()
	{
		holsterTransform = transform.FindChildByName("Holster_Sword");
		handTransform = transform.FindChildByName("Hand_Sword");

    }

	private void SearchTransform(Transform parent)
    {
		print(parent.name);

		for (int i = 0; i < parent.childCount; i++)
			SearchTransform(parent.GetChild(i));
    }


	Vector3 direction;


	private void Update()
    {
		UpdateMoving();
		UpdateRaycast();
		UpdateDrawing();
		UpdateAttacking();
		UpdateItem();
		UpdatePlant();
		UpdateFist();
	}


	private bool bCanMove = true;

	private void UpdateMoving()
    {
		if (bCanMove == false)
			return;


		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		bool bRun = Input.GetButton("Run");

		float speed = bRun ? runSpeed : walkSpeed;


		direction = (Vector3.forward * vertical) + (Vector3.right * horizontal);
		direction = direction.normalized * speed;

		transform.Translate(direction * Time.deltaTime);
		
		animator.SetInteger("SpeedZ", (int)direction.magnitude);
	}

	private void UpdateRaycast()
    {
		if (Input.GetMouseButtonDown(1) == false)
			return;

		Ray ray = new Ray();
		ray.origin = transform.position + new Vector3(0, 1.5f, 0);
		ray.direction = transform.forward;


		//int layerMask = 1 << 6;
		//int layerMask = LayerMask.GetMask("Raycast") | LayerMask.GetMask("Water");
		int layerMask = LayerMask.GetMask("Raycast", "Water");

		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 5, layerMask))
        {
			print(hit.collider.gameObject.name);

			if (particlePrefab != null)
				Instantiate<GameObject>(particlePrefab, hit.transform.position, Quaternion.identity);
        }

		Debug.DrawRay(ray.origin, ray.direction * 5, Color.red, 3);
    }

	private void UpdateItem()
	{
        
    }

	private void UpdatePlant()
	{
        if(Input.GetButtonDown("Plant"))
		{
			animator.SetTrigger("IsPlanting");
        }

    }

	private void Begin_Planting()
	{
        if (treePrefab != null)
        {
            treeDestination = Instantiate<GameObject>(treePrefab, transform.position + Vector3.forward * 1.0f, Quaternion.identity);
            tree = treeDestination.GetComponent<Tree>();
        }
    }

	private void UpdateFist()
	{
        fistTransform = transform.FindChildByName("Fist");
    }
}