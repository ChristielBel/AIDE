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
