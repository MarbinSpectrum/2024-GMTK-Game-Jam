using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HowToPlay : MonoBehaviour
{
    [SerializeField] private List<Sprite> infoImg = new List<Sprite>();
    [SerializeField] private Image img;
    [SerializeField] private RectTransform leftArrow;
    [SerializeField] private RectTransform rightArrow;
    [SerializeField] private RectTransform playGame;
    private int nowImg;
    private bool playBtnFlag = false;

    public void Awake()
    {
        MovePage(0);
    }

    public void MovePage(int dic)
    {
        nowImg += dic;
        img.sprite = infoImg[nowImg];

        leftArrow.gameObject.SetActive(true);
        rightArrow.gameObject.SetActive(true);
        playGame.gameObject.SetActive(false);
        if (nowImg == 0)
            leftArrow.gameObject.SetActive(false);
        if (nowImg == infoImg.Count - 1)
        {
            rightArrow.gameObject.SetActive(false);
            playGame.gameObject.SetActive(true);
        }

    }

    public void PlayGame()
    {
        if (playBtnFlag)
            return;
        playBtnFlag = true;
        SceneManager.LoadScene("InGame");
    }
}
