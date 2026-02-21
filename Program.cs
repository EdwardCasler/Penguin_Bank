class Program
{
    static Bank bank = new();
    static UI ui = new();
    static void Main()
    {
        ui.Run(bank);
    }
}