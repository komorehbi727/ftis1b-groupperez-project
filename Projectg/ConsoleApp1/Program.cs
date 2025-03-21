using static System.Reflection.Metadata.BlobBuilder;
using System.Text.Json;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public class Pokemon
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; } = 1;
        public bool IsAvailable { get; set; } = true;

        public Pokemon() { }

        public Pokemon(string name, int level)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            if (level <= 0) throw new ArgumentException("Level must be greater than 0.");
            Level = level;
        }

        public void DisplayPokemon()
        {
            Console.WriteLine($"{Name}, Level {Level} - {(IsAvailable ? "Available" : "Withdrawn")}");
        }
    }

    public class PokePC
    {
        private List<Pokemon> pokemons = new List<Pokemon>();
        private const string filePath = "pokemans.json";

        public PokePC()
        {
            LoadPokemon();
        }

        private void LoadPokemon()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                pokemons = JsonSerializer.Deserialize<List<Pokemon>>(json) ?? new List<Pokemon>();
            }
        }

        private void SavePokemon()
        {
            string json = JsonSerializer.Serialize(pokemons, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public void AddPokemon(Pokemon pokemon)
        {
            pokemons.Add(pokemon);
            SavePokemon();
            Console.WriteLine($"'{pokemon.Name}' - Level {pokemon.Level} has been added to your PC.");
        }

        public void ListPokemon()
        {
            Console.WriteLine("\nCaught Pokemon:");
            foreach (var pokemon in pokemons)
            {
                pokemon.DisplayPokemon();
            }
        }

        private Pokemon? SelectPokemon(string name)
        {
            var matchingPokemon = pokemons.Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (matchingPokemon.Count == 0)
            {
                Console.WriteLine("Pokemon not found.");
                return null;
            }

            if (matchingPokemon.Count == 1)
                return matchingPokemon.First();

            Console.WriteLine("Multiple Pokemon found. Please select by level:");
            for (int i = 0; i < matchingPokemon.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {matchingPokemon[i].Name}, Level {matchingPokemon[i].Level} - {(matchingPokemon[i].IsAvailable ? "Available" : "Withdrawn")}");
            }

            Console.Write("Enter your choice (1, 2, 3...): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= matchingPokemon.Count)
            {
                return matchingPokemon[choice - 1];
            }

            Console.WriteLine("Invalid choice.");
            return null;
        }

        public void WithdrawPokemon(string name)
        {
            Pokemon? pokemon = SelectPokemon(name);

            if (pokemon != null && pokemon.IsAvailable)
            {
                pokemon.IsAvailable = false;
                SavePokemon();
                Console.WriteLine($"'{pokemon.Name}' has been withdrawn.");
            }
            else
            {
                Console.WriteLine("This Pokemon is not available.");
            }
        }

        public void DepositPokemon(string name)
        {
            Pokemon? pokemon = SelectPokemon(name);

            if (pokemon != null && !pokemon.IsAvailable)
            {
                pokemon.IsAvailable = true;
                SavePokemon();
                Console.WriteLine($"'{pokemon.Name}' has been deposited.");
            }
            else
            {
                Console.WriteLine("Pokemon was not withdrawn.");
            }
        }

        public void ReleasePokemon(string name)
        {
            Pokemon? pokemon = SelectPokemon(name);

            if (pokemon != null)
            {
                pokemons.Remove(pokemon);
                SavePokemon();
                Console.WriteLine($"'{pokemon.Name}' has been released.");
            }
            else
            {
                Console.WriteLine("Pokemon not found.");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            PokePC pokepc = new PokePC();

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. List Pokemon");
                Console.WriteLine("2. Add Pokemon");
                Console.WriteLine("3. Release Pokemon");
                Console.WriteLine("4. Withdraw Pokemon");
                Console.WriteLine("5. Deposit Pokemon");
                Console.WriteLine("7. Logout");
                Console.Write("Enter choice: ");
                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        pokepc.ListPokemon();
                        break;
                    case "2":
                        Console.Write("Enter Pokemon Name: ");
                        string? name = Console.ReadLine() ?? "Missingno.";
                        Console.Write("Enter Level: ");
                    EnteringLevel:
                        if (int.TryParse(Console.ReadLine(), out int level) && level > 0)
                        {
                            pokepc.AddPokemon(new Pokemon(name, level));
                        }
                        else
                        {
                            Console.WriteLine("Invalid level. Please enter a positive integer.");
                            goto EnteringLevel;
                        }
                        break;
                    case "3":
                        Console.Write("Enter Pokemon to release: ");
                        pokepc.ReleasePokemon(Console.ReadLine() ?? string.Empty);
                        break;
                    case "4":
                        Console.Write("Enter Pokemon to withdraw: ");
                        pokepc.WithdrawPokemon(Console.ReadLine() ?? string.Empty);
                        break;
                    case "5":
                        Console.Write("Enter Pokemon to deposit: ");
                        pokepc.DepositPokemon(Console.ReadLine() ?? string.Empty);
                        break;
                    case "7":
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}
