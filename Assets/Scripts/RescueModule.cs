using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueModule : MonoBehaviour
{
    public GameObject RescueHelper;
    bool blinking = false;

    private void Start()
    {
        GameManager.Instance.RescueHelper = this;
        SetBlinkingEnabled(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null && rb.gameObject.tag == "Player")
        {
            if(GameManager.Instance.Player.HasFishHookedUp)
            {
                GameManager.Instance.Player.CanRescueFish = true;
                GameManager.Instance.Player.ReleaseFish();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null && rb.gameObject.tag == "Player")
            GameManager.Instance.Player.CanRescueFish = false;
    }

    private void Update()
    {
        if(blinking)
        {
            var rnd = RescueHelper.GetComponent<Renderer>();
            var color = rnd.material.color;
            color.a = Mathf.Sin(Time.time * 2f) * 0.25f + 0.5f;
            rnd.material.color = color;
        }
    }

    public void SetBlinkingEnabled(bool blink)
    {
        blinking = blink;
        RescueHelper.SetActive(blink);
    }

}
