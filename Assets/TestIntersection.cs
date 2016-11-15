using UnityEngine;
using System.Collections;

public class TestIntersection : MonoBehaviour {

	public Transform a, b, o;
	public Vector3 dir;
	public int segments = 10;

	void OnDrawGizmos () {
		Gizmos.color = Color.blue;

		var wr = 0.1f;
		if (a != null) {
			Gizmos.DrawWireSphere (a.position, wr);
		}

		if (b != null) {
			Gizmos.DrawWireSphere (b.position, wr);
		}

		if (a && b) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine(a.position, b.position);
		}

		if (o != null) {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere (o.position, wr);

			Gizmos.color = Color.green;
			Gizmos.DrawLine(o.position, o.position + dir * 10);
			Gizmos.DrawLine(o.position, o.position - dir * 10);
		}


		Gizmos.color = Color.red;
		if (a != null && b != null && o != null) {
			Vector3 intersect = Vector3.zero;
			if (Card.IntersectLineSegment(o.position, dir.normalized, a.position, b.position, ref intersect)) {
				Gizmos.DrawWireSphere (intersect, wr);
			}
		}

	}
}
