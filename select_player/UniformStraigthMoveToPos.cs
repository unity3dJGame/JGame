using UnityEngine;  
using System.Collections;  
 
public class UniformStraigthMoveToPos: MonoBehaviour  
{  
	public Vector3 TargetPosition;  
	public float MoveSpeed;  

	public void StartMove()  
	{  
		MoveTo (TargetPosition);  
	}  

	public void MoveTo(Vector3 target)  
	{  
		StartCoroutine(Move(target));  
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
	