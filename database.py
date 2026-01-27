import sqlite3
import os
from models import User, Subject, Grade

class Database:
    def __init__(self, db_path='students.db'):
        if not self._can_write_to_directory('.'):
            import tempfile
            temp_dir = tempfile.gettempdir()
            db_path = os.path.join(temp_dir, 'students.db')
            print(f"Используется временная папка для БД: {temp_dir}")
        
        self.db_path = os.path.abspath(db_path)
        print(f"Путь к БД: {self.db_path}")
        self.init_database()
    
    def _can_write_to_directory(self, path):
        try:
            test_file = os.path.join(path, 'test_write.tmp')
            with open(test_file, 'w') as f:
                f.write('test')
            os.remove(test_file)
            return True
        except:
            return False
    
    def init_database(self):
        try:
            db_dir = os.path.dirname(self.db_path)
            if not os.path.exists(db_dir):
                print(f"Создаём директорию: {db_dir}")
                os.makedirs(db_dir, exist_ok=True)
            
            if not self._can_write_to_directory(db_dir):
                raise PermissionError(f"Нет прав для записи в директорию: {db_dir}")
            
            current_dir = os.path.dirname(os.path.abspath(__file__))
            schema_path = os.path.join(current_dir, 'schema.sql')
            
            print("Создаём подключение к БД...")
            with sqlite3.connect(self.db_path) as conn:
                cursor = conn.cursor()
                cursor.execute("SELECT name FROM sqlite_master WHERE type='table'")
                existing_tables = cursor.fetchall()
                
                if existing_tables:
                    print(f"БД уже существует с таблицами: {[t[0] for t in existing_tables]}")
                    return
                
                if os.path.exists(schema_path):
                    print(f"Используем schema.sql: {schema_path}")
                    with open(schema_path, 'r', encoding='utf-8') as f:
                        conn.executescript(f.read())
                    conn.commit()
                    print("База данных успешно инициализирована из schema.sql")
                else:
                    print("schema.sql не найден, создаём минимальную БД...")
                    self._create_minimal_db_direct(conn)
                    
        except PermissionError as e:
            print(f"Ошибка прав доступа: {e}")
            print("Попробуйте запустить от имени администратора или переместить проект в папку пользователя")
        except Exception as e:
            print(f"Ошибка инициализации БД: {e}")
            print("Попробуем создать БД в памяти для демонстрации...")
            self._create_memory_db()
    
    def _create_minimal_db_direct(self, conn):
        cursor = conn.cursor()
        
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username VARCHAR(50) UNIQUE NOT NULL,
                password VARCHAR(100) NOT NULL,
                full_name VARCHAR(100) NOT NULL,
                group_name VARCHAR(20) NOT NULL,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            )
        ''')
        
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS subjects (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name VARCHAR(100) NOT NULL,
                credits INTEGER NOT NULL DEFAULT 3,
                semester INTEGER NOT NULL,
                teacher VARCHAR(100) NOT NULL
            )
        ''')
        
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS grades (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id INTEGER NOT NULL,
                subject_id INTEGER NOT NULL,
                grade INTEGER NOT NULL CHECK (grade >= 2 AND grade <= 5),
                grade_type VARCHAR(20) NOT NULL DEFAULT 'exam',
                date_received DATE NOT NULL DEFAULT CURRENT_DATE,
                FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
                FOREIGN KEY (subject_id) REFERENCES subjects(id) ON DELETE CASCADE
            )
        ''')
        
        cursor.execute('CREATE INDEX IF NOT EXISTS idx_grades_user_id ON grades(user_id)')
        cursor.execute('CREATE INDEX IF NOT EXISTS idx_grades_subject_id ON grades(subject_id)')
        
        cursor.execute('''
            INSERT OR IGNORE INTO subjects (name, credits, semester, teacher) VALUES
            ('Математический анализ', 4, 1, 'Иванов И.И.'),
            ('Программирование', 3, 1, 'Петров П.П.'),
            ('Физика', 3, 1, 'Сидоров С.С.'),
            ('Базы данных', 4, 2, 'Козлов К.К.'),
            ('Алгоритмы и структуры данных', 3, 2, 'Новиков Н.Н.')
        ''')
        
        cursor.execute('''
            INSERT OR IGNORE INTO users (username, password, full_name, group_name) VALUES
            ('student1', '123456', 'Иванов Иван Иванович', 'ИТ-21'),
            ('student2', '123456', 'Петрова Мария Сергеевна', 'ИТ-21'),
            ('student3', '123456', 'Сидоров Алексей Петрович', 'ИТ-22')
        ''')
        
        cursor.execute('''
            INSERT OR IGNORE INTO grades (user_id, subject_id, grade, grade_type, date_received) VALUES
            (1, 1, 5, 'exam', '2024-01-15'),
            (1, 2, 4, 'test', '2024-01-20'),
            (1, 3, 5, 'homework', '2024-01-25'),
            (2, 1, 4, 'exam', '2024-01-15'),
            (2, 2, 5, 'test', '2024-01-20'),
            (3, 1, 3, 'exam', '2024-01-15'),
            (3, 2, 4, 'test', '2024-01-20')
        ''')
        
        conn.commit()
        print("Минимальная база данных создана успешно с тестовыми данными")
    
    def _create_memory_db(self):
        print("Создаём БД в памяти для демонстрации...")
        self.db_path = ':memory:'
        try:
            with sqlite3.connect(self.db_path) as conn:
                self._create_minimal_db_direct(conn)
                print("БД в памяти создана успешно (данные не сохранятся после закрытия)")
        except Exception as e:
            print(f"Критическая ошибка: не удалось создать даже БД в памяти: {e}")
    
    def create_minimal_db(self):
        try:
            with sqlite3.connect(self.db_path) as conn:
                self._create_minimal_db_direct(conn)
        except Exception as e:
            print(f"Ошибка создания минимальной БД: {e}")
            print("Попробуем создать БД в памяти...")
            self._create_memory_db()
    
    def authenticate_user(self, username, password):
        try:
            with sqlite3.connect(self.db_path) as conn:
                cursor = conn.cursor()
                cursor.execute(
                    "SELECT id, username, full_name, group_name FROM users WHERE username = ? AND password = ?",
                    (username, password)
                )
                result = cursor.fetchone()
                if result:
                    return User(id=result[0], username=result[1], 
                              full_name=result[2], group_name=result[3])
                return None
        except Exception as e:
            print(f"Ошибка аутентификации: {e}")
            return None
    
    def register_user(self, username, password, full_name, group_name):
        try:
            with sqlite3.connect(self.db_path) as conn:
                cursor = conn.cursor()
                cursor.execute(
                    "INSERT INTO users (username, password, full_name, group_name) VALUES (?, ?, ?, ?)",
                    (username, password, full_name, group_name)
                )
                conn.commit()
                return True
        except sqlite3.IntegrityError:
            print("Пользователь с таким именем уже существует")
            return False
        except Exception as e:
            print(f"Ошибка регистрации: {e}")
            return False
    
    def get_subjects(self, semester=None):
        try:
            with sqlite3.connect(self.db_path) as conn:
                cursor = conn.cursor()
                if semester:
                    cursor.execute("SELECT * FROM subjects WHERE semester = ?", (semester,))
                else:
                    cursor.execute("SELECT * FROM subjects")
                
                subjects = []
                for row in cursor.fetchall():
                    subjects.append(Subject(
                        id=row[0], name=row[1], credits=row[2], 
                        semester=row[3], teacher=row[4]
                    ))
                return subjects
        except Exception as e:
            print(f"Ошибка получения предметов: {e}")
            return []
    
    def add_grade(self, user_id, subject_id, grade, grade_type='exam'):
        try:
            with sqlite3.connect(self.db_path) as conn:
                cursor = conn.cursor()
                cursor.execute(
                    "INSERT INTO grades (user_id, subject_id, grade, grade_type) VALUES (?, ?, ?, ?)",
                    (user_id, subject_id, grade, grade_type)
                )
                conn.commit()
                return True
        except Exception as e:
            print(f"Ошибка добавления оценки: {e}")
            return False
    
    def get_user_grades(self, user_id, subject_id=None):
        try:
            with sqlite3.connect(self.db_path) as conn:
                cursor = conn.cursor()
                if subject_id:
                    query = """
                    SELECT g.id, g.grade, g.grade_type, g.date_received, s.name 
                    FROM grades g 
                    JOIN subjects s ON g.subject_id = s.id 
                    WHERE g.user_id = ? AND g.subject_id = ?
                    ORDER BY g.date_received DESC
                    """
                    cursor.execute(query, (user_id, subject_id))
                else:
                    query = """
                    SELECT g.id, g.grade, g.grade_type, g.date_received, s.name 
                    FROM grades g 
                    JOIN subjects s ON g.subject_id = s.id 
                    WHERE g.user_id = ?
                    ORDER BY g.date_received DESC
                    """
                    cursor.execute(query, (user_id,))
                
                return cursor.fetchall()
        except Exception as e:
            print(f"Ошибка получения оценок: {e}")
            return []
    
    def calculate_average_grade(self, user_id, subject_id=None):
        try:
            with sqlite3.connect(self.db_path) as conn:
                cursor = conn.cursor()
                if subject_id:
                    cursor.execute(
                        "SELECT AVG(grade) FROM grades WHERE user_id = ? AND subject_id = ?",
                        (user_id, subject_id)
                    )
                else:
                    cursor.execute(
                        "SELECT AVG(grade) FROM grades WHERE user_id = ?",
                        (user_id,)
                    )
                
                result = cursor.fetchone()[0]
                return round(result, 2) if result else 0.0
        except Exception as e:
            print(f"Ошибка расчёта среднего балла: {e}")
            return 0.0
    
    def update_grade(self, grade_id, new_grade):
        try:
            with sqlite3.connect(self.db_path) as conn:
                cursor = conn.cursor()
                cursor.execute(
                    "UPDATE grades SET grade = ? WHERE id = ?",
                    (new_grade, grade_id)
                )
                conn.commit()
                return cursor.rowcount > 0
        except Exception as e:
            print(f"Ошибка обновления оценки: {e}")
            return False
    
    def delete_grade(self, grade_id):
        try:
            with sqlite3.connect(self.db_path) as conn:
                cursor = conn.cursor()
                cursor.execute("DELETE FROM grades WHERE id = ?", (grade_id,))
                conn.commit()
                return cursor.rowcount > 0
        except Exception as e:
            print(f"Ошибка удаления оценки: {e}")
            return False