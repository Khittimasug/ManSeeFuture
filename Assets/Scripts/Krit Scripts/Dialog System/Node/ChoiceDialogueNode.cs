using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// [Data Structure] คลาสสำหรับเก็บข้อมูลของ "หนึ่งตัวเลือก" 
/// ที่จะปรากฏบนปุ่มทางเลือกในเกม
/// </summary>
[Serializable]
public class DialogueChoice
{
    [SerializeField]
    private string m_ChoicePreview; // ข้อความสั้นๆ ที่จะแสดงบนปุ่ม (เช่น "ไปทางซ้าย" หรือ "สู้ต่อ")

    [SerializeField]
    private DialogueNode m_ChoiceNode; // Node ถัดไปที่จะเล่น (ถ้ามี) หลังจากเลือกข้อนี้

    /* * ส่วนสำคัญ: ใช้ระบุว่าเมื่อคลิก Choice นี้แล้ว 
     * จะให้ VideoSequenceManager เปลี่ยนไปเล่นวิดีโอตัวที่เท่าไหร่ใน Playlist
     */
    [SerializeField]
    private int m_VideoIndex;

    // Getter (Public Properties): อนุญาตให้สคริปต์อื่น (เช่น Manager หรือ UI) เข้ามาอ่านค่าได้เท่านั้น
    public string ChoicePreview => m_ChoicePreview;
    public DialogueNode ChoiceNode => m_ChoiceNode;
    public int VideoIndex => m_VideoIndex;
}

/// <summary>
/// [Scriptable Object] ไฟล์ Asset ที่ใช้สร้าง Node ทางเลือกใน Project Window
/// ทำหน้าที่เป็น "จุดแยก" ของเนื้อเรื่อง (Branching Point)
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Narration/Dialogue/Node/Choice")]
public class ChoiceDialogueNode : DialogueNode
{
    [SerializeField]
    private DialogueChoice[] m_Choices; // รายการทางเลือกทั้งหมดที่มีใน Node นี้ (แสดงผลเป็น List ใน Inspector)

    public DialogueChoice[] Choices => m_Choices;

    /// <summary>
    /// ฟังก์ชันตรวจสอบความถูกต้องของลำดับ (Sequence Validation)
    /// ใช้เช็คว่า Node ที่ระบบพยายามจะรันต่อ เป็นหนึ่งในลูกที่ถูกต้องของ Node นี้หรือไม่
    /// </summary>
    public override bool CanBeFollowedByNode(DialogueNode node)
    {
        // ใช้ LINQ เพื่อค้นหาว่ามีทางเลือกใดในลิสต์ที่มี ChoiceNode ตรงกับที่ส่งมาหรือไม่
        return m_Choices.Any(x => x.ChoiceNode == node);
    }

    /// <summary>
    /// ส่วนหนึ่งของ Visitor Pattern
    /// ส่งตัวเองไปให้ Visitor (เช่น UI Controller) จัดการแสดงผลตามประเภทของ Choice Node
    /// </summary>
    public override void Accept(DialogueNodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}