using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject GamePanel;
    public RectTransform TitleImage;
    public AnimationCurve TitleAnimationCurve;
    public float TitleAnimationTime = 1;

    public Image WinImage;
    public Image LoseImage;


    public Text healthText;
    public Text torpedoText;
    public Text fishText;

    public Canvas TheCanvas;

    bool titleVisible = true;
    float titleShownPos;
    float titleHiddenPos;

    [Header("InGameUI")]
    public GameObject TorpedoPrefab;
    public GameObject HealthPrefab;
    public GameObject FishUI;

    private void Start()
    {
        WinImage.gameObject.SetActive(false);
        LoseImage.gameObject.SetActive(false);

        GotoMenu();
        GameManager.Instance.UIController = this;

        float height = TitleImage.sizeDelta.y;
        float pos = TitleImage.anchoredPosition.y;
        
        titleShownPos = pos;
        titleHiddenPos = Mathf.Abs(pos) +  height ;
    }

    public void GotoMenu()
    {
        ShowTitle();
        GamePanel.SetActive(false);
        MenuPanel.SetActive(true);
    }

    public void StartGame()
    {
        WinImage.gameObject.SetActive(false);
        LoseImage.gameObject.SetActive(false);

        HideTitle();
        GameManager.Instance.StartNewGame();
        MenuPanel.SetActive(false);
        GamePanel.SetActive(true);
    }

    public void HideTitle()
    {
        if (titleVisible == false)
            return;
        titleVisible = false;

        StopCoroutine("TitleHideAnimation");
        StartCoroutine("TitleHideAnimation");
    }

    IEnumerator TitleHideAnimation()
    {
        float timer = 0;
        while(timer < TitleAnimationTime)
        {
            timer += Time.deltaTime;

            float t = timer / TitleAnimationTime;
            float at = TitleAnimationCurve.Evaluate(t);
            float y = Mathf.Lerp(titleShownPos, titleHiddenPos, at);

            var pos = TitleImage.anchoredPosition;
            pos.y = y;
            TitleImage.anchoredPosition = pos;

            yield return new WaitForEndOfFrame();
        }
    }

    public void ShowTitle()
    {
        if (titleVisible)
            return;
        titleVisible = true;

        StopCoroutine("TitleShowAnimation");
        StartCoroutine("TitleShowAnimation");
    }

    IEnumerator TitleShowAnimation()
    {
        float timer = 0;
        while (timer < TitleAnimationTime)
        {
            timer += Time.deltaTime;

            float t = timer / TitleAnimationTime;
            float at = TitleAnimationCurve.Evaluate(t);
            float y = Mathf.Lerp(titleHiddenPos, titleShownPos, at);

            var pos = TitleImage.anchoredPosition;
            pos.y = y;
            TitleImage.anchoredPosition = pos;

            yield return new WaitForEndOfFrame();
        }
    }



    void Update ()
    {
		if(GameManager.Instance.Player != null)
        {
            healthText.text = GameManager.Instance.Player.Health.ToString();
            torpedoText.text = GameManager.Instance.Player.TorpedoCount.ToString();
            
        }

        fishText.text = GameManager.Instance.FishCount.ToString();
    }

    public void SpawnFishInfo(Vector3 worldPos)
    {
       var screenPos = GameManager.Instance.CameraManager.GameCamera.WorldToViewportPoint(worldPos);

       screenPos = new Vector2
       (
           screenPos.x * (TheCanvas.transform as RectTransform).sizeDelta.x,
           screenPos.y * (TheCanvas.transform as RectTransform).sizeDelta.y
       );

        var go = Instantiate(FishUI, TheCanvas.transform);
        go.GetComponent<RectTransform>().anchoredPosition = screenPos;
    }

    public void SpawnTorpedoInfo(int amount)
    {
        var pos = GameManager.Instance.Player.transform.position;
        var screenPos = GameManager.Instance.CameraManager.GameCamera.WorldToViewportPoint(pos);

        screenPos = new Vector2
       (
           screenPos.x * (TheCanvas.transform as RectTransform).sizeDelta.x,
           screenPos.y * (TheCanvas.transform as RectTransform).sizeDelta.y
       );

        var go = Instantiate(TorpedoPrefab, TheCanvas.transform);
        var ui = go.GetComponent<UIInfo>();
        ui.SetValue(amount);

        go.GetComponent<RectTransform>().anchoredPosition = screenPos;
    }

    public void SpawnHealthInfo(int amount, Transform target = null)
    {
        var pos = GameManager.Instance.Player.transform.position;
        
        if (target != null)
            pos = target.position;
        var screenPos = GameManager.Instance.CameraManager.GameCamera.WorldToViewportPoint(pos);
        
        screenPos = new Vector2
        (
            screenPos.x * (TheCanvas.transform as RectTransform).sizeDelta.x,
            screenPos.y * (TheCanvas.transform as RectTransform).sizeDelta.y
        );

        var go = Instantiate(HealthPrefab, TheCanvas.transform);
        var ui = go.GetComponent<UIInfo>();
        ui.SetValue(amount);

        go.GetComponent<RectTransform>().anchoredPosition = screenPos;
    }

    Image currentMessage;

    IEnumerator WaitAndReset()
    {
        currentMessage.gameObject.SetActive(true);
        currentMessage.color = new Color(1, 1, 1, 0);

        float fadeTime = 1;

        float timer = 0;
        while(timer < 5)
        {
            timer += Time.deltaTime;

            float ap = timer / fadeTime;
            float a = Mathf.Lerp(0, 1, ap);
            currentMessage.color = new Color(1, 1, 1, a);

            yield return new WaitForEndOfFrame();
        }

        GotoMenu();

        timer = 0;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            float ap = timer / fadeTime;
            float a = Mathf.Lerp(0, 1, ap);
            currentMessage.color = new Color(1, 1, 1, 1f - a);

            yield return new WaitForEndOfFrame();
        }

        currentMessage.gameObject.SetActive(false);
    }

    public void GotoWin()
    {
        GamePanel.SetActive(false);
        currentMessage = WinImage;
        StartCoroutine(WaitAndReset());  
    }

    public void GotoGameOver()
    {
        GamePanel.SetActive(false);
        currentMessage = LoseImage;
        StartCoroutine(WaitAndReset());
    }
}
