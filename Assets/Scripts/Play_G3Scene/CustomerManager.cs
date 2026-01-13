using UnityEngine;
using System.Collections;

public class CustomerManager : MonoBehaviour
{
    [Header("Danh sách 3 nhân vật")]
    public Customer[] customers; // Kéo 3 nhân vật (cs_0, cs_1, cs_3) vào đây

    [Header("Thời gian sinh khách")]
    public float minSpawnTime = 3f;
    public float maxSpawnTime = 8f;

    void Start()
    {
        // Bắt đầu vòng lặp sinh khách
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true) // Lặp vô tận
        {
            // 1. Chờ một khoảng thời gian ngẫu nhiên
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // 2. Tìm xem có nhân vật nào đang "Rảnh" (đang ẩn) không
            TrySpawnCustomer();
        }
    }

    void TrySpawnCustomer()
    {
        // Tạo danh sách các khách đang rảnh
        System.Collections.Generic.List<Customer> availableCustomers = new System.Collections.Generic.List<Customer>();

        foreach (var c in customers)
        {
            if (!c.IsActive) // Nếu khách này đang ẩn
            {
                availableCustomers.Add(c);
            }
        }

        // Nếu có ít nhất 1 người rảnh -> Chọn ngẫu nhiên 1 người để hiện lên
        if (availableCustomers.Count > 0)
        {
            int index = Random.Range(0, availableCustomers.Count);
            availableCustomers[index].Appear();
        }
        else
        {
            // Nếu cả 3 người đều đang ăn rồi -> Kệ, chờ lượt sau
            Debug.Log("Quán đông quá, không còn chỗ cho khách mới!");
        }
    }
}