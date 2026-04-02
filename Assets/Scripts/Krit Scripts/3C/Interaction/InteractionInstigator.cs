using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Component] ติดไว้ที่ตัวผู้เล่น (Player) เพื่อทำหน้าที่ตรวจจับและสั่งการ Interactable
/// โดยใช้ Trigger Collider เพื่อเช็คระยะห่างรอบๆ ตัวละคร
/// </summary>
public class InteractionInstigator : MonoBehaviour
{
    // รายการของวัตถุที่โต้ตอบได้ ซึ่งอยู่ในระยะที่ผู้เล่นกดถึงในขณะนี้
    private List<Interactable> m_NearbyInteractables = new List<Interactable>();

    /// <summary>
    /// ตรวจสอบว่ารอบตัวผู้เล่นมีวัตถุให้โต้ตอบหรือไม่
    /// </summary>
    public bool HasNearbyInteractables()
    {
        return m_NearbyInteractables.Count != 0;
    }

    private void Update()
    {
        // เมื่อมีวัตถุอยู่ใกล้ และผู้เล่นกดปุ่ม "Submit" (ปกติคือ Enter หรือ Space)
        if (HasNearbyInteractables() && Input.GetButtonDown("Submit"))
        {
            /* * คำแนะนำ: ปัจจุบันระบบเลือกอันแรกใน List (index 0) 
             * ในอนาคต กิตสามารถพัฒนาต่อให้เลือกอันที่ "ใกล้ที่สุด" หรือ "อยู่ตรงหน้ากล้องที่สุด" ได้
             */
            m_NearbyInteractables[0].DoInteraction();
        }
    }

    /* --- ระบบตรวจจับขอบเขต (Trigger System) --- */

    private void OnTriggerEnter(Collider other)
    {
        // เมื่อเดินเข้าใกล้ Object ที่มี Component 'Interactable'
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            m_NearbyInteractables.Add(interactable);
            Debug.Log($"[Interaction] เข้าใกล้: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // เมื่อเดินออกจากระยะของ Object นั้น
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            m_NearbyInteractables.Remove(interactable);
            Debug.Log($"[Interaction] เดินออกจากระยะ: {other.name}");
        }
    }
}