using UnityEngine;

/// <summary>
/// [Scriptable Object] ทำหน้าที่เป็น "ช่องสัญญาณควบคุมจังหวะเกม" (Game Flow Control)
/// ใช้สำหรับส่งสัญญาณเพื่อเปลี่ยนสถานะหลักของเกม (Game States) 
/// เช่น เปลี่ยนจากโหมด "เดินเล่น (Gameplay)" ไปเป็น "บทสนทนา (Dialogue)"
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Flow/Flow Channel")]
public class FlowChannel : ScriptableObject
{
    // Delegate สำหรับสร้าง Event ในการรับ-ส่งข้อมูลสถานะ (FlowState)
    public delegate void FlowStateCallback(FlowState state);

    // Event เมื่อมีระบบใดระบบหนึ่ง "ร้องขอ" ให้เปลี่ยนสถานะเกม
    public FlowStateCallback OnFlowStateRequested;

    // Event เมื่อสถานะเกม "ถูกเปลี่ยนสำเร็จแล้ว" (แจ้งให้ระบบอื่นทราบเพื่อปรับตัว)
    public FlowStateCallback OnFlowStateChanged;

    /// <summary>
    /// ฟังก์ชันสำหรับ "ส่งคำขอ" เปลี่ยนสถานะเกม
    /// เช่น เมื่อเริ่มคุย ให้เรียกฟังก์ชันนี้เพื่อขอเปลี่ยนเป็น DialogueState
    /// </summary>
    public void RaiseFlowStateRequest(FlowState state)
    {
        OnFlowStateRequested?.Invoke(state);
    }

    /// <summary>
    /// ฟังก์ชันสำหรับ "ประกาศ" ว่าสถานะเกมเปลี่ยนไปแล้วจริงๆ
    /// เช่น ระบบกล้องอาจจะรอฟังสัญญาณนี้เพื่อเปลี่ยนมุมกล้องตามสถานะใหม่
    /// </summary>
    public void RaiseFlowStateChanged(FlowState state)
    {
        OnFlowStateChanged?.Invoke(state);
    }
}