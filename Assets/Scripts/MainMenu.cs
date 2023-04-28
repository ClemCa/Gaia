using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform planet;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Zoom zoom;
    [SerializeField] private PlanetRotation rotation;
    [SerializeField] private float showcaseRotationSpeed = 10f;
    [SerializeField] private float showcasePixelOffset = 300f;
    [SerializeField] private float startGameSpeed = 100f;
    [SerializeField] private float transitionDuration = 2f;
    void Start()
    {
        StartCoroutine(RotatePlanet());
        StartCoroutine(PlaceCamera());
    }
    public void StartGameButton()
    {
        StopAllCoroutines();
        StartCoroutine(StartGame());
    }
    public void QuitButton()
    {
        Application.Quit();
    }
    public void HomeButton()
    {
        StopAllCoroutines();
        StartCoroutine(Home());
    }
    private IEnumerator PlaceCamera()
    {
        while (true)
        {
            mainCamera.localPosition = new Vector3(GetCameraPosition(), mainCamera.localPosition.y, mainCamera.localPosition.z);
            yield return null;
        }
    }
    private float GetCameraPosition()
    {
        float screenX = Mathf.Max(showcasePixelOffset, Screen.width * 0.3f);
        // screen x is in pixels, we need to convert it to world units
        float cameraX = screenX / Screen.width * 2 * mainCamera.localPosition.y * Mathf.Tan(mainCamera.GetComponent<Camera>().fieldOfView * 0.5f * Mathf.Deg2Rad);
        return cameraX;
    }
    private IEnumerator RotatePlanet()
    {
        while (true)
        {
            Quaternion rot = Quaternion.Euler(0, 0, showcaseRotationSpeed * Time.deltaTime);
            planet.rotation = rot * planet.rotation;
            yield return null;
        }
    }
    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.25f);
        ShrinkMenuButtons();
        GrowGameButtons();
        float Sigmoid(float v, float m)
        {
            float k = Mathf.Exp(-(v*2-1) * m);
            return 1 / (1f + k);
        }
        float t = 0;
        Vector3 cameraPos = mainCamera.localPosition;
        Vector3 basePos = cameraPos;
        Quaternion startRot = planet.rotation;
        Quaternion maxRot = Quaternion.Euler(0, 0, startGameSpeed) * planet.rotation;
        while (t < 1)
        {
            t += Time.deltaTime / transitionDuration;
            float altT = Sigmoid(t, 8);
            cameraPos.x = Mathf.Lerp(basePos.x, 0, altT);
            cameraPos.y = Mathf.Lerp(basePos.y, 12, altT);
            mainCamera.localPosition = cameraPos;
            Quaternion rot = Quaternion.Lerp(startRot, maxRot, altT);
            planet.rotation = rot;
            yield return null;
        }
        rotation.SetEnabled(true);
        zoom.enabled = true;
        cameraPos.x = 0;
        mainCamera.localPosition = cameraPos;
    }

    private IEnumerator Home()
    {
        yield return new WaitForSeconds(0.25f);
        ShrinkGameButtons();
        GrowMenuButtons();
        rotation.SetEnabled(false);
        zoom.enabled = false;
        float Sigmoid(float v, float m)
        {
            float k = Mathf.Exp(-(v*2-1) * m);
            return 1 / (1f + k);
        }
        float tragetPos = GetCameraPosition();
        float t = 0;
        Vector3 cameraPos = mainCamera.localPosition;
        Vector3 basePos = cameraPos;
        Quaternion startRot = planet.rotation;
        Quaternion maxRot = Quaternion.Euler(0, 0, -startGameSpeed) * planet.rotation;
        while (t < 1)
        {
            t += Time.deltaTime / transitionDuration;
            float altT = Sigmoid(t, 8);
            cameraPos.x = Mathf.Lerp(basePos.x, tragetPos, altT);
            cameraPos.y = Mathf.Lerp(basePos.y, 15, altT);
            mainCamera.localPosition = cameraPos;
            Quaternion rot = Quaternion.Lerp(startRot, maxRot, altT);
            planet.rotation = rot;
            yield return null;
        }
        cameraPos.x = tragetPos;
        mainCamera.localPosition = cameraPos;
    }

    private void ShrinkMenuButtons()
    {
        var buttons = GetComponentsInChildren<MainMenuButton>();
        foreach (var button in buttons)
        {
            button.Shrink();
        }
    }
    private void GrowMenuButtons()
    {
        var buttons = GetComponentsInChildren<MainMenuButton>();
        foreach (var button in buttons)
        {
            button.Grow();
        }
    }

    private void ShrinkGameButtons()
    {
        var buttons = GetComponentsInChildren<InGameButton>();
        foreach (var button in buttons)
        {
            button.Shrink();
        }
        var skillButtons = GetComponentsInChildren<SkillButton>();
        foreach (var button in skillButtons)
        {
            button.Shrink();
        }
    }

    private void GrowGameButtons()
    {
        var buttons = GetComponentsInChildren<InGameButton>();
        foreach (var button in buttons)
        {
            button.Grow();
        }
        var skillButtons = GetComponentsInChildren<SkillButton>();
        foreach (var button in skillButtons)
        {
            button.Grow();
        }
    }
}
