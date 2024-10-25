using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class Order
{
    public int OrderId { get; set; }
    public double Weight { get; set; }
    public string District { get; set; }
    public DateTime DeliveryTime { get; set; }
}

class Program
{
    private static readonly string LogFilePath = "delivery_log.txt"; // Файл для логирования

    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            LogError("Необходимо указать район и время доставки.");
            return;
        }

        string district = args[0];
        if (!DateTime.TryParseExact(args[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime deliveryTime))
        {
            LogError("Некорректный формат времени. Используйте: yyyy-MM-dd HH:mm:ss");
            return;
        }

        Console.WriteLine($"Фильтруем заказы для района: {district} и времени: {deliveryTime:dd.MM.yyyy HH:mm:ss}");
        LogInfo($"Запущена фильтрация для района: {district} и времени: {deliveryTime}");

        var orders = LoadOrders("orders.csv");
        var filteredOrders = FilterOrders(orders, district, deliveryTime);
        
        // Вывод данных
        if (!filteredOrders.Any())
        {
            Console.WriteLine("Нет доступных заказов для указанного района и времени.");
            LogInfo("Нет доступных заказов для указанного района и времени.");
        }
        else
        {
            foreach (var order in filteredOrders)
            {
                Console.WriteLine($"Заказ ID: {order.OrderId}, Вес: {order.Weight}, Район: {order.District}, Время доставки: {order.DeliveryTime}");
            }
        }
    }

    static List<Order> LoadOrders(string filePath)
    {
        var orders = new List<Order>();
        foreach (var line in File.ReadAllLines(filePath).Skip(1)) // Пропускаем заголовок
        {
            var parts = line.Split(',');
            if (parts.Length == 4 &&
                int.TryParse(parts[0], out int orderId) &&
                double.TryParse(parts[1], out double weight) &&
                DateTime.TryParse(parts[3], out DateTime deliveryTime))
            {
                orders.Add(new Order
                {
                    OrderId = orderId,
                    Weight = weight,
                    District = parts[2],
                    DeliveryTime = deliveryTime
                });
            }
        }
        return orders;
    }

    static IEnumerable<Order> FilterOrders(IEnumerable<Order> orders, string district, DateTime firstDeliveryTime)
    {
        var validTimeRange = firstDeliveryTime.AddMinutes(30);
        return orders.Where(o => o.District.Equals(district, StringComparison.OrdinalIgnoreCase) &&
                                  o.DeliveryTime >= firstDeliveryTime &&
                                  o.DeliveryTime <= validTimeRange);
    }

    // Метод для логирования информации
    static void LogInfo(string message)
    {
        File.AppendAllText(LogFilePath, $"{DateTime.Now}: INFO: {message}\n");
    }

    // Метод для логирования ошибок
    static void LogError(string message)
    {
        File.AppendAllText(LogFilePath, $"{DateTime.Now}: ERROR: {message}\n");
    }
}
