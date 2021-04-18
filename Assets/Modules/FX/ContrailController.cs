using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContrailController : MonoBehaviour
{
    [Header("Time Params")]
    [Range(0, 1)]
    [SerializeField]
    private float stopPlayProb;
    [SerializeField]
    private MinMax changeStateInterval;
    [Header("Time Params")]
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float checkInterval = 1f;
    [Range(0,10)]
    [SerializeField]
    private float minVelToPlay = 1f;
    [SerializeField]
    private ParticleSystem particles;
    public float vel;
    public bool _playByVel = true;
    public bool _playByTime = true;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ProcessTime());
        StartCoroutine(ProcessBody());
    }

    IEnumerator ProcessBody()
    {
        if (rb == null) yield break;
        for (;;)
        {
            yield return new WaitForSeconds(checkInterval);
            _playByVel = rb.velocity.magnitude >= minVelToPlay;
            vel = rb.velocity.magnitude;
            SetParticlesStatus();
        }
    }

    void SetParticlesStatus()
    {
        if (_playByTime && _playByVel)
        {
            particles.Play();    
        }
        else
        {
            particles.Stop();
        }
    }

    IEnumerator ProcessTime()
    {
        if (changeStateInterval.Max == 0) yield break;
        for (;;)
        {
            yield return new WaitForSeconds(Random.Range(changeStateInterval.Min, changeStateInterval.Max));
            _playByTime = (Random.Range(0f, 1f) > stopPlayProb);
            SetParticlesStatus();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}