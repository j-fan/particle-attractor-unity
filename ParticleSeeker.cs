using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSeeker : MonoBehaviour {

    public Gradient particleColourGradient;
    public float forceMultiplier = 0.5f;
    public float g = 1f;
    public float mass = 3f;

    ParticleSystem ps;
    int numAttractors = 3;
    GameObject[] attractors;

    // Use this for initialization
    void Start () {
        ps = GetComponent<ParticleSystem>();
        initAttractors();
    }

    void Update()
    {
        // add variation to particle colour
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        main.startColor = particleColourGradient.Evaluate(Random.Range(0f,1f));
    }

    void LateUpdate () {

        //put particles of the system into array & update them to gravity algorithm
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount]; 
        ps.GetParticles(particles); 

        for (int i = 0; i < particles.Length; i++){
            ParticleSystem.Particle p = particles[i];
            Vector3 particleWorldPosition;
            if(ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
            {
                particleWorldPosition = transform.TransformPoint(p.position);
            }
            else if(ps.main.simulationSpace == ParticleSystemSimulationSpace.Custom)
            {
                particleWorldPosition = ps.main.customSimulationSpace.TransformPoint(p.position);
            } else
            {
                particleWorldPosition = p.position;
            }

            Vector3 direction = applyGravity(particleWorldPosition);
            float magnitude = direction.magnitude;
            Mathf.Clamp(magnitude, 5.0f, 10.0f); //eliminate extreme result for very close or very far objects

            //Vector3 seekForce = ((direction) * forceMultipler) * Time.deltaTime; //simple seeker
            float gforce = (g * mass * mass) / direction.magnitude * direction.magnitude;
            Vector3 seekForce = ((direction) * gforce) * Time.deltaTime;
            seekForce = seekForce * forceMultiplier; 

            p.velocity += seekForce;
            particles[i] = p;
        }
        ps.SetParticles(particles, particles.Length); //set updated particles into the system
	}

    Vector3 applyGravity(Vector3 particleWorldPosition)
    {
        Vector3 direction = Vector3.zero;
        foreach (GameObject a in attractors)
        {
            direction += (a.transform.position - particleWorldPosition).normalized;
        }
        return direction;
    }

    void initAttractors()
    {
        attractors = new GameObject[numAttractors];
        for (int i = 0; i < numAttractors; i++)
        {
            GameObject newAttractor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newAttractor.transform.position = new Vector3(i*3, 2.0f, 0);
            newAttractor.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            attractors[i] = newAttractor;
        }
    }
}
