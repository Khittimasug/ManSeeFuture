using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// [Custom Exception] คลาสสำหรับแจ้งเตือนข้อผิดพลาดเฉพาะที่เกิดกับระบบ Dialogue
/// ช่วยให้ Debug ได้ง่ายขึ้นเมื่อมีการสั่งรันบทสนทนาผิดขั้นตอน
/// </summary>
public class DialogueException : System.Exception
{
    public DialogueException(string message) : base(message) { }
}

/// <summary>
/// [Core Engine] ตัวควบคุมลำดับเหตุการณ์ (Sequencer) ของบทสนทนา
/// ทำหน้าที่จัดการสถานะ "เริ่ม-เปลี่ยน-จบ" ของ Dialogue และ Node ต่างๆ
/// </summary>
public class DialogueSequencer
{
    // Delegate สำหรับสร้าง Event ให้สคริปต์อื่นมาลงทะเบียนดักฟังได้
    public delegate void DialogueCallback(Dialogue dialogue);
    public delegate void DialogueNodeCallback(DialogueNode node);

    // Event ต่างๆ ที่จะยิงออกไปเมื่อสถานะเปลี่ยน (เช่น เพื่อให้ UI แสดงผล หรือ VideoManager ทำงาน)
    public DialogueCallback OnDialogueStart;      // เมื่อเริ่มบทสนทนาใหญ่
    public DialogueCallback OnDialogueEnd;        // เมื่อจบเข้าสู่สถานะปกติ
    public DialogueNodeCallback OnDialogueNodeStart; // เมื่อข้อความใหม่เริ่มขึ้น
    public DialogueNodeCallback OnDialogueNodeEnd;   // เมื่อข้อความเก่าถูกปิดลง

    private Dialogue m_CurrentDialogue; // เก็บข้อมูลบทสนทนาที่กำลังเล่นอยู่ปัจจุบัน
    private DialogueNode m_CurrentNode; // เก็บข้อมูล Node ปัจจุบันที่แสดงผลอยู่

    /// <summary>
    /// เริ่มต้นการสนทนาใหม่
    /// </summary>
    public void StartDialogue(Dialogue dialogue)
    {
        // ป้องกันการรันบทสนทนาซ้อนกัน
        if (m_CurrentDialogue == null)
        {
            m_CurrentDialogue = dialogue;
            OnDialogueStart?.Invoke(m_CurrentDialogue); // แจ้งเตือนทุกระบบว่า "เริ่มคุยแล้วนะ"
            StartDialogueNode(dialogue.FirstNode);      // ไปสั่งรัน Node แรกสุดของบทนั้น
        }
        else
        {
            throw new DialogueException("ไม่สามารถเริ่มบทสนทนาใหม่ได้ในขณะที่บทเดิมยังเล่นไม่จบ");
        }
    }

    /// <summary>
    /// จบการสนทนาและล้างสถานะทั้งหมด
    /// </summary>
    public void EndDialogue(Dialogue dialogue)
    {
        if (m_CurrentDialogue == dialogue)
        {
            StopDialogueNode(m_CurrentNode); // สั่งปิด Node สุดท้ายที่ค้างอยู่
            OnDialogueEnd?.Invoke(m_CurrentDialogue); // แจ้งเตือนทุกระบบว่า "คุยจบแล้ว"
            m_CurrentDialogue = null; // คืนค่าว่างเพื่อให้เริ่มบทสนทนาอื่นได้ในอนาคต
        }
        else
        {
            throw new DialogueException("พยายามจะสั่งจบบทสนทนาที่ไม่ได้กำลังเล่นอยู่");
        }
    }

    /// <summary>
    /// ตรวจสอบความถูกต้องว่า Node ถัดไปสามารถเล่นต่อจาก Node ปัจจุบันได้หรือไม่
    /// </summary>
    private bool CanStartNode(DialogueNode node)
    {
        // ปัจจุบันตั้งค่าเป็น true ไว้เสมอเพื่อให้ระบบไหลลื่น 
        // แต่สามารถเปิดใช้งาน Logic การเช็คความเชื่อมต่อที่ Comment ไว้ได้
        return true;
    }

    /// <summary>
    /// เปลี่ยนหน้าบทสนทนาไปยัง Node ถัดไป
    /// </summary>
    public void StartDialogueNode(DialogueNode node)
    {
        if (CanStartNode(node))
        {
            // 1. ปิด Node เก่าก่อน
            StopDialogueNode(m_CurrentNode);

            m_CurrentNode = node;

            // 2. ถ้า Node ใหม่มีข้อมูล (ไม่เป็น null) ให้เริ่มทำงาน
            if (m_CurrentNode != null)
            {
                OnDialogueNodeStart?.Invoke(m_CurrentNode);
            }
            else
            {
                // 3. ถ้าส่งค่า null มา หมายความว่าถึงจุดจบของบทสนทนาแล้ว ให้สั่ง EndDialogue
                EndDialogue(m_CurrentDialogue);
            }
        }
        else
        {
            throw new DialogueException("เกิดข้อผิดพลาด: ไม่สามารถข้ามไปยัง Node ที่ระบุได้");
        }
    }

    /// <summary>
    /// ฟังก์ชันภายในสำหรับปิดสถานะของ Node ปัจจุบัน
    /// </summary>
    private void StopDialogueNode(DialogueNode node)
    {
        if (m_CurrentNode == node)
        {
            if (m_CurrentNode != null)
            {
                OnDialogueNodeEnd?.Invoke(m_CurrentNode); // ยิง Event แจ้ง UI ให้ลบปุ่ม/ล้างข้อความ
            }
            m_CurrentNode = null;
        }
        else if (node != null)
        {
            throw new DialogueException("พยายามจะสั่งหยุด Node ที่ไม่ได้กำลังรันอยู่");
        }
    }
}