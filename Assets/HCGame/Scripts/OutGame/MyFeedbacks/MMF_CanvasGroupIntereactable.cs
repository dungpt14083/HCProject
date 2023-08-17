using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("")]
	[FeedbackHelp("Change CanvasGroup interactable property.")]
	[FeedbackPath("UI/CanvasGroup Intereactable")]
	public class MMF_CanvasGroupIntereactable : MMF_Feedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
#if UNITY_EDITOR
		public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.UIColor; } }
		public override bool EvaluateRequiresSetup() { return (TargetCanvasGroup == null); }
		public override string RequiredTargetText { get { return TargetCanvasGroup != null ? TargetCanvasGroup.name : ""; } }
		public override string RequiresSetupText { get { return "This feedback requires that a TargetCanvasGroup be set to be able to work properly. You can set one below."; } }
#endif


		[MMFInspectorGroup("Interactable", true, 54, true)]
		[Tooltip("the target canvas group we want to control the Interactable parameter on")]
		public CanvasGroup TargetCanvasGroup;
		[Tooltip("if this is true, on play, the target canvas group will enable interactive, if false it won't")]
		public bool Interactable = true;

		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}

			if (TargetCanvasGroup == null)
			{
				return;
			}

			TargetCanvasGroup.interactable = Interactable;
		}
	}
}