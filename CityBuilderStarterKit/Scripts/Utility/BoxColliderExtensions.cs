using UnityEngine;

namespace CBSK
{
	/// <summary>
	/// BoxCollider2D extension methods.
	/// </summary>
	public static class BoxCollider2DExtensions
	{
		/// <summary>
		/// Gets the Y extents (0 = min, 1= max) of a box collider 2D.
		/// </summary>
		/// <returns>The bounds.</returns>
		/// <param name="value">Value.</param>
		public static Vector2[] GetRelativeBounds(this BoxCollider2D box)
		{
			Vector2 min;
			Vector2 max;
			min = -box.transform.position + box.transform.TransformPoint (box.Offset() + new Vector2 ((-box.size.x / 2.0f), (-box.size.y / 2.0f)));
			max = -box.transform.position + box.transform.TransformPoint (box.Offset() + new Vector2 (( box.size.x / 2.0f), ( box.size.y / 2.0f)));
			return new Vector2[]{min, max};
		}
		
		/// <summary>
		/// Get the offset. provides consistent interface for Unity 4 and 5.
		/// </summary>
		/// <param name="box">Box.</param>
		public static Vector2 Offset(this BoxCollider2D box)
		{
			#if UNITY_5
			return box.offset;
			#else
			return box.center;
			#endif
		}
	}
}