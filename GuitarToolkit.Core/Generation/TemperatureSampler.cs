namespace GuitarToolkit.Core.Generation;

public sealed class TemperatureSampler
{
    public string Sample(
        IReadOnlyList<ModelTokenProbability> probabilities,
        double temperature,
        int topK,
        Random random)
    {
        if (probabilities.Count == 0)
            return "i";

        temperature = Math.Clamp(temperature, 0.05, 2.0);
        topK = Math.Clamp(topK, 1, probabilities.Count);

        var candidates = probabilities
            .OrderByDescending(item => item.Probability)
            .Take(topK)
            .Select(item => new ModelTokenProbability(
                item.Token,
                Math.Pow(Math.Max(item.Probability, 0.0001), 1.0 / temperature)))
            .ToArray();

        double total = candidates.Sum(item => item.Probability);
        if (total <= 0)
            return candidates[0].Token;

        double roll = random.NextDouble() * total;
        double cumulative = 0;

        foreach (var candidate in candidates)
        {
            cumulative += candidate.Probability;
            if (roll <= cumulative)
                return candidate.Token;
        }

        return candidates[^1].Token;
    }
}
