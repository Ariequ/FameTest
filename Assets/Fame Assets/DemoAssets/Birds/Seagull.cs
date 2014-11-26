using UnityEngine;
using System.Collections;

public class Seagull : MonoBehaviour {

//private AnimationState glide;
private Animation animationComponent;
float myTime = 0;
float nextFlap = 0;
float frequency = 7;
	void Start ()
	{
		animationComponent = GetComponentInChildren<Animation>();
		animationComponent.Blend("fly");
		animationComponent["fly"].normalizedTime = Random.value;
		//glide = animationComponent["glide"];	
	}

	void Update ()
	{ 
		myTime+=Time.deltaTime;
		if(myTime > nextFlap)
		{
			//gliding = false;
			animationComponent.Blend("glide", 0.00f, 0.2f);	
			animationComponent.Blend("fly", 1.00f, 0.2f);
			myTime = 0;
			nextFlap = Random.value * frequency;
		}
		/*
		else
		{
			animationComponent.Blend("glide", 1.00f, 0.2f);	
			animationComponent.Blend("fly", 0.00f, 0.2f);
		}*/
	}
}
