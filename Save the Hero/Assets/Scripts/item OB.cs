using UnityEngine;

public class itemOB : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] itemso data;
    
    public int Getpoint()
    {
        return data.point;
    }
}
