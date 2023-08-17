using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestConfigScene : MonoBehaviour {
    public Text sampleLabel ;

    public ConfigSample configSample;


	// Use this for initialization
	void Start () {
        TextConfigManager.LoadDataConfig(ref configSample, "ConfigSample");
//        SampleConfigManager.Instance.Init();
	}


    public void ShowRecord () {
        sampleLabel.text = string.Empty;
        List<ConfigSampleRecord> samples = configSample.GetAllSamples();
        foreach (var sample in samples) {
            sampleLabel.text += sample.value1 +"====="+ sample.value2 + "\n";
        } // foreach
    } // ShowRecord ()
	
}
