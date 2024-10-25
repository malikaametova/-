using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class OrderService
{
    private List<Order> orders;

    public OrderService(string filePath)
    {
        LoadOrders(filePath);
    }

    private void LoadOrders(string filePath)
    {
        // Загрузка данных из CSV
        orders = File.ReadAllLines(filePath)
            .Skip(1) // Пропускаем заголовок
            .Select(line => line.Split(','))
            .Select(parts => new Order
            {
                OrderId = int.Parse(parts[0]),
                Weight = double.Parse(parts[1]),
                District = parts[2],
                DeliveryTime = DateTime.Parse(parts[3])
            })
            .ToList();
    }

    public IEnumerable<Order> FilterOrders(string district, DateTime firstDeliveryTime)
    {
        var filterTime = firstDeliveryTime.AddMinutes(30);
        return orders.Where(o => o.District == district && o.DeliveryTime <= filterTime);
    }
}
