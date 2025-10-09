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
        readonly Command commandAttack = new Command("attack", new[] { "atk", "attk" });
        readonly Command commandFireBall = new Command("fireball", new[] { "fire" });
        readonly Command commandBlock = new Command("block", new[] { "blk" });
        readonly Command commandHeal = new Command("heal", new[] { "heal" });

        Command[] currentCommands;
        Command chosenCommand = null;
        Command enemyCommand = null;
        Command enemyIntention = null;
        //cool down dictionarys
        Dictionary<Command, int> cooldowns;
        Dictionary<Command, int> maxCooldowns;

        // PLAYER
        Player player = new Player("PLAYER", 100, 20, 0.75f, 100);

        int lastAttackDamage = 0;
        int lastFireballDamage = 0;
        int lastSelfDamage = 0;
        int lastBlockValue = 0;
        int lastHealValue = 0;
        

        // ENEMY
        Entity[] enemies = {
            new Entity("ENEMY 1", 100, 10, 0.5f),   
            new Entity("ENEMY 2", 100, 20, 0.75f),   
            new Entity("ENEMY 3", 100, 30, 1f),
        };

        int lastEnemyAttackValue = 0;
        int lastEnemyHealValue = 0;

        int currentEnemyIndex = 0;

        System.Random random = new System.Random();

        /// Run once before Execute begins
        public void Setup()
        {
            Terminal.SetTitle("Terminal Knight");
            
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
                { commandYes, 0 },
                { commandNo, 0 },

                { commandPlay, 0 },
                { commandLore, 0 },
                { commandRules, 0 },

                { commandAttack, 0 },
                { commandFireBall, 4 },
                { commandBlock, 1 },
                { commandHeal, 3 }
            };
            cooldowns = new Dictionary<Command, int>
            {
                { commandYes, 0 },
                { commandNo, 0 },

                { commandPlay, 0 },
                { commandLore, 0 },
                { commandRules, 0 },

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
                // Main Screen Stuff
                case Screen.Main:
                    PrintMainMenuScreen();
                    break;

                case Screen.Lore:
                    PrintLoreScreen();
                    break;

                case Screen.Rules:
                    PrintRulesScreen();
                    break;

                // Fight Screen Stuff
                case Screen.Fight:
                    PrintFightScreen();

                    if (enemies[currentEnemyIndex].currentHealth <= 0)
                    {
                        currentScreen = Screen.Upgrade;
                        currentEnemyIndex++;
                    }

                    if (currentEnemyIndex >= enemies.Length|| player.currentHealth <= 0)
                    {
                        currentScreen = Screen.End;
                    }
                    break;

                // Upgrade
                case Screen.Upgrade:
                    PrintUpgradeScreen();
                    break;

                // End
                case Screen.End:
                    PrintEndScreen();
                    break;
            }
            
            PrintOptionsText();
            
            ParseInput();

            Terminal.Clear();
            printPlayerFeedback();
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

            switch (currentScreen)
            {
                case Screen.Main:
                    if (chosenCommand == commandPlay) currentScreen = Screen.Fight;
                    else if (chosenCommand == commandLore) currentScreen = Screen.Lore;
                    else if (chosenCommand == commandRules) currentScreen = Screen.Rules;
                    break;

                case Screen.Lore:
                    if (chosenCommand == commandYes) currentScreen = Screen.Main;
                    break;

                case Screen.Rules:
                    if (chosenCommand == commandYes) currentScreen = Screen.Main;
                    break;

                case Screen.Fight:
                    EvaluateFightCommand();
                    ChooseEnemyCommand();
                    break;

                case Screen.Upgrade:
                    //
                    break;
            }


            

        }

        void EvaluateFightCommand()
        {
            if (chosenCommand == commandAttack) Attack();
            else if (chosenCommand == commandFireBall) FireBall();
            else if (chosenCommand == commandBlock) Block();
            else if (chosenCommand == commandHeal) Heal();

            cooldowns[chosenCommand] = maxCooldowns[chosenCommand];
        }
        public void Attack()
        {
            Entity enemy = enemies[currentEnemyIndex];
            lastAttackDamage = random.Next(15, 26);
            enemy.Damage(lastAttackDamage);
        }
        public void FireBall()
        {
            Entity enemy = enemies[currentEnemyIndex];
            lastFireballDamage = random.Next(30, 40);
            lastSelfDamage = random.Next(10, 15);
            enemy.Damage(lastFireballDamage);
            player.playerDamage(lastSelfDamage);
        }
        public void Block()
        {
            lastBlockValue = random.Next(10, 25);
            player.playerBlock(lastBlockValue);
        }
        public void Heal()
        {
            lastHealValue = random.Next(10, 15);
            player.playerHeal(lastHealValue);
        }

        void ChooseEnemyCommand()
        {
            Command[] enemyAllowedCommands = { commandAttack, commandHeal };
            int enemyIndex = random.Next(0, enemyAllowedCommands.Length);

            enemyIntention = enemyAllowedCommands[enemyIndex];

            if (enemyIntention == commandAttack) enemyAttack();
            if (enemyIntention == commandHeal) enemyHeal();
        }
        public void enemyAttack()
        {
            lastEnemyAttackValue = random.Next(15, 25);
            player.playerDamage(lastEnemyAttackValue);
        }

        public void enemyHeal()
        {
            Entity enemy = enemies[currentEnemyIndex];
            lastEnemyHealValue = random.Next(-15, -10);
            enemy.Damage(lastEnemyHealValue);  
        }

        void PrintMainMenuScreen()
        {
            currentCommands = new[] { commandPlay, commandLore, commandRules };
            
            Terminal.WriteLine("This is the Main Menu"); // Placeholder
        }

        void PrintLoreScreen()
        {
            currentCommands = new[] { commandYes };

            Terminal.WriteLine("This is the Lore Screen"); // Placeholder
            Terminal.WriteLine("Return to Main Menu?");
        }

        void PrintRulesScreen()
        {
            currentCommands = new[] { commandYes };

            Terminal.WriteLine("This is the Rule Screen"); // Placeholder
            Terminal.WriteLine("Return to Main Menu?");
        }

        void PrintFightScreen()
        {
            currentCommands = new[] { commandAttack, commandFireBall, commandBlock, commandHeal };

            PrintPlayerText();
            PrintEnemyText();
        }

        void PrintUpgradeScreen()
        {
            Terminal.WriteLine("This is the Upgrade Screen"); // Placeholder
        }

        void PrintEndScreen()
        {
            string endText = (player.currentHealth > 0) ? "You won!" : "You died!";

            Terminal.WriteLine(endText);
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
            Terminal.WriteLine("\nCOMMANDS:", ConsoleColor.Yellow, ConsoleColor.Black);
            foreach (Command command in currentCommands)
            {
                
                int remainingCooldown = cooldowns[command];
                ConsoleColor color = remainingCooldown > 0 ? ConsoleColor.Red : ConsoleColor.Yellow;
                string printTexts = remainingCooldown > 0 ? $"{command.name.ToUpper()} (COOLDOWN: {remainingCooldown})" : $"{command.name.ToUpper()}";
                Terminal.WriteLine($"   {printTexts}", color, ConsoleColor.Black);
                
            }
            Terminal.WriteLine("\n", ConsoleColor.Black, ConsoleColor.Black);
        }
        void printPlayerFeedback()
        {
            Console.WriteLine($"Player chose to {chosenCommand.name.ToUpper()}");

            if (chosenCommand == commandAttack)
            {
                Console.WriteLine($"Enemy took {lastAttackDamage} damage.");
            }
            else if (chosenCommand == commandFireBall)
            {
                Console.WriteLine($"Enemy took {lastFireballDamage} damage.");
                Console.WriteLine($"Player took {lastSelfDamage} damage.");
            }
            else if (chosenCommand == commandBlock)
            {
                Console.WriteLine($"Player gained {lastBlockValue} armor/block.");
            }
            else if (chosenCommand == commandHeal)
            {
                Console.WriteLine($"Player healed {lastHealValue} health.");
            }

            if (enemyIntention == commandEnemyAttack)
            {
                Console.WriteLine($"Enemy attacked player for {lastEnemyAttackValue} damage.");
            }
            else if (enemyIntention == commandEnemyHeal)
            {
                Console.WriteLine($"Enemy healed for {lastEnemyHealValue * -1} health.");
            }
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
