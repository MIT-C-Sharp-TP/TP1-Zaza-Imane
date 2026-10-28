using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

List<int> wins = new List<int>();
int game = 0;

while (true)
{
    game++;

    int turns = PlayGame();

    string result = turns > 0
        ? $"Game {game}: victory in {turns} turns"
        : $"Game {game}: defeat";

    SaveResult(result);

    if (turns > 0)
    {
        wins.Add(turns);
    }

    Console.Write("Play again? (y/n) > ");

    if (Console.ReadLine()?.Trim().ToLower() != "y")
    {
        break;
    }

    Console.WriteLine();
}

Console.WriteLine();
Console.WriteLine($"Victories: {wins.Count} of {game}");

foreach (int turns in wins.OrderBy(t => t).Take(3))
{
    Console.WriteLine($"- {turns} turns");
}

int PlayGame()
{
    Player player = new Player("Aya", 100, 2);

    player.Died += name =>
        Console.WriteLine($"Game over for {name}.");

    List<Enemy> enemies = new List<Enemy>
    {
        new Goblin(),
        new Orc(),
        new Goblin(),
        new Boss()
    };

    Console.WriteLine("=== Arena ===");
    Console.WriteLine("Aya faces 4 enemies:");

    foreach (Enemy enemy in enemies)
    {
        Console.WriteLine($"- {enemy.Name}, {enemy.Health} HP");
    }

    int turn = 0;

    while (player.IsAlive && enemies.Count > 0)
    {
        turn++;

        Enemy enemy = enemies[0];

        Console.WriteLine();
        Console.WriteLine(
            $"Turn {turn}: Aya {player.Health} HP, " +
            $"{player.Potions} potions | {enemy.Name} {enemy.Health} HP");

        Console.Write("1 attack, 2 drink a potion > ");

        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int choice) ||
            (choice != 1 && choice != 2))
        {
            Console.WriteLine("Choose 1 or 2.");
            turn--;
            continue;
        }

        if (choice == 1)
        {
            player.Attack(enemy);
        }
        else
        {
            player.Heal();
        }

        if (!enemy.IsAlive)
        {
            Console.WriteLine($"{enemy.Name} is defeated!");
            enemies.RemoveAt(0);
            continue;
        }

        enemy.Attack(player);
    }

    if (player.IsAlive && enemies.Count == 0)
    {
        Console.WriteLine($"Victory in {turn} turns!");
        return turn;
    }

    Console.WriteLine("Defeat.");
    return 0;
}

void SaveResult(string line)
{
    try
    {
        File.AppendAllText(
            "results.txt",
            line + Environment.NewLine);

        Console.WriteLine($"Saved: {line}");
    }
    catch (IOException e)
    {
        Console.WriteLine(
            $"Could not save the result: {e.Message}");
    }
    catch (UnauthorizedAccessException e)
    {
        Console.WriteLine(
            $"Could not save the result: {e.Message}");
    }
}

public class Player
{
    public string Name { get; }
    public int Health { get; private set; }
    public int Potions { get; private set; }

    public bool IsAlive => Health > 0;

    public event Action<string>? Died;

    public Player(string name, int health, int potions)
    {
        Name = name;
        Health = health;
        Potions = potions;
    }

    public void Attack(Enemy enemy)
    {
        int damage = Random.Shared.Next(12, 25);
        enemy.TakeDamage(damage);
    }

    public void Heal()
    {
        if (Potions <= 0)
        {
            Console.WriteLine("No potion left!");
            return;
        }

        Potions--;
        Health = Math.Min(Health + 30, 100);

        Console.WriteLine($"Aya drinks a potion ({Health} HP)");
    }

    public void TakeDamage(int amount)
    {
        bool wasAlive = IsAlive;

        Health = Math.Max(0, Health - amount);

        Console.WriteLine(
            $"{Name} takes {amount} damage ({Health} HP)");

        if (wasAlive && !IsAlive)
        {
            Died?.Invoke(Name);
        }
    }
}

public abstract class Enemy
{
    public string Name { get; }
    public int Health { get; protected set; }

    public bool IsAlive => Health > 0;

    protected Enemy(string name, int health)
    {
        Name = name;
        Health = health;
    }

    public virtual void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);

        Console.WriteLine(
            $"{Name} takes {amount} damage ({Health} HP)");
    }

    public abstract void Attack(Player player);
}

public class Goblin : Enemy
{
    public Goblin() : base("Goblin", 30)
    {
    }

    public override void Attack(Player player)
    {
        Console.WriteLine("Goblin stabs!");

        int damage = Random.Shared.Next(4, 9);
        player.TakeDamage(damage);
    }
}

public class Orc : Enemy
{
    public Orc() : base("Orc", 60)
    {
    }

    public override void Attack(Player player)
    {
        Console.WriteLine("Orc swings its club!");

        int damage = Random.Shared.Next(8, 15);
        player.TakeDamage(damage);
    }
}

public class Boss : Enemy
{
    public int Armour { get; } = 6;

    public Boss() : base("Boss", 60)
    {
    }

    public override void TakeDamage(int amount)
    {
        int reduced = Math.Max(amount - Armour, 1);

        Console.WriteLine(
            $"The armour absorbs {amount - reduced} damage.");

        base.TakeDamage(reduced);
    }

    public override void Attack(Player player)
    {
        Console.WriteLine("Boss slams the ground!");

        int damage = Random.Shared.Next(9, 17);
        player.TakeDamage(damage);
    }
}