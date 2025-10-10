using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        // COMMANDS / LEVEL UP
        readonly Command commandUpgradeAttack = new Command("upgrade attack", new[] { "upgrade 1" });
        readonly Command commandUpgradeFireBall = new Command("upgrade fireball", new[] { "upgrade 2" });
        readonly Command commandUpgradeBlock = new Command("upgrade block", new[] { "upgrade 3" });
        readonly Command commandUpgradeHeal = new Command("upgrade heal", new[] { "upgrade 4" });
        // COMMANDS / FIGHT
        readonly Command commandAttack = new Command($"attack", new[] { "atk", "attk" });
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
        Player player = new Player("PLAYER", 100, 20, 0.75f, 0.25f, 100);

        int lastAttackDamage = 0;
        int lastFireballDamage = 0;
        int lastSelfDamage = 0;
        int lastBlockValue = 0;
        int lastHealValue = 0;

        int attackLevelIndex = 0;
        int fireBallLevelIndex = 0;
        int blockLevelIndex = 0;
        int healLevelIndex = 0;

        bool didUpgrade = false;
        bool haventLeveledUpState = false;
        bool leveledUpState = false;

        // ENEMY
        Entity[] enemies = {
            new Entity("Imp", 100, 10, 0.5f, 0.25f),   
            new Entity("Skeleton", 100, 20, 0.5f, 0.25f),   
            new Entity("Bear", 100, 30, 0.5f, 0.35f),
            new Entity("Dragon", 100, 30, 1f, 0.4f),
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

                {commandUpgradeAttack, 0 },
                {commandUpgradeFireBall, 0 },
                { commandUpgradeBlock, 0 },
                { commandUpgradeHeal, 0 },

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

                {commandUpgradeAttack, 0 },
                {commandUpgradeFireBall, 0 },
                { commandUpgradeBlock, 0 },
                { commandUpgradeHeal, 0 },

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
            if (haventLeveledUpState == true) haventLeveledUp();
            if (leveledUpState == true) leveledUp();
        }
        private float GetPlayerActualHitChance()
        {
            float hitChange = (float)random.NextDouble() * (player.hitPercentageVariance * 2) - player.hitPercentageVariance;
            float finalChance = player.hitPercentage - hitChange;
            return finalChance * 100f;
        }

        private float GetEnemyActualHitChance(Entity enemy)
        {
            float hitChange = (float)random.NextDouble() * (enemy.hitPercentageVariance * 2) - enemy.hitPercentageVariance;
            float finalChance = enemy.hitPercentage - hitChange;
            return finalChance * 100f;
        }
        private bool DidPlayerHit()
        {
            float chance = GetPlayerActualHitChance();
            return random.Next(0, 100) < chance;
        }

        private bool DidEnemyHit(Entity enemy)
        {
            float chance = GetEnemyActualHitChance(enemy);
            return random.Next(0, 100) < chance;
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

                    if (enemies[currentEnemyIndex].currentHealth <= 0)
                    {
                        currentScreen = Screen.Upgrade;
                        currentEnemyIndex++;
                        didUpgrade = false;
                    }

                    if (currentEnemyIndex >= enemies.Length || player.currentHealth <= 0)
                    {
                        currentScreen = Screen.End;
                    }

                    break;

                case Screen.Upgrade:
                    if (didUpgrade == false)
                    {
                        if (chosenCommand == commandYes) haventLeveledUpState = true;
                        else if (chosenCommand == commandUpgradeAttack)
                        {
                            attackLevelIndex += 1;
                            didUpgrade = true;
                        }
                        else if (chosenCommand == commandUpgradeFireBall)
                        {
                            fireBallLevelIndex += 1;
                            didUpgrade = true;
                        }
                        else if (chosenCommand == commandUpgradeBlock)
                        {
                            blockLevelIndex += 1;
                            didUpgrade = true;
                        }
                        else if (chosenCommand == commandUpgradeHeal)
                        {
                            healLevelIndex += 1;
                            didUpgrade = true;
                        }
                        break;
                    }
                    else
                    {
                        if (chosenCommand != commandYes) leveledUpState = true;
                        if (chosenCommand == commandYes) currentScreen = Screen.Fight;
                        break;
                    }
                        
            }

        }

        void haventLeveledUp()
        {
            Terminal.WriteLine("You haven't Leveled up yet");
        }
        void leveledUp()
        {
            Terminal.WriteLine("you already leveled up");
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
            int[] attackLevels = { 20, 25, 30 };
            int attackValue = attackLevels[attackLevelIndex];
            int highAttackValue = attackValue + 5;
            int lowAttackValue = attackValue - 5;

            if (DidPlayerHit())
            {
                lastAttackDamage = random.Next(lowAttackValue, highAttackValue);
                enemy.Damage(lastAttackDamage);
            }
            else
            {
                lastAttackDamage = -1;
            }
        }
        public void FireBall()
        {
            Entity enemy = enemies[currentEnemyIndex];
            int[] fireBallLevels = { 35, 40, 45 };
            int fireBallValue = fireBallLevels[fireBallLevelIndex];
            int highFireBallValue = fireBallValue + 5;
            int lowFireBallValue = fireBallValue - 5;

            int[] selfDamageLevels = { 10, 15, 20 };
            int selfDamageValue = selfDamageLevels[fireBallLevelIndex];
            int highSelfDamageValue = selfDamageValue + 5;
            int lowSelfDamageValue = selfDamageValue - 5;

            if (DidPlayerHit())
            {
                lastFireballDamage = random.Next(lowFireBallValue, highFireBallValue);
                enemy.Damage(lastFireballDamage);
            }
            else
            {
                lastFireballDamage = -1;
            }

            lastSelfDamage = random.Next(lowSelfDamageValue, highSelfDamageValue);
            player.playerDamage(lastSelfDamage);
        }
        public void Block()
        {
            int[] blockLevels = [15, 20, 30];
            int blockValue = blockLevels[blockLevelIndex];
            int highBlockValue = blockValue + 5;
            int lowBlocklValue = blockValue - 5;

            lastBlockValue = random.Next(lowBlocklValue, highBlockValue);
            player.playerBlock(lastBlockValue);
        }
        public void Heal()
        {
            int[] healLevels = [100, 20, 30];
            int healValue = healLevels[healLevelIndex];
            int highBlockValue = healValue + 5;
            int lowBlocklValue = healValue - 5;

            lastHealValue = random.Next(lowBlocklValue, highBlockValue);
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
            drawStart();
            currentCommands = new[] { commandPlay, commandLore, commandRules };
            
            Terminal.WriteLine( "\r\nWelcome to Terminal Knight, \r\n" +
                                "if you don't know how to play type 3"); // Placeholder
        }

        void PrintLoreScreen()
        {
            currentCommands = new[] { commandYes };

            Terminal.WriteLine( "Welcome, player. \r\n" +
                                "This is the Terminal Knight, a one-way battle into the heart of the dragon’s lair. \r\n"+
                                "Slay every foe to strengthen your resolve, reach the lair of the Dragon, \r\n" +
                                "and claim the legendary treasure within its hoard.\r\n");

            Terminal.WriteLine("Return to Main Menu?");
        }

        void PrintRulesScreen()
        {
            currentCommands = new[] { commandYes };
            

            Terminal.WriteLine("Hello, player. \r\n" +
                "Everytime you enter a new screen you will have a list of commands you can do.\r\n" +
                "To do the command of your choice you have to either type out your selected commands name\r\n" +
                "or type the corresponding number \r\n" +
                "e.g.\r\n" +
                "\t(1)ATTACK\r\n" +
                "\t(2)FIREBALL\r\n" +
                "\t(3)BLOCK\r\n" +
                "\t(4)HEAL\r\n" +
                "\tect.\r\n");
            Terminal.WriteLine("Return to Main Menu?");
        }

        void PrintFightScreen()
        {
            currentCommands = new[] { commandAttack, commandFireBall, commandBlock, commandHeal };

            PrintPlayerText();
            PrintEnemyText();
            drawEnemy();
        }

        void PrintUpgradeScreen()
        {
            Terminal.WriteLine("\nUPGRADE SCREEN", ConsoleColor.Yellow, ConsoleColor.Black);
            drawShop();
            currentCommands = new[]
            {
                commandUpgradeAttack,
                commandUpgradeFireBall,
                commandUpgradeBlock,
                commandUpgradeHeal,
                commandYes
            };
        }

        void PrintEndScreen()
        {
            string endText = (player.currentHealth > 0) ?   "You won! \r\n" +
                                                            "You defeated the dragon and all of his horde\r\n" +
                                                            "Now that you have riches beyond imagine what will life bring you" :
                                                            "You died!\r\n" +
                                                            "Your body cold and lifeless destined to rot into nothing\r\n" +
                                                            "as your names forgotten to time";

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

            Terminal.WriteLine($"\tHit %:  {GetPlayerActualHitChance():F1}%", ConsoleColor.Green, ConsoleColor.Black);
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

            Terminal.WriteLine($"\tHit %: {GetEnemyActualHitChance(currentEnemy):F1}%", ConsoleColor.Red, ConsoleColor.Black);
            Terminal.WriteLine("", ConsoleColor.Black, ConsoleColor.Black);
        }

        void PrintOptionsText()
        {
            Terminal.WriteLine("\nCOMMANDS:", ConsoleColor.Yellow, ConsoleColor.Black);

            foreach (Command command in currentCommands)
            {
                int remainingCooldown = cooldowns[command];
                int maxCooldown = maxCooldowns[command];
                string extraInfo = "";

                // === ATTACK INFO ===
                if (command == commandAttack)
                {
                    int[] attackLevels = [20, 25, 30];
                    int attackValue = attackLevels[Math.Clamp(attackLevelIndex, 0, attackLevels.Length - 1)];
                    int highAttackValue = attackValue + 5;
                    int lowAttackValue = attackValue - 5;

                    extraInfo = $"(DAMAGE: {lowAttackValue}-{highAttackValue})";
                }

                // === FIREBALL INFO ===
                else if (command == commandFireBall)
                {
                    int[] fireBallLevels = [35, 40, 45];
                    int fireBallValue = fireBallLevels[Math.Clamp(fireBallLevelIndex, 0, fireBallLevels.Length - 1)];
                    int highFireBallValue = fireBallValue + 5;
                    int lowFireBallValue = fireBallValue - 5;

                    int[] selfDamageLevels = [10, 15, 20];
                    int selfDamageValue = selfDamageLevels[Math.Clamp(fireBallLevelIndex, 0, selfDamageLevels.Length - 1)];
                    int highSelfDamage = selfDamageValue + 5;
                    int lowSelfDamage = selfDamageValue - 5;

                    extraInfo = $"(DAMAGE: {lowFireBallValue}-{highFireBallValue}) (SELFDAMAGE: {lowSelfDamage}-{highSelfDamage})";
                }

                // === BLOCK INFO ===
                else if (command == commandBlock)
                {
                    int[] blockLevels = [15, 20, 30];
                    int blockValue = blockLevels[Math.Clamp(blockLevelIndex, 0, blockLevels.Length - 1)];
                    int highBlockValue = blockValue + 5;
                    int lowBlockValue = blockValue - 5;

                    extraInfo = $"(BLOCK: {lowBlockValue}-{highBlockValue})";
                }

                // === HEAL INFO ===
                else if (command == commandHeal)
                {
                    int[] healLevels = [15, 20, 30];
                    int healValue = healLevels[Math.Clamp(healLevelIndex, 0, healLevels.Length - 1)];
                    int highHealValue = healValue + 5;
                    int lowHealValue = healValue - 5;

                    extraInfo = $"(HEAL: {lowHealValue}-{highHealValue})";
                }

                // === UPGRADE COMMANDS ===
                else if (command == commandUpgradeAttack)
                    extraInfo = $"(CURRENT LVL: {attackLevelIndex + 1})";
                else if (command == commandUpgradeFireBall)
                    extraInfo = $"(CURRENT LVL: {fireBallLevelIndex + 1})";
                else if (command == commandUpgradeBlock)
                    extraInfo = $"(CURRENT LVL: {blockLevelIndex + 1})";
                else if (command == commandUpgradeHeal)
                    extraInfo = $"(CURRENT LVL: {healLevelIndex + 1})";

                // === COOLDOWN COLOR ===
                ConsoleColor color = remainingCooldown > 0 ? ConsoleColor.Red : ConsoleColor.Yellow;
                string cooldownText = maxCooldown > 0 ? $"(COOLDOWN: {remainingCooldown})" : "";

                // === FINAL OUTPUT ===
                string printText = $"{command.name.ToUpper()} {extraInfo} {cooldownText}";
                Terminal.WriteLine($"   {printText}", color, ConsoleColor.Black);
            }

            Terminal.WriteLine("\n", ConsoleColor.Black, ConsoleColor.Black);
        }
        void printPlayerFeedback()
        {
            Terminal.WriteLine($"Player chose to {chosenCommand.name.ToUpper()}");

            if (chosenCommand == commandAttack)
            {
                if (lastAttackDamage == -1)
                    Terminal.WriteLine("Player missed!");
                else
                    Terminal.WriteLine($"Enemy took {lastAttackDamage} damage.");
            }
            else if (chosenCommand == commandFireBall)
            {
                if (lastFireballDamage == -1)
                    Terminal.WriteLine("Player missed the fireball!");
                else
                    Terminal.WriteLine($"Enemy took {lastFireballDamage} damage.");

                Terminal.WriteLine($"Player took {lastSelfDamage} damage.");
            }
            else if (chosenCommand == commandBlock)
            {
                Terminal.WriteLine($"Player gained {lastBlockValue} armor/block.");
            }
            else if (chosenCommand == commandHeal)
            {
                Terminal.WriteLine($"Player healed {lastHealValue} health.");
            }

            if (enemyIntention == commandAttack)
            {
                Entity enemy = enemies[currentEnemyIndex];
                if (DidEnemyHit(enemy))
                    Terminal.WriteLine($"Enemy attacked player for {lastEnemyAttackValue} damage.");
                else
                    Terminal.WriteLine("Enemy missed!");
            }
            else if (enemyIntention == commandHeal)
            {
                Terminal.WriteLine($"Enemy healed for {lastEnemyHealValue * -1} health.");
            }

            Terminal.WriteLine("\n");
        }
        private void drawEnemy()
        {
            switch (currentEnemyIndex)
            {
                case 0:
                {
                    Terminal.WriteLine(".................##.........................#.#+.................");
                    Terminal.WriteLine("................#*##......................##.+#+.................");
                    Terminal.WriteLine("................#+*###..................##..+##+.................");
                    Terminal.WriteLine("................##***####..########.####..++*#+..................");
                    Terminal.WriteLine("................##*++*+########.++###.+++++++#*..................");
                    Terminal.WriteLine(".................#+*++++++++++******.+++++++#+...................");
                    Terminal.WriteLine("..................#+++++++++++++++*++++++++##+...................");
                    Terminal.WriteLine("..................##*++++++++++*++++++++++##.....................");
                    Terminal.WriteLine("...................##*++++++++++++++++++##++.....................");
                    Terminal.WriteLine("....................###*+++++++++++++++*#+.......................");
                    Terminal.WriteLine("................###...#+*++++*++++++++*##....#.#+................");
                    Terminal.WriteLine("...............##*#....#++**++++++++++*##...##.#+................");
                    Terminal.WriteLine("...............##*#....#++**++++++++++*##...##.#+................");
                    Terminal.WriteLine("...............##*++##.#+*###+++++###++##.##.++#+................");
                    Terminal.WriteLine("...............##*+++#.#+####++++####++####.*++#.................");
                    Terminal.WriteLine("...............##*+++++.+####.+*+####.++*#.++*+#.................");
                    Terminal.WriteLine("...............##**+*++++*## +*+++## +*+*.++**##.................");
                    Terminal.WriteLine("...............##**+**+++*+*++++++***+++*++***+#.................");
                    Terminal.WriteLine("...............##**+***++*++++*+++*++++*++**.##*.................");
                    Terminal.WriteLine("................##*++***+*+++*+++++*++.*+*+####++................");
                    Terminal.WriteLine(".................####*+++*+++*++++++++.*####*+...................");
                    Terminal.WriteLine("....................####+*+#+++++++#+#+##*++.....................");
                    Terminal.WriteLine(".......................##*####++++#+# ##.........................");
                    Terminal.WriteLine(".......................##*########+##.#*.........................");
                    Terminal.WriteLine(".......................##*### ###+##.+#+.........................");
                    Terminal.WriteLine("........................#++*#######.+##+.........................");
                    Terminal.WriteLine("........................#++**#####.+*##..........................");
                    Terminal.WriteLine("........................#+*+**++++**+##..........................");
                    Terminal.WriteLine("........................#.*+++*****++##..........................");
                    Terminal.WriteLine("........................#+*++++*+++++##..........................");
                    Terminal.WriteLine("........................#+*++++*++++++#..........................");
                    Terminal.WriteLine(".............######....##+*++++*++++++#################..........");
                    Terminal.WriteLine("...........#############.+*++++*++++++*+..###.++++++++##.........");
                    Terminal.WriteLine(".........###++++++**##..***+++++++++++*****************#........#");
                    Terminal.WriteLine("###########++**++++******+*+++++++++++*++++***+++++++**##########");
                    break;
                }
                case 1:
                {
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++++     +++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++        ++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++ ;     + +++++++++++++++++++++++++++");
                    Terminal.WriteLine("++++++++++++++++++++++++++++  +     + ;++++++++++++++++++++++++++");
                    Terminal.WriteLine("++++++++++++++++++++++++++++  +        ++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++         ;  ++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++ +;     +++ ++++++++++++++++++++++++++");
                    Terminal.WriteLine("++++++++++++++++++++++++++++++++  ++++ ++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++ ++++;++++++++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++ ;++;++ ++ +++++++++++++++++++++++++++");
                    Terminal.WriteLine("++++++++++++++++++++++++++++++ +++   + ++++++++++++++++++++++++++");
                    Terminal.WriteLine("++++++++++++++++++++++++++++ ++     +++++++++++++++++++++++++++++");
                    Terminal.WriteLine("++++++++++++++++++++++++++++ ++     +;;++++++++++++++++++++++++++");
                    Terminal.WriteLine("++++++++++++++++++++++++++++ ++;+++++ +++++++++++++++++++++++++++");
                    Terminal.WriteLine("++++++++++++++++++++++++++++  ;    +  +++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++ +    + ++++++++++++++++++++++++++++");
                    Terminal.WriteLine("++++++++++++++++++++++++++++++;;  +;+ +++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++   +;    +++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++ ;     + +++++++++++++++++++++++++++");
                    Terminal.WriteLine("+++++++++++++++++++++++++++++  ++++ +;+++++++++++++++++++++++++++");
                    Terminal.WriteLine("++++++++++++++++++   ;+++++ +       +;;  +       ++++++++++++++++");
                    break;
                }
                case 2:
                {
                    Terminal.WriteLine("...............................................................");
                    Terminal.WriteLine("...............................................................");
                    Terminal.WriteLine("...............................................................");
                    Terminal.WriteLine("...............................................................");
                    Terminal.WriteLine("...............................................................");
                    Terminal.WriteLine("...............................................................");
                    Terminal.WriteLine("...............................................................");
                    Terminal.WriteLine("..................................;**+;........................");
                    Terminal.WriteLine("....................;**;..........******.......................");
                    Terminal.WriteLine("..................;*   +;.******;+**+ ;**......................");
                    Terminal.WriteLine("..................*   **++            .*.......................");
                    Terminal.WriteLine("..................+++   ..++.  .     ; * ;.....................");
                    Terminal.WriteLine("................;*+.  ;.     +*      *  ;;.....................");
                    Terminal.WriteLine("...............*. .        **.      +.; *......................");
                    Terminal.WriteLine("..............+  *        .***      +.,*,;.....................");
                    Terminal.WriteLine("........****.*             *;*     * ;*;+;.....................");
                    Terminal.WriteLine("......*.   .**  ;           , ,   ,.+ *;.......................");
                    Terminal.WriteLine("......*      *                *   *****;.......................");
                    Terminal.WriteLine(".....**   ** * ,.     ;          .*****;.......................");
                    Terminal.WriteLine("....*,*  .***;         *         .******.......................");
                    Terminal.WriteLine("...+* +   ***,        ;;          ,*****.......................");
                    Terminal.WriteLine("..;*   *   ;;*   ;     ++  .        ****+......................");
                    Terminal.WriteLine("..*     ,****       .  **  .      .  ****;+*...................");
                    Terminal.WriteLine(".*             ,, *                   ****+*...................");
                    Terminal.WriteLine(";;           *  *  *     *         ;  ***** +..................");
                    Terminal.WriteLine("*            *   * +      ;         ;,  +, *.*.................");
                    Terminal.WriteLine("***+         ** ..            *;      *  .* ,*+................");
                    Terminal.WriteLine("    *        ,; ..,             .;*;   .****. *................");
                    Terminal.WriteLine("   *          .;  ;   *              *;    +*;.................");
                    Terminal.WriteLine("  +   ,       .*      +            ***;* .*+...................");
                    Terminal.WriteLine(" . **         ** +     *         ;* ,;..;;.....................");
                    Terminal.WriteLine(" **        .   * *     ,;   * * *.   *.........................");
                    Terminal.WriteLine("           ;    **      .+  * **     *.........................");
                    Terminal.WriteLine(" +               ;.       * +++       *........................");
                    Terminal.WriteLine(" .        ;        *       ***+       *;.......................");
                    break;
                }
                case 3:
                {
                    Terminal.WriteLine("                                                                 ");
                    Terminal.WriteLine("                                                                 ");
                    Terminal.WriteLine("                                                                 ");
                    Terminal.WriteLine("                                                                 ");
                    Terminal.WriteLine("                                                                 ");
                    Terminal.WriteLine("                                     +                           ");
                    Terminal.WriteLine("                                     +                           ");
                    Terminal.WriteLine("                                     ++++  +                     ");
                    Terminal.WriteLine("                                     +++++ ++                    ");
                    Terminal.WriteLine("                            +      ++ +++ ++ ++                  ");
                    Terminal.WriteLine("                      +   +++      ++  +++ ++ ++                 ");
                    Terminal.WriteLine("                     ++ +++++      +++++ + +  ++++               ");
                    Terminal.WriteLine("                   +++++++++  +     +++     +  +++++             ");
                    Terminal.WriteLine("                   +  ++ ++  ++      +++  +++ ++ +  +++++        ");
                    Terminal.WriteLine("                  ++  +   + +++   +   +  +    ++ +   ++++        ");
                    Terminal.WriteLine("              +++    +     +++    +++++ ++        +++++ +        ");
                    Terminal.WriteLine("         ++ ++    ++ +++   ++     +++++    +++++++++ +++         ");
                    Terminal.WriteLine("        +++++   + ++   ++  +        +++    +++++++++++  +        ");
                    Terminal.WriteLine("         ++ ++   +         +  ++      ++    + +++++++++++++      ");
                    Terminal.WriteLine("         +++  ++++ +++    ++++++  ++++++  + +++++  ++++ ++ +     ");
                    Terminal.WriteLine("         + ++++++++++++    ++++    ++++  ++  ++++     +++++ +    ");
                    Terminal.WriteLine("          +++++++++++ +             ++ ++ +  + ++++++++  +++++   ");
                    Terminal.WriteLine("      ++++++++++++++++  +++++++     +  ++ +   + ++++       ++++  ");
                    Terminal.WriteLine("      ++++++     ++++   +  ++++   +++ ++   +++ ++ +++       +++++");
                    Terminal.WriteLine("    +++++  +++++++++    +++  +    +++     +  ++ +++++            ");
                    Terminal.WriteLine("   +++++     +++++++   ++++  +    ++     ++   ++ ++++            ");
                    Terminal.WriteLine("  +++++       ++ ++  +++  ++++++  ++    +++++++++  +             ");
                    Terminal.WriteLine("   +         ++++   ++   +   +++ +++       +++++++++             ");
                    Terminal.WriteLine("             ++++  +   ++     ++ +++          ++                 ");
                    Terminal.WriteLine("              ++  +++  +++    ++ ++++       + +                  ");
                    Terminal.WriteLine("              ++++ +++   +    ++ +++++       ++                  ");
                    Terminal.WriteLine("                  +  +        ++++++    ++ + +                   ");
                    Terminal.WriteLine("                   + +       +++++++  ++   + +                   ");
                    Terminal.WriteLine("                   +        ++ + ++  +       ++                  ");
                    Terminal.WriteLine("                   ++ + +    + ++++  +    +  ++                  ");
                    break;
                }
            }
        }

        void drawStart()
        {
            Terminal.WriteLine( "               +*******+                                       \r\n" +
                                "              ***`  *******                                    \r\n" +
                                "             **.  ***     ***                                  \r\n" +
                                "            +**  **         **                                 \r\n" +
                                "            **, *`  ******.  *                                 \r\n" +
                                "            ** ;* .***********.                                \r\n" +
                                "            ** *, *****+   ;***                                \r\n" +
                                "            ** *  ****      ***                                \r\n" +
                                "            ** * ;***,     *.***                               \r\n" +
                                "            +* * +***     *. * *.                              \r\n" +
                                "            ;* * +***    *+  *  *                              \r\n" +
                                "             *`* +**`   `*  .*  .*                             \r\n" +
                                "             *** +**,   *`  .*   *.                            \r\n" +
                                "              **,+**   `*   .*    *                            \r\n" +
                                "              **`;**   *`   +*,   *.                           \r\n" +
                                "              +**;**   *    ***    *                           \r\n" +
                                "              ***;*.  ;*   .***    *.                          \r\n" +
                                "             `***;*,  *`   ****+   ,*                          \r\n" +
                                "             ****.*   *   .*****    *`                         \r\n" +
                                "       **   ****,.*   *  ,*******`  `*                         \r\n" +
                                "       *+*;***** ;*  ;* .*************                         \r\n" +
                                "       ,* ****;  +`  ;****************.+                       \r\n" +
                                "        *        *   ;****** *+*******.*.                      \r\n" +
                                "        **      *;  ;******  *. ..`    `+                      \r\n" +
                                "         **`  .*;   +`       ;.     `;***                      \r\n" +
                                "          .***;     .***.    ..  .*****                        \r\n" +
                                "                      *****;,.; *******;                       \r\n" +
                                "                      *******.* *******                        \r\n" +
                                "                      *.*****.* ***.  *                        \r\n" +
                                "                      *`  .** *      `*                        \r\n" +
                                "                      `* `    *      .;                        \r\n" +
                                "                       * *`.* * *`.. *.                        \r\n" +
                                "                       *`.;,* * *.`. *                         \r\n" +
                                "                       ;;.* * * *.`+ *                         \r\n" +
                                "                       ,*   * * ;` , *                         \r\n" +
                                "                        **`   *     ;*                         \r\n" +
                                "                         `**` *  .***                          \r\n" +
                                "                          *,***.**  *,,                        \r\n" +
                                "                      ;****; `**   ;*****                      \r\n" +
                                "                      *`*****,   `****+ *,  `                  \r\n" +
                                "                 **`  *  +***********, `*  ***.                \r\n" +
                                "                 *,*****  .*********   ***** .***              \r\n" +
                                "              ,***,****+*    ;+++.    *****  ***.**            \r\n" +
                                "             ** **;+**  ,**         `**  **  ***  `*,          \r\n" +
                                "            **  ***.*     ;**********    `; ,**+    *          \r\n" +
                                "           ;*   ;** *         `*.,       *; ,***    `*         \r\n" +
                                "           *     **,*`    ,;;``*         *  `***`   **         \r\n" +
                                "           *    ***`;+   ************;   *` ,*********         \r\n" +
                                "           ********` *  ;*.*  `*   *.*   .;  **********        \r\n" +
                                "         .*********, * ,*` *  `*  +* *;   *   ****    **       \r\n" +
                                "         *********.  * *.  +. `* ,*  ,*   **      ****`        \r\n" +
                                "         *          *`;*   `*******   *;   .******* *          \r\n" +
                                "         `***;.;***** **************   *    *       *          \r\n" +
                                "            ;***+  *  **.  *************;   *       *          \r\n" +
                                "            ++     *  ;*   ;*******   `*    *       *          \r\n" +
                                "            *      *   *`   ******    *,    *       *          \r\n" +
                                "            *      *    *  *` ** .*  *.     *      `*          \r\n" +
                                "            *,     *    `**,  .;  .***     ,*      **          \r\n" +
                                "           +**    ,*     .*   .;   .*      ,+     ***          ", ConsoleColor.DarkGray, ConsoleColor.Black);

        }
        void drawShop()
        {
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                         #############                       ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                       ####         #####                    ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                     ###  ############;####                  ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                   ### ####+        ####+###                 ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                  ### ##### ######     ###+###               ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                 #####+###############   #####               ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                ## ##;### + ######; #### ######              ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("               ## ##;## #############+#### ##+##             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("              ## #########;         ###+## + ####            ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("             ##### #####.            # ####  ##+##           ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("             ####;#####             ######+#####+#           ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("            ## #+####+             ##### ##+## ####          ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("            ########+    ##       ####+## #### ##+#          ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("            # #+# ##    ####      ###++### #### ####         ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("           ########    ######   ####++++### #+# ##+#         ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("           # #+# #+ ##### +########++++++## #### # #         ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("           # ##### ######+++#####.+++++++## ##+# ####        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          ######## ###+++++++++++++++++++##  ########        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          ##### #; ##++++++++++++++++++++##  #### #;#        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          # #;# #  ##++++++++++++++++++++##   #;# # #        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          # # # #  ##++++++++++++++++++++##   # # # #        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          # # # #  ##++++++++++++++++++++##   # # # #        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          # # # #  ##++++++++++++++++++++##   # # # #        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          # # # #  ##++++++++++++++++++++##   # #.# #        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          # # # #  ##++++++++++++++++++++##   # #.# #        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          # # # #  ##++++++++++++++++++++##   # #.# #        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          ######## #########+#######+######   # #.# #        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          ########  #########++#######;###+   # #####        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("          #### #+#     # ## +#####;##+##+#   ########        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("           #.# ####    # +#.#### ######+##   ##### ##        ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("           ######+#    # + +###  ### #####   # #+# #         ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("           #### ####   # + +### #### ### .  ########         ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("            #.####+##  ### +### #####  ;   ###### #+         ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("            #### #####   # #### ####+      ########          ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("             #### #####  ###########      ## #+###+          ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("             ##+####+###   ;###+ ###    #####+# ##           ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("              ####  ##+###    #####;   ####### ##;           ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("               ##### #######         ### ##+####+            ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                ##+ #####+############ ###+#####             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                ###+##  ####;  ##    ###+######              ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                  ##+###  ############+ ######               ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                   #######     ##;.  #######+                ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                    ###+ ############## ###.                 ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                      ####+ #######  ####;                   ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                        ###############.                     ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                            #######;                         ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);
            Terminal.WriteLine("                                                             ", ConsoleColor.Yellow, ConsoleColor.Black);


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
