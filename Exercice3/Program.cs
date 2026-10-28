using System;

int health = 40;

for (int turn = 1; turn <= 6; turn++)
{
    health = Math.Max(0, health - 8);

    Console.WriteLine($"Turn {turn}: {health} HP");

    if (health == 0)
    {
        Console.WriteLine("Aya is down.");
        break;
    }
}