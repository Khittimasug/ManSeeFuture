using UnityEngine;

/// <summary>
/// [Abstract Base Class] คลาสแม่ที่เป็นต้นแบบของ Node บทสนทนาทุกประเภทในเกม
/// เนื่องจากเป็น abstract จึงไม่สามารถสร้างเป็นไฟล์ Asset ได้โดยตรง 
/// แต่จะเป็นฐานให้ BasicDialogueNode และ ChoiceDialogueNode นำไปใช้งานต่อ
/// </summary>
public abstract class DialogueNode : ScriptableObject
{
    [SerializeField]
    private NarrationLine m_DialogueLine; // ข้อมูลบทพูดประจำ Node นี้ (ใครพูด, พูดอะไร, มี Event อะไร)

    /* * Property (Getter) สำหรับให้ UI Controller 
     * เข้ามาดึงข้อมูล NarrationLine ไปแสดงผลบนหน้าจอ
     */
    public NarrationLine DialogueLine => m_DialogueLine;

    /// <summary>
    /// [Abstract Method] บังคับให้ Node ลูกทุกตัวต้องเขียนวิธีตรวจสอบความถูกต้อง
    /// ว่าสามารถเชื่อมต่อไปยัง Node อื่นที่ระบุได้หรือไม่
    /// </summary>
    public abstract bool CanBeFollowedByNode(DialogueNode node);

    /// <summary>
    /// [Visitor Pattern] หัวใจสำคัญของการแยกประเภท Node
    /// บังคับให้ Node ลูกต้องยอมรับ Visitor (UI Controller) เข้ามาดึงข้อมูล
    /// เพื่อให้ UI รู้ว่าต้องแสดงผลแบบปกติ หรือแสดงผลแบบปุ่มทางเลือก
    /// </summary>
    public abstract void Accept(DialogueNodeVisitor visitor);
}