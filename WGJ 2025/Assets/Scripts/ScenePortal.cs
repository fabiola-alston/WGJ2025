using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] float fadeDuration = 0.75f;
    [SerializeField] string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (ScreenFader.Instance != null)
            ScreenFader.Instance.TransitionToScene(sceneName, fadeDuration);
        else
            SceneManager.LoadScene(sceneName); 
    }

    private System.Collections.IEnumerator LoadSceneWithFade()
    {
        // Fade out if a fader exists
        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeOut(fadeDuration);

        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;

        // Fade back in after load
        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeIn(fadeDuration);
    }


}