using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UniversitySystem
{
    // 1. КАСТОМНІ ВИHЯТКИ (EXCEPTIONS) Наслідується від базового класу Exception.
    public class ValidationException : Exception
    {
        // Конструктор приймає текст помилки та передає його в базовий клас, автоматично додаючи маркер "[Помилка Валідації]:" для уніфікації виводу.
        public ValidationException(string message) : base($"[Помилка Валідації]: {message}") { }
    }

    // 2. МОДЕЛІ ДАНИХ (MODELS)
     public class Subject
    {
        // Автоматична властивість із закритим сетером. Змінити назву предмета ззовні класу після його створення не можна.
        public string Name { get; private set; }

        // Конструктор класу. Гарантує, що кожен предмет обов'язково матиме назву.
        public Subject(string name)
        {
            // string.IsNullOrWhiteSpace перевіряє, чи рядок не є порожнім, null або просто набором пробілів.
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Назва предмета не може бути порожньою.");
            Name = name;
        }
        // Перевизначення системного методу ToString().
        // Тепер, коли ми викликаємо Console.WriteLine(subject), замість назви типу (UniversitySystem.Subject) виведеться сама назва предмета.
        public override string ToString() => Name;
    }

    public class Student
    {
        private string _name;
        public string Id { get; private set; } // Унікальний ідентифікатор (наприклад, номер студквитка)
        public string Name
        {
            get => _name;
            set
            {
                // Валідація даних прямо під час спроби змінити ім'я студента
                if (string.IsNullOrWhiteSpace(value))
                    throw new ValidationException("Ім'я студента не може бути порожнім.");
                _name = value;
            }
        }

        // Словник, де ключ — це назва предмета, а значення — список оцінок (цілих чисел) з цього предмета.
        // Ініціалізується одразу, щоб уникнути помилки NullReferenceException.
        public Dictionary<string, List<int>> Grades { get; set; } = new Dictionary<string, List<int>>();

        public Student(string id, string name)
        {
            Id = id;
            Name = name; // Виклик сетера властивості Name, де відпрацює валідація
        }
        // Метод додавання оцінки за конкретний предмет
        public void AddGrade(string subjectName, int grade)
        {
            // Перевірка бізнес-правила: оцінка має бути в межах стобальної системи
            if (grade < 1 || grade > 100)
                throw new ValidationException("Оцінка повинна бути в межах від 1 до 100.");

            // Якщо студент ще не має оцінок з цього предмета, створюємо для нього новий список у словнику
            if (!Grades.ContainsKey(subjectName))
            {
                Grades[subjectName] = new List<int>();
            }
            // Додаємо оцінку до списку
            Grades[subjectName].Add(grade);
        }
        // Метод розрахунку загального середнього балу студента за всіма предметами
        public double GetAverageGrade()
        {
            int totalSum = 0;
            int totalCount = 0;

            // Вкладений цикл: перебираємо списки оцінок, а потім кожну оцінку в них
            foreach (var gradesList in Grades.Values)
            {
                foreach (var g in gradesList)
                {
                    totalSum += g;
                    totalCount++;
                }
            }
            // Тернарний оператор для захисту від ділення на нуль (якщо оцінок немає, повертаємо 0).
            // Math.Round(..., 1) округлює результат до одного знаку після коми.
            return totalCount == 0 ? 0 : Math.Round((double)totalSum / totalCount, 1);
        }
    }

    public class Group
    {
        public string GroupCode { get; private set; } // Код групи, наприклад "Б-121"
        public List<Student> Students { get; set; } = new List<Student>(); // Список студентів цієї групи

        public Group(string groupCode)
        {
            if (string.IsNullOrWhiteSpace(groupCode))
                throw new ValidationException("Код групи не може бути порожнім.");
            // Автоматично переводимо код групи у верхній регістр (щоб "кн-21" перетворилось на "Б-121")
            GroupCode = groupCode.ToUpper();
        }

        // Метод безпечного додавання студента до групи
        public void AddStudent(Student student)
        {
            // Перевірка на null за допомогою оператора nameof (повертає назву змінної як рядок для винятку)
            if (student == null) throw new ArgumentNullException(nameof(student));
            Students.Add(student);
        }
    }

    public class ScheduleItem
    {
        private string _classroom;
        private int _lessonNumber;
        private string _teacherName;

        public string GroupCode { get; set; }
        public string SubjectName { get; set; }

        public int LessonNumber
        {
            get => _lessonNumber;
            set
            {
                if (value < 1 || value > 4)
                    throw new ValidationException("Номер пари повинен бути в межах від 1 до 4.");
                _lessonNumber = value;
            }
        }

        public string TeacherName
        {
            get => _teacherName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ValidationException("Ім'я викладача не може бути порожнім.");
                _teacherName = value;
            }
        }

        public string Classroom
        {
            get => _classroom;
            set
            {
                // Регулярний вираз (Regex) для жорсткої перевірки формату аудиторії.
                // ^(1[0-2]|[1-9]) — корпус від 1 до 12.
                // \. — обов'язкова крапка.
                // \d{3}$ — рівно три цифри номера кімнати в кінці рядка
                string pattern = @"^(1[0-2]|[1-9])\.\d{3}$";
                if (!Regex.IsMatch(value, pattern))
                    throw new ValidationException("Некоректний формат аудиторії! Формат: [Корпус 1-12].[3 цифри]. Приклад: 6.202 або 1.001");
                _classroom = value;
            }
        }

        public ScheduleItem(string groupCode, string subjectName, int lessonNumber, string teacherName, string classroom)
        {
            GroupCode = groupCode;
            SubjectName = subjectName;
            LessonNumber = lessonNumber;
            TeacherName = teacherName;
            Classroom = classroom;
        }
    }

    // 3. СЕРВІС КЕРУВАННЯ (SERVICES)
    public class UniversityManager
    {
        // Головні колекції даних системи
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<ScheduleItem> Schedule { get; set; } = new List<ScheduleItem>();

        // Статичний шлях до файлу збереження. Дані лежатимуть поруч із запущеним .exe файлом
        private const string FilePath = "university_data.json";

        // Методи швидкого пошуку об'єктів за допомогою Лямбда-виразів.
        // StringComparison.OrdinalIgnoreCase ігнорує регістр символів при порівнянні рядків (наприклад, "Математика" == "математика").
        public Group FindGroup(string code) => Groups.Find(g => g.GroupCode.Equals(code, StringComparison.OrdinalIgnoreCase));
        public Subject FindSubject(string name) => Subjects.Find(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        // Серіалізація: збереження всіх списків менеджера у текстовий файл JSON
        public void SaveData()
        {
            // Опція WriteIndented = true робить JSON "красивим" (з відступами та переносами рядків), а не в один рядок
            var options = new JsonSerializerOptions { WriteIndented = true };
            // Ключове слово 'this' означає, що ми серіалізуємо поточний екземпляр класу UniversityManager з усіма його списками
            string jsonString = JsonSerializer.Serialize(this, options);
            // Запис рядка у файл (якщо файл існував — він перезапишеться)
            File.WriteAllText(FilePath, jsonString);
            Console.WriteLine("Дані успішно збережено у файл!");
        }

        // Десеріалізація: відновлення стану системи з файлу JSON
        public void LoadData()
        {
            // Якщо файлу ще немає (перший запуск програми), просто виходимо з методу
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("Файл даних не знайдено. Починаємо з порожньої бази.");
                return;
            }

            // Блок try-catch захищає програму від падіння, якщо файл JSON пошкоджений або заблокований системою
            try
            {
                string jsonString = File.ReadAllText(FilePath);
                // Зчитуємо дані назад в об'єкт
                var loadedData = JsonSerializer.Deserialize<UniversityManager>(jsonString);
                if (loadedData != null)
                {
                    // Переносимо списки з тимчасового об'єкта в поточний. 
                    // Оператор ?? (null-coalescing) підстраховує: якщо список у файлі був null, створюється новий порожній список.
                    // Це захищає від критичних помилок (NullReferenceException) надалі.
                    Groups = loadedData.Groups ?? new List<Group>();
                    Subjects = loadedData.Subjects ?? new List<Subject>();
                    Schedule = loadedData.Schedule ?? new List<ScheduleItem>();
                    Console.WriteLine("Дані успішно завантажено з файлу!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні даних: {ex.Message}");
            }
        }
    }

    // 4. ТОЧКА ВХОДУ (MAIN PROGRAM)
    class Program
    {
        static void Main(string[] args)
        {
            // Важливе налаштування для консолі Windows: змушує її коректно зчитувати та виводити українські літери (І, Ї, Є, Ґ) через UTF-8
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            UniversityManager manager = new UniversityManager();
            manager.LoadData(); // Автоматичне завантаження при старті

            bool exit = false;
            // Головний життєвий цикл консольного меню
            while (!exit)
            {
                Console.WriteLine("\n=== СИСТЕМА УПРАВЛІННЯ УНІВЕРСИТЕТОМ ===");
                Console.WriteLine("1. Додати новий предмет");
                Console.WriteLine("2. Створити нову групу");
                Console.WriteLine("3. Додати студента до групи");
                Console.WriteLine("4. Виставити оцінку студенту");
                Console.WriteLine("5. Додати заняття до РОЗКЛАДУ");
                Console.WriteLine("6. Переглянути структуру та Розклад занять");
                Console.WriteLine("7. Зберегти дані у файл");
                Console.WriteLine("0. Вихід");
                Console.Write("Оберіть дію: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                // Глобальний блок try-catch всередині циклу. 
                // Якщо користувач введе некоректні дані у будь-якому пункті меню, програма НЕ вилетить, а просто покаже попередження та повернеться на головне меню
                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Введіть назву предмета: ");
                            string subName = Console.ReadLine();
                            // Бізнес-логіка: забороняємо дублікати предметів
                            if (manager.FindSubject(subName) != null)
                                throw new ValidationException("Такий предмет уже існує!");
                            manager.Subjects.Add(new Subject(subName));
                            Console.WriteLine("Предмет успішно додано.");
                            break;

                        case "2":
                            Console.Write("Введіть код групи (наприклад, КН-21): ");
                            string gCode = Console.ReadLine();
                            if (manager.FindGroup(gCode) != null)
                                throw new ValidationException("Група з таким кодом вже існує!");
                            manager.Groups.Add(new Group(gCode));
                            Console.WriteLine("Групу успішно створено.");
                            break;

                        case "3":
                            Console.Write("Введіть код групи, куди додати студента: ");
                            string targetGroupCode = Console.ReadLine();
                            Group group = manager.FindGroup(targetGroupCode);
                            if (group == null) throw new ValidationException("Такої групи не існує.");

                            Console.Write("Введіть ID (номер квитка) студента: ");
                            string sId = Console.ReadLine();
                            Console.Write("Введіть ПІБ студента: ");
                            string sName = Console.ReadLine();

                            // Створюємо студента та додаємо його до знайденої групи
                            group.AddStudent(new Student(sId, sName));
                            Console.WriteLine($"Студент {sName} успішно зарахований до групи {group.GroupCode}.");
                            break;

                        case "4":
                            Console.Write("Введіть код групи студента: ");
                            string grCode = Console.ReadLine();
                            Group gr = manager.FindGroup(grCode);
                            if (gr == null) throw new ValidationException("Групу не знайдено.");

                            Console.Write("Введіть ПІБ або ID студента: ");
                            string searchCriteria = Console.ReadLine();
                            // Пошук студента всередині конкретної групи за двома критеріями (або за іменем без урахування регістру, або за ID)
                            Student student = gr.Students.Find(s => s.Name.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase) || s.Id == searchCriteria);
                            if (student == null) throw new ValidationException("Студента в цій групі не знайдено.");

                            Console.Write("Введіть назву предмета: ");
                            string subjName = Console.ReadLine();
                            Subject subject = manager.FindSubject(subjName);
                            if (subject == null) throw new ValidationException("Такого предмета немає в базі.");

                            Console.Write("Введіть оцінку (1-100): ");
                            // int.TryParse намагається безпечно конвертувати рядок у число. Якщо користувач введе текст (наприклад "баран"), метод поверне false.
                            if (!int.TryParse(Console.ReadLine(), out int grade))
                                throw new ValidationException("Оцінка повинна бути числом.");

                            student.AddGrade(subject.Name, grade);
                            Console.WriteLine("Оцінку успішно виставлено!");
                            break;

                        case "5":
                            Console.Write("Введіть код групи для заняття: ");
                            string schedGroup = Console.ReadLine();
                            if (manager.FindGroup(schedGroup) == null)
                                throw new ValidationException("Такої групи не існує. Створіть її спочатку.");

                            Console.Write("Введіть назву предмета: ");
                            string schedSubj = Console.ReadLine();
                            if (manager.FindSubject(schedSubj) == null)
                                throw new ValidationException("Такого предмета не існує. Додайте його спочатку.");

                            Console.Write("Введіть номер пари (1-4): ");
                            if (!int.TryParse(Console.ReadLine(), out int lessonNum))
                                throw new ValidationException("Номер пари має бути цифрою.");

                            Console.Write("Введіть ПІБ викладача: ");
                            string teacher = Console.ReadLine();

                            Console.Write("Введіть номер аудиторії (наприклад 6.202): ");
                            string room = Console.ReadLine();

                            // Конструктор ScheduleItem сам завалідує формат аудиторії через Regex
                            ScheduleItem newItem = new ScheduleItem(schedGroup.ToUpper(), schedSubj, lessonNum, teacher, room);
                            manager.Schedule.Add(newItem);
                            Console.WriteLine("Заняття успішно додано до розкладу!");
                            break;

                        case "6":
                            DisplayUniversityStructure(manager);
                            break;

                        case "7":
                            manager.SaveData();
                            break;

                        case "0":
                            manager.SaveData(); // Автозбереження перед виходом
                            exit = true;
                            Console.WriteLine("Роботу завершено. Гарного дня!");
                            break;

                        default:
                            Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                            break;
                    }
                }
                // Перехоплення помилок валідації (користувацькі помилки). Фарбуємо текст у жовтий колір.
                catch (ValidationException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor(); //скидаємо колір консолі до стандартного
                }
                // Перехоплення будь-яких інших непередбачуваних помилок (наприклад, збій системи). Фарбуємо в червоний.
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Критична помилка]: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        // Допоміжний метод для гарного деревовидного відображення структури університету та розкладу
        static void DisplayUniversityStructure(UniversityManager manager)
        {
            Console.WriteLine("====== СТРУКТУРА УНІВЕРСИТЕТУ ======");
            if (manager.Groups.Count == 0)
            {
                Console.WriteLine("База даних груп порожня.");
            }
            else
            {
                // Перебір усіх груп
                foreach (var group in manager.Groups)
                {

                    Console.WriteLine($"\nГрупа: {group.GroupCode} (Студентів: {group.Students.Count})");
                    // Перебір студентів у поточній групі
                    foreach (var student in group.Students)
                    {
                        Console.WriteLine($"  └─ [{student.Id}] {student.Name} | Сер. бал: {student.GetAverageGrade()}");
                        // Якщо у студента є хоча б одна оцінка, виводимо список його оцінок
                        if (student.Grades.Count > 0)
                        {
                            Console.Write("     Оцінки: ");
                            foreach (var kvp in student.Grades)
                            {
                                Console.Write($"{kvp.Key}: [{string.Join(", ", kvp.Value)}] ");
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }

            Console.WriteLine("\n====== РОЗКЛАД ЗАНЯТЬ ======");
            if (manager.Schedule.Count == 0)
            {
                Console.WriteLine("Розклад порожній.");
            }
            else
            {
                // Сортування розкладу перед виводом за допомогою лямбда-компаратора.
                // Сортуємо спочатку за назвою групи (алфавітний порядок), а якщо групи однакові (comp == 0) — то за номером пари (від 1 до 4)
                manager.Schedule.Sort((x, y) => {
                    int comp = string.Compare(x.GroupCode, y.GroupCode, StringComparison.OrdinalIgnoreCase);
                    if (comp != 0) return comp;
                    return x.LessonNumber.CompareTo(y.LessonNumber);
                });

                foreach (var item in manager.Schedule)
                {
                    Console.WriteLine($"Група {item.GroupCode} | Пара №{item.LessonNumber} | Предмет: \"{item.SubjectName}\" | Викладач: {item.TeacherName} | Аудиторія: {item.Classroom}");
                }
            }
            Console.WriteLine("====================================");
        }
    }
}