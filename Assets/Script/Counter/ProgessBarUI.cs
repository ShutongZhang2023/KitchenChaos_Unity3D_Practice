using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgessBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject hasProgressGameObject;

    private IHasProgress hasPogress;

    private void Start()
    {
        hasPogress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (hasPogress == null)
        {
            Debug.LogError("IHasProgress component not found on hasProgressGameObject");
            return;
        }

        hasPogress.OnProgressChanged += HasProgress_OnProgressChanged;
        barImage.fillAmount = 0f;
        Hide();
    }

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedArgs e)
    {
        barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized ==  1 || e.progressNormalized == 0)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
