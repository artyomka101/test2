class User:
    def __init__(self, id=None, username=None, password=None, full_name=None, group_name=None):
        self.id = id
        self.username = username
        self.password = password
        self.full_name = full_name
        self.group_name = group_name

class Subject:
    def __init__(self, id=None, name=None, credits=None, semester=None, teacher=None):
        self.id = id
        self.name = name
        self.credits = credits
        self.semester = semester
        self.teacher = teacher

class Grade:
    def __init__(self, id=None, user_id=None, subject_id=None, grade=None, 
                 grade_type=None, date_received=None):
        self.id = id
        self.user_id = user_id
        self.subject_id = subject_id
        self.grade = grade
        self.grade_type = grade_type
        self.date_received = date_received