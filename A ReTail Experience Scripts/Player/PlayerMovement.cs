using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 1f;
    public float dashSpeed = 3.5f;
    public float turnTime = 0.1f;
    public float turnVelocity;

    public bool can_dash = true;

    public GameObject dust, dash_particles;
    public Animator anim;

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude > 0.1)
        {
            if (can_dash)
            {
                if (Input.GetKeyDown(KeyCode.Space) && !Pause.is_paused)
                {
                    StartCoroutine("dash");
                }
            }

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(direction * speed * Time.deltaTime);
            anim.SetBool("isWalking", true);

            dust.gameObject.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            dust.gameObject.GetComponent<ParticleSystem>().Stop();
            anim.SetBool("isWalking", false);
        }
    }

    IEnumerator dash()
    {
        can_dash = false;
        float temp = speed;
        speed = dashSpeed;
        dash_particles.gameObject.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(.2f);
        speed = temp;
        dash_particles.gameObject.GetComponent<ParticleSystem>().Stop();
        StartCoroutine("allow_dash");
    }

    IEnumerator allow_dash()
    {
        yield return new WaitForSeconds(0.2f);
        can_dash = true;
    }
}
