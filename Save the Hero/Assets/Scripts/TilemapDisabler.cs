using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDisabler : MonoBehaviour
{

    private void Awake()
    {

        GetComponent<TilemapRenderer>().enabled = false;
    }
}