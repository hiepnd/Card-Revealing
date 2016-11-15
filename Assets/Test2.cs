using UnityEngine;
using System.Collections;

public class Test2 : MonoBehaviour {
	public Transform a, b, c, d;
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

		if (c != null) {
			Gizmos.DrawWireSphere (c.position, wr);
		}

		if (d != null) {
			Gizmos.DrawWireSphere (d.position, wr);
		}

		if (a != null && b != null && c != null && d!= null) {
			var m = Card.MakeMesh (a.position, b.position, c.position, d.position, segments);
			GetComponent<MeshFilter> ().sharedMesh = m;
		}
	}

}
