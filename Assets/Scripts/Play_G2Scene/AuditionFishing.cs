using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Play_G2Scene;

public class AuditionFishing : MonoBehaviour
{
    [Header("UI References")]
    public GameObject arrowTemplate;
    public Transform arrowContainer;
    public Slider timeSlider;
    public TextMeshProUGUI containerText;

    [Header("UI Kết Quả")]
    public GameObject resultPanel;
    public TextMeshProUGUI rewardAmountText;
    public Button closeButton;

    [Header("Hình ảnh Mũi tên")]
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    [Header("Cấu hình Game")]
    public string itemRewardKey = "ITEM_FISH_MEAT";

    private int[] arrowsPerRound = { 5, 6, 7, 8 };
    private float[] timePerRound = { 5.0f, 4.5f, 4.0f, 3.5f };

    private enum ArrowDir { Up, Down, Left, Right }
    private List<ArrowDir> currentSequence = new List<ArrowDir>();
    private List<Image> spawnedArrows = new List<Image>();

    private int currentRound = 0;
    private int currentIndex = 0;
    private float timer;
    private bool isGameActive = false;
    private bool isWaitingNextRound = false;

    private int totalPendingFish = 0;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseAndClaimReward);
    }

    void OnEnable()
    {
        // Khi Panel được bật lên (do tương tác), tự động reset game
        currentRound = 0;
        totalPendingFish = 0;
        if (resultPanel != null) resultPanel.SetActive(false);
        StartRound();
    }

    void StartRound()
    {
        isGameActive = true;
        isWaitingNextRound = false;
        currentIndex = 0;
        timer = (currentRound < timePerRound.Length) ? timePerRound[currentRound] : 3.0f;

        if (containerText != null) containerText.gameObject.SetActive(false);
        arrowContainer.gameObject.SetActive(true);

        foreach (Transform child in arrowContainer)
        {
            if (child.gameObject != arrowTemplate && (containerText == null || child.gameObject != containerText.gameObject))
                Destroy(child.gameObject);
        }
        spawnedArrows.Clear();
        currentSequence.Clear();

        int count = (currentRound < arrowsPerRound.Length) ? arrowsPerRound[currentRound] : 8;

        for (int i = 0; i < count; i++)
        {
            ArrowDir randomDir = (ArrowDir)Random.Range(0, 4);
            currentSequence.Add(randomDir);
            CreateArrowUI(randomDir);
        }
    }

    void CreateArrowUI(ArrowDir dir)
    {
        GameObject newArrow = Instantiate(arrowTemplate, arrowContainer);
        newArrow.SetActive(true);
        Image img = newArrow.GetComponent<Image>();
        img.color = Color.white;

        switch (dir)
        {
            case ArrowDir.Up: img.sprite = spriteUp; break;
            case ArrowDir.Down: img.sprite = spriteDown; break;
            case ArrowDir.Left: img.sprite = spriteLeft; break;
            case ArrowDir.Right: img.sprite = spriteRight; break;
        }
        newArrow.transform.rotation = Quaternion.identity;
        spawnedArrows.Add(img);
    }

    void Update()
    {
        // PlayerG2 sẽ chịu trách nhiệm tắt cái Panel này nếu bấm ESC.

        if (!isGameActive || isWaitingNextRound) return;

        timer -= Time.deltaTime;
        float maxTime = (currentRound < timePerRound.Length) ? timePerRound[currentRound] : 3.0f;
        if (timeSlider != null) timeSlider.value = timer / maxTime;

        if (timer <= 0)
        {
            // [VIỆT HÓA] Khi hết giờ
            ShowTextInContainer("Con gà!!!", Color.red);
            StartCoroutine(FinishRoundDelay(false));
            return;
        }

        if (Input.anyKeyDown)
        {
            ArrowDir? inputDir = null;
            if (Input.GetKeyDown(KeyCode.UpArrow)) inputDir = ArrowDir.Up;
            else if (Input.GetKeyDown(KeyCode.DownArrow)) inputDir = ArrowDir.Down;
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) inputDir = ArrowDir.Left;
            else if (Input.GetKeyDown(KeyCode.RightArrow)) inputDir = ArrowDir.Right;

            if (inputDir == null) return;

            if (inputDir == currentSequence[currentIndex])
            {
                spawnedArrows[currentIndex].color = new Color(0.5f, 0.5f, 0.5f, 1f);
                currentIndex++;

                if (currentIndex >= currentSequence.Count)
                {
                    // [VIỆT HÓA] Khi bấm đúng hết
                    ShowTextInContainer("Tuyệt vời ông mặt trời!!!", Color.green);
                    StartCoroutine(FinishRoundDelay(true));
                }
            }
            else
            {
                ResetCurrentRoundProgress();
            }
        }
    }

    void ResetCurrentRoundProgress()
    {
        currentIndex = 0;
        foreach (var img in spawnedArrows) img.color = Color.white;
    }

    void ShowTextInContainer(string text, Color color)
    {
        foreach (var img in spawnedArrows) img.gameObject.SetActive(false);
        if (containerText != null)
        {
            containerText.text = text;
            containerText.color = color;
            containerText.gameObject.SetActive(true);
        }
    }

    IEnumerator FinishRoundDelay(bool isSuccess)
    {
        isGameActive = false;
        isWaitingNextRound = true;

        if (isSuccess)
        {
            int reward = currentRound + 1;
            totalPendingFish += reward;
        }

        yield return new WaitForSeconds(1.0f);

        currentRound++;
        if (currentRound < arrowsPerRound.Length) StartRound();
        else ShowResultPanel();
    }

    void ShowResultPanel()
    {
        isGameActive = false;
        if (containerText != null) containerText.gameObject.SetActive(false);

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            if (rewardAmountText != null)
            {
                // [VIỆT HÓA] Bảng tổng kết
                rewardAmountText.text = "TỔNG SỐ CÁ BẮT ĐƯỢC:\n<size=150%>" + totalPendingFish + "</size>";
            }
        }
    }

    public void CloseAndClaimReward()
    {
        if (totalPendingFish > 0 && InventoryManager.instance != null)
        {
            InventoryManager.instance.AddItem(itemRewardKey, totalPendingFish);
        }

        gameObject.SetActive(false);
        // Mở khóa Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerG2 p = player.GetComponent<PlayerG2>();
            if (p != null) p.isLocked = false;
        }
    }
}