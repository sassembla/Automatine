using Automatine;
using UnityEngine;

public class HoppingThenRunSample : MonoBehaviour
{
    /**
        Automatineが引き回す"コンテキスト"
     */
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
        auto.Update(frame, context);

        /*
            もしhoppingが終了したら、新たにRunへと切り替えを行う。
            次のフレームからはRunが実行される。
        */
        if (auto.JustConsumed(frame))
        {
            auto = new Run<HoppingSample.Context, HoppingSample.Context>(frame, context);
        }

        frame++;
    }
}