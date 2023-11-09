using UnityEngine;

public class randomNinerals
{
    private static int baseWeightType1 = 50; // Базовый шанс для типа 1
    private static int baseWeightType2 = 30; // Базовый шанс для типа 2
    private static int baseWeightType3 = 15; // Базовый шанс для типа 3
    private static int baseWeightType4 = 5;  // Базовый шанс для типа 4
    private static int hello = 1;

    // Коэффициент модификации
    private static float coefficient = 1.0f; 

    // Метод для случайного выбора типа камня с учетом коэффициента
    public static int GetRandomStoneType()
    {
        // Применяем коэффициент к базовым весам
        int weightType1 = Mathf.RoundToInt(baseWeightType1 * coefficient);
        int weightType2 = Mathf.RoundToInt(baseWeightType2 * coefficient);
        int weightType3 = Mathf.RoundToInt(baseWeightType3 * coefficient);
        int weightType4 = Mathf.RoundToInt(baseWeightType4 * coefficient);

        // Общий вес
        int totalWeight = weightType1 + weightType2 + weightType3 + weightType4;
        // Случайное число в этом диапазоне
        int randomNumber = Random.Range(0, totalWeight);
        int accumulatedWeight = 0;

        // Проверяем, в каком диапазоне лежит randomNumber
        if (randomNumber < (accumulatedWeight += weightType1))
        {
            Debug.Log("STONE");
            return 0;
            // stone
        }
        else if (randomNumber < (accumulatedWeight += weightType2))
        {
            Debug.Log("COAL");
            return 1;
            
            //coal
        }
        else if (randomNumber < (accumulatedWeight += weightType3))
        {
            Debug.Log("IRON");
            return 2;
            //iron
        }
        else
        {
            return 3;
            //cuprum
        }
    }

    // Метод для установки коэффициента модификации
    public static void SetCoefficient(float newCoefficient)
    {
        coefficient = newCoefficient;
    }

}