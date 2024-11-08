using TMPro;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ExpManager : MonoBehaviour
{
    public float exp;
    public TMP_Text expScoreUI;
    static public ReactiveProperty<string> ret = new ReactiveProperty<string>("0");

    void Start()
    {
        exp = 0;

        expScoreUI = GameObject.FindGameObjectWithTag("ExpScore").GetComponent<TMP_Text>();
        expScoreUI.text = "exp: 0 %";

        ret.Subscribe(value =>
        {
            Debug.Log("ret가 변경되었습니다: " + value);
            expScoreUI.text = UpdateExp();
        });

        //this.UpdateAsObservable()
        //    .Select(_ => ret)
        //    .DistinctUntilChanged()
        //    .Subscribe(_ => expScoreUI.text = UpdateExp());
    }

    public string UpdateExp()
    {
        Debug.Log("Current ret: " + ret.Value);

        float increment = 5;
        if (float.TryParse(ret.Value, out increment))
        {
            exp += increment;
        }
        else
        {
            exp += 5;
        }

        ret.Value = "0";

        Debug.Log($"exp: {exp} %");
        return $"exp: {exp} %";
    }
}
