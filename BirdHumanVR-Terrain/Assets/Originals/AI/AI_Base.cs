using System.Collections;
using System.Collections.Generic;
using System;

namespace AI_Base
{
    public abstract class State
    {
        // 各動物ごとのStateに応じたメソッドを登録するためのデリゲート
        public Action execution;

        // Stateに応じた処理を実行するメソッドを呼び出す
        // 派生クラスでも処理を書きたい場合を想定して、virtualにする
        public virtual void Execute()
        {
            if(execution != null)
            {
                execution();
            }
        }

        // 現在のState名を取得する
        public abstract string GetStateName();
    }



    public class Idol : State
    {
        public override string GetStateName() {return "Idol";}
    }

    public class Walk : State
    {
        public override string GetStateName() {return "Walk";}
    }

    public class Run : State
    {
        public override string GetStateName() {return "Run";}
    }

    public class Eat : State
    {
        public override string GetStateName() {return "Eat";}
    }

    public class Attack : State
    {
        public override string GetStateName() {return "Attack";}
    }

    public class Dead : State
    {
        public override string GetStateName() {return "Dead";}
    }

    public class Howl : State
    {
        public override string GetStateName() {return "Howl";}
    }
}