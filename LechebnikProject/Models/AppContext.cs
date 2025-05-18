using System.Collections.Generic;
using LechebnikProject.Models;

namespace LechebnikProject
{
    public static class AppContext
    {
        public static User CurrentUser { get; set; }
        public static List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}