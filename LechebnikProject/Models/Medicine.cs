namespace LechebnikProject.Models
{
    /// <summary>
    /// Модель препарата.
    /// </summary>
    public class Medicine
    {
        public int MedicineId { get; set; }         // Уникальный идентификатор
        public string Name { get; set; }            // Название
        public string Form { get; set; }            // Форма (таблетки, сироп и т.д.)
        public string WeightVolume { get; set; }    // Вес/объем
        public string SerialNumber { get; set; }    // Серийный номер
        public string Usage { get; set; }           // Применение
        public string ActiveIngredient { get; set; } // Действующее вещество
        public string ApplicationMethod { get; set; } // Способ применения
        public string AggregateState { get; set; }  // Агрегатное состояние
        public string Type { get; set; }            // Тип препарата
        public int ManufacturerId { get; set; }     // Идентификатор производителя
        public int SupplierId { get; set; }         // Идентификатор поставщика
        public int StockQuantity { get; set; }      // Количество на складе
        public bool RequiresPrescription { get; set; } // Требуется ли рецепт
        public decimal Price { get; set; }          // Цена в рублях
    }
}
