using UnityEngine;
using System;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour{
    //シングルトン実装抽象クラス
    //使い方 https://qiita.com/Teach/items/c146c7939db7acbd7eee

    private static T instance;
    public static T Instance
    {
        get{
            if (instance == null) {
                Type t = typeof(T);

                instance = (T)FindObjectOfType (t);
                if (instance == null) {
                    Debug.LogError (t + " をアタッチしているGameObjectはありません");
                }
            }

            return instance;
        }
    }

    virtual protected void Awake(){
        // 他のゲームオブジェクトにアタッチされているか調べる
        // アタッチされている場合は破棄する。
        CheckInstance();
    }

    protected bool CheckInstance(){
        if (instance == null) {
            instance = this as T;
            return true;
        } else if (Instance == this) {
            return true;
        }
        Debug.Log("シングルトンクラスが複数あります。削除します。");
        Destroy (this);
        return false;
    }
}