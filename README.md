
# Automatine

## TOC
	
1. Introduction & Motivations
1. 導入
1. 構造
1. APIs
1. 逆引きAutomatine


## Introduction & Motivations

### intro
Automatineは、タイムライン上に状態やスクリプトを配置し実行できるユーティリティ。特に次のようなことを簡単に管理するのを目指している。

* タイムラインでキャラクターやゲーム自体の挙動を制御したい
* 条件分岐を入れてAIのようなものを作りたい
* 状態を持たせたい、別のタイムラインに引き継ぎたい(Condition)
* サーバなど、Unity外でも動かしたい(Portability)
* RuntimeとDataを分離したい(Updatable)
* コードからPlay中の実行、更新、追加を行う
* GUIで設定の生成、変更、データ化を行う(Artist Friendly)


### Sequencerとの違い
Unity5.4以降でSeqencer(Timeline tool)が入るが、それとは異なる用途を目指している。


| やりたいこと        | Sequencer           | Automatine  |
| ------------- |-------------:| -----:|
| Assetをタイムラインにセット		| o | x |
|シーン単位でタイムライン実行| o | o |
|キャラ単位でタイムライン実行| difficult | o |
|一部の動作を繰り返して実行| x | o |
|状態の保持、管理| not suite | o |
|状態遷移の定義、実行| x | o |
|スクリプトの実行| o | o |
|スクリプトの継続実行| ? | o |
|AIの実装| not aimed | o |


Sequencerがシネマティックなシーケンスを制御するのを主なMotivationにしているのに対し、Automatineはより細かい粒度での「キャラクターやオブジェクトの状態を管理し、スクリプトの実行と継続動作」を実現することを主な目的としている。


### 特徴
すでに実装されている大きな特徴として次のようなものがある。

* MonoBehaviour、Assetへの依存を持たない、ピュアC#コード
* 実行のためのシングルトンインスタンスなどを一切持たないので導入や廃棄が楽
* 実行時、ゲームごとに独自定義した型とインスタンスを組み込むことができる
* フレーム単位での状態定義、取得が可能
* 通常のC＃スクリプトをセットして実行したり、Coroutineの継続的な実行が可能
* 実時間ではなく、カウント=実行回数ベースで動作する。個別に、1フレーム中で何回も実行することが可能
* GUIから、コードを吐き出す or データを吐き出すことが可能
* データ形式はJSON他が使用可能
* RuntimeがUnityに依存していないので他のプラットフォームでも動作する
* C#以外の言語でも同様のRuntimeを生成することが比較的容易(Go,Rust,Lua等)


## 導入

TODO ☆Automatineを使い始めるにあたってのTutorial。gifアニメと解説の組み合わせになるはず。

in progress...

### インストール
Automatine フォルダをAssetsフォルダ以下に置く。  
Assets/Automatine

in progress...


### Automatine編集ウィンドウを開く
Window > Automatine


### GUIでAutoを編集、出力
1. 初めて開いた場合、☆特定の名前のAutoがあるはず。
1. UI上を右クリックしてタイムラインを追加。
1. タイムラインを右クリックしてタックを追加、位置や長さを適当にかえてみよう。
1. 編集したAutoを出力すると、コードが生成される。


### コードから使う定義をする
1. usingを追加する using Automatine

in progress...


### 動かしてみる
3. autoインスタンスのUpdateメソッドを追加
4. frameをインクリメントするように追記
5. autoの終了を検知したら動くif文を追加
6. Play


### GUIでタック(鋲)に状態をセットしてみる
1. Automatineウインドウを開き、先ほど作ったタックを選択
1. Inspectorから状態を追加 (in progress)
1. 保存


###  autoから状態を取得する
1. 状態を取得するコードを記述
1. Play (in progress)

autoインスタンスから、現在の状態を取得することができる。
「この状態になったので何かする」、というようなトリガーを簡単に実装できる。


## 構造
Auto / Timeline / Tack(鋲) / Coroutine | ConditionKey & Value


## APIs


in progress...
