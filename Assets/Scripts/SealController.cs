using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SealController : MonoBehaviour {

	Rigidbody2D rb2d;
	Animator animator;
	float angle;
	bool isDead;

	public float maxHeight;
	public float flapVelocity;
	public float relativeVelocityX;
	// sprite 오브젝트 참조
	public GameObject sprite;
	void Awake(){
		rb2d = GetComponent<Rigidbody2D>();
		// sprite오브젝트의 Animator 컴포넌트 취득
		animator = sprite.GetComponent<Animator>();
	}

	public bool IsDead(){
		return isDead;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1") && transform.position.y < maxHeight){
			Flap();
		}

		// 각도를 반영
		ApplyAngle();

		// angle이 수평 이상이라면 애니메이터의 flap플래그를 true로 한다
		animator.SetBool("flap", angle >= 0.0f);
	}

	public void Flap(){
		// 죽으면 날아 올라가지 않는다.
		if(isDead) return;

		// 중력을 받지 않을 때는 조작하지 않는다
		if(rb2d.isKinematic)return;

		// Velocity를 직접 바꿔 써서 위쪽 방향으로 가속
		rb2d.velocity = new Vector2(0.0f, flapVelocity);
	}

	void ApplyAngle(){
		float targetAngle;
		// 사망하면 항상 아래를 향한다
		if(isDead){
			targetAngle = -90.0f;
		}else{
			// 현재속도, 상대 속도로부터 진행되고 있는 각도를 구한다
			targetAngle = Mathf.Atan2(rb2d.velocity.y, relativeVelocityX) * Mathf.Rad2Deg;
		}

		// 회전 애니메이션을 스무딩
		angle = Mathf.Lerp(angle, targetAngle, Time.deltaTime * 10.0f);

		// Rotation의 반영
		sprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);
	}

	void OnCollisionEnter2D(Collision2D collision){
		if(isDead) return;

		// 충돌효과
		Camera.main.SendMessage("Clash");

		//뭔가에 부딪치면 사망플래그를 true로 한다
		isDead = true;
	}
	
	public void SetSteerActive(bool active){
		// Rigidbody의 On, Off를 전환한다.
		rb2d.isKinematic = !active;
	}
}
