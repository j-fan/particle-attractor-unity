using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSeeker : MonoBehaviour {

    public Gradient particleColourGradient;
    public float forceMultiplier = 1.0f;
    float g = 1f;
    float mass = 3f;

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

            //simple attractor
            //Vector3 totalForce = applySimple(particleWorldPosition);

            //gravitional field sim
            //Vector3 totalForce = applyGravity(particleWorldPosition);

            //electric field sim
            Vector3 totalForce = applyElectric(p);

            //rotate 90 deg right
            //Vector3 right = Vector3.Cross(totalForce, Vector3.up);
            //totalForce = Quaternion.AngleAxis(0, right) * totalForce;

            //p.velocity += totalForce; //with  acceleration
            p.velocity = totalForce;    //velocity only to visualise field line style
            
            particles[i] = p;
        }
        ps.SetParticles(particles, particles.Length); //set updated particles into the system
	}

    Vector3 applySimple(Vector3 particleWorldPosition)
    {
        Vector3 direction = Vector3.zero;
        foreach (GameObject a in attractors)
        {
            direction += (a.transform.position - particleWorldPosition).normalized;
        }
        Vector3 totalForce = ((direction) * forceMultiplier) * Time.deltaTime;
        return totalForce;
    }

    Vector3 applyGravity(Vector3 particleWorldPosition)
    {
        Vector3 direction = Vector3.zero;
        foreach (GameObject a in attractors)
        {
            direction += (a.transform.position - particleWorldPosition).normalized;
        }
        float magnitude = direction.magnitude;
        Mathf.Clamp(magnitude, 5.0f, 10.0f); //eliminate extreme result for very close or very far objects

        float gforce = (g * mass * mass) / direction.magnitude * direction.magnitude;
        Vector3 totalForce = ((direction) * gforce) * Time.deltaTime;
        totalForce = totalForce * forceMultiplier;
        return totalForce;
    }

    Vector3 applyElectric(ParticleSystem.Particle p) {
        Vector3 totalForce = Vector3.zero;
        Vector3 force = Vector3.zero;
        int i = 0;
        foreach (GameObject a in attractors)
        {
            float dist = Vector3.Distance(p.position, a.transform.position) * 100000;
            float fieldMag = 99999 / dist * dist;
            Mathf.Clamp(fieldMag, 0.0f, 5.0f);

            //alternate postive and negative charges
            if(i % 2 == 0)
            {
                force.x -= fieldMag * (p.position.x - a.transform.position.x) / dist;
                force.y -= fieldMag * (p.position.y - a.transform.position.y) / dist;
                force.z -= fieldMag * (p.position.z - a.transform.position.z) / dist;
            } else
            {
                force.x += fieldMag * (p.position.x - a.transform.position.x) / dist;
                force.y += fieldMag * (p.position.y - a.transform.position.y) / dist;
                force.z += fieldMag * (p.position.z - a.transform.position.z) / dist;
            }

            i++;
        }
        totalForce = force * forceMultiplier; 
        return totalForce;
    }

    void initAttractors()
    {
        attractors = new GameObject[numAttractors];
        for (int i = 0; i < numAttractors; i++)
        {
            GameObject newAttractor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newAttractor.transform.position = new Vector3(i*3, 2.0f, i*3);
            newAttractor.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            attractors[i] = newAttractor;
        }
    }
}
