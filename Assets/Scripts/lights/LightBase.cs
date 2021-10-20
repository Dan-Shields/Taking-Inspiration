using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Lights
{
	public class LightBase : MonoBehaviour
	{
		public float AnimSpeed = 1;
		private Animation Animation;

		void Awake()
		{
			this.Animation = this.GetComponent<Animation>();
			if (this.Animation)
			{
				foreach (AnimationState state in this.Animation)
				{
					state.speed = this.AnimSpeed;
				}
			}

			this.OnAwake();
		}

		protected virtual void OnAwake() { }
	}
}