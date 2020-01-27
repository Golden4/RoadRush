using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCar : MonoBehaviour {

	public float сarSpeed = 20;
	public int curLine;
	bool isDead = false;
    Rigidbody rb;

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
    
    bool IsVisible()
    {
        return MobCarsController.Instance.GetCurPlayerPosition() + MobCarsController.Instance.visibleOffset >= transform.position.x;
    }

    void Awake ()
	{
		mmmat = GetComponent<Renderer> ();

		Material mat = new Material (mmmat.sharedMaterials [1]);

		mat.color = Color.blue;
        rb = GetComponent<Rigidbody>();
        EventManager.OnPlayerRespawnedEvent += OnPlayerRespawnedEvent;
	}

    private void OnPlayerRespawnedEvent()
    {
        if(IsInvoking("AddToReuseBefore"))
            CancelInvoke("AddToReuseBefore");

        AddToReuseBefore();
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

        //if (Mathf.Abs(PlayerCar.Ins.transform.position.x - transform.position.x) > 1000)
        //{
        //AddToReuseBefore();
        //}

        CheckCarSides ();

        if (CameraController.Instance.centerRoadPos.x > transform.position.x + 10) {
			MobCarsController.Instance.AddToReuse (this, curLine);
		}
	}


	void CheckCarSides ()
	{
		RaycastHit hit;

		if (forwardMob == null) {
			if (Physics.Raycast (transform.position + Vector3.right * 1.5f + Vector3.up * .5f, Vector3.right, out hit, 5)) {
				if (hit.transform.CompareTag ("Mob")) {
					сarSpeed = hit.transform.GetComponent<MobCar> ().сarSpeed;
					forwardMob = hit.transform;
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
		Debug.DrawRay (transform.position + Vector3.right * 1.5f + Vector3.up * .5f, Vector3.right * 5, Color.green);
		Debug.DrawRay (transform.position + Vector3.right + Vector3.up + Vector3.forward, Vector3.forward * 2, Color.green);
		Debug.DrawRay (transform.position + Vector3.right + Vector3.up - Vector3.forward, -Vector3.forward * 2, Color.green);
		#endif
	}

    public void SetDirection(MoveDir moveDir)
    {
        if (moveDir == MoveDir.back)
        {
            transform.eulerAngles = Vector3.up * 180;
            moveDir = MoveDir.back;
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
            moveDir = MoveDir.forward;
        }
    }

	void FixedUpdate ()
	{
		if (isDead)
			Break ();
		else if(IsVisible())
		    Move ();
	}

	void Move ()
	{
        if (!rb)
            return;
		Vector3 pos = rb.position;
		pos.x += сarSpeed * Time.fixedDeltaTime * ((moveDir == MoveDir.back) ? -1 : 1);
        rb.position = pos;
	}

	void Break ()
	{
		сarSpeed -= Time.fixedDeltaTime * 3;
		сarSpeed = Mathf.Clamp (сarSpeed, 0, float.MaxValue);
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
        Die();
        //GetComponent<BoxCollider> ().enabled = false;
        Invoke ("AddToReuseBefore", 1);

	}

    void Die()
    {
        isDead = true;
    }

	IEnumerator DeadCoroutine (Collision col)
	{
		yield return null;

		/*Vector3 dir = (col.contacts [0].point - transform.position).normalized;

		Vector3 force = -dir * col.gameObject.GetComponent<PlayerCar> ().speed / 15;

		Rigidbody rb = gameObject.GetComponent<Rigidbody> ();

		rb.AddRelativeForce (force, ForceMode.Impulse);*/
	}

	void AddToReuseBefore ()
	{
		GetComponent<BoxCollider> ().enabled = true;
		isDead = false;
		MobCarsController.Instance.AddToReuse (this, curLine);
        
	}

    private void OnDestroy()
    {
        EventManager.OnPlayerRespawnedEvent -= OnPlayerRespawnedEvent;
    }
}

public enum MoveDir
{
	forward,
	back
}