using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// รูปแบบเงื่อนไขสำหรับการปลดล็อกตัวเลือก (Lock/Unlock)
/// </summary>
public enum PointCondition
{
    None,                   // ไม่มีเงื่อนไข (เลือกได้เสมอ)
    RequireMoreThanOrEqual, // ต้องมีแต้ม "มากกว่าหรือเท่ากับ" ที่กำหนด
    RequireLessThanOrEqual  // ต้องมีแต้ม "น้อยกว่าหรือเท่ากับ" ที่กำหนด
}

/// <summary>
/// [Data Structure] คลาสสำหรับเก็บข้อมูลของ "หนึ่งตัวเลือก" 
/// ที่จะปรากฏบนปุ่มทางเลือกในเกม
/// </summary>
[Serializable]
public class DialogueChoice
{
    [Header("Point Settings (When Clicked)")]
    public CharacterTarget PointTarget; // เลือกตัวละครที่จะได้รับแต้ม (None, Biw, Josephine)
    public int PointAmount;             // แต้มที่จะเพิ่มหรือลดเมื่อกดเลือก

    // --- ส่วนที่เพิ่มเข้ามาใหม่: ระบบ Requirement (เงื่อนไขการปลดล็อก) ---
    [Header("Requirement Settings (Lock/Unlock)")]
    public CharacterTarget RequiredTarget;  // ตัวละครที่ต้องการเช็คแต้ม
    public PointCondition Condition;        // รูปแบบเงื่อนไข
    public int RequiredAmount;              // จำนวนแต้มที่ต้องการเช็ค

    [Header("Choice Display & Flow")]
    [SerializeField]
    private string m_ChoicePreview; // ข้อความสั้นๆ ที่จะแสดงบนปุ่ม

    [SerializeField]
    private DialogueNode m_ChoiceNode; // Node ถัดไปที่จะเล่น

    [SerializeField]
    private int m_VideoIndex; // วิดีโอตัวที่จะเล่นเมื่อเลือกข้อนี้

    // Getter
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
    private DialogueChoice[] m_Choices;

    public DialogueChoice[] Choices => m_Choices;

    public override bool CanBeFollowedByNode(DialogueNode node)
    {
        return m_Choices.Any(x => x.ChoiceNode == node);
    }

    public override void Accept(DialogueNodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}