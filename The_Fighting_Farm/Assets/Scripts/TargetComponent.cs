using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetComponent : MonoBehaviour
{
	[SerializeField]
	private bool bDebug;

	[SerializeField]
	private LayerMask layerMask;

	[SerializeField]
	private float radius = 10.0f;

	[SerializeField]
	private float rotateSpeed = 1.0f;


	private bool bDrawSphere;

	private GameObject targetObject;
	private GameObject cursorPrefab;

    private void Start()
    {
		cursorPrefab = Resources.Load<GameObject>("Target");
    }

    private void Update()
    {
		ToggleTarget();
		UpdateTargeting();
		
		
		if(Input.GetButtonDown("Targeting_Left"))
			ChangeFocus(false);
		else if (Input.GetButtonDown("Targeting_Right"))
			ChangeFocus(true);
	}


	private float deltaRotation;
	private bool bMovingFocus;

	private void UpdateTargeting()
    {
		if (targetObject == null)
			return;


		StatusComponent status = targetObject.GetComponent<StatusComponent>();
		if(status != null)
        {
			if (status.Dead)
            {
				EndTargeting(true);

				return;
			}
        }

		if (Vector3.Distance(targetObject.transform.position, transform.position) > radius)
        {
			EndTargeting(true);

			return;
        }


		Vector3 forward = transform.forward;
		Vector3 position = transform.position;
		Vector3 targetPosition = targetObject.transform.position;

		Vector3 direction = targetPosition - position;


		Quaternion from = transform.localRotation;
		Quaternion to = Quaternion.LookRotation(direction.normalized, Vector3.up);

		if(Quaternion.Angle(from, to) < 2.0f)
        {
			bMovingFocus = false;

			deltaRotation = 0.0f;
			transform.localRotation = to;

			return;
        }


		deltaRotation += rotateSpeed * Time.deltaTime;
		transform.localRotation = Quaternion.RotateTowards(from, to, deltaRotation);
	}

    private void OnGUI()
    {
		//GUI.color = Color.red;
		//GUILayout.Label(transform.localRotation.eulerAngles.y.ToString("0.000000"));
    }

    private void ToggleTarget()
    {
		if (Input.GetButtonDown("Targeting") == false)
			return;

		
		if (targetObject == null)
        {
			BeginTargeting();

			return;
        }

		EndTargeting(true);
    }

	private void BeginTargeting()
    {
		if(bDebug)
        {
			bDrawSphere = true;

			
			if (IsInvoking("End_DrawSphere"))
				CancelInvoke("End_DrawSphere");

			Invoke("End_DrawSphere", 5);
		}


		Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask.value);

		//foreach (Collider collider in colliders)
			//print(collider.name);

		
		GameObject[] candidates = colliders.Select(collider => collider.gameObject).ToArray();

		if(bDebug)
        {
			Vector3 position = transform.position;
			Debug.DrawLine(position, position + transform.forward * 10, Color.red, 5);

			foreach (GameObject candidate in candidates)
            {
				Vector3 direction = candidate.transform.position - position;
				Debug.DrawLine(position, position + direction, Color.blue, 5);

				//float dot = Vector3.Dot(transform.forward, direction.normalized);
				//print($"{candidate.name} : {dot}");
            }
		}


		GameObject nearlyObject = GetNealyFrontAngle(candidates);
		//Destroy(nearlyObject);

		ChangeTarget(nearlyObject);
	}

	private GameObject GetNealyFrontAngle(GameObject[] candidates)
    {
		Vector3 position = transform.position;
		

		GameObject candidate = null;
		float maxAngle = float.MinValue;

		foreach(GameObject obj in candidates)
        {
			Vector3 enemyPosition = obj.transform.position;
			Vector3 direction = enemyPosition - position;

			float angle = Vector3.Dot(transform.forward, direction.normalized);
			if (angle < 1.0f - 0.5f)
				continue;


			if(maxAngle <= angle)
            {
				maxAngle = angle;
				candidate = obj;
            }
		}

		return candidate;
	}

	private void ChangeTarget(GameObject candidate)
	{ 
		if(candidate == null)
        {
			EndTargeting(true);

			return;
        }


		if (cursorPrefab != null)
			Instantiate<GameObject>(cursorPrefab, candidate.transform);
		
		EndTargeting();

		targetObject = candidate;
	}

	private void EndTargeting(bool bLookForward = false)
    {
		if(targetObject != null)
        {
			Transform particle = targetObject.transform.FindChildByName("Target(Clone)");

			if (particle != null)
				Destroy(particle.gameObject);
		}


		deltaRotation = 0.0f;
		targetObject = null;

		if(bLookForward)
			transform.localRotation = Quaternion.identity;
	}

	private void ChangeFocus(bool bRight)
    {
		if (targetObject == null)
			return;


		if (bMovingFocus == true)
			return;

		bMovingFocus = true;


		Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask.value);

		Dictionary<float, GameObject> candidateTable = new Dictionary<float, GameObject>();
		foreach(Collider collider in colliders)
        {
			if (targetObject == collider.gameObject)
				continue;


			Vector3 vec1 = collider.transform.position;
			Vector3 vec2 = transform.position;
			Vector3 direction = vec1 - vec2;

			Vector3 cross = Vector3.Cross(transform.forward, direction.normalized);
			float distance = Vector3.Dot(cross, Vector3.up);

			candidateTable.Add(distance, collider.gameObject);
        }


		float minimum = float.MaxValue;
		GameObject candidate = null;

		foreach(float distance in candidateTable.Keys)
        {
			if (Mathf.Abs(distance) >= minimum)
				continue;

			if(bRight && distance > 0.0f)
            {
				minimum = Mathf.Abs(distance);
				candidate = candidateTable[distance];
            }

			if(bRight == false && distance < 0.0f)
            {
				minimum = Mathf.Abs(distance);
				candidate = candidateTable[distance];
			}
        }

		ChangeTarget(candidate);
    }

	private void End_DrawSphere()
    {
		bDrawSphere = false;
    }

    private void OnDrawGizmos()
    {
        if(bDrawSphere == true)
        {
			Gizmos.color = Color.green;
			
			Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}