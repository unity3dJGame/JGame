using System;
using UnityEngine;

public static class AnimationUtil
{
	
	public static void SwitchAnimation(GameObject obj, string animationName, float moveSpeed)
	{
		Animation ani = obj.GetComponent<Animation> ();
		if (null != ani) {
			AnimationClip clip = ani.GetClip (animationName);
			if (null != clip) {
				if (moveSpeed  > 0)
					ani [animationName].speed = moveSpeed;
				ani.CrossFade (animationName, 0.2f);
			}
		}
		else
		{
			Animator animator = obj.GetComponent<Animator> ();
			if (moveSpeed  > 0)
				animator.speed = moveSpeed;
			animator.CrossFade (animationName, 0.2f);
		}
	}

	public static void LookAt(GameObject lookObject, GameObject lookAtObject)
	{
		lookObject.transform.LookAt(lookAtObject.transform);
	}
	public static void LookAt(GameObject lookObject, Vector3 lookAtVec)
	{
		lookObject.transform.LookAt(lookAtVec);
	}
}

