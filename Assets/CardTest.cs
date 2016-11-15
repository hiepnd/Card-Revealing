using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardTest : MonoBehaviour {
	public float width = 4;
	public float height = 5;

	public float r = 1;
	public int segment = 10;
	public Vector3 intrudeDir = Vector3.up;
	public float intrudeLength = 1;

	Vector3 extruder = Vector3.up;

	[SerializeField]
	Card.Rectangle rectangle = new Card.Rectangle(4, 5);

	public void Apply () {
		rectangle = new Card.Rectangle (width, height);
	}

	void OnDrawGizmos () {
		
		if (rectangle != null) {
			extruder = intrudeDir.normalized * intrudeLength;
			Card.Edge ee;
			var v = rectangle.FindEffectedVertex (extruder, out ee);

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (v.pos, 0.1f);
			Gizmos.DrawLine (v.pos, extruder + v.pos);

			Card.Intersection[] its;
			if (rectangle.FindIntersections (extruder, r, out its)) {
//				Gizmos.color = Color.red;
//				Gizmos.DrawWireSphere (its[0].pos, 0.1f);
//				Gizmos.DrawWireSphere (its[1].pos, 0.1f);
//
//				Gizmos.color = Color.blue;
//				Gizmos.DrawLine (its[0].pos, its[1].pos);
			}

			var o = v.pos + extruder;
			var dd = -extruder.normalized;
			var n = Vector3.Cross (rectangle.Normal, extruder).normalized;

			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere (o, 0.1f);
			Gizmos.DrawWireSphere (o + dd * Mathf.PI * r, 0.1f);

//			for (int i = 0; i <= segment; i++) {
//				var oo = o + i * Mathf.PI * r / segment * dd;
//
//				its = null;
//				if (rectangle.FindIntersections(oo, n, out its)) {
//					Gizmos.color = Color.red;
//					Gizmos.DrawWireSphere (its[0].pos, 0.1f);
//					Gizmos.DrawWireSphere (its[1].pos, 0.1f);
//
//					Gizmos.color = Color.blue;
//					Gizmos.DrawLine (its[0].pos, its[1].pos);
//				}
//			}

			if (rectangle.FindIntersections(o + Mathf.PI * r * dd, n, out its)) {

				Gizmos.color = Color.magenta;
				Gizmos.DrawLine (its[0].pos, its[1].pos);

				List<Vector3> vertices = rectangle.FindFlatInnerArea (its, ee);


				foreach (var vert in vertices) {
					Gizmos.DrawWireSphere (vert, 0.1f);
				}
			}


			its = null;
			if (rectangle.FindIntersections(o, n, out its)) {

				Gizmos.color = Color.blue;
				Gizmos.DrawLine (its[0].pos, its[1].pos);

				List<Vector3> vertices = rectangle.FindFlatOuterArea (its, ee);


				foreach (var vert in vertices) {
					Gizmos.DrawWireSphere (vert, 0.1f);
				}
			}

			Mesh mesh = rectangle.GenerateMesh (extruder, r, segment);
			GetComponent<MeshFilter> ().sharedMesh = mesh;

		}
	}

}
