using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public abstract class Utilities : MonoBehaviour
{
    // valueを0~1に正規化した値を返す
    public static float Normalizer(float value, float min, float max)
    {
        float norm = (value - min)/(max - min);
        return norm;
    }

    IEnumerator Delay(float delay_time, Action method, Action method2 = null, Action method3 = null, Action method4 = null)
    {
        yield return new WaitForSeconds(delay_time);
        method();
        if(method2 != null)
        {
            method2();
        }
        if(method3 != null)
        {
            method3();
        }
        if(method4 != null)
        {
            method4();
        }
    }
    // 最大３つの処理(引数、返り値無し)をdelay_timeだけ遅らせて実行する
    public void DelayCall(float delay_time, Action method, Action method2 = null, Action method3 = null, Action method4 = null)
    {
        StartCoroutine(Delay(delay_time, method, method2, method3, method4));
    }

    public bool IsContaining<T>(T value, IEnumerable<T> iter)
    {
        return iter.Contains(value);
    }

    // ベクトルの大きさ(の２乗)と実数値(の２乗)を比較
    // ベクトルの大きさの方が小さいときにTrue
    public bool SqrComparison(Vector3 vector, float value)
    {
        float sqr_vector_mag = Vector3.SqrMagnitude(vector);
        float sqr_value = Mathf.Pow(value, 2);
        bool judge = sqr_vector_mag < sqr_value ? true : false;
        return judge;
    }

    public void LookAtInOneCall(Vector3 direction, float duration)
    {
        Vector3 projection = new Vector3(direction.x, 0, direction.z);
        float y_angle = Vector3.Angle(Vector3.forward, projection);
        transform.DORotate(new Vector3(0, y_angle, 0), duration, RotateMode.Fast);
    }

    // 条件(Func)を満たすものがList(Array)に存在するか、存在する場合はそのobjectを返す
    public bool IEnumSeeker<S>(IEnumerable<S> iter, Func<S, bool> condition, out S obj)
    {
        if(iter == null)
        {
            Debug.Log("Listが空です");
            obj = default(S);
            return false;
        }
        
        List<S> matches = new List<S>();
        foreach(S i in iter)
        {
            if(condition(i))
            {
                matches.Add(i);
            }
        }
        if(matches.Count < 1)
        {
            obj = default(S);
            return false;
        }
        else
        {
            obj = matches[0];
            return true;
        }
    }
}
