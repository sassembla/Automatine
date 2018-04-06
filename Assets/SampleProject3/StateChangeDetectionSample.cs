using Automatine;
using UnityEngine;

public class StateChangeDetectionSample : MonoBehaviour
{
    private HoppingSample.Context context;

    private Auto<HoppingSample.Context, HoppingSample.Context> auto;

    // Use this for initialization
    void Start()
    {
        context = new HoppingSample.Context(this);

        // Hoppingで初期化
        auto = new Hopping<HoppingSample.Context, HoppingSample.Context>(frame, context);
    }

    int frame;

    // Update is called once per frame
    void Update()
    {
        /*
            もし現在のAutoのframeがActionという属性のDownという値を持っていたら、autoを切り替える。
        */
        if (auto.Contains(AutoConditions.Action.Down))
        {
            auto = new Run<HoppingSample.Context, HoppingSample.Context>(frame, context);
        }

        auto.Update(frame, context);
        frame++;
    }
}