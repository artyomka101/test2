"""
Главный файл системы учёта учебных достижений
Консольный интерфейс для работы с оценками студентов
"""

from database import Database
from models import User

class StudentGradeSystem:
    def __init__(self):
        self.db = Database()
        self.current_user = None
    
    def main_menu(self):
        """Главное меню системы"""
        while True:
            print("\n" + "="*50)
            print("СИСТЕМА УЧЁТА УЧЕБНЫХ ДОСТИЖЕНИЙ")
            print("="*50)
            
            if not self.current_user:
                print("1. Войти в систему")
                print("2. Регистрация")
                print("0. Выход")
                
                choice = input("\nВыберите действие: ").strip()
                
                if choice == '1':
                    self.login()
                elif choice == '2':
                    self.register()
                elif choice == '0':
                    print("До свидания!")
                    break
                else:
                    print("Неверный выбор!")
            else:
                print(f"Добро пожаловать, {self.current_user.full_name}!")
                print(f"Группа: {self.current_user.group_name}")
                print("\n1. Просмотр оценок")
                print("2. Добавить оценку")
                print("3. Редактировать оценку")
                print("4. Удалить оценку")
                print("5. Статистика")
                print("6. Поиск по предметам")
                print("7. Выйти из аккаунта")
                print("0. Выход из программы")
                
                choice = input("\nВыберите действие: ").strip()
                
                if choice == '1':
                    self.view_grades()
                elif choice == '2':
                    self.add_grade()
                elif choice == '3':
                    self.edit_grade()
                elif choice == '4':
                    self.delete_grade()
                elif choice == '5':
                    self.show_statistics()
                elif choice == '6':
                    self.search_by_subject()
                elif choice == '7':
                    self.current_user = None
                    print("Вы вышли из аккаунта")
                elif choice == '0':
                    print("До свидания!")
                    break
                else:
                    print("Неверный выбор!")
    
    def login(self):
        """Вход в систему"""
        print("\n--- ВХОД В СИСТЕМУ ---")
        username = input("Логин: ").strip()
        password = input("Пароль: ").strip()
        
        user = self.db.authenticate_user(username, password)
        if user:
            self.current_user = user
            print(f"Добро пожаловать, {user.full_name}!")
        else:
            print("Неверный логин или пароль!")
    
    def register(self):
        """Регистрация нового пользователя"""
        print("\n--- РЕГИСТРАЦИЯ ---")
        username = input("Логин: ").strip()
        password = input("Пароль: ").strip()
        full_name = input("ФИО: ").strip()
        group_name = input("Группа: ").strip()
        
        if self.db.register_user(username, password, full_name, group_name):
            print("Регистрация успешна! Теперь вы можете войти в систему.")
        else:
            print("Ошибка регистрации!")
    
    def view_grades(self):
        """Просмотр всех оценок пользователя"""
        print("\n--- ВАШИ ОЦЕНКИ ---")
        grades = self.db.get_user_grades(self.current_user.id)
        
        if not grades:
            print("У вас пока нет оценок.")
            return
        
        print(f"{'ID':<5} {'Предмет':<25} {'Оценка':<8} {'Тип':<12} {'Дата':<12}")
        print("-" * 70)
        
        for grade in grades:
            print(f"{grade[0]:<5} {grade[4]:<25} {grade[1]:<8} {grade[2]:<12} {grade[3]:<12}")
        
        avg = self.db.calculate_average_grade(self.current_user.id)
        print(f"\nСредний балл: {avg}")
    
    def add_grade(self):
        """Добавление новой оценки"""
        print("\n--- ДОБАВЛЕНИЕ ОЦЕНКИ ---")
        
        # Показываем доступные предметы
        subjects = self.db.get_subjects()
        if not subjects:
            print("Нет доступных предметов.")
            return
        
        print("Доступные предметы:")
        for subject in subjects:
            print(f"{subject.id}. {subject.name} (семестр {subject.semester})")
        
        try:
            subject_id = int(input("\nВыберите предмет (ID): "))
            grade = int(input("Оценка (2-5): "))
            
            if grade < 2 or grade > 5:
                print("Оценка должна быть от 2 до 5!")
                return
            
            print("Тип оценки:")
            print("1. Экзамен")
            print("2. Зачёт")
            print("3. Домашнее задание")
            
            grade_type_choice = input("Выберите тип (1-3): ").strip()
            grade_types = {'1': 'exam', '2': 'test', '3': 'homework'}
            grade_type = grade_types.get(grade_type_choice, 'exam')
            
            if self.db.add_grade(self.current_user.id, subject_id, grade, grade_type):
                print("Оценка успешно добавлена!")
            else:
                print("Ошибка добавления оценки!")
                
        except ValueError:
            print("Введите корректные числовые значения!")
    
    def edit_grade(self):
        """Редактирование существующей оценки"""
        print("\n--- РЕДАКТИРОВАНИЕ ОЦЕНКИ ---")
        
        grades = self.db.get_user_grades(self.current_user.id)
        if not grades:
            print("У вас нет оценок для редактирования.")
            return
        
        print("Ваши оценки:")
        print(f"{'ID':<5} {'Предмет':<25} {'Оценка':<8} {'Тип':<12} {'Дата':<12}")
        print("-" * 70)
        
        for grade in grades:
            print(f"{grade[0]:<5} {grade[4]:<25} {grade[1]:<8} {grade[2]:<12} {grade[3]:<12}")
        
        try:
            grade_id = int(input("\nВведите ID оценки для редактирования: "))
            new_grade = int(input("Новая оценка (2-5): "))
            
            if new_grade < 2 or new_grade > 5:
                print("Оценка должна быть от 2 до 5!")
                return
            
            if self.db.update_grade(grade_id, new_grade):
                print("Оценка успешно обновлена!")
            else:
                print("Ошибка обновления оценки!")
                
        except ValueError:
            print("Введите корректные числовые значения!")
    
    def delete_grade(self):
        """Удаление оценки"""
        print("\n--- УДАЛЕНИЕ ОЦЕНКИ ---")
        
        grades = self.db.get_user_grades(self.current_user.id)
        if not grades:
            print("У вас нет оценок для удаления.")
            return
        
        print("Ваши оценки:")
        print(f"{'ID':<5} {'Предмет':<25} {'Оценка':<8} {'Тип':<12} {'Дата':<12}")
        print("-" * 70)
        
        for grade in grades:
            print(f"{grade[0]:<5} {grade[4]:<25} {grade[1]:<8} {grade[2]:<12} {grade[3]:<12}")
        
        try:
            grade_id = int(input("\nВведите ID оценки для удаления: "))
            confirm = input("Вы уверены? (да/нет): ").strip().lower()
            
            if confirm in ['да', 'yes', 'y']:
                if self.db.delete_grade(grade_id):
                    print("Оценка успешно удалена!")
                else:
                    print("Ошибка удаления оценки!")
            else:
                print("Удаление отменено.")
                
        except ValueError:
            print("Введите корректный ID!")
    
    def show_statistics(self):
        """Показ статистики по оценкам"""
        print("\n--- СТАТИСТИКА ---")
        
        # Общий средний балл
        avg_total = self.db.calculate_average_grade(self.current_user.id)
        print(f"Общий средний балл: {avg_total}")
        
        # Статистика по предметам
        subjects = self.db.get_subjects()
        print("\nСтатистика по предметам:")
        print(f"{'Предмет':<25} {'Средний балл':<15}")
        print("-" * 40)
        
        for subject in subjects:
            avg_subject = self.db.calculate_average_grade(self.current_user.id, subject.id)
            if avg_subject > 0:
                print(f"{subject.name:<25} {avg_subject:<15}")
    
    def search_by_subject(self):
        """Поиск оценок по предмету"""
        print("\n--- ПОИСК ПО ПРЕДМЕТАМ ---")
        
        subjects = self.db.get_subjects()
        if not subjects:
            print("Нет доступных предметов.")
            return
        
        print("Доступные предметы:")
        for subject in subjects:
            print(f"{subject.id}. {subject.name} (семестр {subject.semester})")
        
        try:
            subject_id = int(input("\nВыберите предмет (ID): "))
            grades = self.db.get_user_grades(self.current_user.id, subject_id)
            
            if not grades:
                print("По этому предмету у вас нет оценок.")
                return
            
            print(f"\nОценки по выбранному предмету:")
            print(f"{'ID':<5} {'Оценка':<8} {'Тип':<12} {'Дата':<12}")
            print("-" * 40)
            
            for grade in grades:
                print(f"{grade[0]:<5} {grade[1]:<8} {grade[2]:<12} {grade[3]:<12}")
            
            avg = self.db.calculate_average_grade(self.current_user.id, subject_id)
            print(f"\nСредний балл по предмету: {avg}")
            
        except ValueError:
            print("Введите корректный ID предмета!")

if __name__ == "__main__":
    app = StudentGradeSystem()
    app.main_menu()