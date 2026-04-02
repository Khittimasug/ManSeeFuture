using UnityEngine;

/// <summary>
/// [Manager Component] ทำหน้าที่เป็นผู้ติดตั้งและเชื่อมโยงระบบ Dialogue ทั้งหมดเข้าด้วยกัน
/// สคริปต์นี้ต้องถูกใส่ไว้ใน Object ในฉาก (เช่น Managers) เพื่อให้ระบบเริ่มทำงานได้
/// </summary>
public class DialogueInstigator : MonoBehaviour
{
    [Header("Channels")]
    [SerializeField] private DialogueChannel m_DialogueChannel; // ช่องสื่อสารของระบบบทสนทนา
    [SerializeField] private FlowChannel m_FlowChannel;         // ช่องสื่อสารสำหรับเปลี่ยนสถานะของเกม (เช่น Gameplay -> Dialogue)

    [Header("Game States")]
    [SerializeField] private FlowState m_DialogueState;         // สถานะที่บอกว่า "ขณะนี้กำลังอยู่ในบทสนทนา"

    private DialogueSequencer m_DialogueSequencer;             // ตัวประมวลผลตรรกะ (สร้างขึ้นมาใหม่ใน Awake)
    private FlowState m_CachedFlowState;                        // ตัวเก็บสถานะก่อนหน้า (เพื่อเปลี่ยนกลับเมื่อคุยจบ)

    private void Awake()
    {
        // 1. สร้าง "สมอง" (Sequencer) ขึ้นมาเพื่อใช้ประมวลผลบทสนทนาในฉากนี้
        m_DialogueSequencer = new DialogueSequencer();

        // 2. [Binding Events] เชื่อมโยงสัญญาณจาก Sequencer ออกไปยัง Channel
        // เมื่อ Sequencer สั่งเริ่ม/จบ Node ให้ไป "ประกาศ" บอกทุกคนผ่าน Channel
        m_DialogueSequencer.OnDialogueStart += OnDialogueStart;
        m_DialogueSequencer.OnDialogueEnd += OnDialogueEnd;
        m_DialogueSequencer.OnDialogueNodeStart += m_DialogueChannel.RaiseDialogueNodeStart;
        m_DialogueSequencer.OnDialogueNodeEnd += m_DialogueChannel.RaiseDialogueNodeEnd;

        // 3. [Listening Events] รอฟังคำขอจาก Channel เพื่อไปสั่งงาน Sequencer
        // เมื่อมีใคร (เช่น VideoManager) ส่งคำขอให้เริ่มคุย ให้ Sequencer จัดการทันที
        m_DialogueChannel.OnDialogueRequested += m_DialogueSequencer.StartDialogue;
        m_DialogueChannel.OnDialogueNodeRequested += m_DialogueSequencer.StartDialogueNode;
    }

    private void OnDestroy()
    {
        // ยกเลิกการเชื่อมโยงทั้งหมดเมื่อ Object ถูกทำลาย เพื่อป้องกัน Memory Leak
        m_DialogueChannel.OnDialogueNodeRequested -= m_DialogueSequencer.StartDialogueNode;
        m_DialogueChannel.OnDialogueRequested -= m_DialogueSequencer.StartDialogue;

        m_DialogueSequencer.OnDialogueNodeEnd -= m_DialogueChannel.RaiseDialogueNodeEnd;
        m_DialogueSequencer.OnDialogueNodeStart -= m_DialogueChannel.RaiseDialogueNodeStart;
        m_DialogueSequencer.OnDialogueEnd -= OnDialogueEnd;
        m_DialogueSequencer.OnDialogueStart -= OnDialogueStart;

        m_DialogueSequencer = null;
    }

    /// <summary>
    /// ทำงานเมื่อบทสนทนาเริ่มขึ้นจริงๆ
    /// </summary>
    private void OnDialogueStart(Dialogue dialogue)
    {
        m_DialogueChannel.RaiseDialogueStart(dialogue);

        // [Flow Management] 
        // บันทึกสถานะเกมปัจจุบันไว้ (เช่น กำลังเดินเล่น) 
        // แล้วสั่งให้เกมเปลี่ยนเข้าสู่ "Dialogue State" (เช่น หยุดการเคลื่อนไหวตัวละคร)
        m_CachedFlowState = FlowStateMachine.Instance.CurrentState;
        m_FlowChannel.RaiseFlowStateRequest(m_DialogueState);
    }

    /// <summary>
    /// ทำงานเมื่อจบบทสนทนา
    /// </summary>
    private void OnDialogueEnd(Dialogue dialogue)
    {
        // [Flow Management]
        // เมื่อคุยจบ ให้เปลี่ยนสถานะเกมกลับไปเป็นสถานะก่อนหน้าที่จะคุย (Resume Game)
        m_FlowChannel.RaiseFlowStateRequest(m_CachedFlowState);
        m_CachedFlowState = null;

        m_DialogueChannel.RaiseDialogueEnd(dialogue);
    }
}