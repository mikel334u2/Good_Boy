using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Highly dependent script used for specific handling of dialogue and events
// If there's a dependency issue, it's likely here
public class DialogueEvents : MonoBehaviour
{
    // Singleton pattern
    private static DialogueEvents _instance;
    public static DialogueEvents Manager { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    [Header("Element 0 for Switching Dialogue")]
    [SerializeField] private string PASSING_FENCE = "2) After Passing the Fence";
    [SerializeField] private string CAT_CAMP_GUARD = "8) At the Gate of the Cat Camp";
    [SerializeField] private string SQUIRREL = "9) Meeting the Squirrel";

    [Header("Event Names")]
    [SerializeField] private string CART_ANIMATION = "Cart Animation";
    [SerializeField] private string DEDUCT_COINS = "Deduct Coins";
    [SerializeField] private string FLOAT_KEY_DOWN = "Float Key Down";
    [SerializeField] private string LOAD_CREDITS = "Load Credits";

    [Header("Other Known Fields")]
    [SerializeField] private string CREDITS_SCENE = "Good_Boy_Updated";

    [Header("Required Assets")]
    [SerializeField] private Animation cartAnimation;
    [SerializeField] private Animation floatKeyDownAnimation;
    [SerializeField] private FenceBreak fence;
    private PlayerInfo player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerInfo.Player;
    }

    public void HandleEvent(string eventName)
    {
        if (eventName == CART_ANIMATION)
            cartAnimation.Play();
        else if (eventName == DEDUCT_COINS)
        {
            player.coinCount -= 50;
            player.scoreText.text = player.coinCount.ToString();
        }
        else if (eventName == FLOAT_KEY_DOWN)
            floatKeyDownAnimation.Play();
        else if (eventName == LOAD_CREDITS)
            SceneManager.LoadScene(CREDITS_SCENE);
    }

    // Loads text based on conditions
    public TextAsset CheckAndLoadText(List<TextAsset> textFileAssets)
    {
        if (textFileAssets.Count < 2)
            return textFileAssets[0];

        string textAsset0 = textFileAssets[0].name;

        if (textAsset0 == PASSING_FENCE && (fence == null || fence.velocity.magnitude != 0))
            return textFileAssets[1];

        else if (textAsset0 == CAT_CAMP_GUARD && player.quests.ContainsKey(player.nameOfKeyQuest))
        {
            if (player.quests[player.nameOfKeyQuest].Completed)
                return textFileAssets[2];
            return textFileAssets[1];
        }

        else if (textAsset0 == SQUIRREL && player.quests.ContainsKey(player.nameOfCoinQuest))
        {
            if (player.quests[player.nameOfCoinQuest].Completed)
                return textFileAssets[2];
            return textFileAssets[1];
        }

        return textFileAssets[0];
    }
}
