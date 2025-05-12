-- Основная информация о сотрудниках

CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    department VARCHAR(100),
    balance NUMERIC(15,2) DEFAULT 0
);

employee_id	Уникальный идентификатор сотрудника (первичный ключ)
name	ФИО сотрудника 
department	Название отдела, где работает сотрудник
balance	Текущий остаток подотчетных средств у сотрудника (по умолчанию 0)

-- Учет выданных сумм

CREATE TABLE advances (
    id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL,
    date DATE NOT NULL,
    sum NUMERIC(15,2) NOT NULL,
    reported BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);

id	Уникальный идентификатор выдачи (первичный ключ)
employee_id	Ссылка на сотрудника 
date	Дата выдачи денег
sum	Сумма выданного аванса
reported	Статус отчета (FALSE — не отчитался, TRUE — отчитался)


-- Учет предоставленных отчетов

CREATE TABLE reports (
    id SERIAL PRIMARY KEY,
    advance_id INT NOT NULL,
    date DATE NOT NULL,
    sum NUMERIC(15,2) NOT NULL,
    FOREIGN KEY (advance_id) REFERENCES advances(id)
);

id	Уникальный идентификатор отчета (первичный ключ)
advance_id	Ссылка на выданный аванс 
date	Дата подачи отчета
sum	Общая сумма расходов по отчету

-- Детализация расходов по авансовому отчету

CREATE TABLE expenses (
    id SERIAL PRIMARY KEY,
    report_id INT NOT NULL,
    category VARCHAR(100) NOT NULL,
    quantity NUMERIC(10,2) NOT NULL, 
    sum NUMERIC(15,2) NOT NULL,
    FOREIGN KEY (report_id) REFERENCES reports(id)
);

id	Уникальный идентификатор строки расхода (первичный ключ)
report_id	Ссылка на отчет 
category	Категория расхода (например, "Гостиница", "Транспорт")
quantity	Количество (например, 2 ночи в гостинице)
sum	Сумма расхода по данной статье

-- История остатков для отчетности

CREATE TABLE balances (
    id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL,
    month CHAR(7) NOT NULL, -- YYYY-MM
    end_balance NUMERIC(15,2) NOT NULL,
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);

id	Уникальный идентификатор записи (первичный ключ)
employee_id	Ссылка на сотрудника 
month	Месяц и год в формате YYYY-MM 
end_balance	Остаток средств у сотрудника на конец месяца

-- Заполнение таблицы employees
INSERT INTO employees (name, department, balance) VALUES
('Иванов Иван Иванович', 'Отдел продаж', 25000.00),
('Петрова Светлана Викторовна', 'Бухгалтерия', 0.00),
('Сидоров Алексей Дмитриевич', 'ИТ-отдел', 15000.00),
('Кузнецова Елена Сергеевна', 'Отдел маркетинга', 5000.00),
('Васильев Дмитрий Петрович', 'Отдел логистики', 30000.00);

-- Заполнение таблицы advances
INSERT INTO advances (employee_id, date, sum, reported) VALUES
(1, '2025-05-10', 20000.00, TRUE),
(1, '2025-05-20', 15000.00, FALSE),
(2, '2025-05-15', 10000.00, TRUE),
(3, '2025-05-05', 25000.00, TRUE),
(4, '2025-05-12', 8000.00, FALSE),
(5, '2025-05-18', 20000.00, TRUE);

-- Заполнение таблицы reports
INSERT INTO reports (advance_id, date, sum) VALUES
(1, '2024-05-17', 18500.00),
(3, '2024-05-22', 9500.00),
(4, '2024-05-15', 23000.00),
(6, '2024-05-25', 19500.00);

-- Заполнение таблицы expenses
INSERT INTO expenses (report_id, category, quantity, sum) VALUES
(1, 'Командировочные расходы', 1, 12000.00),
(1, 'Транспорт', 4, 4500.00),
(1, 'Представительские расходы', 1, 2000.00),
(2, 'Офисные расходы', 1, 9500.00),
(3, 'Гостиница', 3, 15000.00),
(3, 'Питание', 5, 5000.00),
(3, 'Транспорт', 2, 3000.00),
(4, 'Логистика', 1, 15000.00),
(4, 'Упаковка', 100, 4500.00);

-- Заполнение таблицы balances
INSERT INTO balances (employee_id, month, end_balance) VALUES
(1, '2025-04', 5000.00),
(2, '2025-04', 0.00),
(3, '2025-04', 10000.00),
(4, '2025-04', 2000.00),
(5, '2025-04', 25000.00),
(1, '2025-05', 6500.00),
(2, '2025-05', 500.00),
(3, '2025-05', 2000.00),
(4, '2025-05', 5000.00),
(5, '2025-05', 30500.00);
