using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class DeadLockDetectorGraph
    {
        private Dictionary<Task, Task> transition = new Dictionary<Task, Task>();

        public DeadLockDetectorGraph()
        {

        }

        public void AddTransition(Task task1, Task task2)
        {
            transition.Add(task1, task2);
            if (HasCycle(task1))
            {
                transition.Remove(task1);
                //Console.WriteLine("Deadlock occures!");
                task1.deadLockDetected = true;
                throw new Exception("Deadlock prevention!");
            }
        }

        public void removeTransition(Task task)
        {
            transition.Remove(task);
        }

        private bool HasCycle(Task task)
        {
            HashSet<Task> visited = new HashSet<Task>();
            Stack<Task> cycleStack = new Stack<Task>();
            return dfs(visited, cycleStack, task);
        }

        private bool dfs(HashSet<Task> visited, Stack<Task> recursion, Task task)
        {
            visited.Add(task);
            recursion.Push(task);
            Task? nextTask = null;
            bool status = transition.TryGetValue(task, out nextTask);
            if (status)
            {
                if (!visited.Contains(nextTask) && dfs(visited, recursion, nextTask))
                {
                    return true; // deadlock detected
                }
                else if (recursion.Contains(nextTask))
                {
                    return true; // deadlock detected
                }
            }
            recursion.Pop();
            return false;
        }
    }
}
