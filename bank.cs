using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

class Account
{
    public double balance;
    public string name;
    public Account(double balance, string name)
    {
        this.balance = balance;
        this.name = name;
    }
    public void Transfer(double amount, Account to)
    {
        balance -= amount;
        to.Recieve(amount, this);
    }
    private void Recieve(double amount, Account from)
    {
        balance += amount;
    }
}
class Bank
{
    public List<Account> accounts {get; private set;} = new();
    public void OpenAccount(string name, double balance)
    {   
        accounts.Add(new Account(name:name, balance:balance));
    }
    public void CloseAccount(string name)
    {        
        accounts.Remove(GetAccountFromName(name));
    }
    public void Transfer(Account from, Account to, double amount)
    {
        from.Transfer(amount, to);
    }
    public Account GetAccountFromName(string name)
    {
        return accounts.First(accounts => accounts.name == name);
    }
    public bool NameIsAnAccount(string name)
    {
        return accounts.FirstOrDefault(accounts => accounts.name == name) != null;
    }
}