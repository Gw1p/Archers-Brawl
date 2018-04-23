using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Values")]
/**
 * This scriptable object holds values for music and overall game volumes
 */ 
public class MusicObject : ScriptableObject {

	// Music volume
	public float music;
	// Overall game volume
	public float volume;

}
