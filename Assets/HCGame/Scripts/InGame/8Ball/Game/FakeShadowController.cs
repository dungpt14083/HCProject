using UnityEngine;
using System.Collections;

public class FakeShadowController : MonoBehaviour {

	public GameObject prefab;
	private GameObject shadow;
	private float initZPos;
	private GameObject table;
	private Rigidbody rigid;
	// Use this for initialization
	void Start () {
		shadow = Instantiate (prefab);
		initZPos = shadow.transform.position.z;
		table = GameObject.Find ("Table");
		rigid = GetComponent <Rigidbody> ();
	}
	public void DisableShadow()
    {
		shadow.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

		//TODO: check lockZ later
		// if (!script.ballActive) {
		// 	shadow.SetActive (false);
		// 	enabled = false;
		// } else {
		//
		// 	Vector3 dir = transform.position - table.transform.position;
		// 	dir = dir.normalized * 0.04f;
		// 	Vector3 newPos = transform.position + dir;
		// 	newPos.z = transform.position.z + 0.04f;
		// 	shadow.transform.position = newPos;
		// }

		if (!shadow.activeSelf) return;
		Vector3 dir = transform.position - table.transform.position;
		dir = dir.normalized * 0.04f;
		Vector3 newPos = transform.position + dir;
		newPos.z = transform.position.z + 0.04f;
		shadow.transform.position = newPos;
	}
	public void ClearFakeShadow()
    {
		Destroy(shadow);
    }
}
