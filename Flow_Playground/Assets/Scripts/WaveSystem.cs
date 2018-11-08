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

	private int TextureSize = 512;
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

        if (Input.GetMouseButtonDown(2))
        {
            EruptParticles(new Vector2(0.5f, 0.5f), 120, 0.005f);
        }
        Debug.Log(Particles.Count);
		if(Particles.Count != 0)
		{
            RenderTexture ProcessingTexture1 = new RenderTexture(TextureSize, TextureSize,8);
            RenderTexture ProcessingTexture2 = new RenderTexture(TextureSize, TextureSize, 8);
            int counter = 0;
            foreach (WaterParticle particle in Particles)
			{
				particle.UpdateParticle();

				Vector2 TexturePos = particle.Position;
				float x = TexturePos.x;
				float y = TexturePos.y;

                Material m = new Material(ParticleRender);
                m.SetFloat("_PosX", x);
                m.SetFloat("_PosY", y);
                if (counter % 2 == 0)
                    Graphics.Blit(ProcessingTexture1, ProcessingTexture2, m);
                else
                    Graphics.Blit(ProcessingTexture2, ProcessingTexture1, m);
                RenderTexture.active = null;

                counter++;
			}

            if (counter % 2 == 0)
                RenderTexture.active = ProcessingTexture1;
            else
                RenderTexture.active = ProcessingTexture2;

			WaterTexture.ReadPixels(new Rect(0,0,TextureSize, TextureSize),0,0);
			WaterTexture.Apply();

            RenderTexture.active = null;

			DummyMat.mainTexture = WaterTexture;
		}

	}
}
