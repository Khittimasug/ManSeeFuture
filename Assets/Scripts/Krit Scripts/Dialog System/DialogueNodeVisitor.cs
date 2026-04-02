using UnityEngine;

/// <summary>
/// [Interface] สำหรับระบบ Visitor Pattern
/// ทำหน้าที่เป็น "แบบฟอร์ม" บังคับให้สคริปต์ใดก็ตามที่ต้องการอ่านข้อมูลจาก Dialogue Node 
/// (เช่น UI Controller) ต้องมีฟังก์ชันจัดการ Node ทั้ง 2 ประเภทนี้เสมอ
/// </summary>
public interface DialogueNodeVisitor
{
    /// <summary> 
    /// ฟังก์ชันจัดการเมื่อระบบพบว่าเป็น Node บทสนทนาปกติ (Basic) 
    /// </summary>
    void Visit(BasicDialogueNode node);

    /// <summary> 
    /// ฟังก์ชันจัดการเมื่อระบบพบว่าเป็น Node ทางเลือก (Choice) 
    /// </summary>
    void Visit(ChoiceDialogueNode node);
}