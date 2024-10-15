using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SoundMgr : SerializedMonoBehaviour
{
    private bool InitFlag = false;
    public static SoundMgr Instance { get; private set; }


    [SerializeField] private Dictionary<ESound, AudioClip> soundData = new Dictionary<ESound, AudioClip>();
    [SerializeField] private AudioSource audioSource;
    private Dictionary<ESound, int> soundCnt = new Dictionary<ESound, int>();

    private void Init()
    {
        if (InitFlag)
            return;
        InitFlag = true;
        Instance = this;
    }

    private void Awake()
    {
        Init();
    }

    public void PlaySound(Transform transform, ESound eSound)
    {
        if (soundCnt.ContainsKey(eSound) == false)
            soundCnt[eSound] = 0;

        if (soundCnt[eSound] > 5)
            return;

        if (Vector3.Distance((Vector2)Camera.main.transform.position, (Vector2)transform.position) > 10)
            return;

        IEnumerator Run()
        {
            soundCnt[eSound]++;

            AudioClip audio = soundData[eSound];
            audioSource.PlayOneShot(audio);
            yield return new WaitForSeconds(audio.length);

            soundCnt[eSound]--;
        }
        StartCoroutine(Run());
    }
}
