using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveSystem : MonoBehaviour {

	public static float StopSubdevideCount = .1f;
	public static int SubdivitionCount = 3;
	public static float DieCount = 2f;
	List<WaterParticle> Particles;
	public Texture2D WaterTexture;
	private Color[] SetArray;
    public Material ParticleRender;

	public Material DummyMat;

    [Header("Particle Settings")]
    public float Particle_Radius = 0.2f;

	private int TextureSize = 128;
	private void Start()
	{
		Particles = new List<WaterParticle>();
		WaterTexture = new Texture2D(TextureSize, TextureSize);
		SetArray = Enumerable.Repeat<Color>(Color.black, TextureSize * TextureSize).ToArray();
		WaterTexture.SetPixels(SetArray);
		WaterTexture.Apply();
		DummyMat.mainTexture = WaterTexture;

        SetupParticleRender();
	}

    public void SetupParticleRender()
    {
        ParticleRender = new Material(Shader.Find("Hidden/ParticleRendere"));
        ParticleRender.SetFloat("_Radius", Particle_Radius);
        Material Particle = new Material(Shader.Find("Hidden/ParticleGenerator"));

        int size = 256;
        RenderTexture RenderTex = new RenderTexture(size, size, 8);
        Graphics.Blit(RenderTex, RenderTex, Particle);  

        Texture2D ParticleTex = new Texture2D(size, size);
        RenderTexture.active = RenderTex;
        ParticleTex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        ParticleTex.Apply();

        RenderTexture.active = null;

        ParticleRender.SetTexture("_ParticleTex", ParticleTex);
    }

	public void EruptParticles(Vector2 inputPoint, int SubdivitionCount, float speed)
	{
		for (int i = 0; i < SubdivitionCount; i++)
		{
			float angle = (360 / SubdivitionCount) * i;
			float x = Mathf.Cos(angle);
			float y = Mathf.Sin(angle);
			Vector2 Direction = new Vector2(x, y) * speed;

			Particles.Add(new WaterParticle(inputPoint, Direction, angle));
		}
	}

    public void SpawnParticleTest()
    {
        Particles.Clear();
        SpawnSingleParticle(new Vector2(0.5f, 0.5f), new Vector2(1, 0), 0.003f);
    }

    public void SpawnSingleParticle(Vector2 inputPoint, Vector2 Direction, float speed)
    {
        Vector2 dir = Direction * speed;
        Particles.Add(new WaterParticle(inputPoint, dir, 0));
    }

	void Update()
	{
		if (Input.GetMouseButtonDown(1))
			Particles.Clear();

		if (Input.GetMouseButtonDown(0))
			EruptParticles(new Vector2(0.5f, 0.5f), 120, 0.005f);

		if(Particles.Count != 0)
		{
			List<WaterParticle> NewParticles = new List<WaterParticle>();

			SetArray = Enumerable.Repeat<Color>(Color.black, TextureSize * TextureSize).ToArray();
			foreach (WaterParticle particle in Particles)
			{
				WaterParticle[] subParticles = particle.UpdateParticle();
				foreach(WaterParticle subParticle in subParticles)
				{
					Vector2 TexturePos = subParticle.Position * TextureSize;
					int x = Mathf.FloorToInt(TexturePos.x);
					int y = Mathf.FloorToInt(TexturePos.y);
					try
					{
						SetArray[x + TextureSize * y] = Color.white;
					}
					catch
					{
						Debug.Log("x: " + x + "    y: " + y);
					}
						NewParticles.Add(subParticle);
				}
			}
			WaterTexture.SetPixels(SetArray);
			WaterTexture.Apply();
			DummyMat.mainTexture = WaterTexture;
			Particles = NewParticles;
		}

	}
}
