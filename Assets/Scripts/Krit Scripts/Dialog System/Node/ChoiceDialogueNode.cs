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
    public CharacterTarget PointTarget; // เลือกตัวละครที่จะได้รับแต้ม (อ้างอิงจาก CharacterPointManager)
    public int PointAmount;             // แต้มที่จะเพิ่มหรือลดเมื่อกดเลือก

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

    /// <summary>
    /// ฟังก์ชันเช็คว่าตัวเลือกนี้ถูกปลดล็อกหรือไม่ 
    /// โดยดึงค่าแต้มปัจจุบันจาก CharacterPointManager มาคำนวณโดยตรง
    /// </summary>
    public bool IsUnlocked()
    {
        // 1. ถ้าไม่มีเงื่อนไข ให้ผ่านเสมอ
        if (Condition == PointCondition.None) return true;

        // 2. ดึงแต้มปัจจุบันของตัวละครที่ต้องการเช็ค
        int currentPoints = 0;
        if (RequiredTarget == CharacterTarget.Biw)
        {
            currentPoints = CharacterPointManager.Instance.biwPoints;
        }
        else if (RequiredTarget == CharacterTarget.Josephine)
        {
            currentPoints = CharacterPointManager.Instance.josephinePoints;
        }

        // 3. ตรวจสอบเงื่อนไข
        switch (Condition)
        {
            case PointCondition.RequireMoreThanOrEqual:
                return currentPoints >= RequiredAmount;

            case PointCondition.RequireLessThanOrEqual:
                return currentPoints <= RequiredAmount;

            default:
                return true;
        }
    }
}

/// <summary>
/// [Scriptable Object] ไฟล์ Asset ที่ใช้สร้าง Node ทางเลือกใน Project Window
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