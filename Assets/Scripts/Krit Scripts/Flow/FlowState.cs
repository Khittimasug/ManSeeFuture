using UnityEngine;

/// <summary>
/// [Scriptable Object] ใช้สำหรับสร้าง "ชื่อสถานะ" ของเกมในรูปแบบไฟล์ Asset
/// เช่น สร้างไฟล์ชื่อ "GameplayState", "DialogueState", หรือ "PauseState"
/// เพื่อนำไปใช้เปรียบเทียบในระบบ FlowChannel และ FlowListener
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Flow/Flow State")]
public class FlowState : ScriptableObject
{
    // คลาสนี้ตั้งใจให้ว่างไว้ เพราะเราใช้ "ตัวตนของไฟล์ Asset" (Instance) 
    // เป็นตัวจำแนกสถานะ แทนการใช้ Enum หรือ String เพื่อป้องกันการพิมพ์ชื่อผิด
}