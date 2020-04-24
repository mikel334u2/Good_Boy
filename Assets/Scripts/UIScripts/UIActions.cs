using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIActions : MonoBehaviour
{
    [Tooltip("UI to hide when the game starts.")]
    public List<GameObject> UIToHide;
    private M_Camera m_camera;
    private M_PlayerController controller;

    // Singleton pattern
    private static UIActions _instance;
    public static UIActions UIManager { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        if (!PlayerInfo.Player.TryGetComponent<M_PlayerController>(out controller))
        {
            Debug.LogError("Attach a player controller to Player");
        }
        if (!Camera.main.TryGetComponent<M_Camera>(out m_camera))
        {
            Debug.LogError("Attach a player camera to PlayerInfo");
        }

        foreach (GameObject go in UIToHide)
        {
            go.SetActive(false);
        }
    }

    public void ToggleShowHide(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }

    public void Pause()
    {
        controller.zeroMovement = !controller.zeroMovement;
        m_camera.isRotatable = !m_camera.isRotatable;
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    public void LoadLevel(string levelToLoad)
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
