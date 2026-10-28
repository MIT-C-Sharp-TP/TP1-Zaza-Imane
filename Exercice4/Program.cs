using System;
using System.Collections.Generic;
int Heal(int health, int amount, int maxHealth)
{
    return Math.Min(health + amount, maxHealth);
}

Console.WriteLine($"50 + 30 -> {Heal(50, 30, 100)}");
Console.WriteLine($"90 + 30 -> {Heal(90, 30, 100)}");
Console.WriteLine($"0 + 25 -> {Heal(0, 25, 100)}");


// ==========================================
// PROGRAMME PRINCIPAL (Instructions globales)
// ==========================================

Random rng = new Random(); // Utilisez new Random(7) si vous voulez fixer la graine pour le test
Player player = new Player("Aya", 100, 2);

List<Enemy> enemies = new List<Enemy>();
enemies.Add(new Goblin());
enemies.Add(new Orc());
enemies.Add(new Goblin());

Console.WriteLine("=== Arena ===");
Console.WriteLine($"{player.Name} faces {enemies.Count} enemies:");
foreach (Enemy e in enemies)
{
    Console.WriteLine($"- {e.Name}, {e.Health} HP");
}
Console.WriteLine($"First in line: {enemies[0].Name}");

int turn = 0;

while (player.IsAlive && enemies.Count > 0)
{
    turn++;
    Enemy enemy = enemies[0];
    
    Console.WriteLine();
    Console.WriteLine($"Turn {turn}: {player.Name} {player.Health} HP, {player.Potions} potions | {enemy.Name} {enemy.Health} HP");
    Console.Write("1 attack, 2 drink a potion, 3 run away > ");
    
    string text = Console.ReadLine();
    if (!int.TryParse(text, out int choice))
    {
        Console.WriteLine("Type 1, 2 or 3.");
        turn--; // Ne pas compter ce tour invalide
        continue;
    }

    switch (choice)
    {
        case 1:
            // 1 : L'ennemi subit 12 à 24 dégâts
            int damageToEnemy = rng.Next(12, 25);
            enemy.TakeDamage(damageToEnemy);
            break;
            
        case 2:
            // 2 : La joueuse boit une potion
            player.DrinkPotion();
            break;
            
        case 3:
            // 3 : Fuir et arrêter
            Console.WriteLine("You run away.");
            return;
            
        default:
            Console.WriteLine("Type 1, 2 or 3.");
            turn--;
            continue;
    }

    // Vérification si l'ennemi en face est vaincu
    if (!enemy.IsAlive)
    {
        Console.WriteLine($"{enemy.Name} is defeated!");
        enemies.RemoveAt(0);
        continue; // Passe au tour suivant sans riposte de l'ennemi vaincu
    }

    // L'ennemi survivant attaque la joueuse
    enemy.Attack(player, rng);
}

// Fin de partie
if (player.IsAlive)
{
    Console.WriteLine($"Victory in {turn} turns!");
}
else
{
    Console.WriteLine("Defeat...");
}


// ==========================================
// DÉFINITION DES CLASSES (À la fin du fichier)
// ==========================================

public class Player
{
    private int health;
    public string Name { get; }
    public int MaxHealth { get; }
    public int Potions { get; private set; }
    
    public int Health
    {
        get { return health; }
        private set { health = Math.Clamp(value, 0, MaxHealth); }
    }
    
    public bool IsAlive => health > 0;

    public Player(string name, int maxHealth, int potions)
    {
        Name = name;
        MaxHealth = maxHealth;
        Potions = potions;
        Health = maxHealth; // Commence avec la vie maximale
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        Console.WriteLine($"{Name} takes {amount} damage ({Health} HP)");
    }

    public void DrinkPotion()
    {
        if (Potions <= 0)
        {
            Console.WriteLine("No potion left!");
            return;
        }

        Potions--;
        int healAmount = 30;
        int oldHealth = Health;
        Health += healAmount;
        int actualHeal = Health - oldHealth; // Pour afficher le gain réel si plafonné
        Console.WriteLine($"{Name} drinks a potion ({Health} HP)");
    }
}

public abstract class Enemy
{
    public string Name { get; }
    public int Health { get; private set; }
    public bool IsAlive => Health > 0;

    protected Enemy(string name, int health)
    {
        Name = name;
        Health = health;
    }

    public void TakeDamage(int amount)
    {
        Health = Math.Max(Health - amount, 0);
        Console.WriteLine($"{Name} takes {amount} damage ({Health} HP)");
    }

    public abstract void Attack(Player target, Random rng);
}

public class Goblin : Enemy
{
    public Goblin() : base("Goblin", 30)
    {
    }

    public override void Attack(Player target, Random rng)
    {
        Console.WriteLine("Goblin stabs!");
        target.TakeDamage(rng.Next(4, 9)); // 4 à 8 (la borne haute 9 est exclue)[cite: 27]
    }
}

public class Orc : Enemy
{
    public Orc() : base("Orc", 60)
    {
    }

    public override void Attack(Player target, Random rng)
    {
        Console.WriteLine("Orc swings its club!");
        target.TakeDamage(rng.Next(8, 15)); // 8 à 14 (la borne haute 15 est exclue)
    }
}