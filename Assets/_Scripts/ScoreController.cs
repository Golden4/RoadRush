using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : SingletonGeneric<ScoreController> {

	public float distance = 0;
	public float highSpeedTime = 0;
	public float averageSpeed = 0;
	public float drivingOppositeTime = 0;
	public float score = 0;
	public int dangerousOvertacing = 0;

	float relativeHighSpeed = .1f;
	float relativeHighSpeedLastTime = -1;

	public void CalculateHighSpeedInfo (float speed, float speedForBonus)
	{
		if (speed > speedForBonus) {

			relativeHighSpeed += Time.deltaTime;
			relativeHighSpeedLastTime = Time.time;

			highSpeedTime += Time.deltaTime;
			if (relativeHighSpeed > .5f)
				UIManager.Instance.ShowHighSpeedInfo (relativeHighSpeed);

		} else if (relativeHighSpeedLastTime + .5f < Time.time && relativeHighSpeed > 0) {
			relativeHighSpeed = 0;
			UIManager.Instance.HideHighSpeedInfo ();
		}
	}

	float relativeDrivingOpposite = .1f;
	float relativeDrivingOppositeLastTime = -1;

	public void CalculateDrivingOppositeInfo (float pos)
	{

		if (pos > .1f && MobCarsController.Instance.roadLineInfo [0].dir == MoveDir.back) {
			drivingOppositeTime += Time.deltaTime;
		

			relativeDrivingOpposite += Time.deltaTime;
			relativeDrivingOppositeLastTime = Time.time;

			relativeDrivingOpposite += Time.deltaTime;
			if (relativeDrivingOpposite > .5f)
				UIManager.Instance.ShowDrivingOppositeInfo (relativeDrivingOpposite);

		} else if (relativeDrivingOppositeLastTime + .5f < Time.time && relativeDrivingOpposite > 0) {
			relativeDrivingOpposite = 0;
			UIManager.Instance.HideDrivingOppositeInfo ();
		}
	}

}
