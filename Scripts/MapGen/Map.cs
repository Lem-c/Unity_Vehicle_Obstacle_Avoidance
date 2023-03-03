using UnityEngine;
using System.Collections;
using System.Collections.Generic;//�����õ���C#����

public class Map : MonoBehaviour
{

    void Start()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("EasyMap");//����Map.csv��ע�ⲻҪ��������ʽ��Map�ģ�������δ֪����
        string[] map_row_string = textAsset.text.Trim().Split('\n');//������Map.csvǰǰ���Ļ��У��ո�֮��ģ��������з��ָ�ÿһ��
        int map_row_max_cells = 0;//���������ά���У����������Ҳ������һ��������и���Ԫ��
        List<List<string>> map_Collections = new List<List<string>>();//����һ��C#����map_Collections
        for (int i = 0; i < map_row_string.Length; i++)//��ȡÿһ�е�����
        {
            List<string> map_row = new List<string>(map_row_string[i].Split(','));//�����ŷָ�ÿ��һ����Ԫ��
            if (map_row_max_cells < map_row.Count)
            {//��һ��������и���Ԫ��δ��Ҫ�ݴ�����һ��Plane����Cube��
                map_row_max_cells = map_row.Count;
            }
            map_Collections.Add(map_row);//����ã��ŵ�����map_Collections��
        }

        /*����һ���պ÷ź�Cube��Plane*/
        GameObject map_plane = GameObject.CreatePrimitive(PrimitiveType.Plane);//����һ��Plane
        map_plane.transform.position = new Vector3(0, 0, 0);//�ŵ�(0,0,0)���λ��
        //����ԭʼ��С
        float map_plane_original_x_size = map_plane.GetComponent<MeshFilter>().mesh.bounds.size.x;
        float map_plane_original_z_size = map_plane.GetComponent<MeshFilter>().mesh.bounds.size.z;
        //�������Map�������С���պúͶ�ά��ƥ��
        float map_plane_x = map_row_max_cells / map_plane_original_x_size;
        float map_plane_z = map_Collections.Count / map_plane_original_z_size;
        map_plane.transform.localScale = new Vector3(map_plane_x, 1, map_plane_z);

        /*��Plane�Ϸ�Cube*/
        for (int i = 0; i < map_Collections.Count; i++)//Z�����ǳ��Ⱦ��������Ĵ�С��Ҳ����map.csv�ж�����Ч����
        {
            for (int j = 0; j < map_Collections[i].Count; j++)
            {//X����Ŀ�Ⱦ�������һ���е����ĳ��ȣ�Ҳ����map.csv��ÿ����󳤶�
                int cube_num = int.Parse(map_Collections[i][j]);//��ÿ����Ԫ�������ת��������                
                for (int k = 0; k < cube_num; k++)
                {//�������֣���һ����Ԫ��������cube
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(-(map_row_max_cells / 2) + i, (float)0.5 + k, -(map_Collections.Count / 2) + j);
                    /*
                     cube�������������(-(map_row_max_cells / 2) + i, (float)0.5 + k, -(map_Collections.Count / 2) + j)
                     */
                }
            }
        }
    }

}
