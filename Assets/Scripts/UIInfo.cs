using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInfo : MonoBehaviour
{
    public Text Text;
    public Image Image;

    Color color;
    RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();

        //Text = GetComponentInChildren<Text>();
        //Image = GetComponent<Image>();
        color = Image.color;

        StartCoroutine(Animate());
    }

    public void SetValue(int value)
    {
        if (value < 0)
        {
            SetColor(GameManager.Instance.BadColor);
            Text.text = string.Format("-{0}", Mathf.Abs(value));
        }
        else
        {
            SetColor(GameManager.Instance.GoodColor);
            Text.text = string.Format("+{0}", Mathf.Abs(value));
        }
    }

    public void SetColor(Color color)
    {
        this.color = color;
        Text.color = color;
        Image.color = color;
    }

    IEnumerator Animate()
    {
        float duration = 1.5f;
        float timer = 0;
        
        while(timer < duration)
        {
            timer += Time.deltaTime;
            float p = timer / duration;

            rt.position += new Vector3(0, 40 * Time.deltaTime, 0);
            color.a = 1f - p;
            SetColor(color);

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }


}
