using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RaycasterMoveDirection : Raycaster {

	public RaycasterMoveDirection(LayerMask layerMask, Vector2 direction, float margin, RaycastState state) 
	:	base(layerMask, direction, margin, state)
	{

	}

	protected RaycastHit2D Raycast(Vector2 start, Vector2 direction, float length, LayerMask layerMask) {
		Debug.DrawLine(start, start + direction * length, Color.blue);
		return Physics2D.Raycast(start, direction, length, layerMask);
	}


	public abstract void Cast(Vector2 origin, float distance, ref Vector2 move);
}
