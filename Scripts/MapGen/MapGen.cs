using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGen
{
    // list saves each row
    List<List<string>> map;
    // Max row num
    int width = 0;


    public MapGen(string _file)
    {
        GetObjctByRow(GetMapFile(_file));
    }

    /// <summary>
    /// Method used to generte all map
    /// Generate ground first and then walls
    /// </summary>
    public void GenerateMap()
    {
        PlaceGround();
        PlaceObstacles();
    }

    private TextAsset GetMapFile(string _file)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(_file);

        if (textAsset == null || _file == "")
        {
            throw new ArgumentException("File can't find");
        }

        return textAsset;
    }

    protected void GetObjctByRow(TextAsset textAsset)
    {
        string[] mapRow = textAsset.text.Trim().Split('\n');
        map = new List<List<string>>();

        for (int i = 0; i < mapRow.Length; i++)                           // Read each line
        {
            List<string> map_row = new List<string>(mapRow[i].Split(','));// split by ','

            if (width < map_row.Count)
            {                                                             // Update length of plane
                width = map_row.Count;
            }

            map.Add(map_row);                                             // Add data into list
        }
    }

    protected void PlaceGround()
    {
        if (map == null)
        {
            throw new ArgumentNullException("Empty map file!");
        }

        // TODO:No direction fit!

        GameObject map_plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        map_plane.transform.position = new Vector3(0, 0, 0);
        // Get original size
        float map_plane_original_x_size = map_plane.GetComponent<MeshFilter>().mesh.bounds.size.x;
        float map_plane_original_z_size = map_plane.GetComponent<MeshFilter>().mesh.bounds.size.z;
        // Scale to fit
        float map_plane_x = width / map_plane_original_x_size;
        float map_plane_z = map.Count / map_plane_original_z_size;
        map_plane.transform.localScale = new Vector3(map_plane_x, 1, map_plane_z);
    }

    protected void PlaceObstacles()
    {
        // TODO:No square/size check!
        
        for (int i = 0; i < map.Count; i++)
        {
            // The height of map is: map.Count
            for (int j = 0; j < map[i].Count; j++)
            {
                int cube_num = int.Parse(map[i][j]);
                for (int k = 0; k < cube_num; k++)
                {
                    // Generate Cube
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // Change material and layer of cube
                    if (cube_num != 2)
                    {
                        Material mat = Resources.Load<Material>("Mat/Floar");
                        cube.GetComponent<Renderer>().material = mat;
                        // Change cube layer => Obstacles
                        cube.layer = LayerMask.NameToLayer("Obstacles");
                    }
                    else
                    {
                        Material mat = Resources.Load<Material>("Mat/Wall");
                        cube.GetComponent<Renderer>().material = mat;
                        // Change cube layer => Obstacles
                        /*cube.layer = LayerMask.NameToLayer("MapBoundary");*/
                    }
                    // Coordinate of cube: (-(map_row_max_cells / 2) + i, (float)0.5 + k, -(map_Collections.Count / 2) + j)
                    cube.transform.position = new Vector3(-(width / 2) + i, (float)0.5 + k, -(map.Count / 2) + j);
                }
            }
        }
    }

}