using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogueChoiceController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Choice;
    [SerializeField]
    private DialogueChannel m_DialogueChannel;

    private DialogueNode m_ChoiceNextNode;
    private int m_VideoIndex; // เพิ่มเพื่อเก็บลำดับวิดีโอ
    private VideoSequenceManager m_VideoManager; // เพิ่มเพื่ออ้างอิงถึงตัวจัดการวิดีโอ

    public DialogueChoice Choice
    {
        set
        {
            m_Choice.text = value.ChoicePreview;
            m_ChoiceNextNode = value.ChoiceNode;
            m_VideoIndex = value.VideoIndex; // รับค่า Index วิดีโอมาจาก DialogueChoice
        }
    }

    // ฟังก์ชันสำหรับให้ Manager ส่งตัวเองมาให้ปุ่มรู้จัก
    public void SetupManager(VideoSequenceManager manager)
    {
        m_VideoManager = manager;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        // หา Manager ในฉากโดยอัตโนมัติ
        m_VideoManager = FindFirstObjectByType<VideoSequenceManager>(); 
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // 1. เล่นวิดีโอที่กำหนดไว้ใน Choice (จากที่เราทำกันไว้ก่อนหน้า)
        if (m_VideoManager != null)
        {
            m_VideoManager.PlaySegment(m_VideoIndex);
        }

        // 2. ส่งค่า null เพื่อจบบทสนทนาทันที
        // การส่ง null จะทำให้ DialogueSequencer เรียก EndDialogue และปิด UI ทั้งหมด
        m_DialogueChannel.RaiseRequestDialogueNode(null);

        //Debug.Log($"พยายามเปลี่ยนไปเล่น Node: {m_ChoiceNextNode.name}");

        //// เรียกคำสั่งเปลี่ยน Node
        //m_DialogueChannel.RaiseRequestDialogueNode(m_ChoiceNextNode);

        //// เล่นวิดีโอ
        //if (m_VideoManager != null)
        //{
        //    m_VideoManager.PlaySegment(m_VideoIndex);
        //}
    }
}