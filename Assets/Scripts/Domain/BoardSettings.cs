namespace Domain {
    public class BoardSettings {
        public int Width { get; set; }
        public int Height { get; set; }

        public BoardSettings() {
            Width = 5;
            Height = 7;
        }
    }
}