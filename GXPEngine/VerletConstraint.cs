// Basic constraint: two points should be at a given distance from each other:
public class VerletConstraint {
	public VerletPoint one;
	public VerletPoint two;
	public readonly float length;
	public readonly float strength=0.2f;
	public readonly bool push;

	public VerletConstraint(VerletPoint pOne, VerletPoint pTwo, float pStrength=1, bool pPush=true) {
		one = pOne;
		two = pTwo;
		length = (pTwo.position - pOne.position).Length ();
		strength = pStrength;
		push = pPush;
	}

	public void Apply() {
		Vec2 diff = two.position - one.position;
		float currentlength = diff.Length ();
		if (!push && currentlength < length) { // pull-only constraint (for ropes, fabric, etc)
			return;
		}
		diff.Normalize ();
		diff = (length - currentlength) * strength * diff;

		if (!one._fixed && !two._fixed) {
			one.position -= diff * 0.5f;
			two.position += diff * 0.5f;
		} else if (!one._fixed && two._fixed) {
			one.position -= diff;
		} else if (one._fixed && !two._fixed) {
			two.position += diff;
		}
	}
}