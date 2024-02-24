using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Newtonsoft.Json;
using Scheduler;
using TaskSchedulerDemo;

/* FIFO RASPOREDJIVAC */

/*
FifoScheduler fifoScheduler = new FifoScheduler(1);
SimpleTask firstTask = new SimpleTask(15, "A");
firstTask.durationTime = 3000;
firstTask.endTime = new DateTime(2023, 7, 3, 20, 51, 0);
fifoScheduler.Add(firstTask);
SimpleTask secondTask = new SimpleTask(12, "B");
secondTask.durationTime = 5000;
secondTask.endTime = new DateTime(2023, 7, 3, 20, 34, 0);
fifoScheduler.Add(secondTask);
SimpleTask thirdTask = new SimpleTask(12, "C");
thirdTask.durationTime = 5000;
thirdTask.endTime = new DateTime(2023, 7, 3, 20, 34, 0);
fifoScheduler.Add(thirdTask);
SimpleTask fourthTask = new SimpleTask(12, "D");
fourthTask.durationTime = 5000;
fourthTask.endTime = new DateTime(2023, 7, 3, 20, 34, 0);
fifoScheduler.Add(fourthTask);
fifoScheduler.StartScheduler();*/

/* PRIMJER RADA SA POKRETANJEM NA ZADATO VRIJEME */

/* PRIMJER CEKANJA VREMENA ZA ZAPOCINJANJE
DateTime start = new DateTime(2023, 7, 6, 11, 29, 30);

TimeSpan delay = start - DateTime.Now;
if (delay > TimeSpan.Zero)
{
    Thread.Sleep(delay);
}
Console.WriteLine("Pocinjemo!");
*/

/*
FifoScheduler fifoScheduler = new FifoScheduler(1);
SimpleTask firstTask = new SimpleTask(15, "A");
firstTask.startTime = new DateTime(2023, 9, 17, 13, 49, 0);
firstTask.durationTime = 3000;
firstTask.endTime = new DateTime(2023, 9, 17, 20, 51, 0);
fifoScheduler.waitStartTime(firstTask);*/

/* PRIORITETNI RASPOREDJIVAC */
/*
PriorityScheduler priorityScheduler = new PriorityScheduler(1);
SimpleTask firstTask = new SimpleTask(1, "A");
firstTask.priority = 1;
priorityScheduler.Schedule(firstTask);
SimpleTask secondTask = new SimpleTask(12, "B");
secondTask.priority = -5;
priorityScheduler.Schedule(secondTask);
SimpleTask thirdTask = new SimpleTask(12, "C");
thirdTask.priority = 3;
priorityScheduler.Schedule(thirdTask);
SimpleTask fourthTask = new SimpleTask(12, "D");
fourthTask.priority = 0;
priorityScheduler.Schedule(fourthTask);
*/

/* RASPOREDJIVAC SA PREUZIMANJEM(PREEMPTIVE) */

/*
PreemptiveScheduler preemptiveScheduler = new PreemptiveScheduler(1);
SimpleTask firstTask = new SimpleTask(8, "A");
firstTask.priority = 1;
preemptiveScheduler.Schedule(firstTask);
SimpleTask secondTask = new SimpleTask(12, "B");
secondTask.priority = -5;
preemptiveScheduler.Schedule(secondTask);
SimpleTask thirdTask = new SimpleTask(12, "C");
thirdTask.priority = 3;
preemptiveScheduler.Schedule(thirdTask);
SimpleTask fourthTask = new SimpleTask(12, "D");
fourthTask.priority = 0;
preemptiveScheduler.Schedule(fourthTask);
SimpleTask fifthTask = new SimpleTask(10, "E");
fifthTask.priority = -10;
preemptiveScheduler.Schedule(fifthTask);
*/


/* ROUND ROBIN RAPOREDJIVANJE */

/*
RoundRobinScheduler roundRobinScheduler = new RoundRobinScheduler(2);
SimpleTask firstTask = new SimpleTask(10, "A");
firstTask.priority = 1;
roundRobinScheduler.Schedule(firstTask);
SimpleTask secondTask = new SimpleTask(10, "B");
secondTask.priority = 5;
roundRobinScheduler.Schedule(secondTask);
SimpleTask thirdTask = new SimpleTask(10, "C");
thirdTask.priority = 8;
roundRobinScheduler.Schedule(thirdTask);
SimpleTask fourthTask = new SimpleTask(10, "D");
fourthTask.priority = 0;
roundRobinScheduler.Schedule(fourthTask);


Resource resource = new Resource();*/


/* UPOTREBA SEMAPHORESLIM */
/*
SemaphoreSlim semaphore = new SemaphoreSlim(1);

new Thread(() => metoda()).Start();
new Thread(() => metoda()).Start();
new Thread(() => metoda()).Start();

// ovo je dio koda koji omogucava samo jednoj niti da ga obradjuje, ostale niti se zaustavljaju kod Wait() i cekaju da
// neka nit koja je unutar koda pozove Release();


void metoda()
{
    semaphore.Wait();

    try
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(i.ToString());
        }
    }
    finally
    {
        semaphore.Release();
    }
}*/


/*
Task task = metoda();


async Task metoda()
{
    Console.WriteLine("Async 1");
    await metoda2();
    Console.WriteLine("Async 2");
}

async Task metoda2()
{
    Console.WriteLine("Await1");
    for (int i=0; i < 10; i++)
    {
        Console.WriteLine(i);
    }
}*/


