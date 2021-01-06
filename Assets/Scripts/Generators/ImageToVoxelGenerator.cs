using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageToVoxelGenerator : MonoBehaviour
{
  public Texture2D image;
  public QuadtreeComponent quadtree;
  public float threshold = 0.5f;

  // Start is called before the first frame update
  void Start()
  {
    Generate();
  }

  void Update ()
  {

  }

  private void Generate()
  {
    int cells = (int)Mathf.Pow(2, quadtree.depth);
    int h = cells / 2;
    for (int x = 0; x <= cells; ++x)
    {
      for (int y = 0; y <= cells; ++y)
      {
        Vector2 position = quadtree.transform.position;
        position.x += (x - h) / (float)cells * quadtree.size;
        position.y += (y - h) / (float)cells * quadtree.size;

        var pixel = image.GetPixelBilinear(x / (float)cells, y / (float)cells);

        if (pixel.r > threshold)
        {
          quadtree.Quadtree.Insert(true, position);
        }

      }
    }
  }
}
