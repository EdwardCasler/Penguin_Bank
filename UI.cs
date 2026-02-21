using System;
using System.Collections;
using Spectre.Console;

class UI
{
    enum MenuOption
    {
        OPEN, CLOSE, TRANSFER, LIST, HISTORY, EXIT
    }
    List<string> transferHistory = new();
    private readonly Dictionary<string, MenuOption> optionEnum = new()
    {
        { "Open an account.", MenuOption.OPEN },
        { "Close an account.", MenuOption.CLOSE },
        { "Make a transfer.", MenuOption.TRANSFER },
        { "List accounts.", MenuOption.LIST },
        { "List transfer history", MenuOption.HISTORY },
        { "Exit.", MenuOption.EXIT },
    };

    public void Run(Bank bank)
    {
        AnsiConsole.Clear();
        while(true) {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Welcome to [black]PENGUIN[/][white]_[/][#FFA500]BANK[/]");
            string choiceText = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(optionEnum.Keys));
            AnsiConsole.MarkupLine("[blue]You chose to " + choiceText.ToLower() + "[/]");
            
            MenuOption menuOption = optionEnum[choiceText];

            switch (menuOption) {
                case MenuOption.OPEN :
                    RunSafely(() => { OpenAccount(bank); } ); break;
                case MenuOption.CLOSE :
                    RunSafely(() => { CloseAccount(bank); }); break;
                case MenuOption.TRANSFER :
                    RunSafely(() => { Transfer(bank); }); break;
                case MenuOption.LIST :
                    RunSafely(() => { ListAccounts(bank); }); break;
                case MenuOption.HISTORY :
                    RunSafely(() => { ListHistory(); }); break;
                case MenuOption.EXIT :
                    return;
            }
        }
    }
    private void OpenAccount(Bank bank)
    {
        string name = AnsiConsole.Ask<string>("Account name?");
        double balance = AskForValidMoneyAmount("Starting Balance?");
        bank.OpenAccount(name, balance);

        AnsiConsole.MarkupLine("[green]Account created in " + name + "'s name with $" + balance + "." + "[/]");
    }
    private void CloseAccount(Bank bank)
    {
        string name = AskForARegisteredAccount("Account name?", bank);
        bank.CloseAccount(name);

        AnsiConsole.MarkupLine("[green]Account closed with name " + name + "[/]");
    }
    private void ListAccounts(Bank bank)
    {
        foreach (Account account in bank.accounts)
        {
            AnsiConsole.MarkupLine("[blue]" + account.name + ", [/][green]$" + account.balance + "[/]");
        }
    }
    private void ListHistory()
    {
        if(transferHistory.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There is no history to list.[/]");
            return;
        }

        foreach (string transfer in transferHistory)
        {
            AnsiConsole.MarkupLine(transfer);
        }
    }
    private void Transfer(Bank bank)
    {
        string fromName = AskForARegisteredAccount("Account name that gives?", bank);
        Account from = bank.GetAccountFromName(fromName);
        
        string toName = AskForARegisteredAccount("Account name that receives?", bank);
        Account to = bank.GetAccountFromName(toName);

        double amount = AskForValidTransferAmount("How much to transfer?", from);
        bank.Transfer(from, to, amount); 

        AnsiConsole.MarkupLine("[green]Successfully transfered $" + amount + " from " + fromName + " to " + toName + ".[/]");
        transferHistory.Add("[black]Transfered [/][green]$" + amount + "[/][black] from [/][white]" + fromName + "[/][black] to [/][white]" + toName + "[/][black].[/]");
    }
    private double AskForValidMoneyAmount(string ask)
    {
        return AnsiConsole.Prompt(new TextPrompt<double>(ask)
            .Validate(amount =>
            {
                if(amount < 0)
                {
                    return ValidationResult.Error("[red]Balance cannot be negative[/]");
                }
                return ValidationResult.Success();
            }));
    }
    private double AskForValidTransferAmount(string ask, Account from)
    {
        return AnsiConsole.Prompt(new TextPrompt<double>(ask)
            .Validate(amount =>
            {
                if(amount > from.balance)
                {
                    return ValidationResult.Error("[red]Transfer amount is greater than sources balance[/]");
                } 
                else if(amount < 0)
                {
                    return ValidationResult.Error("[red]Transfer amount cannot be negative[/]");
                }
                return ValidationResult.Success();
            }));
    }
    private string AskForARegisteredAccount(string ask, Bank bank)
    {
        return AnsiConsole.Prompt(new TextPrompt<string>(ask)
            .Validate(name =>
            {
                if(!bank.NameIsAnAccount(name))
                {
                    return ValidationResult.Error("[red]No account registered under that name[/]");
                }
                return ValidationResult.Success();
            }));
    }
    private void RunSafely(Action action)
    {
        try
        {
            action();
            WaitUntilInput();
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[red]" + e.Message + "[/]");
        }
    }
    private void WaitUntilInput()
    {
        AnsiConsole.MarkupLine("[italic black]enter to continue[/]");
        Console.ReadLine();
        AnsiConsole.Clear();
    }
}