using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSeeker : MonoBehaviour {

    public Transform target;
    public Transform target2;
    public float forceMultiplier = 0.5f;
    ParticleSystem ps;
    public Gradient particleColourGradient;
    float g = 1f;
    float mass = 10f;

    // Use this for initialization
    void Start () {
        ps = GetComponent<ParticleSystem>();
        
        
    }

    void Update()
    {
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        main.startColor = particleColourGradient.Evaluate(Random.Range(0f,1f));
    }

    void LateUpdate () {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount]; 
        ps.GetParticles(particles); //put particles of the system into array

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

            // gravitational attraction alog from
            // https://github.com/shiffman/The-Nature-of-Code-Examples/blob/master/chp02_forces/Exercise_2_10_attractrepel/Attractor.pde
            Vector3 directionToTarget = (target.position - particleWorldPosition).normalized;
            Vector3 directionToTarget2 = (target2.position - particleWorldPosition).normalized;

            Vector3 direction = directionToTarget + directionToTarget2;
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
}
