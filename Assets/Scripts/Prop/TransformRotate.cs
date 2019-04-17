using UnityEngine;
using System.Collections;


	public class TransformRotate : MonoBehaviour
	{
		public enum AroundAxis
		{
			Non,
			X,
			Y,
			Z
		}

		public float speed = 10;
		public AroundAxis axis = AroundAxis.Y;
		public Space relativeTo = Space.World;

		void Update ()
		{
			Vector3 eulerAngles = Vector3.zero;
			switch (axis) {
			case AroundAxis.X:
				eulerAngles.x = speed * Time.deltaTime;
				break;
			case AroundAxis.Y:
				eulerAngles.y = speed * Time.deltaTime;
				break;
			case AroundAxis.Z:
				eulerAngles.z = speed * Time.deltaTime;
				break;
			default:
				return;
			}

			transform.Rotate (eulerAngles, relativeTo);
		}
	}

