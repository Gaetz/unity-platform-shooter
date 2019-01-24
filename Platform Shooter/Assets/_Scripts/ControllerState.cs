public class RaycastState {

	public RaycastState() {
		climbing = false;
		descending = false;
		grounded = false;
		falling = false;
	}

	public bool Climbing { get { return climbing; } set { climbing = value; } }
	bool climbing;

	public bool Descending { get { return descending; } set { descending = value; } }
	bool descending;

	public bool Grounded { get { return grounded; } set { grounded = value; } }
	bool grounded;

	public bool Falling { get { return falling; } set { falling = value; } }
	bool falling;

	public MovingPlatform MovingPlatform { get { return movingPlatform; } set { movingPlatform = value; } }
	MovingPlatform movingPlatform;

}
