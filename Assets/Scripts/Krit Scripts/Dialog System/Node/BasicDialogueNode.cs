using UnityEngine;

/// <summary>
/// [Scriptable Object] สำหรับสร้าง Node บทสนทนาแบบพื้นฐาน (เส้นตรง)
/// ใช้ในกรณีที่มีการพูดคุยต่อเนื่องกันโดยไม่มีทางเลือกให้ผู้เล่นกด
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Narration/Dialogue/Node/Basic")]
public class BasicDialogueNode : DialogueNode
{
    [SerializeField]
    private DialogueNode m_NextNode; // เก็บข้อมูล Node ถัดไปที่จะเล่นต่อจาก Node นี้

    /* * Property (Getter) สำหรับให้สคริปต์อื่น (เช่น UIDialogueTextBoxController) 
     * มาดึงค่า Node ถัดไปไปใช้งานเมื่อผู้เล่นกดปุ่ม Submit/Next
     */
    public DialogueNode NextNode => m_NextNode;

    /// <summary>
    /// ฟังก์ชันตรวจสอบความถูกต้องของลำดับการเชื่อมต่อ
    /// ใช้เช็คว่า Node ที่ระบบพยายามจะกระโดดไปหานั้น ตรงกับ m_NextNode ที่ตั้งค่าไว้หรือไม่
    /// </summary>
    /// <param name="node">Node ที่ต้องการตรวจสอบ</param>
    /// <returns>True หากเป็น Node ที่เชื่อมต่อกันไว้ถูกต้อง</returns>
    public override bool CanBeFollowedByNode(DialogueNode node)
    {
        return m_NextNode == node;
    }

    /// <summary>
    /// ส่วนหนึ่งของ Visitor Pattern
    /// เมื่อระบบทำงานมาถึง Node นี้ มันจะส่งตัวเอง (this) ไปให้ Visitor (UI Controller) 
    /// เพื่อให้ Visitor เรียกฟังก์ชัน Visit(BasicDialogueNode node) และแสดงผลข้อความตามปกติ
    /// </summary>
    public override void Accept(DialogueNodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}