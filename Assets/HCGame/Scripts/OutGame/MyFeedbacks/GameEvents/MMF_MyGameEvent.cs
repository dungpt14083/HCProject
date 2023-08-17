using UnityEngine;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("")]
	[FeedbackHelp("Bắn 1 sự kiện GameEvent thông qua EventBus. Yêu cầu bắt buộc trong Scene phải có 1 Event Dispatcher.")]
	[FeedbackPath("AGame Event/Game Event")]
	public class MMF_MyGameEvent : MMF_Feedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
#if UNITY_EDITOR
		public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.UIColor; } }
		public override bool EvaluateRequiresSetup() { return (EventType == GameEventType.NONE); }
		public override string RequiredTargetText { get { return EventType != GameEventType.NONE ? EventType.ToString() : "Thiếu EventDispatcher"; } }
		public override string RequiresSetupText { get { return "Yêu cầu cần có 1 Event Dispatcher trong scene."; } }
#endif


		[MMFInspectorGroup("Event Type", true, 54, true)]
		[Tooltip("Chọn loại sự kiện cần phát đi")]
		public GameEventType EventType = GameEventType.NONE;

		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}
			var EventDispatcher = GameObject.FindObjectOfType<EventDispatcher>();
			if (EventDispatcher != null)
			{
				EventDispatcher.GetComponent<EventDispatcher>().DispatchEvent(EventType);
			}
			else
			{
				Debug.LogError("Không tìm thấy EventDispatcher trong Scene");
			}


		}
	}
}