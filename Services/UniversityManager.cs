using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using UniversitySystem.Models;

namespace UniversitySystem.Services
{
    // Головний сервіс для управління даними університету та їх збереження
    public class UniversityManager
    {
        // Колекції для збереження об'єктів у пам'яті програми
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<ScheduleItem> Schedule { get; set; } = new List<ScheduleItem>();

        // Шлях до файлу локальної "бази даних"
        private const string FilePath = "university_data.json";

        // Швидкий пошук об'єктів у списках за допомогою Лямбда-виразів (ігноруючи регістр літер)
        public Group FindGroup(string code) => Groups.Find(g => g.GroupCode.Equals(code, StringComparison.OrdinalIgnoreCase));
        public Subject FindSubject(string name) => Subjects.Find(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        // Серіалізація: переведення об'єктів програми у текстовий формат JSON та запис у файл
        public void SaveData()
        {
            // Налаштування для "красивого" виводу JSON (із відступами та переносами)
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);

            File.WriteAllText(FilePath, jsonString);
            Console.WriteLine("Дані успішно збережено у файл!");
        }

        // Десеріалізація: зчитування JSON-файлу та відновлення об'єктів у пам'яті
        public void LoadData()
        {
            // Перевірка, чи файл взагалі існує (актуально для першого запуску)
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("Файл даних не знайдено. Починаємо з порожньої бази.");
                return;
            }

            try
            {
                string jsonString = File.ReadAllText(FilePath);
                var loadedData = JsonSerializer.Deserialize<UniversityManager>(jsonString);

                if (loadedData != null)
                {
                    // Оператор '??' підстраховує: якщо у файлі список був порожнім, створюється новий List
                    Groups = loadedData.Groups ?? new List<Group>();
                    Subjects = loadedData.Subjects ?? new List<Subject>();
                    Schedule = loadedData.Schedule ?? new List<ScheduleItem>();
                    Console.WriteLine("Дані успішно завантажено з файлу!");
                }
            }
            catch (Exception ex)
            {
                // Захист від падіння програми, якщо JSON-файл виявився пошкодженим
                Console.WriteLine($"Помилка при завантаженні даних: {ex.Message}");
            }
        }
    }
}