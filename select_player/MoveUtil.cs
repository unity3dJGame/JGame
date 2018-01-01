using UnityEngine;

public static class MoveUtil
{
	public enum MoveType
	{
		UniformStraight = 0,
		UniformStraight3D,
	}

	public static void MoveToObj(GameObject moveObj, GameObject targetObj, float speed, MoveType type)
	{
		if (null == moveObj || null == targetObj)
			return;
		
		if (type == MoveType.UniformStraight || type == MoveType.UniformStraight3D) 
		{
			UniformStraigthMoveToObj moveScript = moveObj.GetComponent<UniformStraigthMoveToObj> ();
			if (null == moveScript)
			{
				moveObj.AddComponent<UniformStraigthMoveToObj> ();
				moveScript = moveObj.GetComponent<UniformStraigthMoveToObj> ();
			}
			if (type == MoveType.UniformStraight) 
			{
				targetObj.transform.position =  new Vector3(
						targetObj.transform.position.x,
						moveObj.transform.position.y,
						targetObj.transform.position.z);
			}
			
			moveScript.TargetObject = targetObj;
			moveScript.MoveSpeed = speed;
			moveScript.StartMove ();
		}
	}

	public static void MoveToPos(GameObject moveObj, Vector3 targetPos, float speed, MoveType type)
	{
		if (null == moveObj)
			return;
		
		if (type == MoveType.UniformStraight || type == MoveType.UniformStraight3D) 
		{
			UniformStraigthMoveToPos moveScript = moveObj.GetComponent<UniformStraigthMoveToPos> ();
			if (null == moveScript)
			{
				moveObj.AddComponent<UniformStraigthMoveToPos> ();
				moveScript = moveObj.GetComponent<UniformStraigthMoveToPos> ();
			}
			if (type == MoveType.UniformStraight) 
			{
				targetPos.y = moveObj.transform.position.y;
			}

			moveScript.TargetPosition = targetPos;
			moveScript.MoveSpeed = speed;
			moveScript.StartMove ();
		}
	}
}