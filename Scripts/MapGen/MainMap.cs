using UnityEngine;

public class MainMap : MonoBehaviour
{
    MapGen mapGenerator;
    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = new MapGen("EasyMap_2");
        mapGenerator.GenerateMap();
    }
}
