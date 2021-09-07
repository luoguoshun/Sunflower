using System;
using System.Threading.Tasks;

namespace OHEXML.Infrastructure.Test
{
    public class Account
    {
        private readonly object balanceLock = new object();
        private decimal balance;
        public Account(decimal initialBalance)
        {
            //定义初始余额
            balance = initialBalance;
        }
        /// <summary>
        /// 借记
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public decimal Debit(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "借方金额不能为负数.");
            }
            decimal appliedAmount = 0;
            lock (balanceLock)
            {
                //在进行减少余额的时候进行加锁操作，防止其他线程进入
                if (balance >= amount)
                {
                    balance -= amount;
                    appliedAmount = amount;
                }
            }
            return appliedAmount;
        }
        /// <summary>
        /// 信用卡
        /// </summary>
        /// <param name="amount"></param>
        public void Credit(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "贷方金额不能为负.");
            }
            lock (balanceLock)
            {
                //在进行增加余额的时候进行加锁操作，防止其他线程进入
                balance += amount;
            }
        }
        /// <summary>
        /// 获取余额
        /// </summary>
        /// <returns></returns>
        public decimal GetBalance()
        {
            lock (balanceLock)
            {
                return balance;
            }
        }
    }
    public class AccountTest
    {

        static async Task Main()
        {
            Account account = new Account(1000);
            Task[] tasks = new Task[100];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() => Update(account));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine($"账户的余额为 {account.GetBalance()}");
        }
        /// <summary>
        /// 更新余额
        /// </summary>
        /// <param name="account"></param>
        static void Update(Account account)
        {
            decimal[] amounts = { 0, 2, -3, 6, -2, -1, 8, -5, 11, -6 };
            foreach (var amount in amounts)
            {
                if (amount >= 0)
                {
                    account.Credit(amount);
                }
                else
                {
                    account.Debit(Math.Abs(amount));
                }
            }
        }
    }
}
