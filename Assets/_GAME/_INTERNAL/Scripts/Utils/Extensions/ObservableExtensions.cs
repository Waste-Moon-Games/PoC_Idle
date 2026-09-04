using R3;

namespace Utils.Extensions
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Фильтрует поток объектов, оставляя только конкретный тип
        /// </summary>
        /// <typeparam name="T">Тип элементов для фильтрации</typeparam>
        /// <param name="source">Исходный поток данных</param>
        /// <returns>Поток элементов типа<typeparam name="T"/></returns>
        public static Observable<T> OfType<T>(this Observable<object> source)
        {
            return source
                .Where(x => x is T)
                .Select(x => (T)x);
        }
    }
}
