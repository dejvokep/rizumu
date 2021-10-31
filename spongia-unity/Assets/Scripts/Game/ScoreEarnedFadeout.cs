using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEarnedFadeout : MonoBehaviour
{

    private const float FADE_DURATION = 1;
    private const float MOVE_SPEED = 0.2f;


    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine(Animate());
    }

    private IEnumerator Animate() {
        float fadeSpeed = 1.0f / FADE_DURATION;
        Color color = text.color;

        for (float t = 0; t < 1; t += Time.deltaTime * fadeSpeed) {
            color.a = Mathf.Lerp(1, 0, t);
            text.color = color;
            yield return true;
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * MOVE_SPEED);
    }
}
