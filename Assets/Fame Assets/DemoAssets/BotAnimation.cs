using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BotAnimation : MonoBehaviour {

    float runSpeedScale = 1.0f;
    float walkSpeedScale = 1.0f;

    void Start ()
    {
	    // By default loop all animations
	    animation.wrapMode = WrapMode.Loop;

	    animation["run"].layer = 1;
	    animation["walk"].layer = 1;
	    animation["idle"].layer = 1;

	    animation["idle"].wrapMode = WrapMode.Loop;
	    animation["walk"].wrapMode = WrapMode.Loop;
	    animation["run"].wrapMode = WrapMode.Loop;

	    animation["idle"].normalizedSpeed = 0.5f;

	    // We are in full control here - don't let any other animations play when we start
	    animation.Stop();
	    animation.Play("idle");
    }

    void Update ()
    {
        FlockMember member = GetComponent<FlockMember>();

        float currentSpeed = member.MovementSpeed;

	    if (currentSpeed >= 8)
	    {
		    runSpeedScale = currentSpeed / 20;
		    if (runSpeedScale > 1.5){
			    runSpeedScale = 1.5f;
		    }
		    animation.CrossFade("run");
	    }	else if (currentSpeed > 0.2) {
		    walkSpeedScale = currentSpeed / 15;
		    animation.CrossFade("walk");
	    } else {
		    animation.Play("idle");
	    }
	
	    animation["run"].normalizedSpeed = runSpeedScale;
	    animation["walk"].normalizedSpeed = walkSpeedScale;
    }
}
