using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	
	
	public Transform camTransform;

	
	public float shakeDuration ;

	
	public float shakeAmount ;
	


	float timer;
	float timeOfStart;

	Vector3 originalPos;

	[HideInInspector] public bool shaking = false;

	

	void OnEnable()
	{
		originalPos = camTransform.localPosition;

		timer = shakeDuration;
	}

    private void Update()
    {
		timer -= Time.deltaTime;

        if (shaking && timer > 0.0f )
        {
			Shake();
        }
        else
        {
			camTransform.localPosition = originalPos;
			shaking = false;
		}
    }

    public void StartShake()
    {
		
		shaking = true;
		timer = shakeDuration;

	}

	void Shake()
    {
		camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

		



		
	}



}
