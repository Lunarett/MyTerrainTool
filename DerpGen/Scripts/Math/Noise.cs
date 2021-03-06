﻿using System;
using System.ComponentModel;

namespace DerpGen
{
	public class Noise
	{
		public static float[,] GenerateNoiseMap( Parameters parameter, BackgroundWorker bgWorker)
		{
			int mapWidth = parameter.MapSize;
			int mapHeight = parameter.MapSize;

			int seed = parameter.Seed;
			float scale = parameter.Scale;
			int octaves = parameter.Octaves;
			float persistance = parameter.Persistence;
			float lacunarity = parameter.Lacunarity;

			Vector2 offset = parameter.Offset;
			float radius = parameter.Radius;


			float[,] noiseMap = new float[mapWidth, mapHeight];
			Vector2 mapSize = new Vector2(mapWidth, mapHeight);

			EMath.SetSeed(seed);

			if (scale <= 0)
				scale = 0.0001f;

			float maxNoiseHeight = float.MinValue;
			float minNoiseHeight = float.MaxValue;

			for (int x = 0; x < noiseMap.GetLength(0); x++)
			{
				for (int y = 0; y < noiseMap.GetLength(1); y++)
				{
					float amplitude = 1;
					float frequency = 1;
					float noiseHeight = 0;

					for (int i = 0; i < octaves; i++)
					{
						float sampleX = x / scale * frequency + offset.x;
						float sampleY = y / scale * frequency + offset.y;

						float perlinValue = EMath.PerlinNoise(sampleX, sampleY);
						noiseHeight += perlinValue * amplitude;

						amplitude *= persistance;
						frequency *= lacunarity;
					}

					if (noiseHeight > maxNoiseHeight)
						maxNoiseHeight = noiseHeight;
					else if (noiseHeight < minNoiseHeight)
						minNoiseHeight = noiseHeight;

					mapSize = new Vector2(noiseMap.GetLength(0), noiseMap.GetLength(1));

					noiseMap[x, y] = noiseHeight;
				}
				bgWorker.ReportProgress((int)((float)x / noiseMap.GetLength(0) * 100));

				//On Aborted
				if(bgWorker.CancellationPending)
				{
					return null;
				}

			}

			for (int x = 0; x < noiseMap.GetLength(0); x++)
			{
				for (int y = 0; y < noiseMap.GetLength(1); y++)
				{
					float radialEffect = EMath.Clamp01(2 - (Vector2.Distance(new Vector2(x, y), mapSize / 2)) / radius);
					noiseMap[x, y] = EMath.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]) * radialEffect;
				}
			}

			return noiseMap;
		}
	}
}
