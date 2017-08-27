using UnityEngine;
using UnityEngine.Networking;

public class ObjectSpawner : NetworkBehaviour {

	public float aidCd;
	public GameObject aidPrefab;
	public GameObject[] aidSpawn = new GameObject[3];

	float aidTime;

	void Update () {

		aidTime += Time.deltaTime;

		if (aidTime > aidCd) {
			aidCd = Random.Range (10f, 40f);
			aidTime = 0;
			int randomSpawn = Random.Range (0, aidSpawn.Length);
			GameObject newAid = Instantiate (aidPrefab, aidSpawn [randomSpawn].transform.position, Quaternion.identity) as GameObject;
			CmdSpawnAid (newAid);
		}

	}

	[Command]
	void CmdSpawnAid(GameObject obj){
		NetworkServer.Spawn (obj);
	}

}
