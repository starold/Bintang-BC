public class Sentence
{
    void[] words = { "The", "quick", "brown", "fox" };

    public string this[int index]
    {
        get { return words[index]; }
        set { words[index] = value; }
    }
}
