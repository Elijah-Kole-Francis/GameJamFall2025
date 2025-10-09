using System;
using System.Collections.Generic;
using System.Linq;
using TerminalGameWithAudio;

namespace MohawkTerminalGame
{
    public class TerminalGame
    {
        // SCREEN
        enum Screen
        {
            Main,
            Lore,
            Rules,

            Fight,
            
            Upgrade,

            End,
        }
        Screen currentScreen = Screen.Main;
        
        // COMMANDS
        readonly Command commandYes = new Command("yes", new[] { "y" });
        readonly Command commandNo = new Command("no", new[] { "n" });
        
        // COMMANDS / MAIN
        readonly Command commandPlay = new Command("play");
        readonly Command commandLore = new Command("lore");
        readonly Command commandRules = new Command("rules");


        // COMMANDS / FIGHT
        readonly Command commandAttack = new Command("attack", new[] { "atk" });
        readonly Command commandFireBall = new Command("fireball", new[] { "fire" });
        readonly Command commandBlock = new Command("block", new[] { "blk" });
        readonly Command commandHeal = new Command("heal", new[] { "heal" });

        Command[] currentCommands;
        Command chosenCommand = null;
        Command enemyCommand = null;
        //cool down dictionarys
        Dictionary<Command, int> cooldowns;
        Dictionary<Command, int> maxCooldowns;

        // PLAYER
        Player player = new Player("PLAYER", 100, 20, 0.75f, 100);

        // ENEMY
        Entity[] enemies = {
            new Entity("ENEMY 1", 100, 10, 0.5f),   
            new Entity("ENEMY 2", 100, 20, 0.75f),   
            new Entity("ENEMY 3", 100, 30, 1f),
        };
        
        int currentEnemyIndex = 0;

        System.Random random = new System.Random();

        /// Run once before Execute begins
        public void Setup()
        {
            // Program configuration
            Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteLoop;
            Program.TerminalInputMode = TerminalInputMode.KeyboardReadAndReadLine;

            // Hide raylib console output
            Terminal.ForegroundColor = ConsoleColor.Black;
            Terminal.CursorVisible = false;
            Audio.Initialize();
            
            // Move curosr to overwrite previously drawn (black) text
            Terminal.SetCursorPosition(0, 0);
            Terminal.ResetColor();
            Terminal.CursorVisible = true;

            currentCommands = new[] { commandAttack, commandFireBall, commandBlock, commandHeal};

            maxCooldowns = new Dictionary<Command, int>
            {
                { commandAttack, 0 },
                { commandFireBall, 4 },
                { commandBlock, 1 },
                { commandHeal, 3 }
            };
            cooldowns = new Dictionary<Command, int>
            {
                { commandAttack, 0 },
                { commandFireBall, 0 },
                { commandBlock, 0 },
                { commandHeal, 0 }
            };


        }

        // Execute() runs based on Program.TerminalExecuteMode (assign to it in Setup).
        //  ExecuteOnce: runs only once. Once Execute() is done, program closes.
        //  ExecuteLoop: runs in infinite loop. Next iteration starts at the top of Execute().
        //  ExecuteTime: runs at timed intervals (eg. "FPS"). Code tries to run at Program.TargetFPS.
        //               Code must finish within the alloted time frame for this to work well.
        public void Execute()
        {
            switch (currentScreen)
            {
                case Screen.Main:
                    PrintMainMenu();
                    break;

                case Screen.Fight:
                    PrintFightScreen();
                    break;

                case Screen.Upgrade:
                    PrintUpgradeScreen();
                    break;
            }
            
            PrintOptionsText();
            
            ParseInput();

            Terminal.Clear();
        }
        
