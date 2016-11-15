using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	public Transform a;
	public Transform b;
	public Transform p;
	public Transform o;
	public float r = 2;

	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.blue;

		var wr = 0.1f;
		if (a != null) {
			Gizmos.DrawWireSphere (a.position, wr);
		}

		if (b != null) {
			Gizmos.DrawWireSphere (b.position, wr);
		}

		if (p != null) {
			Gizmos.DrawWireSphere (p.position, wr);
		}


		if (a != null && b != null && p != null && o != null) {
			
			o.transform.position = Card.Something (a.position, b.position, p.position, r);

			var d = (b.position - a.position).normalized;
			var h = d * Vector3.Dot (d, p.position - a.position) + a.position;

			var n = (p.position - h).normalized;
			var splits = 20;
			var c = Mathf.PI * r;
			for (int i = 0; i <= splits; i++) {
				var ee = h + n * (c / splits) * i;
				var oo = Card.Something (a.position, b.position, ee, r);

				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere (ee, wr);

				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere (oo, wr);
			}
		}
	}
}