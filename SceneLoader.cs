using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Runtime.CompilerServices;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Image _fadeImage;
    float _fadeSpeed = 1f;

    [SerializeField] Slider _loadingSlider;
    TextMeshProUGUI _loadingSliderText;

    IEnumerator _fadeIn;

    public event EventHandler loadingComplete = delegate { };

    void Awake()
    {
        _fadeImage.gameObject.SetActive(true);
        _loadingSliderText = _loadingSlider.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        GameManager.Instance.SceneLoader = this;
        StartCoroutine(_fadeIn = FadeIn());
    }

    public void LoadScene(int sceneIndex)
    {
        StopCoroutine(_fadeIn);
        StartCoroutine(FadeOut(sceneIndex));
    }

    IEnumerator FadeIn()
    {
        Color fadeColor = _fadeImage.color;
        float fadeRate = 0f;

        while (fadeColor.a > 0f)
        {
            fadeRate += Time.deltaTime * _fadeSpeed;
            fadeColor.a = Mathf.Lerp(1f, 0f, fadeRate);
            _fadeImage.color = fadeColor;

            yield return null;
        }

        _fadeImage.gameObject.SetActive(false);
        loadingComplete.Invoke(this, null);
    }

    IEnumerator FadeOut(int sceneIndex)
    {
        _fadeImage.gameObject.SetActive(true);
        Color fadeColor = _fadeImage.color;
        float fadeRate = 0f;

        while (fadeColor.a < 1f)
        {
            fadeRate += Time.deltaTime * _fadeSpeed;
            fadeColor.a = Mathf.Lerp(0f, 1f, fadeRate);
            _fadeImage.color = fadeColor;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        _loadingSlider.gameObject.SetActive(true);
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingOperation.allowSceneActivation = false;

        while (loadingOperation.progress < 0.9f)
        {
            _loadingSlider.value = Mathf.Lerp(0f, 100f, loadingOperation.progress);
            _loadingSliderText.text = $"LOADING...{_loadingSlider.value:F0}%";

            yield return null;
        }

        _loadingSlider.value = 100f;
        _loadingSliderText.text = "LOADING...100%";

        yield return new WaitForSeconds(0.5f);
        _loadingSlider.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        loadingOperation.allowSceneActivation = true;
    }
}