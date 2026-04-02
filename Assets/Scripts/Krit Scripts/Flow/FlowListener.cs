using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// [Data Structure] สำหรับจับคู่ระหว่าง "สถานะเกม" และ "เหตุการณ์ที่จะให้เกิดขึ้น"
/// </summary>
[Serializable]
public class FlowListenerEntry
{
    public FlowState m_State;  // สถานะที่ต้องการดักฟัง (เช่น DialogueState, GameplayState)
    public UnityEvent m_Event; // คำสั่งหรือฟังก์ชันใน Unity ที่จะให้ทำงานเมื่อเข้าสู่สถานะนั้น
}

/// <summary>
/// [Component] ตัวดักฟังสถานะเกม (Listener)
/// ใช้สำหรับวางไว้บนวัตถุในฉากเพื่อให้ตอบสนองต่อการเปลี่ยนสถานะของ FlowChannel
/// </summary>
public class FlowListener : MonoBehaviour
{
    [SerializeField]
    private FlowChannel m_Channel; // ช่องสัญญาณ Flow ที่ต้องการดักฟัง

    [SerializeField]
    private FlowListenerEntry[] m_Entries; // รายการสถานะและเหตุการณ์ที่ตั้งค่าไว้ใน Inspector

    private void Awake()
    {
        // ลงทะเบียน: เริ่มดักฟังเมื่อมีการประกาศว่าสถานะเกมเปลี่ยนไป (OnFlowStateChanged)
        m_Channel.OnFlowStateChanged += OnDialogueFlowStateChanged;
    }

    private void OnDestroy()
    {
        // ยกเลิกการดักฟังเมื่อ Object ถูกทำลาย
        m_Channel.OnFlowStateChanged -= OnDialogueFlowStateChanged;
    }

    /// <summary>
    /// ฟังก์ชันที่จะทำงานทุกครั้งเมื่อได้รับสัญญาณว่าสถานะเกมเปลี่ยน
    /// </summary>
    /// <param name="state">สถานะใหม่ที่เพิ่งเปลี่ยนไป</param>
    private void OnDialogueFlowStateChanged(FlowState state)
    {
        // ค้นหาในรายการ (Entries) ว่ามีสถานะที่ตรงกับที่ส่งมาหรือไม่
        FlowListenerEntry foundEntry = Array.Find(m_Entries, x => x.m_State == state);

        // ถ้าเจอสถานะที่ตรงกัน ให้รัน Unity Event ทันที
        if (foundEntry != null)
        {
            foundEntry.m_Event.Invoke();
        }
    }
}