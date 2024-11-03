using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    // ѕеременна€ дл€ хранени€ игрока
    public Transform player;

    // —мещение камеры относительно игрока (по умолчанию позици€ -7, 9, 3 относительно игрока)
    private Vector3 offset = new Vector3(-7, 9, 3);

    // ‘иксированный угол наклона камеры
    private Vector3 fixedRotation = new Vector3(46, 166, 0);

    void Start()
    {
        // ”станавливаем начальный угол наклона камеры
        transform.eulerAngles = fixedRotation;
    }

    void LateUpdate()
    {
        // ≈сли игрок не указан, не выполн€ем обновление
        if (player == null) return;

        // ќбновл€ем позицию камеры относительно позиции игрока с учЄтом смещени€
        transform.position = player.position + offset;

        // Ќаправл€ем камеру на игрока
        transform.LookAt(player.position);

        // —охран€ем фиксированный угол наклона камеры
        transform.eulerAngles = new Vector3(fixedRotation.x, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
