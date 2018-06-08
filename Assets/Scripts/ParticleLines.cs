using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLines : MonoBehaviour {

    public int lineCount = 15;

    public float dist = 1.0f;

    public LineRenderer lineTemplate;

    List<LineRenderer> lines;

    public ParticleSystem ps;

    private void Awake()
    {
        lines = new List<LineRenderer>();
    }

    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(particles);

        int lineIndex = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].enabled = false;
        }

        for (int i = 0; i < ps.particleCount; ++i)
        {
            for (int j = i + 1; j < ps.particleCount; ++j)
            {
                if (lineIndex < lineCount)
                {
                    if (lineIndex >= lines.Count)
                    {
                        for (int k = lines.Count; k < lineIndex + 1; ++k)
                        {
                            LineRenderer lr = Instantiate<LineRenderer>(lineTemplate, transform);
                            lr.material = new Material(Shader.Find("Particles/Additive"));
                            lines.Add(lr);
                        }
                    }
                    Vector3[] points = { particles[i].position, particles[j].position };
                    
                    if (Vector3.SqrMagnitude(points[0] - points[1]) < dist)
                    {
                        var line = lines[lineIndex];
                        line.SetPositions(points);
                        line.enabled = true;
                        line.startColor = particles[i].GetCurrentColor(ps);
                        line.endColor = particles[j].GetCurrentColor(ps);
                        lineIndex++;
                    }
                }
                else
                {
                    break;
                }
            }
        }
	}
}
