﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Guard : MonoBehaviour
{

	private static Color TransparentBlack = new Color(0, 0, 0, 0);
	
	private Animator animator;
	private Transform armJoint;
	private SpriteRenderer armRenderer;
	private Gun gun;
	
	private bool cover = true;
	private bool aimed = false;
	private bool targets = false;
	
	void Awake()
	{
		animator = GetComponent<Animator>();
		armJoint = transform.Find("ArmJoint");
		armRenderer = armJoint.GetComponentInChildren<SpriteRenderer>();
		armRenderer.color = TransparentBlack;
		gun = armJoint.GetComponentInChildren<Gun>();

		gun.OnEmpty += TakeCover;
		gun.OnReloaded += ReadyUp;
	}
	
	void Update() {
		if (!cover && aimed && targets)
		{
			gun.Fire(armJoint);
			targets = false;
		}
	}
	
	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.GetComponent<Health>()) // Assume soldier
		{
			targets = true;
		}
	}

	public void ToggleCover()
	{
		if (cover)
		{
			ReadyUp();
		}
		else
		{
			TakeCover();
		}
	}
	
	public void ReadyUp()
	{
		cover = false;
		animator.SetBool("Cover", cover);
	}

	public void TakeCover()
	{
		StartCoroutine(RotateArmToUnready());
	}

	public void ReadyArm()
	{
		StartCoroutine(RotateArmToReady());
	}

	public void UnreadyArm()
	{
		armRenderer.color = TransparentBlack;
	}

	IEnumerator RotateArmToReady()
	{
		armRenderer.color = Color.white;
		var rotation = 0.0f;
		var ratePerSecond = -90.0f;
		while (rotation > -90.0f)
		{
			var amount = ratePerSecond * Time.deltaTime;
			armJoint.transform.Rotate(0, 0, amount);
			rotation += amount;
			
			yield return null;
		}

		aimed = true;
	}
	
	IEnumerator RotateArmToUnready()
	{
		aimed = false;
		
		var rotation = -90.0f;
		var ratePerSecond = 90.0f;
		while (rotation < 0.0f)
		{
			var amount = ratePerSecond * Time.deltaTime;
			armJoint.transform.Rotate(0, 0, amount);
			rotation += amount;
			
			yield return null;
		}

		cover = true;
		animator.SetBool("Cover", cover);
		gun.Reload();
	}
}