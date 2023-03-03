using UnityEngine;
using System.Collections;
using System.Collections.Generic;//这里用到了C#容器

public class Map : MonoBehaviour
{

    void Start()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("EasyMap");//载入Map.csv，注意不要有其它格式叫Map的，或许有未知错误
        string[] map_row_string = textAsset.text.Trim().Split('\n');//清除这个Map.csv前前后后的换行，空格之类的，并按换行符分割每一行
        int map_row_max_cells = 0;//计算这个二维表中，最大列数，也就是在一行中最多有个单元格
        List<List<string>> map_Collections = new List<List<string>>();//设置一个C#容器map_Collections
        for (int i = 0; i < map_row_string.Length; i++)//读取每一行的数据
        {
            List<string> map_row = new List<string>(map_row_string[i].Split(','));//按逗号分割每个一个单元格
            if (map_row_max_cells < map_row.Count)
            {//求一行中最多有个单元格，未来要据此生成一个Plane来放Cube的
                map_row_max_cells = map_row.Count;
            }
            map_Collections.Add(map_row);//整理好，放到容器map_Collections中
        }

        /*生成一个刚好放好Cube的Plane*/
        GameObject map_plane = GameObject.CreatePrimitive(PrimitiveType.Plane);//生成一个Plane
        map_plane.transform.position = new Vector3(0, 0, 0);//放到(0,0,0)这个位置
        //求其原始大小
        float map_plane_original_x_size = map_plane.GetComponent<MeshFilter>().mesh.bounds.size.x;
        float map_plane_original_z_size = map_plane.GetComponent<MeshFilter>().mesh.bounds.size.z;
        //缩放这个Map到所需大小，刚好和二维表匹配
        float map_plane_x = map_row_max_cells / map_plane_original_x_size;
        float map_plane_z = map_Collections.Count / map_plane_original_z_size;
        map_plane.transform.localScale = new Vector3(map_plane_x, 1, map_plane_z);

        /*在Plane上放Cube*/
        for (int i = 0; i < map_Collections.Count; i++)//Z方向是长度就是容器的大小，也就是map.csv有多少有效的行
        {
            for (int j = 0; j < map_Collections[i].Count; j++)
            {//X方向的宽度就是容器一行中的最大的长度，也就是map.csv中每行最大长度
                int cube_num = int.Parse(map_Collections[i][j]);//将每个单元格的数字转换成整形                
                for (int k = 0; k < cube_num; k++)
                {//根据数字，在一个单元格内生成cube
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(-(map_row_max_cells / 2) + i, (float)0.5 + k, -(map_Collections.Count / 2) + j);
                    /*
                     cube所处的坐标就是(-(map_row_max_cells / 2) + i, (float)0.5 + k, -(map_Collections.Count / 2) + j)
                     */
                }
            }
        }
    }

}
