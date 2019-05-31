using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    [HideInInspector]
    public float movingSpeed = 0;

    private bool stopCoroutine = false;
    private void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
        transform.localScale = new Vector3(0, 0, 1f);
        StartCoroutine(ScaleUp());
        StartCoroutine(Move());
    }


    private void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
        stopCoroutine = false;
    }

    private void GameManager_GameStateChanged(GameState obj)
    {
        if (obj == GameState.GameOver)
        {
            stopCoroutine = true;
            StartCoroutine(MoveAfterGameOver());
        }
    }

    IEnumerator ScaleUp()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one;
        float t = 0;
        while (t < GameManager.Instance.wallScaleUpTime)
        {
            t += Time.deltaTime;
            float factor = t / GameManager.Instance.wallScaleUpTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, factor);
            yield return null;
        }
    }

    IEnumerator Move()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.back * 60f;
        float t = 0;
        float time = Vector3.Distance(startPos, endPos) / movingSpeed;
        while (t < time)
        {
            t += Time.deltaTime;
            float factor = t / time;
            transform.position = Vector3.Lerp(startPos, endPos, EaseType.EaseInSine(factor));
            yield return null;

            if (stopCoroutine)
                yield break;
        }
    }

    IEnumerator MoveAfterGameOver()
    {
        while (true)
        {
            transform.position += Vector3.back * GameManager.Instance.wallSpeedAfterSlowMotion * Time.deltaTime;
            yield return null;
        }
    }


}
