using UnityEngine;
using System.Collections;
using Automatine;

public class Cont : MonoBehaviour
{
    // Auto <Cont, int> auto;

    // Use this for initialization
    void Start()
    {
        // auto = new Camera<Cont, int>(0, this);
    }
    int frame = 0;
    // Update is called once per frame
    void Update()
    {
        // auto.Update(frame, 100);
        frame++;
    }
}
