using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Card : MonoBehaviour {
	public float width = 4;
	public float height = 6;
	public float r = 0.2f;
	public int segments = 10;

	Rectangle rect;
	void Start () {
		rect = new Rectangle (width, height);
		UpdateMesh (Vector3.zero);
	}

	public void UpdateMesh (Vector3 intrude) {
		var mesh = rect.GenerateMesh (intrude, r, segments);
		GetComponent<MeshFilter> ().sharedMesh = mesh;
	}

	public static Vector3 Something (Vector3 a, Vector3 b, Vector3 p, float r) {
		var d = (b - a).normalized; 

		var h = d * Vector3.Dot (d, p - a) + a;

		var n = (p - h).normalized;


		var hh = Vector3.Cross (d, n).normalized;

		var o = h + hh * r;

		var l = Mathf.Abs(Vector3.Dot (n, p - a));

		var alpha = l / r;
		var ll = r * Mathf.Sin (Mathf.PI - alpha);

		var pp = h + n * ll;

		return pp + hh * (r + r * Mathf.Cos(Mathf.PI - alpha)) ;
	}

	public static Vector3 SomethingElse (Vector3 a, Vector3 b, Vector3 p, float r) {
		var d = (b - a).normalized;
		var h = d * Vector3.Dot (d, p - a) + a;
		var ph = h - p;
		var lph = ph.magnitude;
		ph = ph / lph;
		var n = p - h;
		var hh = Vector3.Cross (d, n).normalized;
		return (lph - Mathf.PI * r) * ph + h + hh * 2 * r;
	}

	public static Mesh MakeMesh (Vector3 a, Vector3 b, Vector3 c, Vector3 d, int segments) {
		Mesh m = new Mesh ();

		var ab = (b - a).normalized; 

		var h = ab * Vector3.Dot (ab, c - a) + a;
		var hc = (c - h).magnitude;
		var r = hc / Mathf.PI;

		var vertices = new Vector3[(segments + 1) * 2];
		var indices = new int[segments * 6 * 2];

		var d0 = d - a;
		var l0 = d0.magnitude / segments;
		d0.Normalize ();
		var d1 = c - b;
		var l1 = d1.magnitude / segments;
		d1.Normalize ();

		int vIndex = 0;
		int iIndex = 0;
		vertices [vIndex++] = a;
		vertices [vIndex++] = b;
		for (int i = 1; i <= segments; i++) {
			var p0 = a + i * l0 * d0;
			var p1 = b + i * l1 * d1;

			vertices [vIndex++] = Something(a, b, p0, r);
			vertices [vIndex++] = Something(a, b, p1, r);

			indices [iIndex++] = (i - 1) * 2;
			indices [iIndex++] = (i - 1) * 2 + 1;
			indices [iIndex++] = i * 2;

			// Back face
			indices [segments * 6 + iIndex - 3] = indices [iIndex - 3];
			indices [segments * 6 + iIndex - 2] = indices [iIndex - 1];
			indices [segments * 6 + iIndex - 1] = indices [iIndex - 2];

			indices [iIndex++] = i * 2;
			indices [iIndex++] = (i - 1) * 2 + 1;
			indices [iIndex++] = i * 2 + 1;

			// Back face
			indices [segments * 6 + iIndex - 3] = indices [iIndex - 3];
			indices [segments * 6 + iIndex - 2] = indices [iIndex - 1];
			indices [segments * 6 + iIndex - 1] = indices [iIndex - 2];
		}

		m.vertices = vertices;
		m.triangles = indices;
//		m.triangles = indices.Skip(6 * segments).Take(6 * segments).ToArray();

//		Debug.Log ("Indices = " + m.triangles.Aggregate("", (acc, next) => { 
//			return acc + ", " + next;
//		}));

//		Debug.Log ("Count = " + m.triangles.Length);

		return m;
	}

	public static bool IntersectLineSegment (Vector3 o, Vector3 dir, Vector3 a, Vector3 b, ref Vector3 intersect) {
		var ha = Vector3.Dot (dir, a - o) * dir + o;
		var hb = Vector3.Dot (dir, b - o) * dir + o;

		var aha = ha - a;
		var bhb = hb - b;
		if (Vector3.Dot(aha, bhb) > 0) {
			return false;
		}

		var laha = aha.magnitude;
		var lbhb = bhb.magnitude;
		intersect = a + (b - a) * (laha / (laha + lbhb));

		return true;
	}

	public class Vertex {
		public Vector3 pos;

		public Vertex next;
		public Vertex prev;

		public Vertex () {
			this.pos = Vector3.zero;
		}

		public Vertex (Vector3 pos) {
			this.pos = pos;
		}

		public void Set (Vector3 pos) {
			this.pos = pos;
		}
	}

	public class Edge {
		public Vector3 from;
		public Vector3 to;

		public Edge next;
		public Edge prev;

		public Edge (Vector3 from, Vector3 to) {
			this.from = from;
			this.to = to;
		}
			
		public Vector3 Dir {
			get { return to - from; }
		}

		public Vector3 NormDir {
			get { return (to - from).normalized; }
		}
	}

	public class Intersection {
		public Vector3 pos;
		public Edge edge;

		public Intersection (Vector3 pos, Edge edge) {
			this.pos = pos;
			this.edge = edge;
		}
	}

	[System.Serializable]
	public class Rectangle {
		Vertex a;
		Vertex b;
		Vertex c;
		Vertex d;

		float width;
		float height;

		public Edge e0;
		public Edge e1;
		public Edge e2;
		public Edge e3;

		Rectangle () {
			a = new Vertex();
			b = new Vertex();
			c = new Vertex();
			d = new Vertex();

			a.next = b;
			a.prev = d;

			b.next = c;
			b.prev = a;

			c.next = d;
			c.prev = b;

			d.next = a;
			d.prev = c;
		}

		public Rectangle (float width, float height): this() {
			this.width = width;
			this.height = height;

			a.pos = new Vector3(-width/2, -height/2, 0);
			b.pos = new Vector3(-width/2, height/2, 0);
			c.pos = new Vector3(width/2, height/2, 0);
			d.pos = new Vector3(width/2, -height/2, 0);

			e0 = new Edge(a.pos, b.pos);
			e1 = new Edge(b.pos, c.pos);
			e2 = new Edge(c.pos, d.pos);
			e3 = new Edge(d.pos, a.pos);

			e0.next = e1;
			e0.prev = e3;

			e1.next = e2;
			e1.prev = e0;

			e2.next = e3;
			e2.prev = e1;

			e3.next = e0;
			e3.prev = e2;
		}

		public Vector3 Normal {
			get { return Vector3.Cross (e0.Dir, e1.Dir).normalized; }
		}

		public bool FindIntersections (Vector3 o, Vector3 dir, out Intersection[] intersections) {
			List<Intersection> ls = new List<Intersection> (2);
			Edge e = e0;
			for (int i = 0; i < 4; i++) {
				Vector3 intersect = Vector3.zero;
				if (Card.IntersectLineSegment(o, dir, e.from, e.to, ref intersect)) {
					ls.Add (new Intersection(intersect, e));
				}
				if (ls.Count == 2) {
					break;
				}
				e = e.next;
			}

			if (ls.Count > 0) {
				intersections = ls.ToArray ();
				return true;
			}

			intersections = null;
			return false;
		}

		public Vertex FindEffectedVertex (Vector3 extrude, out Edge edge) {
			Vertex v = a;
			for (int i = 0; i < 4; i++) {
				if (
					Vector3.Dot(v.next.pos - v.pos, extrude) >= 0 &&
					Vector3.Dot(v.prev.pos - v.pos, extrude) >= 0
				) {
					edge = this[i];
					return v;
				}

				v = v.next;
			}

			edge = null;
			return null;
		}

		public Edge this[int i] {
			get {
				switch (i) {
				case 0:
					return e0;
					break;

				case 1:
					return e1;
					break;

				case 2:
					return e2;
					break;

				case 3:
					return e3;
					break;
				}

				return null;
			}
		}

		public bool FindIntersections (Vector3 extrude, float r, out Intersection[] intersections) {
			intersections = null;

			Edge edge;
			var v = FindEffectedVertex (extrude, out edge);
			var c = Mathf.PI * r;

			var n = Vector3.Cross(Vector3.Cross (e0.Dir, e1.Dir), extrude).normalized;
			var o1 = v.pos + extrude;
			extrude.Normalize ();
			var o2 = o1 - extrude * c;

			Intersection[] its1;
			if (!FindIntersections(o1, n, out its1)) {
				return false;
			}

			Intersection[] its2;
			FindIntersections(o2, n, out its2);

			intersections = its1;

			return true;
		}

		public List<Vector3> FindFlatOuterArea (Intersection[] its, Edge ee) {
			List<Vector3> vertices = new List<Vector3> ();

			while (ee != its[0].edge && ee != its[1].edge) {
				ee = ee.next;
			}

			Card.Intersection i0, i1;
			if (ee == its[0].edge) {
				i0 = its [0];
				i1 = its [1];
			} else {
				i0 = its [1];
				i1 = its [0];
			}

			vertices.Add (i0.pos);
			vertices.Add (i0.edge.to);

			var edge = i0.edge;
			while (edge != i1.edge) {
				vertices.Add (edge.next.from);
				edge = edge.next;
			}

			vertices.Add (i1.pos);

			return vertices;
		}

		public List<Vector3> FindFlatInnerArea (Intersection[] its, Edge origin) {
			List<Vector3> vertices = new List<Vector3> ();

			vertices.Add (origin.from);

			var ee = origin;
			while (ee != its[0].edge && ee != its[1].edge) {
				vertices.Add (ee.to);
				ee = ee.next;
			}

			if (ee == its[0].edge) {
				vertices.Add (its [0].pos);
				vertices.Add (its [1].pos);
				ee = its [1].edge;
			} else {
				vertices.Add (its [1].pos);
				vertices.Add (its [0].pos);
				ee = its [0].edge;
			}

			while(ee != origin.prev) {
				vertices.Add (ee.to);
				ee = ee.next;
			}

			return vertices;
		}

		public Mesh GenerateMesh (Vector3 intrude, float r, int splits) {
			Mesh mesh = new Mesh ();

			Edge origin;
			Vertex v = FindEffectedVertex (intrude, out origin);

			var o = v.pos + intrude;
			var dd = -intrude.normalized;
			var n = Vector3.Cross (Normal, intrude).normalized;

			List<Vector3> vertices = new List<Vector3> ();
			List<int> indices = new List<int> ();
			List<Vector2> uvs = new List<Vector2> ();

			Intersection[] its0;
			if (FindIntersections (o, n, out its0)) {
				EnsureClockWise (its0, origin);

				var vs = FindFlatOuterArea (its0, origin);
				vertices.AddRange (vs);

				foreach (var i in vs) {
					uvs.Add (Uv(i));
				}

				for (int i = 0; i <= vs.Count - 3; i++) {
					indices.Add (0);
					indices.Add (i + 1);
					indices.Add (i + 2);
				}

				// Back face
//				for (int i = 0; i <= vs.Count - 3; i++) {
//					indices.Add (0);
//					indices.Add (i + 2);
//					indices.Add (i + 1);
//				}

				int offset = vertices.Count;

				int ii0 = 0;
				int ii1 = vertices.Count - 1;
				for (int k = 1; k <= splits; k++) {
					var oo = o + k * Mathf.PI * r / splits * dd;

					Intersection[] its;
					if (FindIntersections(oo, n, out its)) {
						EnsureClockWise (its, origin);

						var p0 = Card.Something (its0[0].pos, its0[1].pos, its[0].pos, r);// its [0].pos;
						var p1 = Card.Something (its0[0].pos, its0[1].pos, its[1].pos, r);

						// Vertex
						vertices.Add (p0);
						vertices.Add (p1);

						// UV
						uvs.Add(Uv(its[0].pos));
						uvs.Add (Uv(its [1].pos));

						indices.Add (ii0);
						indices.Add (ii1);
						indices.Add (vertices.Count - 2);

						indices.Add (vertices.Count - 2);
						indices.Add (ii1);
						indices.Add (vertices.Count - 1);

						// Back face
//						indices.Add (ii0);
//						indices.Add (vertices.Count - 2);
//						indices.Add (ii1);
//
//						indices.Add (vertices.Count - 2);
//						indices.Add (vertices.Count - 1);
//						indices.Add (ii1);

						ii0 = vertices.Count - 2;
						ii1 = vertices.Count - 1;
					} else {
						break;
					}
				}

				Intersection[] its1;
				offset = vertices.Count;
				if (FindIntersections(o + Mathf.PI * r * dd, n, out its1)) {
					EnsureClockWise (its1, origin);

					vs = FindFlatInnerArea (its1, origin);
					foreach (var vx in vs) {
						vertices.Add (SomethingElse(its0[0].pos, its0[1].pos, vx, r));
						uvs.Add (Uv(vx));
					}
					for (int i = 0; i <= vs.Count - 3; i++) {
						indices.Add (offset);
						indices.Add (offset + i + 1);
						indices.Add (offset + i + 2);

						// Back face
//						indices.Add (offset);
//						indices.Add (offset + i + 2);
//						indices.Add (offset + i + 1);
					}
				}
			}

			mesh.vertices = vertices.ToArray ();
			mesh.triangles = indices.ToArray ();
			mesh.uv = uvs.ToArray ();

			return mesh;
		}

		private void EnsureClockWise (Intersection[] its, Edge origin) {
			while (origin != its[0].edge && origin != its[1].edge) {
				origin = origin.next;
			}

			if (origin == its[1].edge) {
				// Swap
				var t = its[0];
				its [0] = its [1];
				its [1] = t;
			}
		}

		private Vector2 Uv (Vector3 pos) {
			var uv = new Vector2 (pos.x - width/2, pos.y - height/2);
			uv.y = uv.y / height;
			uv.x = uv.x / width;

			return uv;
		}
	}
}
