using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    private Animator animator;
    private new Rigidbody rigidbody;
    private AudioSource audioSource;
    private StatusComponent status;

    [SerializeField]
    private GameObject player;

    private List<Material> materialList;
    private List<Color> originColorList;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        status = GetComponent<StatusComponent>();


        materialList = new List<Material>();
        originColorList = new List<Color>();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer render in renderers)
        {
            materialList.Add(render.material);
            originColorList.Add(render.material.color);
        }
    }

    public void Damage(GameObject attacker, Sword causer, DoActionData data)
    {
        status.Damage(data.Power);

        
        foreach (Material material in materialList)
            material.color = Color.red;

        Invoke("RestoreColor", 0.15f);

        FrameComponent.Instance.Delay(data.StopFrame);


        if(status.Dead == false)
            transform.LookAt(attacker.transform, Vector3.up);

        
        if(data.HitParticle != null)
        {
            GameObject obj = Instantiate(data.HitParticle, transform, false);
            obj.transform.localPosition = data.HitParticlePositionOffset;
            obj.transform.localScale = data.HitParticleScaleOffset;
        }

        if(data.HitAudioClip != null)
            audioSource.PlayOneShot(data.HitAudioClip);


        if (status.Dead == false)
        {
            animator.SetTrigger("Damaged");

            
            rigidbody.isKinematic = false;

            float launch = rigidbody.drag * data.Distance * 10.0f;
            rigidbody.AddForce(-transform.forward * launch);

            StartCoroutine(Change_IsKinemetics(5));


            return;
        }

        animator.SetTrigger("Dead");

        
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        Destroy(gameObject, 5.0f);
    }

    private void RestoreColor()
    {
        for(int i = 0; i < materialList.Count; i++)
            materialList[i].color = originColorList[i];
    }

    private IEnumerator Change_IsKinemetics(int frame)
    {
        for (int i = 0; i < frame; i++)
            yield return new WaitForFixedUpdate();

        rigidbody.isKinematic = true;
    }



    private Vector3 moveToPosition;

    private void Start()
    {
        StartCoroutine(MoveTo());
    }

    private IEnumerator MoveTo()
    {
        float speed = 0.0f;
        bool bFirst = false;
        moveToPosition = transform.position;

        while (true)
        {
            if(Vector3.Distance(moveToPosition, transform.position) < 0.1f) //µµ´Þ
            {
                animator.SetFloat("SpeedZ", 0);
                speed = Random.Range(0.15f, 0.03f);
                float time = Random.Range(1.0f, 2.0f);
                
                if(bFirst == false)
                {
                    bFirst = true;
                    time = 0.0f;
                }
                yield return new WaitForSeconds(time);
                

                moveToPosition = GetRandomPosition();

                Vector3 look = moveToPosition - transform.position;
                Quaternion rotation = Quaternion.LookRotation(look.normalized, Vector3.up);
                transform.rotation = rotation;
            }

            transform.position += transform.forward * speed;
            animator.SetFloat("SpeedZ", 2);

            yield return new WaitForFixedUpdate();
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (Selection.activeGameObject != gameObject)
    //        return;

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(moveToPosition, 0.25f);
    //}


    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-23.0f, +23.0f);
        float z = Random.Range(-23.0f, +23.0f);

        Vector3 a = new Vector3();
        a = player.transform.position;

        //return new Vector3(x, 0.0f, z);
        return a;
    }
}