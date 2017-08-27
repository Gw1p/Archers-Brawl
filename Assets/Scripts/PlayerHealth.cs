using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

	[SerializeField] int maxHealth = 100;

	[SyncVar (hook = "OnHealthChanged")] int health;

	public AudioSource audio;
	public AudioClip hit;
	public AudioClip death;

	Player player;
	NetworkAnimator anim;

	[ServerCallback]
	void OnEnable()
	{
		health = maxHealth;
	}

	void Start(){
		anim = gameObject.transform.GetComponent<NetworkAnimator> ();
		player = gameObject.transform.GetComponent<Player> ();
		health = maxHealth;
	}

	[Server]
	public bool TakeDamage(int damage)
	{
		bool died = false;
		if (health <= 0)
			return died;
		
		health -= damage;
		died = health <= 0;

		if (died)
			health = 0;

		RpcTakeDamage (died);

		return died;

	}

	[ClientRpc]
	void RpcTakeDamage(bool died){

		if (isLocalPlayer) {
			PlayerCanvas.canvas.FlashDamageEffect ();
		}

		audio.clip = hit;
		audio.Play ();
		anim.animator.SetTrigger ("Hit");

		if (died) {
			audio.clip = death;
			audio.Play ();	
			player.Die ();
		}
	}

	void OnHealthChanged(int value){

		health = value;

		if (isLocalPlayer) {
			if (PlayerCanvas.canvas)
			PlayerCanvas.canvas.SetHealth (value);
		}
	}

	[Server]
	public void AddHealth(int hp){
		health += hp;
		if (health > maxHealth)
			health = maxHealth;
	}

	public void ChangeHealth(int newHp){
		maxHealth = newHp;
		health = maxHealth;
		PlayerCanvas.canvas.hpBar.maxValue = maxHealth;
		PlayerCanvas.canvas.hpBar.value = maxHealth;
	}
}