        void ParseInput()
        {
            chosenCommand = null;
            
            while (true)
            {
                Terminal.Write("Please enter a command: ");

                string input = Terminal.ReadAndClearLine();
                int commandIndex = 0;

                foreach (Command command in currentCommands)
                {
                    if (command.DoesMatch(input))
                    {
                        if (cooldowns[command] == 0)
                        {
                            chosenCommand = command;
                            foreach (var key in cooldowns.Keys.ToList())
                            {
                                if (cooldowns[key] > 0)
                                    cooldowns[key]--;
                            }
                            break;
                        }
                        else
                        {
                            Terminal.WriteLine($"That command is on cooldown for {cooldowns[command]} more turn(s).");
                        }
                    }
                }

                if (int.TryParse(input, out commandIndex) && chosenCommand == null)
                {
                    Command selectedCommand = currentCommands[commandIndex - 1];

                    if (cooldowns[selectedCommand] == 0)
                    {
                        chosenCommand = selectedCommand;
                        foreach (var key in cooldowns.Keys.ToList())
                        {
                            if (cooldowns[key] > 0)
                                cooldowns[key]--;
                        }
                        break;
                    }
                    else
                    {
                        Terminal.WriteLine($"That command is on cooldown for {cooldowns[selectedCommand]} more turn(s).");
                    }
                }

                if (chosenCommand != null) break;

                Terminal.ClearLine();
                Terminal.Write("Not a valid command. ");
            }

            ChooseEnemyCommand(); // can be moved after calling player command if need be - JM

            // Wanted to do switch statement but wasnt working and dont have time to figure ts out - JM
            if (chosenCommand == commandAttack) Attack();
            else if (chosenCommand == commandFireBall) FireBall();
            else if (chosenCommand == commandBlock) Block();
            else if (chosenCommand == commandHeal) Heal();

            cooldowns[chosenCommand] = maxCooldowns[chosenCommand];
        }

        void Attack()
        {
            
            Terminal.WriteLine("Played Attack"); // For debug, can be removed - JM
            Entity enemy = enemies[currentEnemyIndex];
            int attackValue = random.Next(15, 26);
            enemy.Damage(attackValue);

        }

        void FireBall()
        {
            Entity enemy = enemies[currentEnemyIndex];
            int attackValue = random.Next(30, 40);
            int selfdamage = random.Next(10, 15);
            enemy.Damage(attackValue);
            player.playerDamage(selfdamage);
        }
        void Block()
        {
            Terminal.WriteLine("Played Blocked"); // For debug, can be removed - JM
            int blockValue = random.Next(10, 25);
            player.playerBlock(blockValue);
        }

        void Heal()
        {
            Terminal.WriteLine("Played Heal"); // For debug, can be removed - JM
            int healValue = random.Next(10, 15);
            player.playerHeal(healValue);
        }

        void ChooseEnemyCommand()
        {
            Command[] enemyAllowedCommands = {};
            enemyAllowedCommands = new[] { commandAttack, commandHeal };
            int enemyIndex = random.Next(0, enemyAllowedCommands.Length);

            Command enemyIntention = enemyAllowedCommands[enemyIndex]; 

            if (enemyIntention == commandAttack) enemyAttack();
            if (enemyIntention == commandHeal) enemyHeal();
        }
        void enemyAttack()
        {
            int attackValue = random.Next(15, 25);
            player.playerDamage(attackValue);
        }
        void enemyHeal()
        {
            Entity enemy = enemies[currentEnemyIndex];
            int healValue = random.Next(-15, -10);
            enemy.Damage(healValue);
        }

        void PrintMainMenu()
        {
            Terminal.WriteLine("This is the Main Menu"); // Placeholder
        }

        void PrintFightScreen()
        {
            PrintPlayerText();
            PrintEnemyText();
        }

        void PrintUpgradeScreen()
        {

        }

