using System.Collections.Generic;

[System.Serializable]
public class Sentence {
    public string text;
}

[System.Serializable]
public class SentenceCollection {
    public List<Sentence> sentences;
}