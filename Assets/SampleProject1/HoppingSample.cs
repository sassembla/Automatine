using Automatine;
using UnityEngine;

public class HoppingSample : MonoBehaviour
{
    /**
        Automatineが引き回す"コンテキスト"
     */
    private Context context;

    /**
        毎フレームUpdateを施す対象に対して、GUIで作成した"要素やタイミング"での操作を加える、Auto(Automatineの目的とするフレーム単位での実行を行う単位)

        型パラメータには、
        1.初期化時に与える引数
        2.Update時に与える引数
        の2つのパラメータの型を渡す。

        これらのパラメータは、Autoの初期化時とUpdate時にそれぞれ渡し、その内容をGUIで設定した要素やタイミングで更新することができる。
     */
    private Auto<Context, Context> auto;

    // Use this for initialization
    void Start()
    {
        /*
			コンテキストを初期化
		*/
        context = new Context(this);


        /*
            Autoを初期化、ここでは最初にHoppingというAutoを適応する。
            GUI上でHoppingを作成 -> Autoを拡張した型としてコード出力が行われるので、コード上でAutoとして生成できる。

            ここでは、第一引数はframe、第二引数にContextを渡して初期化する。
         */
        auto = new Hopping<Context, Context>(frame, context);
    }

    int frame;

    // Update is called once per frame
    void Update()
    {
        /*
            frameを毎フレーム加算し、Contextを与えてUpdate = 更新動作を行う。
            この時、HoppingというAutoのタイムライン上のいろいろな要素が、contextに対して作用する。
         */
        auto.Update(frame, context);

        /*
            frameを加算する。ここで足す数値を2倍とかにすると、倍速で動作する。
         */
        frame++;
    }

    /**
        このサンプルでAutomatineが「動かす対象」としてセットされるコンテキストクラス。
        生成するAutoの引数にセットされているため、初期化時とUpdate時にパラメータとして渡すことができる。
        
        Update時などに、Auto内からこのクラスのインスタンスに干渉が発生する。
        具体的に言うと、このインスタンスをAuto内のCoroutineから触ることができる。
     */
    public class Context
    {
        public MonoBehaviour player;


        public Context(MonoBehaviour player)
        {
            this.player = player;
        }
    }
}