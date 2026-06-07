using System;

namespace UniversitySystem.Models
{
    // Клас, що описує навчальну дисципліну
    public class Subject
    {
        // Автоматична властивість для назви предмета з приватною ініціалізацією
        public string Name { get; private set; }

        // Конструктор для створення предмета
        public Subject(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва предмета не може бути порожньою.");

            Name = name;
        }

        // Перевизначення методу для зручного виведення
        public override string ToString() => Name;
    }
}