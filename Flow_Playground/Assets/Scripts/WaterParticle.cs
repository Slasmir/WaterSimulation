using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterParticle {

	public Vector2 Position;
	public Vector2 Direction;
	float DispersionAngle;
	float BirthTime;

	public WaterParticle(Vector2 _BirthPostion, Vector2 _Direction, float _DispersionAngle)
	{
		Position = _BirthPostion;
		Direction = _Direction;
		DispersionAngle = _DispersionAngle;
		BirthTime = Time.time;
	}

	public WaterParticle[] UpdateParticle()
	{
		WaterParticle[] self = new WaterParticle[] { this };
		if(!WithinOneZero(this.Position + Direction))
		{
			Direction *= -1f;
		}
		this.Position += Direction;
		return self;
 	}

	public bool WithinOneZero(Vector2 vector)
	{
		if (vector.x > 1 || vector.x < 0) return false;
		if (vector.y > 1 || vector.y < 0) return false;
		return true;
	}

	//NOT WORKING 
	public WaterParticle[] SubdevideParticles()
	{

		WaterParticle []
		SubParticles = new WaterParticle[WaveSystem.SubdivitionCount];

		for (int i = 0; i<WaveSystem.SubdivitionCount; i++)
		{
			SubParticles[i] = new WaterParticle(Position + Direction, Direction, DispersionAngle / WaveSystem.SubdivitionCount);
}

		return SubParticles;
	}
}
