using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DoActionData
{
    public float Power;
    public int StopFrame;
    public float Distance;


    public AudioClip HitAudioClip;

    public GameObject HitParticle;
    public Vector3 HitParticlePositionOffset;
    public Vector3 HitParticleScaleOffset = Vector3.one;
}

public class Sword : MonoBehaviour
{
    [SerializeField]
    DoActionData[] doActionDatas;

    private new Collider collider;
    private GameObject rootObject;

    private List<GameObject> hittedList;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rootObject = transform.root.gameObject;

        hittedList = new List<GameObject>();
    }

    private void Start ()
	{
        End_Collision();	
	}

    private void OnTriggerEnter(Collider other)
    {
        if (rootObject == other.gameObject)
            return;

        if (hittedList.Contains(other.gameObject))
            return;

        print("collision");

        hittedList.Add(other.gameObject);

        IDamagable damage = other.gameObject.GetComponent<IDamagable>();
        {
            Player player = rootObject.GetComponent<Player>();

            if(player != null && damage != null)
                damage.Damage(rootObject, this, doActionDatas[player.ComboIndex]);
        }
        
    }

    public void Begin_Collision()
    {
        collider.enabled = true;
    }

    public void End_Collision()
    {
        collider.enabled = false;

        hittedList.Clear();
    }

#if UNITY_EDITOR
    public void Save_DoActionDatas(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Create);
        {
            BinaryWriter writer = new BinaryWriter(stream);
            {
                foreach (DoActionData data in doActionDatas)
                {
                    writer.Write(data.Power);
                    writer.Write(data.StopFrame);
                    writer.Write(data.Distance);
                }
            }
            writer.Close();
        }
        stream.Close();
    }
#endif
}