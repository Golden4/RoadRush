using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GUIAnim : MonoBehaviour {
	[HideInInspector]
	public float m_CameraBottomEdge;
	[HideInInspector]
	public float m_CameraLeftEdge;
	[HideInInspector]
	public float m_CameraRightEdge;
	[HideInInspector]
	public float m_CameraTopEdge;
	private CanvasRenderer m_CanvasRenderer;
	[HideInInspector]
	public Vector3 m_CanvasWorldBottomCenter;
	[HideInInspector]
	public Vector3 m_CanvasWorldBottomLeft;
	[HideInInspector]
	public Vector3 m_CanvasWorldBottomRight;
	[HideInInspector]
	public Vector3 m_CanvasWorldMiddleCenter;
	[HideInInspector]
	public Vector3 m_CanvasWorldMiddleLeft;
	[HideInInspector]
	public Vector3 m_CanvasWorldMiddleRight;
	[HideInInspector]
	public Vector3 m_CanvasWorldTopCenter;
	[HideInInspector]
	public Vector3 m_CanvasWorldTopLeft;
	[HideInInspector]
	public Vector3 m_CanvasWorldTopRight;
	public cFade m_FadeIn;
	public cPingPongFade m_FadeLoop;
	[HideInInspector]
	public float m_FadeOriginal;
	[HideInInspector]
	public float m_FadeOriginalTextOutline;
	[HideInInspector]
	public float m_FadeOriginalTextShadow;
	public cFade m_FadeOut;
	[HideInInspector]
	public float m_FadeVariable;
	private GETweenItem m_GETweenFadeLoop;
	private GETweenItem m_GETweenScaleLoop;
	private Image m_Image;
	private bool m_InitialDone;
	private int m_MoveIdle_Attemp;
	private int m_MoveIdle_AttempMax = 5;
	private float m_MoveIdle_AttempMax_TimeInterval = 0.5f;
	private bool m_MoveIdle_StartSucceed;
	public cMoveIn m_MoveIn;
	[HideInInspector]
	public Vector3 m_MoveOriginal;
	public cMoveOut m_MoveOut;
	[HideInInspector]
	public float m_MoveVariable;
	private Canvas m_Parent_Canvas;
	private RectTransform m_ParentCanvasRectTransform;
	private RawImage m_RawImage;
	[HideInInspector]
	private RectTransform m_RectTransform;
	public cRotationIn m_RotationIn;
	[HideInInspector]
	public Quaternion m_RotationOriginal;
	public cRotationOut m_RotationOut;
	[HideInInspector]
	public float m_RotationVariable;
	public cScaleIn m_ScaleIn;
	public cPingPongScale m_ScaleLoop;
	[HideInInspector]
	public Vector3 m_ScaleOriginal;
	public cScaleOut m_ScaleOut;
	[HideInInspector]
	public float m_ScaleVariable;
	private Slider m_Slider;
	private Text m_Text;
	private Outline m_TextOutline;
	private Shadow m_TextShadow;
	private Toggle m_Toggle;
	[HideInInspector]
	public Bounds m_TotalBounds;

	private void AnimIn_FadeComplete ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (this.m_FadeIn.Sounds.m_End != null) {
				this.PlaySoundOneShot (this.m_FadeIn.Sounds.m_End);
			}
			this.m_FadeIn.Began = false;
			this.m_FadeIn.Animating = false;
			this.m_FadeIn.Done = true;
			this.m_FadeOut.Began = false;
			this.m_FadeOut.Animating = false;
			this.m_FadeOut.Done = false;
			this.AnimIn_FadeUpdateByValue (1f);
			this.StartMoveIdle ();
		}
	}

	private void AnimIn_FadeUpdateByValue (float Value)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			float fade = this.m_FadeIn.Fade + ((this.m_FadeOriginal - this.m_FadeIn.Fade) * Value);
			this.RecursiveFade (base.transform, fade, this.m_FadeIn.FadeChildren);
			if (this.m_TextOutline != null) {
				float num2 = Value - 0.75f;
				if (num2 < 0f) {
					num2 = 0f;
				}
				if (num2 > 0f) {
					num2 *= 4f;
				}
				float fadeOutline = this.m_FadeOriginalTextOutline * num2;
				this.FadeTextOutline (base.transform, fadeOutline);
			}
			if (this.m_TextShadow != null) {
				float num4 = Value - 0.75f;
				if (num4 < 0f) {
					num4 = 0f;
				}
				if (num4 > 0f) {
					num4 *= 4f;
				}
				float fadeShadow = this.m_FadeOriginalTextShadow * num4;
				this.FadeTextShadow (base.transform, fadeShadow);
			}
			if (!this.m_FadeIn.Animating && this.m_FadeIn.Began) {
				this.m_FadeIn.Animating = true;
				if (this.m_FadeIn.Sounds.m_AfterDelay != null) {
					this.PlaySoundOneShot (this.m_FadeIn.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimIn_MoveComplete ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (this.m_MoveIn.Sounds.m_End != null) {
				this.PlaySoundOneShot (this.m_MoveIn.Sounds.m_End);
			}
			this.m_MoveIn.Began = false;
			this.m_MoveIn.Animating = false;
			this.m_MoveIn.Done = true;
			this.m_MoveOut.Began = false;
			this.m_MoveOut.Animating = false;
			this.m_MoveOut.Done = false;
			this.StartMoveIdle ();
		}
	}

	private void AnimIn_MoveUpdateByValue (float Value)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			this.m_RectTransform.anchoredPosition = new Vector3 (this.m_MoveIn.BeginPos.x + ((this.m_MoveIn.EndPos.x - this.m_MoveIn.BeginPos.x) * Value), this.m_MoveIn.BeginPos.y + ((this.m_MoveIn.EndPos.y - this.m_MoveIn.BeginPos.y) * Value), this.m_MoveIn.BeginPos.z + ((this.m_MoveIn.EndPos.z - this.m_MoveIn.BeginPos.z) * Value));
			if (!this.m_MoveIn.Animating && this.m_MoveIn.Began) {
				this.m_MoveIn.Animating = true;
				if (this.m_MoveIn.Sounds.m_AfterDelay != null) {
					this.PlaySoundOneShot (this.m_MoveIn.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimIn_RotationComplete ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (this.m_RotationIn.Sounds.m_End != null) {
				this.PlaySoundOneShot (this.m_RotationIn.Sounds.m_End);
			}
			this.m_RotationIn.Began = false;
			this.m_RotationIn.Animating = false;
			this.m_RotationIn.Done = true;
			this.m_RotationOut.Began = false;
			this.m_RotationOut.Animating = false;
			this.m_RotationOut.Done = false;
			this.AnimIn_RotationUpdateByValue (1f);
			this.StartMoveIdle ();
		}
	}

	private void AnimIn_RotationUpdateByValue (float Value)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			float x = this.m_RotationIn.BeginRotation.x + ((this.m_RotationIn.EndRotation.x - this.m_RotationIn.BeginRotation.x) * Value);
			float y = this.m_RotationIn.BeginRotation.y + ((this.m_RotationIn.EndRotation.y - this.m_RotationIn.BeginRotation.y) * Value);
			float z = this.m_RotationIn.BeginRotation.z + ((this.m_RotationIn.EndRotation.z - this.m_RotationIn.BeginRotation.z) * Value);
			this.m_RectTransform.localRotation = Quaternion.Euler (x, y, z);
			if (!this.m_RotationIn.Animating && this.m_RotationIn.Began) {
				this.m_RotationIn.Animating = true;
				if (this.m_RotationIn.Sounds.m_AfterDelay != null) {
					this.PlaySoundOneShot (this.m_RotationIn.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimIn_ScaleComplete ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (this.m_ScaleIn.Sounds.m_End != null) {
				this.PlaySoundOneShot (this.m_ScaleIn.Sounds.m_End);
			}
			this.m_ScaleIn.Began = false;
			this.m_ScaleIn.Animating = false;
			this.m_ScaleIn.Done = true;
			this.m_ScaleOut.Began = false;
			this.m_ScaleOut.Animating = false;
			this.m_ScaleOut.Done = false;
			this.AnimIn_ScaleUpdateByValue (1f);
			this.StartMoveIdle ();
		}
	}

	private void AnimIn_ScaleUpdateByValue (float Value)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			base.transform.localScale = new Vector3 ((this.m_ScaleIn.ScaleBegin.x * this.m_ScaleOriginal.x) + ((this.m_ScaleOriginal.x - (this.m_ScaleIn.ScaleBegin.x * this.m_ScaleOriginal.x)) * Value), (this.m_ScaleIn.ScaleBegin.y * this.m_ScaleOriginal.y) + ((this.m_ScaleOriginal.y - (this.m_ScaleIn.ScaleBegin.y * this.m_ScaleOriginal.y)) * Value), (this.m_ScaleIn.ScaleBegin.z * this.m_ScaleOriginal.z) + ((this.m_ScaleOriginal.z - (this.m_ScaleIn.ScaleBegin.z * this.m_ScaleOriginal.z)) * Value));
			if (!this.m_ScaleIn.Animating && this.m_ScaleIn.Began) {
				this.m_ScaleIn.Animating = true;
				if (this.m_ScaleIn.Sounds.m_AfterDelay != null) {
					this.PlaySoundOneShot (this.m_ScaleIn.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimOut_FadeComplete ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (this.m_FadeOut.Sounds.m_End != null) {
				this.PlaySoundOneShot (this.m_FadeOut.Sounds.m_End);
			}
			this.m_FadeIn.Began = false;
			this.m_FadeIn.Animating = false;
			this.m_FadeIn.Done = false;
			this.m_FadeOut.Began = false;
			this.m_FadeOut.Animating = false;
			this.m_FadeOut.Done = true;
			this.AnimOut_FadeUpdateByValue (1f);
		}
	}

	private void AnimOut_FadeUpdateByValue (float Value)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			float fade = this.m_FadeOriginal + ((this.m_FadeOut.Fade - this.m_FadeOriginal) * Value);
			this.RecursiveFade (base.transform, fade, this.m_FadeOut.FadeChildren);
			if (this.m_TextOutline != null) {
				float num2 = Value * 3f;
				if (num2 > 1f) {
					num2 = 1f;
				}
				float fadeOutline = this.m_FadeOriginalTextOutline * (1f - num2);
				this.FadeTextOutline (base.transform, fadeOutline);
			}
			if (this.m_TextShadow != null) {
				float num4 = Value * 3f;
				if (num4 > 1f) {
					num4 = 1f;
				}
				float fadeShadow = this.m_FadeOriginalTextShadow * (1f - num4);
				this.FadeTextShadow (base.transform, fadeShadow);
			}
			if (!this.m_FadeOut.Animating && this.m_FadeOut.Began) {
				this.m_FadeOut.Animating = true;
				if (this.m_FadeOut.Sounds.m_AfterDelay != null) {
					this.PlaySoundOneShot (this.m_FadeOut.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimOut_MoveComplete ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (this.m_MoveOut.Sounds.m_End != null) {
				this.PlaySoundOneShot (this.m_MoveOut.Sounds.m_End);
			}
			this.m_MoveIn.Began = false;
			this.m_MoveIn.Animating = false;
			this.m_MoveIn.Done = false;
			this.m_MoveOut.Began = false;
			this.m_MoveOut.Animating = false;
			this.m_MoveOut.Done = true;
		}
	}

	private void AnimOut_MoveUpdateByValue (float Value)
	{
		if (((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) && (this.m_RectTransform != null)) {
			this.m_RectTransform.anchoredPosition = new Vector3 (this.m_MoveOut.BeginPos.x + ((this.m_MoveOut.EndPos.x - this.m_MoveOut.BeginPos.x) * Value), this.m_MoveOut.BeginPos.y + ((this.m_MoveOut.EndPos.y - this.m_MoveOut.BeginPos.y) * Value), this.m_MoveOut.BeginPos.z + ((this.m_MoveOut.EndPos.z - this.m_MoveOut.BeginPos.z) * Value));
			if (!this.m_MoveOut.Animating && this.m_MoveOut.Began) {
				this.m_MoveOut.Animating = true;
				if (this.m_MoveOut.Sounds.m_AfterDelay != null) {
					this.PlaySoundOneShot (this.m_MoveOut.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimOut_RotationComplete ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (this.m_RotationOut.Sounds.m_End != null) {
				this.PlaySoundOneShot (this.m_RotationOut.Sounds.m_End);
			}
			this.m_RotationIn.Began = false;
			this.m_RotationIn.Animating = false;
			this.m_RotationIn.Done = false;
			this.m_RotationOut.Began = false;
			this.m_RotationOut.Animating = false;
			this.m_RotationOut.Done = true;
			this.AnimOut_RotationUpdateByValue (1f);
		}
	}

	private void AnimOut_RotationUpdateByValue (float Value)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			float x = this.m_RotationOut.BeginRotation.x + ((this.m_RotationOut.EndRotation.x - this.m_RotationOut.BeginRotation.x) * Value);
			float y = this.m_RotationOut.BeginRotation.y + ((this.m_RotationOut.EndRotation.y - this.m_RotationOut.BeginRotation.y) * Value);
			float z = this.m_RotationOut.BeginRotation.z + ((this.m_RotationOut.EndRotation.z - this.m_RotationOut.BeginRotation.z) * Value);
			this.m_RectTransform.localRotation = Quaternion.Euler (x, y, z);
			if (!this.m_RotationOut.Animating && this.m_RotationOut.Began) {
				this.m_RotationOut.Animating = true;
				if (this.m_RotationOut.Sounds.m_AfterDelay != null) {
					this.PlaySoundOneShot (this.m_RotationOut.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void AnimOut_ScaleComplete ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (this.m_ScaleOut.Sounds.m_End != null) {
				this.PlaySoundOneShot (this.m_ScaleOut.Sounds.m_End);
			}
			this.m_ScaleIn.Began = false;
			this.m_ScaleIn.Animating = false;
			this.m_ScaleIn.Done = false;
			this.m_ScaleOut.Began = false;
			this.m_ScaleOut.Animating = false;
			this.m_ScaleOut.Done = true;
			this.AnimOut_ScaleUpdateByValue (1f);
		}
	}

	private void AnimOut_ScaleUpdateByValue (float Value)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (this.m_ScaleOut.ScaleBegin == this.m_ScaleOriginal) {
				base.transform.localScale = new Vector3 (this.m_ScaleOut.ScaleBegin.x + ((this.m_ScaleOut.ScaleEnd.x - this.m_ScaleOut.ScaleBegin.x) * Value), this.m_ScaleOut.ScaleBegin.y + ((this.m_ScaleOut.ScaleEnd.y - this.m_ScaleOut.ScaleBegin.y) * Value), this.m_ScaleOut.ScaleBegin.z + ((this.m_ScaleOut.ScaleEnd.z - this.m_ScaleOut.ScaleBegin.z) * Value));
			}
			else {
				float x = Mathf.Lerp (this.m_ScaleOut.ScaleBegin.x, this.m_ScaleOriginal.x, Value);
				float y = Mathf.Lerp (this.m_ScaleOut.ScaleBegin.y, this.m_ScaleOriginal.y, Value);
				float z = Mathf.Lerp (this.m_ScaleOut.ScaleBegin.z, this.m_ScaleOriginal.z, Value);
				Vector3 vector = new Vector3 (x, y, z);
				base.transform.localScale = new Vector3 (vector.x + ((this.m_ScaleOut.ScaleEnd.x - vector.x) * Value), vector.y + ((this.m_ScaleOut.ScaleEnd.y - vector.y) * Value), vector.z + ((this.m_ScaleOut.ScaleEnd.z - vector.z) * Value));
			}
			if (!this.m_ScaleOut.Animating && this.m_ScaleOut.Began) {
				this.m_ScaleOut.Animating = true;
				if (this.m_ScaleOut.Sounds.m_AfterDelay != null) {
					this.PlaySoundOneShot (this.m_ScaleOut.Sounds.m_AfterDelay);
				}
			}
		}
	}

	private void Awake ()
	{
		if ((this != null) && (base.gameObject != null)) {
			GETween.Init (0x640);
			this.m_RectTransform = (RectTransform)base.transform;
			this.m_MoveOriginal = (Vector3)this.m_RectTransform.anchoredPosition;
			this.m_RotationOriginal = this.m_RectTransform.localRotation;
			this.m_ScaleOriginal = base.transform.localScale;
			this.m_FadeOriginal = 1f;
			this.m_FadeOriginalTextOutline = 0.5f;
			this.m_FadeOriginalTextShadow = 0.5f;
			this.m_Image = base.gameObject.GetComponent<Image> ();
			this.m_Toggle = base.gameObject.GetComponent<Toggle> ();
			this.m_Text = base.gameObject.GetComponent<Text> ();
			this.m_TextOutline = base.gameObject.GetComponent<Outline> ();
			this.m_TextShadow = base.gameObject.GetComponent<Shadow> ();
			this.m_RawImage = base.gameObject.GetComponent<RawImage> ();
			this.m_Slider = base.gameObject.GetComponent<Slider> ();
			this.m_CanvasRenderer = base.gameObject.GetComponent<CanvasRenderer> ();
			this.m_FadeOriginal = this.GetFadeValue (base.transform);
			this.m_FadeOriginalTextOutline = this.GetFadeTextOutlineValue (base.transform);
			this.m_FadeOriginalTextShadow = this.GetFadeTextShadowValue (base.transform);
		}
	}

	private void CalculateCameraArea ()
	{
		if ((this != null) && (base.gameObject != null)) {
			if (this.m_Parent_Canvas == null) {
				this.m_Parent_Canvas = GUIAnimSystem.Instance.GetParent_Canvas (base.transform);
			}
			if (this.m_Parent_Canvas != null) {
				this.m_ParentCanvasRectTransform = this.m_Parent_Canvas.GetComponent<RectTransform> ();
				this.m_CameraRightEdge = (this.m_ParentCanvasRectTransform.rect.width / 2f) + this.m_TotalBounds.size.x;
				this.m_CameraLeftEdge = -this.m_CameraRightEdge;
				this.m_CameraTopEdge = (this.m_ParentCanvasRectTransform.rect.height / 2f) + this.m_TotalBounds.size.y;
				this.m_CameraBottomEdge = -this.m_CameraTopEdge;
				this.m_CanvasWorldTopLeft = this.m_ParentCanvasRectTransform.TransformPoint (new Vector3 (this.m_CameraLeftEdge, this.m_CameraTopEdge, 0f));
				this.m_CanvasWorldTopCenter = this.m_ParentCanvasRectTransform.TransformPoint (new Vector3 (0f, this.m_CameraTopEdge, 0f));
				this.m_CanvasWorldTopRight = this.m_ParentCanvasRectTransform.TransformPoint (new Vector3 (this.m_CameraRightEdge, this.m_CameraTopEdge, 0f));
				this.m_CanvasWorldMiddleLeft = this.m_ParentCanvasRectTransform.TransformPoint (new Vector3 (this.m_CameraLeftEdge, 0f, 0f));
				this.m_CanvasWorldMiddleCenter = this.m_ParentCanvasRectTransform.TransformPoint (new Vector3 (0f, 0f, 0f));
				this.m_CanvasWorldMiddleRight = this.m_ParentCanvasRectTransform.TransformPoint (new Vector3 (this.m_CameraRightEdge, 0f, 0f));
				this.m_CanvasWorldBottomLeft = this.m_ParentCanvasRectTransform.TransformPoint (new Vector3 (this.m_CameraLeftEdge, this.m_CameraBottomEdge, 0f));
				this.m_CanvasWorldBottomCenter = this.m_ParentCanvasRectTransform.TransformPoint (new Vector3 (0f, this.m_CameraBottomEdge, 0f));
				this.m_CanvasWorldBottomRight = this.m_ParentCanvasRectTransform.TransformPoint (new Vector3 (this.m_CameraRightEdge, this.m_CameraBottomEdge, 0f));
			}
		}
	}

	private void CalculateTotalBounds ()
	{
		this.m_TotalBounds = new Bounds (Vector3.zero, Vector3.zero);
		if (((this.m_Slider != null) || (this.m_Toggle != null)) || (this.m_CanvasRenderer != null)) {
			RectTransform component = base.gameObject.GetComponent<RectTransform> ();
			this.m_TotalBounds.size = new Vector3 (component.rect.width, component.rect.height, 0f);
		}
	}

	private IEnumerator CoroutineMoveIdle (float Delay)
	{
		yield return new WaitForSeconds (Delay);
		this.m_MoveIdle_Attemp++;
		if (this.m_MoveIdle_Attemp <= this.m_MoveIdle_AttempMax) {
			this.MoveIdle ();
		}
	}

	private IEnumerator CoroutineMoveOut (float Delay)
	{
		yield return new WaitForSeconds (Delay);
		this.MoveOut ();
	}

	private void FadeLoopComplete ()
	{
	}

	private void FadeLoopStart (float delay)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			this.m_FadeLoop.Began = true;
			this.m_FadeLoop.Animating = false;
			this.m_FadeLoop.IsOverriding = false;
			this.m_FadeLoop.IsOverridingDelayTimeCount = 0f;
			this.m_FadeLoop.Done = false;
			this.m_FadeLoop.m_FadeLast = this.GetFadeValue (base.transform);
			this.m_GETweenFadeLoop = GETween.TweenValue (base.gameObject, new Action<float> (this.FadeLoopUpdateByValue), 0f, 1f, this.m_FadeLoop.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (this.GETweenEaseType (this.m_FadeLoop.EaseType)).SetLoopPingPong ().SetOnComplete (new Action (this.FadeLoopComplete)).SetUseEstimatedTime (true);
		}
	}

	private void FadeLoopUpdateByValue (float Value)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			float fadeValue = this.GetFadeValue (base.transform);
			if (this.m_FadeLoop.m_FadeLast != fadeValue) {
				this.StopFadeLoop ();
			}
			else {
				if (this.m_FadeLoop.IsOverriding) {
					this.m_FadeLoop.IsOverridingDelayTimeCount += Time.deltaTime;
					if (this.m_FadeLoop.IsOverridingDelayTimeCount > 2f) {
						this.m_FadeLoop.IsOverridingDelayTimeCount = 0f;
						this.m_FadeLoop.IsOverriding = false;
					}
				}
				if (!this.m_FadeLoop.IsOverriding) {
					if (!this.m_FadeLoop.Animating) {
						this.StartFadeLoopSound ();
					}
					fadeValue = this.m_FadeLoop.Min + ((this.m_FadeLoop.Max - this.m_FadeLoop.Min) * Value);
					this.RecursiveFade (base.transform, fadeValue, this.m_FadeLoop.FadeChildren);
					if (this.m_TextOutline != null) {
						float num2 = this.m_FadeOriginalTextOutline * Value;
						float fadeOutline = 0f;
						if (num2 > this.m_TextOutline.effectColor.a) {
							num2 = Value - 0.75f;
							if (num2 < 0f) {
								num2 = 0f;
							}
							if (num2 > 0f) {
								num2 *= 4f;
							}
							fadeOutline = this.m_FadeOriginalTextOutline * num2;
							this.FadeTextOutline (base.transform, fadeOutline);
						}
						if (num2 < this.m_TextOutline.effectColor.a) {
							num2 = Value * 2f;
							if (num2 > 1f) {
								num2 = 1f;
							}
							fadeOutline = this.m_FadeOriginalTextOutline * (1f - num2);
							this.FadeTextOutline (base.transform, fadeOutline);
						}
					}
					if (this.m_TextShadow != null) {
						float num4 = this.m_FadeOriginalTextShadow * Value;
						float fadeShadow = 0f;
						if (num4 > this.m_TextShadow.effectColor.a) {
							num4 = Value - 0.75f;
							if (num4 < 0f) {
								num4 = 0f;
							}
							if (num4 > 0f) {
								num4 *= 4f;
							}
							fadeShadow = this.m_FadeOriginalTextShadow * num4;
							this.FadeTextShadow (base.transform, fadeShadow);
						}
						if (num4 < this.m_TextShadow.effectColor.a) {
							num4 = Value * 2f;
							if (num4 > 1f) {
								num4 = 1f;
							}
							fadeShadow = this.m_FadeOriginalTextShadow * (1f - num4);
							this.FadeTextShadow (base.transform, fadeShadow);
						}
					}
				}
			}
			this.m_FadeLoop.m_FadeLast = fadeValue;
		}
	}

	private void FadeTextOutline (Transform trans, float FadeOutline)
	{
		if (this.m_TextOutline != null) {
			this.m_TextOutline.effectColor = new Color (this.m_TextOutline.effectColor.r, this.m_TextOutline.effectColor.g, this.m_TextOutline.effectColor.b, FadeOutline);
		}
	}

	private void FadeTextShadow (Transform trans, float FadeShadow)
	{
		if (this.m_TextShadow != null) {
			this.m_TextShadow.effectColor = new Color (this.m_TextShadow.effectColor.r, this.m_TextShadow.effectColor.g, this.m_TextShadow.effectColor.b, FadeShadow);
		}
	}

	private float GetFadeTextOutlineValue (Transform trans)
	{
		float a = 1f;
		if (this.m_TextOutline != null) {
			a = this.m_TextOutline.effectColor.a;
		}
		return a;
	}

	private float GetFadeTextShadowValue (Transform trans)
	{
		float a = 1f;
		if (this.m_TextShadow != null) {
			a = this.m_TextShadow.effectColor.a;
		}
		return a;
	}

	private float GetFadeValue (Transform trans)
	{
		float a = 1f;
		if (this.m_Image != null) {
			a = this.m_Image.color.a;
		}
		if (this.m_Toggle != null) {
			a = this.m_Toggle.GetComponentInChildren<Image> ().color.a;
		}
		if (this.m_Text != null) {
			a = this.m_Text.color.a;
		}
		if (this.m_RawImage != null) {
			a = this.m_RawImage.color.a;
		}
		if (this.m_Slider != null) {
			a = this.m_Slider.colors.normalColor.a;
		}
		return a;
	}

	public GETween.GETweenType GETweenEaseType (eEaseType easeType)
	{
		switch (easeType) {
		case eEaseType.InQuad:
			return GETween.GETweenType.easeInQuad;

		case eEaseType.OutQuad:
			return GETween.GETweenType.easeOutQuad;

		case eEaseType.InOutQuad:
			return GETween.GETweenType.easeInOutQuad;

		case eEaseType.InCubic:
			return GETween.GETweenType.easeOutCubic;

		case eEaseType.OutCubic:
			return GETween.GETweenType.easeOutCubic;

		case eEaseType.InOutCubic:
			return GETween.GETweenType.easeInOutCubic;

		case eEaseType.InQuart:
			return GETween.GETweenType.easeInQuart;

		case eEaseType.OutQuart:
			return GETween.GETweenType.easeOutQuart;

		case eEaseType.InOutQuart:
			return GETween.GETweenType.easeInOutQuart;

		case eEaseType.InQuint:
			return GETween.GETweenType.easeInQuint;

		case eEaseType.OutQuint:
			return GETween.GETweenType.easeOutQuint;

		case eEaseType.InOutQuint:
			return GETween.GETweenType.easeInOutQuint;

		case eEaseType.InSine:
			return GETween.GETweenType.easeInSine;

		case eEaseType.OutSine:
			return GETween.GETweenType.easeOutSine;

		case eEaseType.InOutSine:
			return GETween.GETweenType.easeInOutSine;

		case eEaseType.InExpo:
			return GETween.GETweenType.easeInExpo;

		case eEaseType.OutExpo:
			return GETween.GETweenType.easeOutExpo;

		case eEaseType.InOutExpo:
			return GETween.GETweenType.easeInOutExpo;

		case eEaseType.InCirc:
			return GETween.GETweenType.easeInCirc;

		case eEaseType.OutCirc:
			return GETween.GETweenType.easeOutCirc;

		case eEaseType.InOutCirc:
			return GETween.GETweenType.easeInOutCirc;

		case eEaseType.linear:
			return GETween.GETweenType.linear;

		case eEaseType.InBounce:
			return GETween.GETweenType.easeInBounce;

		case eEaseType.OutBounce:
			return GETween.GETweenType.easeOutBounce;

		case eEaseType.InOutBounce:
			return GETween.GETweenType.easeInOutBounce;

		case eEaseType.InBack:
			return GETween.GETweenType.easeInBack;

		case eEaseType.OutBack:
			return GETween.GETweenType.easeOutBack;

		case eEaseType.InOutBack:
			return GETween.GETweenType.easeInOutBack;

		case eEaseType.InElastic:
			return GETween.GETweenType.easeInElastic;

		case eEaseType.OutElastic:
			return GETween.GETweenType.easeOutElastic;

		case eEaseType.InOutElastic:
			return GETween.GETweenType.easeInOutElastic;
		}
		return GETween.GETweenType.linear;
	}

	private void InitFadeIn ()
	{
		if (((this != null) && (base.gameObject != null)) && this.m_FadeIn.Enable) {
			this.RecursiveFade (base.transform, this.m_FadeIn.Fade, this.m_FadeIn.FadeChildren);
		}
	}

	private void InitMoveIn ()
	{
		if (((this != null) && (base.gameObject != null)) && (this.m_MoveIn.Enable && !this.m_MoveIn.Done)) {
			this.CalculateTotalBounds ();
			this.CalculateCameraArea ();
			RectTransform component = base.transform.parent.GetComponent<RectTransform> ();
			switch (this.m_MoveIn.MoveFrom) {
			case ePosMove.ParentPosition:
				if (base.transform.parent != null) {
					this.m_MoveIn.BeginPos = new Vector3 (0f, 0f, this.m_RectTransform.localPosition.z);
				}
				break;

			case ePosMove.LocalPosition:
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.Position.x, this.m_MoveIn.Position.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.UpperScreenEdge:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldTopCenter);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_RectTransform.localPosition.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.LeftScreenEdge:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldMiddleLeft);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_RectTransform.localPosition.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.RightScreenEdge:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldMiddleRight);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_RectTransform.localPosition.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.BottomScreenEdge:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldBottomCenter);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_RectTransform.localPosition.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.UpperLeft:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldTopLeft);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.UpperCenter:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldTopCenter);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.UpperRight:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldTopRight);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.MiddleLeft:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldMiddleLeft);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.MiddleCenter:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldMiddleCenter);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.MiddleRight:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldMiddleRight);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.BottomLeft:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldBottomLeft);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.BottomCenter:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldBottomCenter);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.BottomRight:
				this.m_MoveIn.BeginPos = component.InverseTransformPoint (this.m_CanvasWorldBottomRight);
				this.m_MoveIn.BeginPos = new Vector3 (this.m_MoveIn.BeginPos.x, this.m_MoveIn.BeginPos.y, this.m_RectTransform.localPosition.z);
				break;

			case ePosMove.SelfPosition:
				this.m_MoveIn.BeginPos = this.m_MoveOriginal;
				break;
			}
			this.m_RectTransform.anchoredPosition = this.m_MoveIn.BeginPos;
			this.m_MoveIn.EndPos = this.m_MoveOriginal;
		}
	}

	private void InitMoveOut ()
	{
		if (((this != null) && (base.gameObject != null)) && this.m_MoveOut.Enable) {
			this.CalculateTotalBounds ();
			this.CalculateCameraArea ();
			RectTransform component = base.transform.parent.GetComponent<RectTransform> ();
			switch (this.m_MoveOut.MoveTo) {
			case ePosMove.ParentPosition:
				if (base.transform.parent != null) {
					this.m_MoveOut.EndPos = new Vector3 (0f, 0f, this.m_RectTransform.localPosition.z);
				}
				return;

			case ePosMove.LocalPosition:
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.Position.x, this.m_MoveOut.Position.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.UpperScreenEdge:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldTopCenter);
				this.m_MoveOut.EndPos = new Vector3 (this.m_RectTransform.localPosition.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.LeftScreenEdge:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldMiddleLeft);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_RectTransform.localPosition.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.RightScreenEdge:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldMiddleRight);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_RectTransform.localPosition.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.BottomScreenEdge:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldBottomCenter);
				this.m_MoveOut.EndPos = new Vector3 (this.m_RectTransform.localPosition.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.UpperLeft:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldTopLeft);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.UpperCenter:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldTopCenter);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.UpperRight:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldTopRight);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.MiddleLeft:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldMiddleLeft);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.MiddleCenter:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldMiddleCenter);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.MiddleRight:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldMiddleRight);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.BottomLeft:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldBottomLeft);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.BottomCenter:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldBottomCenter);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.BottomRight:
				this.m_MoveOut.EndPos = component.InverseTransformPoint (this.m_CanvasWorldBottomRight);
				this.m_MoveOut.EndPos = new Vector3 (this.m_MoveOut.EndPos.x, this.m_MoveOut.EndPos.y, this.m_RectTransform.localPosition.z);
				return;

			case ePosMove.SelfPosition:
				this.m_MoveOut.EndPos = this.m_MoveOriginal;
				return;
			}
		}
	}

	private void InitRotationIn ()
	{
		if (((this != null) && (base.gameObject != null)) && this.m_RotationIn.Enable) {
			this.m_RotationIn.BeginRotation = this.m_RotationIn.Rotation;
			this.m_RotationIn.EndRotation = this.m_RotationOriginal.eulerAngles;
		}
	}

	private void InitScaleIn ()
	{
		if (((this != null) && (base.gameObject != null)) && this.m_ScaleIn.Enable) {
			base.transform.localScale = this.m_ScaleIn.ScaleBegin;
		}
	}

	private IEnumerator InitScaleIn (float Delay)
	{
		yield return new WaitForSeconds (Delay);
		this.InitScaleIn ();
	}

	public bool IsAnimating ()
	{
		if (this == null) {
			return false;
		}
		if (base.gameObject == null) {
			return false;
		}
		if (!base.gameObject.activeSelf) {
			return false;
		}
		if (!base.enabled) {
			return false;
		}
		if (((!this.m_MoveIn.Began && !this.m_MoveOut.Began) && (!this.m_RotationIn.Began && !this.m_RotationOut.Began)) && ((!this.m_ScaleIn.Began && !this.m_ScaleOut.Began) && (!this.m_FadeIn.Began && !this.m_FadeOut.Began))) {
			return false;
		}
		return true;
	}

	private void MoveIdle ()
	{
		if (((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) && !this.m_MoveIdle_StartSucceed) {
			if (!this.m_MoveIn.Began && !this.m_RotationIn.Began) {
				if (this.m_ScaleLoop.Enable && !this.m_ScaleIn.Began) {
					this.m_MoveIdle_StartSucceed = true;
					this.ScaleLoopStart (this.m_ScaleLoop.Delay);
				}
				if (this.m_FadeLoop.Enable && !this.m_FadeIn.Began) {
					this.m_MoveIdle_StartSucceed = true;
					this.FadeLoopStart (this.m_FadeLoop.Delay);
				}
			}
			if (!this.m_MoveIdle_StartSucceed && gameObject.activeInHierarchy) {
				base.StartCoroutine (this.CoroutineMoveIdle (this.m_MoveIdle_AttempMax_TimeInterval));
			}
		}
	}

	public void MoveIn ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			this.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
		}
	}

	public void MoveIn (GUIAnimSystem.eGUIMove GUIMoveType)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if ((GUIMoveType == GUIAnimSystem.eGUIMove.Self) || (GUIMoveType == GUIAnimSystem.eGUIMove.SelfAndChildren)) {
				if (this.m_MoveIn.Enable && !this.m_MoveIn.Began) {
					this.m_MoveIn.Began = true;
					this.m_MoveIn.Animating = false;
					this.m_MoveIn.Done = false;
					GETween.TweenValue (base.gameObject, new Action<float> (this.AnimIn_MoveUpdateByValue), 0f, 1f, this.m_MoveIn.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (this.m_MoveIn.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (this.GETweenEaseType (this.m_MoveIn.EaseType)).SetOnComplete (new Action (this.AnimIn_MoveComplete)).SetUseEstimatedTime (true);
					if (this.m_MoveIn.Sounds.m_Begin != null) {
						this.PlaySoundOneShot (this.m_MoveIn.Sounds.m_Begin);
					}
				}
				if (this.m_RotationIn.Enable && !this.m_RotationIn.Began) {
					this.m_RotationIn.Began = true;
					this.m_RotationIn.Animating = false;
					this.m_RotationIn.Done = false;
					GETween.TweenValue (base.gameObject, new Action<float> (this.AnimIn_RotationUpdateByValue), 0f, 1f, this.m_RotationIn.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (this.m_RotationIn.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (this.GETweenEaseType (this.m_RotationIn.EaseType)).SetOnComplete (new Action (this.AnimIn_RotationComplete)).SetUseEstimatedTime (true);
					if (this.m_RotationIn.Sounds.m_Begin != null) {
						this.PlaySoundOneShot (this.m_RotationIn.Sounds.m_Begin);
					}
				}
				if (this.m_ScaleIn.Enable && !this.m_ScaleIn.Began) {
					this.m_ScaleIn.Began = true;
					this.m_ScaleIn.Animating = false;
					this.m_ScaleIn.Done = false;
					GETween.TweenValue (base.gameObject, new Action<float> (this.AnimIn_ScaleUpdateByValue), 0f, 1f, this.m_ScaleIn.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (this.m_ScaleIn.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (this.GETweenEaseType (this.m_ScaleIn.EaseType)).SetOnComplete (new Action (this.AnimIn_ScaleComplete)).SetUseEstimatedTime (true);
					if (this.m_ScaleIn.Sounds.m_Begin != null) {
						this.PlaySoundOneShot (this.m_ScaleIn.Sounds.m_Begin);
					}
				}
				if (this.m_FadeIn.Enable && !this.m_FadeIn.Began) {
					this.m_FadeIn.Began = true;
					this.m_FadeIn.Animating = false;
					this.m_FadeIn.Done = false;
					GETween.TweenValue (base.gameObject, new Action<float> (this.AnimIn_FadeUpdateByValue), 0f, 1f, this.m_FadeIn.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (this.m_FadeIn.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (this.GETweenEaseType (this.m_FadeIn.EaseType)).SetOnComplete (new Action (this.AnimIn_FadeComplete)).SetUseEstimatedTime (true);
					if (this.m_FadeIn.Sounds.m_Begin != null) {
						this.PlaySoundOneShot (this.m_FadeIn.Sounds.m_Begin);
					}
				}
				if (((!this.m_MoveIn.Enable && !this.m_RotationIn.Enable) && (!this.m_ScaleIn.Enable && !this.m_FadeIn.Enable)) && (this.m_ScaleLoop.Enable || this.m_FadeLoop.Enable)) {
					this.StartMoveIdle ();
				}
			}
			if ((GUIMoveType == GUIAnimSystem.eGUIMove.Children) || (GUIMoveType == GUIAnimSystem.eGUIMove.SelfAndChildren)) {
				this.RecuresiveMoveIn (base.transform);
			}
		}
	}

	public void MoveOut ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			this.MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);
		}
	}

	public void MoveOut (GUIAnimSystem.eGUIMove GUIMoveType)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if ((GUIMoveType == GUIAnimSystem.eGUIMove.Self) || (GUIMoveType == GUIAnimSystem.eGUIMove.SelfAndChildren)) {
				if (this.m_ScaleLoop.Enable && this.m_ScaleLoop.Began) {
					this.m_ScaleLoop.Began = false;
					this.m_ScaleLoop.Done = true;
					this.StopScaleLoop ();
				}
				if (this.m_FadeLoop.Enable && this.m_FadeLoop.Began) {
					this.m_FadeLoop.Began = false;
					this.m_FadeLoop.Done = true;
					this.StopFadeLoop ();
				}
				if (this.m_MoveOut.Enable && !this.m_MoveOut.Began) {
					this.m_MoveOut.Began = true;
					this.m_MoveOut.Animating = false;
					this.m_MoveOut.Done = false;
					this.m_MoveOut.BeginPos = (Vector3)this.m_RectTransform.anchoredPosition;
					GETween.TweenValue (base.gameObject, new Action<float> (this.AnimOut_MoveUpdateByValue), 0f, 1f, this.m_MoveOut.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (this.m_MoveOut.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (this.GETweenEaseType (this.m_MoveOut.EaseType)).SetOnComplete (new Action (this.AnimOut_MoveComplete)).SetUseEstimatedTime (true);
					if (this.m_MoveOut.Sounds.m_Begin != null) {
						this.PlaySoundOneShot (this.m_MoveOut.Sounds.m_Begin);
					}
				}
				if (this.m_RotationOut.Enable && !this.m_RotationOut.Began) {
					this.m_RotationOut.Began = true;
					this.m_RotationOut.Animating = false;
					this.m_RotationOut.Done = false;
					this.m_RotationOut.BeginRotation = this.m_RectTransform.localRotation.eulerAngles;
					this.m_RotationOut.EndRotation = this.m_RotationOut.Rotation;
					GETween.TweenValue (base.gameObject, new Action<float> (this.AnimOut_RotationUpdateByValue), 0f, 1f, this.m_RotationOut.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (this.m_RotationOut.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (this.GETweenEaseType (this.m_RotationOut.EaseType)).SetOnComplete (new Action (this.AnimOut_RotationComplete)).SetUseEstimatedTime (true);
					if (this.m_RotationOut.Sounds.m_Begin != null) {
						this.PlaySoundOneShot (this.m_RotationOut.Sounds.m_Begin);
					}
				}
				if (this.m_ScaleOut.Enable && !this.m_ScaleOut.Began) {
					this.m_ScaleOut.Began = true;
					this.m_ScaleOut.Animating = false;
					this.m_ScaleOut.Done = false;
					this.m_ScaleOut.ScaleBegin = base.transform.localScale;
					GETween.TweenValue (base.gameObject, new Action<float> (this.AnimOut_ScaleUpdateByValue), 0f, 1f, this.m_ScaleOut.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (this.m_ScaleOut.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (this.GETweenEaseType (this.m_ScaleOut.EaseType)).SetOnComplete (new Action (this.AnimOut_ScaleComplete)).SetUseEstimatedTime (true);
					if (this.m_ScaleOut.Sounds.m_Begin != null) {
						this.PlaySoundOneShot (this.m_ScaleOut.Sounds.m_Begin);
					}
				}
				if (this.m_FadeOut.Enable && !this.m_FadeOut.Began) {
					this.m_FadeOut.Began = true;
					this.m_FadeOut.Animating = false;
					this.m_FadeOut.Done = false;
					GETween.TweenValue (base.gameObject, new Action<float> (this.AnimOut_FadeUpdateByValue), 0f, 1f, this.m_FadeOut.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (this.m_FadeOut.Delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (this.GETweenEaseType (this.m_FadeOut.EaseType)).SetOnComplete (new Action (this.AnimOut_FadeComplete)).SetUseEstimatedTime (true);
					if (this.m_FadeOut.Sounds.m_Begin != null) {
						this.PlaySoundOneShot (this.m_FadeOut.Sounds.m_Begin);
					}
				}
			}
			if ((GUIMoveType == GUIAnimSystem.eGUIMove.Children) || (GUIMoveType == GUIAnimSystem.eGUIMove.SelfAndChildren)) {
				this.RecuresiveMoveOut (base.transform);
			}
		}
	}

	private AudioSource PlaySoundLoop (AudioClip pAudioClip)
	{
		if (this == null) {
			return null;
		}
		if (base.gameObject == null) {
			return null;
		}
		if (!base.gameObject.activeSelf) {
			return null;
		}
		if (!base.enabled) {
			return null;
		}
		AudioSource source = null;
		AudioListener listener = UnityEngine.Object.FindObjectOfType<AudioListener> ();
		if (listener != null) {
			bool flag = false;
			AudioSource[] components = listener.gameObject.GetComponents<AudioSource> ();
			if (components.Length != 0) {
				for (int i = 0; i < components.Length; i++) {
					if (!components [i].isPlaying) {
						flag = true;
						source = components [i];
						source.clip = pAudioClip;
						source.loop = true;
						source.Play ();
						break;
					}
				}
			}
			if (!flag && (components.Length < 0x10)) {
				source = listener.gameObject.AddComponent<AudioSource> ();
				source.rolloffMode = AudioRolloffMode.Linear;
				source.playOnAwake = false;
				source.clip = pAudioClip;
				source.loop = true;
				source.Play ();
			}
		}
		return source;
	}

	private void PlaySoundOneShot (AudioClip pAudioClip)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			AudioListener listener = UnityEngine.Object.FindObjectOfType<AudioListener> ();
			if (listener != null) {
				bool flag = false;
				AudioSource[] components = listener.gameObject.GetComponents<AudioSource> ();
				if (components.Length != 0) {
					for (int i = 0; i < components.Length; i++) {
						if (!components [i].isPlaying) {
							flag = true;
							components [i].PlayOneShot (pAudioClip);
							break;
						}
					}
				}
				if (!flag && (components.Length < 0x20)) {
					AudioSource local1 = listener.gameObject.AddComponent<AudioSource> ();
					local1.rolloffMode = AudioRolloffMode.Linear;
					local1.playOnAwake = false;
					local1.PlayOneShot (pAudioClip);
				}
			}
		}
	}

	private IEnumerator PlaySoundOneShotWithDelay (AudioClip pAudioClip, float Delay)
	{
		yield return new WaitForSeconds (Delay);
		this.PlaySoundOneShot (pAudioClip);
	}

	private void RecuresiveMoveIn (Transform tf)
	{
		foreach (Transform transform in tf) {
			if (transform.gameObject.activeSelf) {
				GUIAnim component = transform.gameObject.GetComponent<GUIAnim> ();
				if ((component != null) && component.enabled) {
					component.MoveIn (GUIAnimSystem.eGUIMove.SelfAndChildren);
				}
				this.RecuresiveMoveIn (transform);
			}
		}
	}

	private void RecuresiveMoveOut (Transform tf)
	{
		foreach (Transform transform in tf) {
			if (transform.gameObject.activeSelf) {
				GUIAnim component = transform.gameObject.GetComponent<GUIAnim> ();
				if ((component != null) && component.enabled) {
					component.MoveOut (GUIAnimSystem.eGUIMove.SelfAndChildren);
				}
				this.RecuresiveMoveOut (transform);
			}
		}
	}

	private void RecursiveFade (Transform trans, float Fade, bool IsFadeChildren)
	{
		bool flag = false;
		if (base.transform != trans) {
			GUIAnim component = trans.GetComponent<GUIAnim> ();
			if (component != null) {
				if (component.m_FadeIn.Enable && component.m_FadeIn.Began) {
					flag = true;
				}
				else if (component.m_FadeOut.Enable && component.m_FadeOut.Began) {
					flag = true;
				}
			}
		}
		if (!flag) {
			Image image = trans.gameObject.GetComponent<Image> ();
			if (image != null) {
				image.color = new Color (image.color.r, image.color.g, image.color.b, Fade);
			}
			Text text = trans.gameObject.GetComponent<Text> ();
			if (text != null) {
				text.color = new Color (text.color.r, text.color.g, text.color.b, Fade);
			}
			RawImage image2 = trans.gameObject.GetComponent<RawImage> ();
			if (image2 != null) {
				image2.color = new Color (image2.color.r, image2.color.g, image2.color.b, Fade);
			}
		}
		if (IsFadeChildren) {
			foreach (Transform transform in trans) {
				if (transform.gameObject.activeSelf) {
					this.RecursiveFade (transform.transform, Fade, IsFadeChildren);
				}
			}
		}
	}

	public void Reset ()
	{
		if ((this != null) && (base.gameObject != null)) {
			if (this.m_MoveIn == null) {
				if (UnityEngine.Object.FindObjectOfType<GUIAnimSystem> () == null) {
					GameObject target = new GameObject {
						transform = { localPosition = new Vector3 (0f, 0f, 0f) },
						name = "GUIAnimSystem"
					};
					target.AddComponent<GUIAnimSystem> ();
					GUIAnimSystem.Instance = UnityEngine.Object.FindObjectOfType<GUIAnimSystem> ();
					UnityEngine.Object.DontDestroyOnLoad (target);
				}
			}
			else {
				this.InitMoveIn ();
				this.InitMoveOut ();
				this.InitRotationIn ();
				this.InitFadeIn ();
				if (this.m_ScaleIn.Enable) {
					this.InitScaleIn ();
				}
			}
		}
	}

	public void ResetAllChildren ()
	{
		if ((this != null) && (base.gameObject != null)) {
			this.InitMoveIn ();
			this.InitMoveOut ();
			this.InitRotationIn ();
			this.InitFadeIn ();
			foreach (Transform transform in base.transform) {
				if (transform.gameObject.activeSelf) {
					GUIAnim component = transform.gameObject.GetComponent<GUIAnim> ();
					if ((component != null) && component.enabled) {
						component.ResetAllChildren ();
					}
				}
			}
			if (this.m_ScaleIn.Enable) {
				this.InitScaleIn ();
			}
		}
	}

	private void ScaleLoopComplete ()
	{
	}

	private void ScaleLoopStart (float delay)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			this.m_ScaleLoop.Began = true;
			this.m_ScaleLoop.Animating = false;
			this.m_ScaleLoop.IsOverriding = false;
			this.m_ScaleLoop.IsOverridingDelayTimeCount = 0f;
			this.m_ScaleLoop.Done = false;
			this.m_ScaleLoop.m_ScaleLast = base.transform.localScale;
			this.m_GETweenScaleLoop = GETween.TweenValue (base.gameObject, new Action<float> (this.ScaleLoopUpdateByValue), 0f, 1f, this.m_ScaleLoop.Time / GUIAnimSystem.Instance.m_GUISpeed).SetDelay (delay / GUIAnimSystem.Instance.m_GUISpeed).SetEase (this.GETweenEaseType (this.m_ScaleLoop.EaseType)).SetLoopPingPong ().SetOnComplete (new Action (this.ScaleLoopComplete)).SetUseEstimatedTime (true);
		}
	}

	private void ScaleLoopUpdateByValue (float Value)
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			if (this.m_ScaleLoop.m_ScaleLast != base.transform.localScale) {
				this.StopScaleLoop ();
			}
			else {
				if (this.m_ScaleLoop.IsOverriding) {
					this.m_ScaleLoop.IsOverridingDelayTimeCount += Time.deltaTime;
					if (this.m_ScaleLoop.IsOverridingDelayTimeCount > 2f) {
						this.m_ScaleLoop.IsOverridingDelayTimeCount = 0f;
						this.m_ScaleLoop.IsOverriding = false;
					}
				}
				if (!this.m_ScaleLoop.IsOverriding) {
					if (!this.m_ScaleLoop.Animating) {
						this.StartScaleLoopSound ();
					}
					base.transform.localScale = new Vector3 ((this.m_ScaleLoop.Min.x * this.m_ScaleOriginal.x) + (((this.m_ScaleLoop.Max.x * this.m_ScaleOriginal.x) - (this.m_ScaleLoop.Min.x * this.m_ScaleOriginal.x)) * Value), (this.m_ScaleLoop.Min.y * this.m_ScaleOriginal.y) + (((this.m_ScaleLoop.Max.y * this.m_ScaleOriginal.y) - (this.m_ScaleLoop.Min.y * this.m_ScaleOriginal.y)) * Value), (this.m_ScaleLoop.Min.z * this.m_ScaleOriginal.z) + (((this.m_ScaleLoop.Max.z * this.m_ScaleOriginal.z) - (this.m_ScaleLoop.Min.z * this.m_ScaleOriginal.z)) * Value));
				}
			}
			this.m_ScaleLoop.m_ScaleLast = base.transform.localScale;
		}
	}

	public void ScreenResolutionChange ()
	{
		if ((this != null) && (base.gameObject != null)) {
			this.InitMoveIn ();
			this.InitMoveOut ();
		}
	}

	private void Start ()
	{
		if (((this != null) && (base.gameObject != null)) && !this.m_InitialDone) {
			this.m_InitialDone = true;
			bool flag = true;
			if (GUIAnimSystem.Instance.m_AutoAnimation) {
				if ((GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.In) || (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.All)) {
					if ((this.m_MoveIn.Enable || this.m_RotationIn.Enable) || (this.m_ScaleIn.Enable || this.m_FadeIn.Enable)) {
						this.MoveIn ();
					}
					else if (this.m_ScaleLoop.Enable || this.m_FadeLoop.Enable) {
						flag = false;
						this.StartMoveIdle ();
					}
				}
				else if (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.Idle) {
					flag = false;
					this.StartMoveIdle ();
				}
				else if (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.Out) {
					flag = false;
					this.InitMoveOut ();
				}
				else if (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.None) {
					flag = false;
				}
			}
			if (flag) {
				this.Reset ();
			}
			if (GUIAnimSystem.Instance.m_AutoAnimation && ((GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.Out) || (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.All))) {
				float delay = 1f;
				if ((GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.In) || (GUIAnimSystem.Instance.m_AnimationMode == GUIAnimSystem.eAnimationMode.All)) {
					delay = GUIAnimSystem.Instance.m_IdleTime;
				}
				//base.StartCoroutine (this.CoroutineMoveOut (delay));
			}
		}
	}

	private void StartFadeLoopSound ()
	{
		if (!this.m_FadeLoop.Animating && (this.m_FadeLoop.Sound.m_AudioClip != null)) {
			this.m_FadeLoop.Animating = true;
			this.m_FadeLoop.Sound.m_AudioSource = null;
			if (this.m_FadeLoop.Sound.m_Loop) {
				this.m_FadeLoop.Sound.m_AudioSource = this.PlaySoundLoop (this.m_FadeLoop.Sound.m_AudioClip);
			}
			else {
				this.PlaySoundOneShot (this.m_FadeLoop.Sound.m_AudioClip);
			}
		}
	}

	private void StartMoveIdle ()
	{
		if ((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) {
			this.m_MoveIdle_StartSucceed = false;
			this.m_MoveIdle_Attemp = 0;
			this.MoveIdle ();
		}
	}

	private void StartScaleLoopSound ()
	{
		if (((((this != null) && (base.gameObject != null)) && base.gameObject.activeSelf) && base.enabled) && (!this.m_ScaleLoop.Animating && (this.m_ScaleLoop.Sound.m_AudioClip != null))) {
			this.m_ScaleLoop.Animating = true;
			this.m_ScaleLoop.Sound.m_AudioSource = null;
			if (this.m_ScaleLoop.Sound.m_Loop) {
				this.m_ScaleLoop.Sound.m_AudioSource = this.PlaySoundLoop (this.m_ScaleLoop.Sound.m_AudioClip);
			}
			else {
				this.PlaySoundOneShot (this.m_ScaleLoop.Sound.m_AudioClip);
			}
		}
	}

	private void StopFadeLoop ()
	{
		if ((this != null) && (base.gameObject != null)) {
			this.m_FadeLoop.Animating = false;
			this.m_FadeLoop.IsOverriding = true;
			this.m_FadeLoop.IsOverridingDelayTimeCount = 0f;
			this.m_FadeLoop.Began = false;
			this.m_FadeLoop.Done = true;
			if (this.m_GETweenFadeLoop != null) {
				this.m_GETweenFadeLoop.Cancel ();
				this.m_GETweenFadeLoop = null;
			}
			this.StopFadeLoopSound ();
		}
	}

	private void StopFadeLoopSound ()
	{
		if ((this.m_FadeLoop.Sound.m_AudioClip != null) && (this.m_FadeLoop.Sound.m_AudioSource != null)) {
			this.m_FadeLoop.Sound.m_AudioSource.Stop ();
			this.m_FadeLoop.Sound.m_AudioSource = null;
		}
	}

	private void StopScaleLoop ()
	{
		if ((this != null) && (base.gameObject != null)) {
			this.m_ScaleLoop.Animating = false;
			this.m_ScaleLoop.IsOverriding = true;
			this.m_ScaleLoop.IsOverridingDelayTimeCount = 0f;
			this.m_ScaleLoop.Began = false;
			this.m_ScaleLoop.Done = true;
			if (this.m_GETweenScaleLoop != null) {
				this.m_GETweenScaleLoop.Cancel ();
				this.m_GETweenScaleLoop = null;
			}
			this.StopScaleLoopSound ();
		}
	}

	private void StopScaleLoopSound ()
	{
		if (((this != null) && (base.gameObject != null)) && ((this.m_ScaleLoop.Sound.m_AudioClip != null) && (this.m_ScaleLoop.Sound.m_AudioSource != null))) {
			this.m_ScaleLoop.Sound.m_AudioSource.Stop ();
			this.m_ScaleLoop.Sound.m_AudioSource = null;
		}
	}

	private void Update ()
	{
	}


	/*	public class cFade {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	public class cMoveIn {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	public class cMoveOut {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	public class cPingPongFade {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	public class cPingPongScale {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	public class cRotationIn {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	public class cRotationOut {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	public class cScaleIn {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	public class cScaleOut {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}*/


	[Serializable]
	public class cFade {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public float Fade;
		public bool FadeChildren;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	[Serializable]
	public class cMoveIn {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public Vector3 BeginPos;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.OutBack;
		[HideInInspector]
		public Vector3 EndPos;
		public GUIAnim.ePosMove MoveFrom = GUIAnim.ePosMove.UpperScreenEdge;
		public Vector3 Position;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	[Serializable]
	public class cMoveOut {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public Vector3 BeginPos;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.InBack;
		[HideInInspector]
		public Vector3 EndPos;
		public GUIAnim.ePosMove MoveTo = GUIAnim.ePosMove.UpperScreenEdge;
		public Vector3 Position;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	[Serializable]
	public class cPingPongFade {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		public bool FadeChildren;
		[HideInInspector]
		public bool IsOverriding;
		[HideInInspector]
		public float IsOverridingDelayTimeCount;
		[HideInInspector]
		public float m_FadeLast;
		public float Min;
		public float Max = 1f;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSoundsForPingPongAnim Sound;
	}

	[Serializable]
	public class cPingPongScale {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.linear;
		[HideInInspector]
		public bool IsOverriding;
		[HideInInspector]
		public float IsOverridingDelayTimeCount;
		[HideInInspector]
		public Vector3 m_ScaleLast;
		public Vector3 Min = new Vector3 (1f, 1f, 1f);
		public Vector3 Max = new Vector3 (1.05f, 1.05f, 1.05f);
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSoundsForPingPongAnim Sound;
	}

	[Serializable]
	public class cRotationIn {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public Vector3 BeginRotation;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.OutBack;
		[HideInInspector]
		public Vector3 EndRotation;
		public Vector3 Rotation;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	[Serializable]
	public class cRotationOut {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public Vector3 BeginRotation;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.InBack;
		[HideInInspector]
		public Vector3 EndRotation;
		public Vector3 Rotation;
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	[Serializable]
	public class cScaleIn {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		public float Delay;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.OutBack;
		public Vector3 ScaleBegin = new Vector3 (0f, 0f, 0f);
		public float Time = 1f;
		public GUIAnim.cSounds Sounds;
	}

	[Serializable]
	public class cScaleOut {
		public bool Enable;
		[HideInInspector]
		public bool Animating;
		[HideInInspector]
		public bool Began;
		[HideInInspector]
		public bool Done;
		public GUIAnim.eEaseType EaseType = GUIAnim.eEaseType.InBack;
		[HideInInspector]
		public Vector3 ScaleBegin = new Vector3 (1f, 1f, 1f);
		public Vector3 ScaleEnd = new Vector3 (0f, 0f, 0f);
		public float Time = 1f;
		public float Delay;
		public GUIAnim.cSounds Sounds;
	}

	[Serializable]
	public class cSounds {
		public AudioClip m_Begin;
		public AudioClip m_AfterDelay;
		public AudioClip m_End;
	}

	[Serializable]
	public class cSoundsForPingPongAnim {
		public AudioClip m_AudioClip;
		[HideInInspector]
		public AudioSource m_AudioSource;
		public bool m_Loop;
	}

	public enum eAlignment
	{
		Current,
		TopLeft,
		TopCenter,
		TopRight,
		LeftCenter,
		Center,
		RightCenter,
		BottomLeft,
		BottomCenter,
		BottomRight
	}

	public enum eEaseType
	{
		InQuad,
		OutQuad,
		InOutQuad,
		InCubic,
		OutCubic,
		InOutCubic,
		InQuart,
		OutQuart,
		InOutQuart,
		InQuint,
		OutQuint,
		InOutQuint,
		InSine,
		OutSine,
		InOutSine,
		InExpo,
		OutExpo,
		InOutExpo,
		InCirc,
		OutCirc,
		InOutCirc,
		linear,
		spring,
		InBounce,
		OutBounce,
		InOutBounce,
		InBack,
		OutBack,
		InOutBack,
		InElastic,
		OutElastic,
		InOutElastic
	}

	public enum ePosMove
	{
		ParentPosition,
		LocalPosition,
		UpperScreenEdge,
		LeftScreenEdge,
		RightScreenEdge,
		BottomScreenEdge,
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
		SelfPosition
	}
}

