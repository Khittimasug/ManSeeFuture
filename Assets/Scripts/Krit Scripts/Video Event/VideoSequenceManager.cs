using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.UI; // [เพิ่ม] เรียกใช้งาน UI ทั่วไป (เช่น Image)
using TMPro;

/// <summary>
/// โครงสร้างสำหรับเก็บข้อมูลตัวเลือก QTE แต่ละปุ่ม
/// </summary>
[System.Serializable]
public class QTEAction
{
    public string actionName;
    public KeyCode requiredKey;
    public int successVideoIndex;

    [Header("UI Settings")]
    public TextMeshProUGUI actionTextUI;
}

/// <summary>
/// [Data Structure] สำหรับเก็บข้อมูลวิดีโอแต่ละช่วง
/// </summary>
[System.Serializable]
public class VideoSegment
{
    public string segmentName;
    public VideoClip clip;

    [Header("Normal Choice Settings")]
    public bool hasChoice;

    [Header("QTE Settings")]
    public bool hasQTE;
    public float qteStartTime;
    public float qteDuration;
    public List<QTEAction> qteActions;
    public int qteFailVideoIndex;

    [Tooltip("เหตุการณ์ที่จะเกิดขึ้นเมื่อจบวิดีโอ")]
    public UnityEvent onSegmentEnd;
}

public class VideoSequenceManager : MonoBehaviour
{
    [Header("Components Settings")]
    public VideoPlayer videoPlayer;
    public GameObject choicePanel;

    [Header("QTE UI Settings")]
    public GameObject qtePanel;
    public Image qteTimerImage;     // [เพิ่ม] ตัวแปรอ้างอิงถึง UI Image ที่เป็นหลอดเวลา

    [Header("Video Playlist")]
    public List<VideoSegment> playlist;

    private int currentIndex = 0;
    private bool isQTEActive = false;

    [SerializeField] private float qteTimer = 0f;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;

        if (choicePanel != null) choicePanel.SetActive(false);
        if (qtePanel != null) qtePanel.SetActive(false);

        PlaySegment(0);
    }

    void Update()
    {
        if (!videoPlayer.isPlaying && !isQTEActive) return;

        VideoSegment currentSegment = playlist[currentIndex];

        // ----------------------------------------------------
        // [LOGIC 1: ระบบจัดการ QTE]
        // ----------------------------------------------------
        if (currentSegment.hasQTE)
        {
            if (!isQTEActive && videoPlayer.time >= currentSegment.qteStartTime)
            {
                StartQTE();
                videoPlayer.Pause();
            }

            if (isQTEActive)
            {
                qteTimer -= Time.deltaTime;

                // --- [เพิ่ม] อัปเดตหลอดเวลา (Fill Amount) ---
                if (qteTimerImage != null)
                {
                    // เอาเวลาที่เหลือ หารด้วย เวลาทั้งหมด จะได้สัดส่วนเป็น 0.0 - 1.0 พอดี
                    qteTimerImage.fillAmount = qteTimer / currentSegment.qteDuration;
                }

                foreach (var action in currentSegment.qteActions)
                {
                    if (Input.GetKeyDown(action.requiredKey))
                    {
                        Debug.Log($"[QTE] Player pressed {action.requiredKey}! Executing: {action.actionName}");
                        EndQTE(action.successVideoIndex);
                        return;
                    }
                }

                if (qteTimer <= 0f)
                {
                    Debug.Log("[QTE] Time out! Player failed the QTE.");
                    EndQTE(currentSegment.qteFailVideoIndex);
                    return;
                }
            }
        }

        // ----------------------------------------------------
        // [LOGIC 2: การตรวจหาทางเลือกล่วงหน้าตอนจบ]
        // ----------------------------------------------------
        if (currentSegment.hasChoice && videoPlayer.time > 0.5f && !isQTEActive)
        {
            double timeBeforeEnd = videoPlayer.length - videoPlayer.time;

            if (timeBeforeEnd <= 0.1f)
            {
                videoPlayer.Pause();

                if (!choicePanel.activeSelf)
                {
                    currentSegment.onSegmentEnd?.Invoke();
                }
            }
        }
    }

    // ฟังก์ชันเริ่ม QTE
    private void StartQTE()
    {
        isQTEActive = true;
        qteTimer = playlist[currentIndex].qteDuration;

        // --- [เพิ่ม] รีเซ็ตหลอดเวลาให้เต็ม 100% (1.0) ตอนเริ่ม QTE ---
        if (qteTimerImage != null)
        {
            qteTimerImage.fillAmount = 1f;
        }

        foreach (var action in playlist[currentIndex].qteActions)
        {
            if (action.actionTextUI != null)
            {
                action.actionTextUI.text = $"{action.actionName}";
                action.actionTextUI.gameObject.SetActive(true);
            }
        }

        if (qtePanel != null) qtePanel.SetActive(true);
        Debug.Log("[QTE] Started! Waiting for player input...");
    }

    // ฟังก์ชันจบ QTE และเปลี่ยนวิดีโอ
    private void EndQTE(int targetVideoIndex)
    {
        isQTEActive = false;

        foreach (var action in playlist[currentIndex].qteActions)
        {
            if (action.actionTextUI != null)
            {
                action.actionTextUI.gameObject.SetActive(false);
            }
        }

        if (qtePanel != null) qtePanel.SetActive(false);

        PlaySegment(targetVideoIndex);
    }

    public void PlaySegment(int index)
    {
        if (index >= 0 && index < playlist.Count)
        {
            currentIndex = index;

            if (choicePanel != null) choicePanel.SetActive(false);
            if (qtePanel != null) qtePanel.SetActive(false);
            isQTEActive = false;

            videoPlayer.Stop();
            videoPlayer.clip = playlist[index].clip;
            videoPlayer.Prepare();
            videoPlayer.Play();

            Debug.Log($"[VideoManager] Playing: {playlist[index].segmentName}");
        }
        else
        {
            Debug.LogWarning("[VideoManager] Index out of range");
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        VideoSegment currentSegment = playlist[currentIndex];

        if (!currentSegment.hasChoice && !isQTEActive)
        {
            currentSegment.onSegmentEnd?.Invoke();
            PlayNext();
        }
    }

    public void PlayNext()
    {
        PlaySegment(currentIndex + 1);
    }

    public void OnChoiceSelected(DialogueChoice choice)
    {
        PlaySegment(choice.VideoIndex);
        if (choicePanel != null) choicePanel.SetActive(false);
    }
}