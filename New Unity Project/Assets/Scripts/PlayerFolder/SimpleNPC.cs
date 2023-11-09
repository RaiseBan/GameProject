using UnityEngine;

public class SimpleNPC : MonoBehaviour
{
    public float health = 10f; // Здоровье NPC
    public float moveSpeed = 2f; // Скорость движения NPC
    public float changeDirectionTime = 2f; // Как часто NPC меняет направление

    private Rigidbody2D rb;
    private Vector2 movement;
    private float timeSinceLastChange = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChangeDirection();
    }

    void Update()
    {
        // Проверка времени для смены направления
        if (timeSinceLastChange >= changeDirectionTime)
        {
            ChangeDirection();
            timeSinceLastChange = 0f;
        }
        else
        {
            timeSinceLastChange += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // Перемещение NPC
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // Меняем направление на случайное
    void ChangeDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        movement = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
    }

    // Функция для получения урона
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    } 

    // Функция для смерти NPC
    void Die()
    {
        // Здесь код для выпадения шерсти
        Destroy(gameObject);
    }
}