namespace GuitarToolkit.Core.DSP;

/// <summary>
/// Комплексное число (float) для FFT.
/// </summary>
public struct ComplexFloat
{
    public float Re, Im;

    public ComplexFloat(float re, float im) { Re = re; Im = im; }

    public float Magnitude => MathF.Sqrt(Re * Re + Im * Im);

    public static ComplexFloat operator +(ComplexFloat a, ComplexFloat b)
        => new(a.Re + b.Re, a.Im + b.Im);

    public static ComplexFloat operator -(ComplexFloat a, ComplexFloat b)
        => new(a.Re - b.Re, a.Im - b.Im);

    public static ComplexFloat operator *(ComplexFloat a, ComplexFloat b)
        => new(a.Re * b.Re - a.Im * b.Im, a.Re * b.Im + a.Im * b.Re);
}

/// <summary>
/// Быстрое преобразование Фурье (Cooley–Tukey, radix-2, in-place).
/// Размер массива ДОЛЖЕН быть степенью двойки.
/// </summary>
public static class Fft
{
    public static void Forward(ComplexFloat[] data)
    {
        int n = data.Length;

        // Бит-реверсивная перестановка
        for (int i = 1, j = 0; i < n; i++)
        {
            int bit = n >> 1;
            while ((j & bit) != 0) { j ^= bit; bit >>= 1; }
            j ^= bit;
            if (i < j) (data[i], data[j]) = (data[j], data[i]);
        }

        // Бабочки
        for (int len = 2; len <= n; len <<= 1)
        {
            float angle = -2f * MathF.PI / len;
            var wLen = new ComplexFloat(MathF.Cos(angle), MathF.Sin(angle));

            for (int i = 0; i < n; i += len)
            {
                var w = new ComplexFloat(1f, 0f);
                for (int j = 0; j < len / 2; j++)
                {
                    var u = data[i + j];
                    var v = data[i + j + len / 2] * w;
                    data[i + j] = u + v;
                    data[i + j + len / 2] = u - v;
                    w = w * wLen;
                }
            }
        }
    }
}