/* PRIORITETNI RED KAKO RADI(PO DEFAULT-U IDE 1,2,3,4,5)
PriorityQueue<string, int> pomocna = new PriorityQueue<string, int>(Comparer<int>.Create((x, y) => y - x));
pomocna.Enqueue("prvi", 1);
pomocna.Enqueue("drugi", 45);
pomocna.Enqueue("treci", 33);
pomocna.Enqueue("cetvrti", 5);
pomocna.Enqueue("peti", 1456);
pomocna.Enqueue("sesti", 8);
pomocna.Enqueue("sedmi", 9);

while(pomocna.Count > 0)
{
    Console.WriteLine(pomocna.Dequeue());
}*/

//DEADLOCK PRIMJER 

/*
Resource r1 = new FileResource("prviResurs");
Resource r2 = new FileResource("drugiResurs");
SampleTask task1 = new SampleTask();
SampleTask task2 = new SampleTask();
task1.InitializeThread(r1, r2, 3000);
task2.InitializeThread(r2, r1, 2000);
task1.Run();
task2.Run(); */

/*Thread task1 = new Thread(() =>
{
    r1.Lock();
    r1.Release();
    Thread.Sleep(4000);
    r2.Lock();
    r2.Release();
    Console.WriteLine("Izvrsio se prvi zadatak1");
});

Thread task2 = new Thread(() =>
{
    r2.Lock();
    r2.Release();
    Thread.Sleep(1000);
    r1.Lock();
    r1.Release();
    Console.WriteLine("Izvrsio se drugi zadatak2");
});

task1.Start();
task2.Start();*/

// IMAGE SHARPEINING

/*
List<Resource> resources = new();
resources.Add(new FileResource("C:\\Users\\aleks\\Desktop\\mountine.png"));
//resources.Add(new FileResource("C:\\Users\\aleks\\Desktop\\car2.png"));
ImageSharpeningTask task = new ImageSharpeningTask(resources, "C:\\Users\\aleks\\Desktop\\sharpned", 5);
task.Run();
*/

/*
List<Resource> resources = new();
resources.Add(new FileResource("C:\\Users\\aleks\\Desktop\\car.png"));
resources.Add(new FileResource("C:\\Users\\aleks\\Desktop\\mountine.png"));
ImageSharpeningTask task = new ImageSharpeningTask(resources, "C:\\Users\\aleks\\Desktop\\sharpned", 4);
task.Run();
*/

// SERIJALIZACIJA JE OMOGUCENA UPOTREBOM JSONPROPETY ZA PRIVATNE CLANOVE

// SERIJALIZACIJA //

/*
SpecificClass pom1 = new SpecificClass("pom1", "pom1pom1", 12);
string jsonString = JsonConvert.SerializeObject(pom1, Formatting.Indented, new JsonSerializerSettings
{
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    TypeNameHandling = TypeNameHandling.All
});
Console.WriteLine(jsonString);

//DESERIJALIZACIJA

SpecificClass deserializedPom1 = JsonConvert.DeserializeObject<SpecificClass>(jsonString);
Console.WriteLine(deserializedPom1.ToString());*/

// PRIMJER SERIJALIZACIJE/DESERIJALIZACIJE SA ZADATKOM
/*
SimpleTask task = new SimpleTask(100, "Zadatak1");
task.Start();
Thread.Sleep(5000);
task.jobState = Scheduler.Task.JobState.Finished;
task.Serialize();

SimpleTask desTask = (SimpleTask) task.Deserialize();
desTask.jobState = Scheduler.Task.JobState.NotStarted;
desTask.Start();
*/
// SERIJALIZACIJA SCHEDULER-A
/*
FifoScheduler fifo = new FifoScheduler(1);
fifo.Schedule(new SimpleTask(100, "1"));
fifo.Schedule(new SimpleTask(100, "2"));
fifo.Schedule(new SimpleTask(100, "3"));
Thread.Sleep(1000);
fifo.Serialize();
Thread.Sleep(2000);
Scheduler.TaskScheduler fifo1 = Scheduler.TaskScheduler.Deserialize();
if (fifo1.GetType() == typeof(FifoScheduler)) Console.WriteLine("Taj je tip!");*/


/*
FifoScheduler fifo = new FifoScheduler(1);
SimpleTask task1 = new SimpleTask(10, "task1");
SimpleTask task2 = new SimpleTask(10, "task2");
fifo.Add(task1);
fifo.Add(task2);
fifo.StartScheduler();*/

/*RESOURCES BLOCKING AND UNBLOCKING*/

/*
PreemptiveScheduler fifo = new PreemptiveScheduler(1); // set >1 in order to change priority of task
SimpleTask task1 = new SimpleTask(10, "A"); task1.priority = 10;
SimpleTask task2 = new SimpleTask(10, "B"); task2.priority = 2;
SimpleTask task3 = new SimpleTask(10, "C"); task3.priority = 1;
Resource r = new FileResource("path");
task1.AddResource(r);
task2.AddResource(r);
task3.AddResource(r);
fifo.Schedule(task1);
Thread.Sleep(200);
fifo.Schedule(task2);
fifo.Schedule(task3);
Thread.Sleep(1000);
Console.WriteLine(task1.priority);
*/
