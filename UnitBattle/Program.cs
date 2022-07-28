using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitBattle
{
    public interface IUnit
    {
        int Id { get; }
        string Name { get; }
        int Defence { get; }
        int Attack { get; }
        int Health { get; }
        int Cost { get; }
    }

    public abstract class IUnitClass : IUnit
    {
       
        public IUnitClass(string path, string name, int id, int defence, int attack, int health, int cost) :
            base()
        {
            Name = name;
            Id = id;
            Defence = defence;
            Attack = attack;
            Health = health;
            Cost = cost;
        }

        public string Name { get; set; }
        public int Defence { get; set; }
        public int Attack { get; set; }
        public int Health { get; set; }
        public int Cost { get; set; }
        public int Id { get; set; }
        
        public void SpecialAction(NodeStack<IUnitClass> s1, NodeStack<IUnitClass> s2) {}
    }

    public class Archer : IUnitClass
    {
        public Archer(string path) : base(path, "Archer", 0, 10, 25, 20, 20) { }
        public void SpecialAction(NodeStack<IUnitClass> enemies, NodeStack<IUnitClass> friends)
        {
            foreach(var i in enemies)
            {
                i.Health -= 15;
            }
        }
    }

    public class Knight : IUnitClass
    {
        public Knight(string path) : base(path, "Knight", 1, 50, 30, 100, 30) { }
        
    }

    public class Swordsman : IUnitClass
    {
        public Swordsman(string path) : base(path, "Swordsman", 2, 20, 20, 50, 15) { }
        public void SpecialAction(NodeStack<IUnitClass> enemies, NodeStack<IUnitClass> friends)
        {
            foreach(var u in friends)
            {
                if(u.Name == "Knight")
                {
                    u.Attack += 10;
                }
            }
        }
    }

    public class Healer : IUnitClass
    {
        public Healer(string path) : base(path, "Healer", 3, 5, 5, 30, 35) { }
        public void SpecialAction(NodeStack<IUnitClass> enemies, NodeStack<IUnitClass> friends)
        {
            friends.Peek().Health += 5;
        }
    }

    public class Node<IUnitClass>
    {
        public Node(IUnitClass data)
        {
            Data = data;
        }
        public IUnitClass Data { get; set; }
        public Node<IUnitClass> Next { get; set; }
    }

    public class NodeStack<IUnitClass> : IEnumerable<IUnitClass>
    {
        Node<IUnitClass> head;
        int count;

        public bool IsEmpty
        {
            get { return count == 0; }
        }
        public int Count
        {
            get { return count; }
        }

        public void Push(IUnitClass item)
        {
            // увеличиваем стек
            Node<IUnitClass> node = new Node<IUnitClass>(item);
            node.Next = head; // переустанавливаем верхушку стека на новый элемент
            head = node;
            count++;
        }
        public void Pop()
        {
            // если стек пуст, выбрасываем исключение
            if (IsEmpty)
                throw new InvalidOperationException("Стек пуст");
            head = head.Next; // переустанавливаем верхушку стека на следующий элемент
            count--;
        }
        public IUnitClass Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Стек пуст");
            return head.Data;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this).GetEnumerator();
        }

        IEnumerator<IUnitClass> IEnumerable<IUnitClass>.GetEnumerator()
        {
            Node<IUnitClass> current = head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }
    }

    class Program
    {
        static void ArmyBuilder(int val, NodeStack<IUnitClass> stack)
        {
            switch (val)
            {
                case 0:
                    stack.Push(new Archer("Archer"));
                    break;
                case 1:
                    stack.Push(new Knight("Knight"));
                    break;
                case 2:
                    stack.Push(new Swordsman("Swordsman"));
                    break;
                case 3:
                    stack.Push(new Healer("Healer"));
                    break;
            }
        }

        static int AttackPower(int attack1, int defence2)
        {
            return attack1 - (int)((double)attack1 / 100 * (double)defence2);
        }
        
        static void AbilityOn(NodeStack<IUnitClass> first, NodeStack<IUnitClass> second)
        {
            foreach(var u in first)
            {
                u.SpecialAction(second, first);
            }

            foreach (var u in second)
            {
                u.SpecialAction(first, second);
            }
        }
        static void Main(string[] args)
        {
            Random random = new Random();
            Random WhoseStep = new Random();

            NodeStack<IUnitClass> stack1 = new NodeStack<IUnitClass>();
            NodeStack<IUnitClass> stack2 = new NodeStack<IUnitClass>();
            
            for(int i = 0; i < 5; i++)
            {
                int val1 = random.Next(0, 4);
                ArmyBuilder(val1, stack1);
            }

            for (int j = 0; j < 5; j++)
            {
                int val2 = random.Next(0, 4);
                ArmyBuilder(val2, stack2);
            }
            
            foreach (var item in stack1)
            {
                Console.WriteLine(item.Name + " Attack " + item.Attack + " Health " + item.Health + " Def " + item.Defence);
            }

            Console.WriteLine();

            foreach (var item in stack2)
            {
                Console.WriteLine(item.Name + " Attack " + item.Attack + " Health " + item.Health + " Def " + item.Defence);
            }

            int step = 0;

            while (stack1.IsEmpty == false && stack2.IsEmpty == false)
            {
                step = WhoseStep.Next(0, 2);

                if (step == 0)
                {
                    stack2.Peek().Health -= AttackPower(stack1.Peek().Attack, stack2.Peek().Defence);

                    if (stack2.Peek().Health <= 0)
                    {
                        stack2.Pop();
                    }
                    else
                    {
                        stack1.Peek().Health -= AttackPower(stack2.Peek().Attack, stack1.Peek().Defence);

                        if (stack1.Peek().Health <= 0)
                        {
                            stack1.Pop();
                        }
                    }
                    AbilityOn(stack1, stack2);
                }
                else
                {
                    stack1.Peek().Health -= AttackPower(stack2.Peek().Attack, stack1.Peek().Defence);

                    if (stack1.Peek().Health <= 0)
                    {
                        stack1.Pop();
                    }
                    else
                    {
                        stack2.Peek().Health -= AttackPower(stack1.Peek().Attack, stack2.Peek().Defence);
                        if (stack2.Peek().Health <= 0)
                        {
                            stack2.Pop();
                        }
                    }
                    AbilityOn(stack2, stack1);
                }
            }

            if (stack1.Count == 0)
            {
                Console.WriteLine("Player2 win!");
            }
            if(stack2.Count == 0)
            {
                Console.WriteLine("Player1 win!");
            }
            Console.ReadKey();
        }
    }
}
