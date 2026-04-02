using System;
using UnityEngine;

/// <summary>
/// [Scriptable Object] ทำหน้าที่เป็น "ศูนย์กลางการสื่อสาร" (Communication Hub)
/// เปรียบเสมือนสถานีวิทยุที่สคริปต์ต่างๆ (UI, Video, Sequencer) มาลงทะเบียนเพื่อ "รับ" หรือ "ส่ง" สัญญาณ
/// ข้อดีคือทำให้ Code แต่ละส่วนแยกจากกันอย่างเด็ดขาด (Decoupling)
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Narration/Dialogue/Dialogue Channel")]
public class DialogueChannel : ScriptableObject
{
    // --- สัญญาณระดับบทสนทนา (Dialogue Level) ---
    public delegate void DialogueCallback(Dialogue dialogue);

    public DialogueCallback OnDialogueRequested; // มีคนส่งคำขอให้เริ่มบทสนทนา (เช่น Player เดินไปคุยกับ NPC)
    public DialogueCallback OnDialogueStart;     // สัญญาณบอกว่าบทสนทนาเริ่มขึ้นจริงแล้ว
    public DialogueCallback OnDialogueEnd;       // สัญญาณบอกว่าบทสนทนาจบลงแล้ว

    // --- สัญญาณระดับหน้าบทพูด (Node Level) ---
    public delegate void DialogueNodeCallback(DialogueNode node);

    public DialogueNodeCallback OnDialogueNodeRequested; // มีคนขอเปลี่ยนหน้าบทพูด (เช่น กดปุ่ม Next หรือเลือก Choice)
    public DialogueNodeCallback OnDialogueNodeStart;     // สัญญาณบอกว่าหน้าบทพูดใหม่เริ่มแสดงผล
    public DialogueNodeCallback OnDialogueNodeEnd;       // สัญญาณบอกว่าหน้าบทพูดเก่าถูกปิดไป

    /*
     * [Broadcasting Methods]
     * ฟังก์ชันเหล่านี้เปรียบเสมือนการ "กดปุ่มส่งสัญญาณ" 
     * สคริปต์ใดก็ตามที่มีไฟล์ Channel นี้อยู่ในมือ สามารถเรียกฟังก์ชันเหล่านี้เพื่อกระจายข่าวได้
     */

    /// <summary> ส่งคำขอเพื่อเริ่มเล่นบทสนทนาที่ระบุ </summary>
    public void RaiseRequestDialogue(Dialogue dialogue)
    {
        OnDialogueRequested?.Invoke(dialogue);
    }

    /// <summary> ประกาศให้ระบบรู้ว่าบทสนทนาเริ่มทำงานแล้ว </summary>
    public void RaiseDialogueStart(Dialogue dialogue)
    {
        OnDialogueStart?.Invoke(dialogue);
    }

    /// <summary> ประกาศให้ระบบรู้ว่าบทสนทนาจบการทำงานแล้ว </summary>
    public void RaiseDialogueEnd(Dialogue dialogue)
    {
        OnDialogueEnd?.Invoke(dialogue);
    }

    /// <summary> 
    /// ส่งคำขอเพื่อเปลี่ยนไปยัง Node ถัดไป 
    /// (ถ้าส่ง null ไป ระบบ Sequencer จะเข้าใจว่าต้องการจบบทสนทนา)
    /// </summary>
    public void RaiseRequestDialogueNode(DialogueNode node)
    {
        OnDialogueNodeRequested?.Invoke(node);
    }

    /// <summary> ประกาศว่าข้อความใน Node นี้เริ่มแสดงผลแล้ว </summary>
    public void RaiseDialogueNodeStart(DialogueNode node)
    {
        OnDialogueNodeStart?.Invoke(node);
    }

    /// <summary> ประกาศว่าข้อความใน Node นี้แสดงผลเสร็จสิ้นแล้ว </summary>
    public void RaiseDialogueNodeEnd(DialogueNode node)
    {
        OnDialogueNodeEnd?.Invoke(node);
    }
}