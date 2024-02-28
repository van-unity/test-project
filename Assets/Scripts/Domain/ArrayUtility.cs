namespace Domain {
    public static class ArrayUtility {
        public static void Swap<T>(this T[,] array, BoardPos from, BoardPos to) {
            (array[from.x, from.y], array[to.x, to.y]) = (array[to.x, to.y], array[from.x, from.y]);
        }
    }
}