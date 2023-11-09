using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight; // Ваш Directional Light
    public float dayCycleDuration = 120f; // Продолжительность дня в секундах

    public Color dayColor = Color.white;
    public Color nightColor = Color.black;

    public float maxIntensity = 1f;
    public float minIntensity = 0f;

    private float cycleTime;

    void Start()
    {
        if (directionalLight == null)
        {
            Debug.LogError("Directional Light is not assigned. Please assign a Directional Light in the Inspector.");
            this.enabled = false;
            return;
        }

        // Устанавливаем начальное время дня
        cycleTime = 0f;
    }

    void Update()
    {
        // Обновляем время дня
        cycleTime += Time.deltaTime / dayCycleDuration;
        if (cycleTime >= 1f) // Если достигнут конец цикла, начинаем заново
        {
            cycleTime -= 1f;
        }

        // Вычисляем значение на основе косинуса, которое плавно меняется от 0 до 1 и обратно
        float cosineCurve = (Mathf.Cos(cycleTime * 2 * Mathf.PI) + 1) / 2;

        // Интерполируем цвет света и интенсивность между заданными значениями для дня и ночи
        directionalLight.color = Color.Lerp(nightColor, dayColor, cosineCurve);
        directionalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, cosineCurve);
    }
}