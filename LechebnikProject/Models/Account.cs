namespace LechebnikProject.Models
{
    public class Account
    {
        public int AccountId { get; set; }    // Уникальный идентификатор аккаунта
        public int UserId { get; set; }       // Ссылка на пользователя
        public decimal Balance { get; set; }  // Баланс аккаунта в рублях
    }
}