using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public float maxDistance = 7.5f;
	public float speed = 5;

	public GameObject effects;
	public Transform arrowHold, startPos;
	public GameObject indicatorArrow;
	public LineRenderer lineRenderer;

	private bool clicked;
	private Camera myCam;

	private bool onGround = true;
	private Vector2 destinationPoint;

	private bool foundPlatform;

	private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
		anim = GetComponent<Animator>();
		myCam = Camera.main;
		destinationPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

		anim.SetBool("onGround", onGround);

		if (Input.GetButtonDown("Fire1") && onGround)
		{
			effects.SetActive(true);
			clicked = true;
		}


		if (clicked)
		{
			Vector2 mousePosition = myCam.ScreenToWorldPoint(Input.mousePosition);
			Vector2 direction = mousePosition - (Vector2)transform.position;
			arrowHold.up = direction;

			lineRenderer.SetPosition(0, startPos.position);

			RaycastHit2D hit = Physics2D.Raycast(startPos.position, startPos.up, maxDistance);

			if(hit.collider != null)
			{
				Platform platform = hit.collider.GetComponent<Platform>();
				if(platform != null)
				{
					lineRenderer.SetPosition(1, hit.point);
					destinationPoint = platform.Point(hit.point);
					indicatorArrow.transform.position = platform.Point(hit.point) - ((Vector2)indicatorArrow.transform.up * 0.25f);
					indicatorArrow.SetActive(true);
					foundPlatform = true;
				}
				else
				{
					lineRenderer.SetPosition(1, hit.point);
					indicatorArrow.SetActive(false);
					foundPlatform = false;
				}

				
			}
			else
			{
				lineRenderer.SetPosition(1, startPos.position + (startPos.up * maxDistance));
				indicatorArrow.SetActive(false);
				foundPlatform = false;
			}


			if(arrowHold.localEulerAngles.z <= 270 && arrowHold.localEulerAngles.z >= 90)
			{
				effects.SetActive(false);
				foundPlatform = false;
			}
			else
			{
				effects.SetActive(true);
			}

			if (Input.GetButtonUp("Fire1"))
			{

				if (foundPlatform)
				{
					Flip(hit.transform.eulerAngles.z);
					onGround = false;
				}


				foundPlatform = false;
				clicked = false;
				effects.SetActive(false);
				
			}

		}

		if (!onGround)
		{
			transform.position = Vector2.Lerp(transform.position, destinationPoint, speed * Time.deltaTime);
		}

		float distance = Vector2.Distance(transform.position, destinationPoint);
		if(distance <= 0.5f && !onGround)
		{
			onGround = true;
			transform.position += transform.up * 0.5f;
		}

    }

	void Flip(float rot)
	{
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));
	}
}
