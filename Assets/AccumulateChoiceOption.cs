using UnityEngine;

[System.Serializable]
public class AccumulateChoiceOption : BaseChoiceOption
{
    [Header("Accumulate Settings")]
    public string statName; // ชื่อ Status หรือ Event
    public int pointsToAdd; // แต้มที่ได้

    // เขียนทับ (Override) ฟังก์ชันเดิมเพื่อเพิ่มลอจิกการให้คะแนน
    public override void ExecuteChoice()
    {
        // ให้มันแสดงผลตอบกลับตามปกติของ Hollow Choice
        base.ExecuteChoice();

        // เพิ่มการทำงานของ Accumulate
        Debug.Log($"ได้รับแต้ม {pointsToAdd} ให้กับ {statName}");
        // GameStateManager.Instance.AddScore(statName, pointsToAdd); // ตัวอย่างโค้ดระบบเกมจริง
    }
}