using UnityEngine;  
using System.Collections;  

public class UniformStraigthMoveToObj: MonoBehaviour  
{  
	public GameObject TargetObject;
	public float MoveSpeed;  

	public void StartMove()  
	{  
		MoveTo (TargetObject);  
	}  

	public void MoveTo(GameObject target)  
	{  
		StartCoroutine(Move(target.transform.position));  
	}  

	IEnumerator Move(Vector3 target)  
	{  
		while(transform.position != target)  
		{  
			transform.position = Vector3.MoveTowards(transform.position, target, MoveSpeed * Time.deltaTime);  
			yield return 0;  
		}  
	}  
} 
