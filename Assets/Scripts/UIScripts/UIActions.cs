using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIActions : MonoBehaviour
{
    private M_PlayerController m_controller;
    private M_Camera m_camera;
    [Tooltip("UI to hide when the game starts.")]
    public List<GameObject> UIToHide;
    [Tooltip("UI to hide when the game starts.")]
    [SerializeField] private List<Button> tabButtons;
    [SerializeField] private Button bookButton;
    [SerializeField] private Button hiddenGearButton;
    [SerializeField] private string creditsSceneName = "Good_Boy_Updated";
    private int index = -1;
    [HideInInspector] public bool paused = false;
    private bool zm = true;
    private bool rot = false;

    private Controls controls;

    // Singleton pattern
    private static UIActions _instance;
    public static UIActions UI { get { return _instance; } }
    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
        SetupInput();
    }

    void Start()
    {
        if (PlayerInfo.Player != null && !PlayerInfo.Player.TryGetComponent<M_PlayerController>(out m_controller))
            Debug.LogError("Attach a player controller to player");
        if (!Camera.main.TryGetComponent<M_Camera>(out m_camera))
            Debug.LogError("Attach M_Camera to main camera");
        StartCoroutine("HideObjects");
    }

    IEnumerator HideObjects()
    {
        yield return null;
        foreach (GameObject go in UIToHide)
            go.SetActive(false);
    }

    // New input system
    void OnEnable()
    {
        controls.Enable();
    }
    void OnDisable()
    {
        controls.Disable();
    }
    private void SetupInput()
    {
        controls = new Controls();
        if (tabButtons.Count > 4)
            controls.Player.Tab5.performed += _ =>
            {
                tabButtons[4].onClick.Invoke();
                InvokeOnClick(hiddenGearButton);
            };
        if (tabButtons.Count > 3)
            controls.Player.Tab4.performed += _ => tabButtons[3].onClick.Invoke();
        if (tabButtons.Count > 2)
            controls.Player.Tab3.performed += _ => tabButtons[2].onClick.Invoke();
        if (tabButtons.Count > 1)
            controls.Player.Tab2.performed += _ => tabButtons[1].onClick.Invoke();
        if (tabButtons.Count > 0)
        {
            controls.Player.Tab1.performed += _ => tabButtons[0].onClick.Invoke();
            controls.Player.TabLeft.started += _ => TabLeft();
            controls.Player.TabRight.started += _ => TabRight();
        }
        controls.Player.Menu.performed += _ =>
        {
            if (SceneManager.GetActiveScene().name == creditsSceneName)
                Application.Quit();
            else
                InvokeOnClick(bookButton);
        };
        controls.UI.Cancel.performed += _ => Application.Quit();
    }

    // PUBLIC FUNCTIONS ------------------------------------------------------


    public void ToggleShowHide(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }

    public void InvokeOnClick(Button button)
    {
        if (button != null && button.gameObject.activeSelf)
            button.onClick.Invoke();
    }

    // disable interact, jump, move, look, bark, twerk, sprint
    public void Pause()
    {
        index = -1;
        paused = !paused;
        if (paused)
        {
            zm = m_controller.zeroMovement;
            rot = m_camera.isRotatable;
            m_controller.zeroMovement = true;
            m_camera.isRotatable = false;
        }
        else
        {
            m_controller.zeroMovement = zm;
            m_camera.isRotatable = rot;
        }
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

    public void EnableChildrenInteractable(GameObject parent)
    {
        Selectable[] selectables = parent.GetComponentsInChildren<Selectable>();
        foreach (Selectable s in selectables)
        {
            s.interactable = true;
        }
    }

    public void DisableChildrenInteractable(GameObject parent)
    {
        Selectable[] selectables = parent.GetComponentsInChildren<Selectable>();
        foreach (Selectable s in selectables)
        {
            s.interactable = false;
        }
    }

    public void SelectFirstChild(GameObject parent)
    {
        Selectable s = parent.GetComponentInChildren<Selectable>();
        // Debug.Log(s.name);
        if (s != null && s.gameObject.activeInHierarchy)
        {
            IEnumerator function = SelectLater(s);
            StartCoroutine(function);
        }
    }

    public void TabRight()
    {
        if (index == -1)
            index = 0;
        else
            index = (index + 1) % tabButtons.Count;
        if (tabButtons[index].gameObject.activeInHierarchy)
        {
            Debug.Log(tabButtons[index].name);
            IEnumerator function = SelectLater(tabButtons[index]);
            StartCoroutine(function);
            tabButtons[index].onClick.Invoke();
        }
    }

    public void TabLeft()
    {
        if (index == -1)
            index = 0;
        else
        {
            index--;
            if (index < 0)
                index += tabButtons.Count;
        }
        if (tabButtons[index].gameObject.activeInHierarchy)
        {
            Debug.Log(tabButtons[index].name);
            IEnumerator function = SelectLater(tabButtons[index]);
            StartCoroutine(function);
        }
    }

    public IEnumerator SelectLater(Selectable selectable)
    {
        while (EventSystem.current.alreadySelecting)
        {
            yield return null;
        }
        EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        // Debug.Log("Current selected: " + EventSystem.current.currentSelectedGameObject);
    }
}