        void PrintPlayerText()
        {
            string healthBar = HealthDisplayText(player.currentHealth, player.maxHealth);

            string[] healthParts = healthBar.Split('|');
            if (healthParts.Length == 2)
            {
                Terminal.Write("PLAYER:\n\tHealth: [", ConsoleColor.Green, ConsoleColor.Black);
                Terminal.Write(healthParts[0], ConsoleColor.Green, ConsoleColor.Black);
                Terminal.Write(healthParts[1], ConsoleColor.DarkRed, ConsoleColor.Black);     
                Terminal.WriteLine("]", ConsoleColor.Green, ConsoleColor.Black);
            }

            string armourBar = HealthDisplayText(player.currentArmour, player.maxArmour);
            string[] armourParts = armourBar.Split('|');
            if (armourParts.Length == 2)
            {
                Terminal.Write("\tArmor:  [", ConsoleColor.Gray, ConsoleColor.Black);
                Terminal.Write(armourParts[0], ConsoleColor.Gray, ConsoleColor.Black);
                Terminal.Write(armourParts[1], ConsoleColor.Black, ConsoleColor.Black);
                Terminal.WriteLine("]", ConsoleColor.Gray, ConsoleColor.Black);
            }
            else
            {
                Terminal.WriteLine("PLAYER:\n" +
                    $"\tHealth: {healthBar}", ConsoleColor.Green, ConsoleColor.Black);
            }

            Terminal.WriteLine($"\tHit %:  {player.hitPercentage * 100}%\n", ConsoleColor.Green, ConsoleColor.Black);
            Terminal.WriteLine("", ConsoleColor.Black, ConsoleColor.Black);
        }

        void PrintEnemyText()
        {
            Entity currentEnemy = enemies[currentEnemyIndex];
            string enemyhealthBar = HealthDisplayText(currentEnemy.currentHealth, currentEnemy.maxHealth);

            string[] parts = enemyhealthBar.Split('|');
            if (parts.Length == 2)
            {
                Terminal.Write("ENEMY:\n\tHealth: [", ConsoleColor.Red, ConsoleColor.Black);
                Terminal.Write(parts[0], ConsoleColor.Red, ConsoleColor.Black);
                Terminal.Write(parts[1], ConsoleColor.DarkMagenta, ConsoleColor.Black);
                Terminal.WriteLine("]", ConsoleColor.Red, ConsoleColor.Black);
            }
            

            
            else
            {
                Terminal.WriteLine("ENEMY:\n" +
                    $"\tHealth: {enemyhealthBar}");
            }

            Terminal.WriteLine($"\tHit %: {currentEnemy.hitPercentage * 100}%\n", ConsoleColor.Red, ConsoleColor.Black);
            Terminal.WriteLine("", ConsoleColor.Black, ConsoleColor.Black);
        }

        void PrintOptionsText()
        {
            Terminal.WriteLine("\nPLAYER OPTIONS:\n", ConsoleColor.Yellow, ConsoleColor.Black);
            foreach (Command command in currentCommands)
            {
                int remainingCooldown = cooldowns[command];
                ConsoleColor color = remainingCooldown > 0 ? ConsoleColor.Red : ConsoleColor.Yellow;
                Terminal.WriteLine($"\t{command.name.ToUpper()} ({remainingCooldown})", color, ConsoleColor.Black);
            }
            Terminal.WriteLine("\n", ConsoleColor.Black, ConsoleColor.Black);
        }
        private string HealthDisplayText(int health, int maxHealth)
        {
            float healthPercentage = (float)health / maxHealth;
            if (healthPercentage < 0f) healthPercentage = 0f;
            if (healthPercentage > 1f) healthPercentage = 1f;

            int totalBars = 10;
            int filledBars = (int)(healthPercentage * totalBars);
            int emptyBars = totalBars - filledBars;

            string filled;

            if (filledBars > 0)
            {
                filled = new string('-', filledBars - 1) + $"{(int)(healthPercentage * 100)}%";
            }
            else
            {
                filled = $"{(int)(healthPercentage * 100)}% ";
            }
            
            
            string empty = new string('-', emptyBars);
            return $"{filled}|{empty}";
            
            
        }

    }
}
