public class VerletPoint {
	public float x {
		get {
			return position.x;
		}
		set {
			position.x = value;
		}
	}
	public float y {
		get {
			return position.y;
		}
		set {
			position.y = value;
		}
	}
	// Verlet: velocity is implicit!
	public Vec2 velocity {
		get {
			return position - oldPosition;
		}
	}
	public Vec2 oldPosition;
	public Vec2 position;
	public Vec2 acceleration;
	public readonly bool _fixed;

	public VerletPoint(Vec2 newPosition, bool pFixed=false) {
		oldPosition = newPosition;
		position = newPosition;
		acceleration = new Vec2();
		_fixed = pFixed;
	}

	public VerletPoint (float px, float py, bool pFixed=false) : this (new Vec2 (px, py), pFixed) {}

	public void Step() {
		if (_fixed) {
			return;
		}
		// Key step: Verlet integration:
		Vec2 tmp = position;
		position += position - oldPosition + acceleration;
		oldPosition = tmp;

		acceleration.SetXY (0, 0);
	}
}
