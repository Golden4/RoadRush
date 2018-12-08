using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCar : MonoBehaviour {

	public float CarSpeed = 20;
	public int curLine;
	bool isDead = false;

	public MoveDir moveDir;

	Color[] colors = {
		Color.black,
		Color.blue,
		Color.cyan,
		Color.gray,
		Color.green,
		Color.red,
		Color.white,
		Color.yellow
	};

	Renderer mmmat;

	void Awake ()
	{
		mmmat = GetComponent<Renderer> ();

		Material mat = new Material (mmmat.sharedMaterials [1]);

		mat.color = Color.blue;

	}

	void OnEnable ()
	{
		mmmat.materials [1].color = colors [Random.Range (0, colors.Length)];
	}

	Transform forwardMob = null;

	void Update ()
	{
		if (isDead)
			return;

		//CheckCarSides ();

		if (CameraController.Instance.centerRoadPos.x > transform.position.x + 10) {
			MobCarsController.Instance.SpawnMob (curLine);
			MobCarsController.Instance.AddToReuse (this, curLine);
		}
	}


	void CheckCarSides ()
	{
		RaycastHit hit;

		if (forwardMob == null) {
			if (Physics.Raycast (transform.position + Vector3.right * 1.5f + Vector3.up * .5f, Vector3.right, out hit, 5)) {
				if (hit.transform.CompareTag ("Mob")) {
					CarSpeed = hit.transform.GetComponent<MobCar> ().CarSpeed;
					forwardMob = hit.transform;
					print (forwardMob);
				}
			}
		}
/*
		if (MobCarsController.Instance.roadLineInfo [curLine - 1].dir == moveDir)
		if (Physics.Raycast (transform.position + Vector3.right + Vector3.up * .5f + Vector3.forward, Vector3.forward, out hit, 2)) {
			if (hit.transform.CompareTag ("Mob") && CarSpeed == hit.transform.GetComponent<MobCar> ().CarSpeed) {
				CarSpeed = CarSpeed + 2;
			}
		}

		if (MobCarsController.Instance.roadLineInfo [curLine + 1].dir == moveDir)
		if (Physics.Raycast (transform.position + Vector3.right + Vector3.up * .5f - Vector3.forward, -Vector3.forward, out hit, 2)) {
			if (hit.transform.CompareTag ("Mob") && CarSpeed == hit.transform.GetComponent<MobCar> ().CarSpeed) {
				CarSpeed = CarSpeed + 2;
			}
		}*/

		#if UNITY_EDITOR
		Debug.DrawRay (transform.position + Vector3.right * 1.5f + Vector3.up * .5f, Vector3.right * 5);
		Debug.DrawRay (transform.position + Vector3.right + Vector3.up * .5f + Vector3.forward, Vector3.forward * 2, Color.green);
		Debug.DrawRay (transform.position + Vector3.right + Vector3.up * .5f - Vector3.forward, -Vector3.forward * 2, Color.green);
		#endif
	}

	void FixedUpdate ()
	{
		if (isDead)
			Break ();
		
		Move ();
	}

	void Move ()
	{
		Vector3 pos = transform.position;
		pos.x += CarSpeed * Time.fixedDeltaTime * ((moveDir == MoveDir.back) ? -1 : 1);
		transform.position = pos;
	}

	void Break ()
	{
		CarSpeed -= Time.fixedDeltaTime * 3;
		CarSpeed = Mathf.Clamp (CarSpeed, 0, float.MaxValue);
	}

	float collisionTime;

	void OnCollisionEnter (Collision col)
	{
		if (col.transform.CompareTag ("Player") && !isDead) {
			CollideWithPlayer ();
			StartCoroutine ("DeadCoroutine", col);
		}
	}

	void CollideWithPlayer ()
	{
		isDead = true;
		//GetComponent<BoxCollider> ().enabled = false;
		Invoke ("AddToReuseBefore", 2);

	}

	IEnumerator DeadCoroutine (Collision col)
	{
		yield return null;

		/*Vector3 dir = (col.contacts [0].point - transform.position).normalized;

		Vector3 force = -dir * col.gameObject.GetComponent<PlayerCar> ().speed / 15;

		Rigidbody rb = gameObject.AddComponent<Rigidbody> ();

		rb.AddRelativeForce (force, ForceMode.Impulse);*/
	}

	void AddToReuseBefore ()
	{
		GetComponent<BoxCollider> ().enabled = true;
		isDead = false;
		MobCarsController.Instance.SpawnMob (curLine);
		MobCarsController.Instance.AddToReuse (this, curLine);

		if (gameObject.GetComponent<Rigidbody> ())
			Destroy (gameObject.GetComponent<Rigidbody> ());
	}


}

public enum MoveDir
{
	forward,
	back
}