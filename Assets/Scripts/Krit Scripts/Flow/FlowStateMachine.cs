using UnityEngine;

/// <summary>
/// [Core Controller] ตัวจัดการสถานะหลักของเกม (State Machine)
/// ทำหน้าที่เป็น "ศูนย์กลาง" ที่คอยตัดสินใจและเปลี่ยนสถานะเกมตามคำขอที่ได้รับ
/// โดยใช้รูปแบบ Singleton เพื่อให้สคริปต์อื่นเข้าถึงสถานะปัจจุบันได้ง่าย
/// </summary>
public class FlowStateMachine : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private FlowChannel m_Channel;    // ช่องสัญญาณ Flow สำหรับรับคำขอและประกาศการเปลี่ยนแปลง
    [SerializeField]
    private FlowState m_StartupState; // สถานะเริ่มต้นเมื่อเข้าเกม (เช่น Gameplay หรือ Intro)

    private FlowState m_CurrentState; // เก็บสถานะปัจจุบันที่เกมกำลังดำเนินอยู่
    public FlowState CurrentState => m_CurrentState;

    // --- Singleton Pattern ---
    // ช่วยให้สคริปต์อื่นเรียกใช้ FlowStateMachine.Instance ได้จากทุกที่
    private static FlowStateMachine ms_Instance;
    public static FlowStateMachine Instance => ms_Instance;

    private void Awake()
    {
        // ตั้งค่า Instance เมื่อเริ่ม Object
        ms_Instance = this;

        // ลงทะเบียน: รอฟัง "คำขอ" เปลี่ยนสถานะจาก FlowChannel
        m_Channel.OnFlowStateRequested += SetFlowState;
    }

    private void Start()
    {
        // เมื่อเริ่มเกม ให้เข้าสู่สถานะเริ่มต้นที่ตั้งค่าไว้ใน Inspector
        if (m_StartupState != null)
        {
            SetFlowState(m_StartupState);
        }
    }

    private void OnDestroy()
    {
        // ยกเลิกการดักฟังและล้างค่า Instance เพื่อป้องกัน Error เมื่อเปลี่ยนฉาก
        if (m_Channel != null)
        {
            m_Channel.OnFlowStateRequested -= SetFlowState;
        }

        ms_Instance = null;
    }

    /// <summary>
    /// ฟังก์ชันหัวใจหลักสำหรับ "เปลี่ยนสถานะเกม"
    /// </summary>
    /// <param name="state">สถานะใหม่ที่ต้องการเปลี่ยนไป</param>
    private void SetFlowState(FlowState state)
    {
        // ตรวจสอบก่อนว่าสถานะใหม่ "ต่างจาก" สถานะปัจจุบันหรือไม่ (เพื่อไม่ให้รันซ้ำซ้อน)
        if (m_CurrentState != state)
        {
            m_CurrentState = state;

            // ประกาศบอกทุกคนในฉาก (ผ่าน Channel) ว่า "ตอนนี้สถานะเปลี่ยนแล้วนะ!"
            // เพื่อให้ FlowListener หรือระบบอื่นๆ ทำงานตามที่ตั้งค่าไว้
            m_Channel.RaiseFlowStateChanged(m_CurrentState);

            Debug.Log($"[Game Flow] State changed to: {(state != null ? state.name : "Null")}");
        }
    }
}