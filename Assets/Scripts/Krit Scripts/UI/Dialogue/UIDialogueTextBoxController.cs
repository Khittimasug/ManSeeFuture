using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [UI Controller] ทำหน้าที่ควบคุมการแสดงผลกล่องข้อความ (Text Box) 
/// และการสร้างปุ่มทางเลือก (Choices) บนหน้าจอ โดยทำงานผ่านระบบ Visitor Pattern
/// </summary>
public class UIDialogueTextBoxController : MonoBehaviour, DialogueNodeVisitor
{
    [Header("UI Elements")]
    [SerializeField] protected TextMeshProUGUI m_SpeakerText;  // ข้อความชื่อตัวละครที่กำลังพูด
    [SerializeField] protected TextMeshProUGUI m_DialogueText; // ข้อความเนื้อหาบทสนทนา

    [Header("Choices Settings")]
    [SerializeField] protected RectTransform m_ChoicesBoxTransform; // Container (Panel) สำหรับเก็บปุ่มทางเลือก
    [SerializeField] protected UIDialogueChoiceController m_ChoiceControllerPrefab; // Prefab ของปุ่มทางเลือก

    [Header("Communication")]
    [SerializeField] protected DialogueChannel m_DialogueChannel; // ช่องทางรับ-ส่งสัญญาณของระบบ Dialogue

    protected bool m_ListenToInput = false; // ตัวแปรเช็คว่า "อนุญาตให้กดข้าม/ไปต่อ" ด้วยคีย์บอร์ดได้หรือไม่
    protected DialogueNode m_NextNode = null; // เก็บข้อมูล Node ถัดไปที่จะเล่น

    protected virtual void Awake()
    {
        // ลงทะเบียน Event: เมื่อเริ่ม Node ใหม่ และ เมื่อจบ Node ให้รันฟังก์ชันที่กำหนด
        m_DialogueChannel.OnDialogueNodeStart += OnDialogueNodeStart;
        m_DialogueChannel.OnDialogueNodeEnd += OnDialogueNodeEnd;

        // เริ่มต้นเกม: ซ่อนตัวเองและกล่องทางเลือกไว้ก่อน
        gameObject.SetActive(false);
        m_ChoicesBoxTransform.gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        // ยกเลิกการลงทะเบียน Event เมื่อ Object นี้ถูกทำลาย (ป้องกัน Memory Leak)
        m_DialogueChannel.OnDialogueNodeEnd -= OnDialogueNodeEnd;
        m_DialogueChannel.OnDialogueNodeStart -= OnDialogueNodeStart;
    }

    protected virtual void Update()
    {
        // ถ้าอยู่ในสถานะรอ Input และผู้เล่นกดปุ่มตกลง (เช่น Enter/Space/คลิก)
        if (m_ListenToInput && Input.GetButtonDown("Submit"))
        {
            // ส่งคำสั่งขอเริ่มเล่น Node ถัดไป (ถ้า m_NextNode เป็น null ระบบจะจบการสนทนา)
            m_DialogueChannel.RaiseRequestDialogueNode(m_NextNode);
        }
    }

    /// <summary>
    /// [Event Handler] ทำงานเมื่อมีสัญญาณเริ่ม Node ใหม่
    /// </summary>
    protected virtual void OnDialogueNodeStart(DialogueNode node)
    {
        gameObject.SetActive(true); // แสดงกล่องข้อความ

        // ดึงข้อมูลชื่อและบทพูดมาแสดงผล
        m_DialogueText.text = node.DialogueLine.Text;
        m_SpeakerText.text = node.DialogueLine.Speaker.CharacterName;

        // ใช้ Visitor Pattern: ให้ Node เป็นตัวบอกว่ามันคือ Node ประเภทไหน (Basic หรือ Choice)
        // เพื่อให้ฟังก์ชัน Visit ด้านล่างทำงานตามประเภทนั้นๆ
        node.Accept(this);
    }

    /// <summary>
    /// [Event Handler] ทำงานเมื่อมีสัญญาณจบ Node หรือจบการสนทนา
    /// </summary>
    protected virtual void OnDialogueNodeEnd(DialogueNode node)
    {
        // ล้างค่าสถานะต่างๆ
        m_NextNode = null;
        m_ListenToInput = false;
        m_DialogueText.text = "";
        m_SpeakerText.text = "";

        // ทำลายปุ่มทางเลือกทั้งหมดที่เคยสร้างไว้ใน Choice Box
        foreach (Transform child in m_ChoicesBoxTransform)
        {
            Destroy(child.gameObject);
        }

        // ซ่อน UI ทั้งหมดออกจากหน้าจอ
        gameObject.SetActive(false);
        m_ChoicesBoxTransform.gameObject.SetActive(false);
    }

    /// <summary>
    /// กรณีที่เป็น Node บทสนทนาปกติ (BasicDialogueNode)
    /// </summary>
    public virtual void Visit(BasicDialogueNode node)
    {
        m_ListenToInput = true;   // อนุญาตให้กดปุ่มเพื่อไปต่อได้
        m_NextNode = node.NextNode; // เตรียมข้อมูล Node ถัดไป
    }

    /// <summary>
    /// กรณีที่เป็น Node ที่มีทางเลือก (ChoiceDialogueNode)
    /// </summary>
    public virtual void Visit(ChoiceDialogueNode node)
    {
        m_ChoicesBoxTransform.gameObject.SetActive(true); // แสดง Panel สำหรับเก็บปุ่ม

        // วนลูปสร้างปุ่มตามจำนวนทางเลือก (Choices) ที่มีใน Node นั้นๆ
        foreach (DialogueChoice choice in node.Choices)
        {
            // สร้างปุ่ม (Instantiate) จาก Prefab เข้าไปใน Choice Box
            UIDialogueChoiceController newChoice = Instantiate(m_ChoiceControllerPrefab, m_ChoicesBoxTransform);

            // ส่งข้อมูลทางเลือกไปให้ปุ่มจัดการต่อ (เช่น ข้อความบนปุ่ม, วิดีโอที่จะเล่น)
            newChoice.Choice = choice;
        }

        // หมายเหตุ: ในประเภท Choice เราจะไม่เซ็ต m_ListenToInput = true 
        // เพราะเราต้องการให้ผู้เล่น "คลิกปุ่มบนจอ" แทนการกดปุ่ม Submit ที่คีย์บอร์ด
    }
}