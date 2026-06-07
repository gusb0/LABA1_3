using System;
using System.Collections.Generic;
using UniversitySystem.Exceptions;

namespace UniversitySystem.Models
{
    // Клас, що описує студента
    public class Student
    {
        // Інкапсульовані поля (закриті від прямого доступу)
        private string _name;

        // Властивості з валідацією (Інкапсуляція)
        public string Id { get; private set; }
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ValidationException("Ім'я студента не може бути порожнім.");
                _name = value;
            }
        }

        // Словник для збереження оцінок: Ключ - Предмет, Значення - Список оцінок (Generics)
        public Dictionary<string, List<int>> Grades { get; set; } = new Dictionary<string, List<int>>();

        // Конструктор класу
        public Student(string id, string name)
        {
            Id = id;
            Name = name;
        }

        // Метод для додавання оцінки з валідацією
        public void AddGrade(string subjectName, int grade)
        {
            if (grade < 1 || grade > 100)
                throw new ValidationException("Оцінка повинна бути в межах від 1 до 100.");

            if (!Grades.ContainsKey(subjectName))
            {
                Grades[subjectName] = new List<int>();
            }
            Grades[subjectName].Add(grade);
        }

        // Обчислення середнього балу студента
        public double GetAverageGrade()
        {
            int totalSum = 0;
            int totalCount = 0;

            foreach (var gradesList in Grades.Values)
            {
                foreach (var g in gradesList)
                {
                    totalSum += g;
                    totalCount++;
                }
            }

            return totalCount == 0 ? 0 : Math.Round((double)totalSum / totalCount, 1);
        }
    }
}