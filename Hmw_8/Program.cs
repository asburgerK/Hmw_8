using System;
using System.Collections.Generic;
using System.Linq;
enum WorkStatus
{
    Assigned,     // Назначена
    InProgress,   // В работе
    OnReview,     // На проверке
    Completed     // Выполнена
}

enum ProjectStatus
{
    Draft,        // Проект
    Execution,    // Исполнение
    Closed        // Закрыт
}

class Person
{
    public int Id { get; }
    public string Name { get; }

    public Person(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => $"{Name} (id={Id})";
}


class Report
{
    public string Text { get; set; }
    public DateTime CompletedAt { get; set; }
    public Person Executor { get; set; }
}

class Task
{
    public int Id { get; }
    public string Description { get; set; }
    public DateTime Deadline { get; set; }
    public Person Initiator { get; set; }        // кто создал (тимлид)
    public Person Worker { get; private set; }   // исполнитель
    public WorkStatus Status { get; private set; } = WorkStatus.Assigned;
    public Report Report { get; private set; }
    public Project Project { get; }

    public Task(int id, string description, DateTime deadline, Person initiator, Project project)
    {
        Id = id;
        Description = description;
        Deadline = deadline;
        Initiator = initiator;
        Project = project;
    }

    // тимлид назначает/переназначает
    public void Assign(Person worker)
    {
        if (Status != WorkStatus.Assigned)
            throw new InvalidOperationException("Назначить можно только задачу в статусе 'Назначена'");
        Worker = worker;
    }

    // исполнитель берёт в работу
    public void Start(Person worker)
    {
        EnsureWorker(worker);
        if (Status != WorkStatus.Assigned)
            throw new InvalidOperationException("Взять в работу можно только назначенную задачу");
        Status = WorkStatus.InProgress;
    }

    // исполнитель делегирует
    public void Delegate(Person worker, Person newWorker)
    {
        EnsureWorker(worker);
        if (Status != WorkStatus.Assigned)
            throw new InvalidOperationException("Делегировать можно только назначенную задачу");
        Worker = newWorker; // статус остаётся Assigned
    }

    // исполнитель отклоняет
    public void Reject(Person worker)
    {
        EnsureWorker(worker);
        if (Status != WorkStatus.Assigned)
            throw new InvalidOperationException("Отклонить можно только назначенную задачу");
        Worker = null;
    }

    // сдать отчёт
    public Report SubmitReport(string text)
    {
        if (Status != WorkStatus.InProgress)
            throw new InvalidOperationException("Отчёт сдаётся только из статуса 'В работе'");
        if (Report != null)
            throw new InvalidOperationException("По разовой задаче создаётся один отчёт");

        Report = new Report
        {
            Text = text,
            CompletedAt = DateTime.Now,
            Executor = Worker
        };
        Status = WorkStatus.OnReview;
        return Report;
    }

    // инициатор утверждает
    public void ApproveReport()
    {
        if (Status != WorkStatus.OnReview)
            throw new InvalidOperationException("Утвердить можно только отчёт на проверке");
        Status = WorkStatus.Completed;
    }

    // инициатор возвращает на доработку
    public void ReturnForRevision()
    {
        if (Status != WorkStatus.OnReview)
            throw new InvalidOperationException("Вернуть можно только отчёт на проверке");
        Report = null;
        Status = WorkStatus.InProgress;
    }

    private void EnsureWorker(Person worker)
    {
        if (Worker == null || Worker.Id != worker.Id)
            throw new InvalidOperationException($"{worker.Name} не является исполнителем задачи");
    }
}

class Project
{
    public int Id { get; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DeadLine { get; set; }
    public Person Customer { get; set; }      
    public Person ProjectLead { get; set; }   
    public ProjectStatus Status { get; private set; } = ProjectStatus.Draft;

    public List<Task> Tasks { get; } = new();
    private int _nextTaskId = 1;

    public Project(int id, string description, DateTime start, DateTime deadline,
                   Person customer, Person projectLead)
    {
        Id = id;
        Description = description;
        StartDate = start;
        DeadLine = deadline;
        Customer = customer;
        ProjectLead = projectLead;
    }

   public Task CreateTask(string description, DateTime deadline, Person initiator)
    {
        if (Status != ProjectStatus.Draft)
            throw new InvalidOperationException("Задачи создаются только в статусе 'Проект'");
        if (initiator.Id != ProjectLead.Id)
            throw new InvalidOperationException("Создавать задачи может только тимлид проекта");

        var task = new Task(_nextTaskId++, description, deadline, initiator, this);
        Tasks.Add(task);
        return task;
    }

    public void RemoveTask(Task task)
    {
        if (task.Worker != null)
            throw new InvalidOperationException("Нельзя удалить задачу с исполнителем");
        Tasks.Remove(task);
    }


    public void StartExecution()
    {
        if (Status != ProjectStatus.Draft)
            throw new InvalidOperationException("Перевести в исполнение можно только из статуса 'Проект'");
        if (Tasks.Count == 0)
            throw new InvalidOperationException("Нельзя начать проект без задач");
        if (Tasks.Any(t => t.Worker == null))
            throw new InvalidOperationException("Все задачи должны быть назначены");
        Status = ProjectStatus.Execution;
    }

    public void TryClose()
    {
        if (Status != ProjectStatus.Execution)
            throw new InvalidOperationException("Закрыть можно только проект в исполнении");
        if (Tasks.Any(t => t.Status != WorkStatus.Completed))
            throw new InvalidOperationException("Не все задачи выполнены");
        Status = ProjectStatus.Closed;
    }
}

class TaskManager
{
    public List<Person> Team { get; } = new();
    public List<Project> Projects { get; } = new();

    public Person AddPerson(string name)
    {
        var p = new Person(Team.Count + 1, name);
        Team.Add(p);
        return p;
    }

    public Project AddProject(string description, DateTime start, DateTime deadline,
                              Person customer, Person projectLead)
    {
        var p = new Project(Projects.Count + 1, description, start, deadline, customer, projectLead);
        Projects.Add(p);
        return p;
    }
}