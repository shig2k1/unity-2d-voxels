using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class QuadMeshCreator : MonoBehaviour
{

  public bool generate = false;
  public QuadtreeComponent quadtree;
  public Material voxelMaterial;

  private bool initialized = false;
  private GameObject previousMesh;

  // Start is called before the first frame update
  void Start()
  {
   
  }

  // Update is called once per frame
  void Update()
  {
    if (quadtree.Quadtree != null && !initialized)
    {
      initialized = true;
      quadtree.Quadtree.QuadtreeUpdated += (obj, args) => { generate = true; };
    }

    if (generate)
    {
      System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
      stopwatch.Start();
      var generatedMesh = GenerateMesh();
      stopwatch.Stop();

      if (previousMesh != null)
      {
        previousMesh = generatedMesh;
      }
      Debug.Log(stopwatch.ElapsedMilliseconds);
      generate = false;
    }
  }

  private GameObject GenerateMesh()
  {
    GameObject chunk = new GameObject();
    chunk.name = "Voxel chunk";
    chunk.transform.parent = this.transform;
    chunk.transform.localPosition = Vector3.zero;

    var mesh = new Mesh();
    var vertices = new List<Vector3>();
    var triangles = new List<int>();
    var uvs = new List<Vector2>();
    var normals = new List<Vector3>();

    foreach (var leaf in quadtree.Quadtree.GetLeafNodes().Where(leaf => leaf.Data))
    {
      Vector3 upperLeft = new Vector3(leaf.Position.x - leaf.Size * 0.5f, leaf.Position.y + leaf.Size * 0.5f, 0);
      Vector3 upperRight = upperLeft + Vector3.right * leaf.Size;
      Vector3 lowerLeft = upperLeft + Vector3.down * leaf.Size;
      Vector3 lowerRight = upperLeft + Vector3.down * leaf.Size + Vector3.right * leaf.Size;
        
      var initialIndex = vertices.Count;
        
      vertices.Add(upperLeft);
      vertices.Add(upperRight);
      vertices.Add(lowerLeft);
      vertices.Add(lowerRight);

      uvs.Add(upperLeft);
      uvs.Add(upperRight);
      uvs.Add(lowerLeft);
      uvs.Add(lowerRight);

      normals.Add(Vector3.back);
      normals.Add(Vector3.back);
      normals.Add(Vector3.back);
      normals.Add(Vector3.back);

      triangles.Add(initialIndex);
      triangles.Add(initialIndex + 1);
      triangles.Add(initialIndex + 2);

      triangles.Add(initialIndex + 3);
      triangles.Add(initialIndex + 2);
      triangles.Add(initialIndex + 1);
    }

    mesh.SetVertices(vertices);
    mesh.SetTriangles(triangles, 0);
    mesh.SetUVs(0, uvs);
    mesh.SetNormals(normals);
    
    var meshRenderer = chunk.AddComponent<MeshRenderer>();
    var filter = chunk.AddComponent<MeshFilter>();
    var box = chunk.AddComponent<BoxCollider2D>();

    meshRenderer.material = voxelMaterial;

    filter.mesh = mesh;
    return chunk;
  }
}
