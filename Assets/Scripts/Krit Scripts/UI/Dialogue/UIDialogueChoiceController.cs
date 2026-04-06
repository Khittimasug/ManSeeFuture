using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [Controller] สำหรับควบคุมปุ่มทางเลือก (Choice Button)
/// </summary>
public class UIDialogueChoiceController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Choice;
    [SerializeField]
    private DialogueChannel m_DialogueChannel;

    private DialogueNode m_ChoiceNextNode;
    private int m_VideoIndex;
    private VideoSequenceManager m_VideoManager;

    // เก็บข้อมูลแต้ม
    private CharacterTarget m_PointTarget;
    private int m_PointAmount;

    public DialogueChoice Choice
    {
        set
        {
            // 1. เช็คว่าแต้มถึงเงื่อนไขที่กำหนดหรือไม่
            bool isUnlocked = CheckCondition(value);

            Button buttonComponent = GetComponent<Button>();

            if (isUnlocked)
            {
                // ถ้าแต้มถึง: แสดงข้อความปกติ และเปิดให้กดปุ่มได้
                m_Choice.text = value.ChoicePreview;
                buttonComponent.interactable = true;
            }
            else
            {
                // ถ้าแต้มไม่ถึง: ปิดปุ่ม และอาจจะเปลี่ยนข้อความให้ดูรู้ว่าล็อคอยู่ (เปลี่ยนสีข้อความเป็นสีเทา)
                m_Choice.text = $"[Locked] {value.ChoicePreview}";
                buttonComponent.interactable = false;
            }

            m_ChoiceNextNode = value.ChoiceNode;
            m_VideoIndex = value.VideoIndex;

            // ข้อมูลสำหรับตอนกดปุ่ม
            m_PointTarget = value.PointTarget;
            m_PointAmount = value.PointAmount;
        }
    }

    /// <summary>
    /// ฟังก์ชันเช็คว่าผู้เล่นมีแต้มความสัมพันธ์ตรงตามเงื่อนไข (Requirement) ของ Choice นี้หรือไม่
    /// </summary>
    private bool CheckCondition(DialogueChoice choiceData)
    {
        // ถ้าไม่มีเงื่อนไข ให้ถือว่าผ่าน (Unlocked)
        if (choiceData.Condition == PointCondition.None || choiceData.RequiredTarget == CharacterTarget.None)
            return true;

        if (CharacterPointManager.Instance == null)
            return true; // ป้องกัน Error หากในฉากไม่มี Manager

        // ดึงแต้มปัจจุบันของตัวละครที่ต้องการเช็ค
        int currentPoints = 0;
        if (choiceData.RequiredTarget == CharacterTarget.Biw)
            currentPoints = CharacterPointManager.Instance.biwPoints;
        else if (choiceData.RequiredTarget == CharacterTarget.Josephine)
            currentPoints = CharacterPointManager.Instance.josephinePoints;

        // เช็คเงื่อนไข
        if (choiceData.Condition == PointCondition.RequireMoreThanOrEqual)
        {
            // ต้องการแต้ม มากกว่าหรือเท่ากับ
            return currentPoints >= choiceData.RequiredAmount;
        }
        else if (choiceData.Condition == PointCondition.RequireLessThanOrEqual)
        {
            // ต้องการแต้ม น้อยกว่าหรือเท่ากับ (เช่น กรณีอยากให้แต้มติดลบ -10 เป็นต้นไป ถึงจะเลือกได้)
            return currentPoints <= choiceData.RequiredAmount;
        }

        return true;
    }

    public void SetupManager(VideoSequenceManager manager)
    {
        m_VideoManager = manager;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);

        if (m_VideoManager == null)
        {
            m_VideoManager = FindFirstObjectByType<VideoSequenceManager>();
        }
    }

    private void OnClick()
    {
        // เช็คเผื่อไว้ (จริงๆ ถ้าปุ่มโดนตั้ง interactable = false ผู้เล่นจะกดไม่ได้อยู่แล้ว)
        if (!GetComponent<Button>().interactable) return;

        // ให้แต้ม/ลดแต้ม ตามที่เซ็ตไว้
        if (m_PointTarget != CharacterTarget.None && m_PointAmount != 0)
        {
            if (CharacterPointManager.Instance != null)
                CharacterPointManager.Instance.ModifyPoints(m_PointTarget, m_PointAmount);
        }

        // เล่นวิดีโอ
        if (m_VideoManager != null)
        {
            m_VideoManager.PlaySegment(m_VideoIndex);
        }

        // ไปต่อ/ปิดหน้าต่าง Dialogue
        if (m_DialogueChannel != null)
        {
            m_DialogueChannel.RaiseRequestDialogueNode(null);
        }
    }
}