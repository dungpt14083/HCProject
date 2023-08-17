using UnityEngine;
using Zenject;
using DG.Tweening;
using Unity.Services.Core;
using System;

[CreateAssetMenu(fileName = "ProjectScriptObject", menuName = "Installers/ProjectScriptObject")]
public class ProjectScriptObject : ScriptableObjectInstaller<ProjectScriptObject>
{


	public override void InstallBindings()
	{
		// Platform settings
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 120;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		// Load caching data
		SaveSystem.Initialize("game.data");
		// Inits sdks
		DOTween.Init();

	}

}