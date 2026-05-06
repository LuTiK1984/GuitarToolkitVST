namespace GuitarToolkit.Core.Generation;

public sealed class OnnxProgressionModel : IProgressionNextTokenModel
{
    public OnnxProgressionModel(string modelPath)
    {
        ModelPath = modelPath;
    }

    public string ModelPath { get; }

    public ProgressionModelOutput PredictNext(ProgressionModelInput input)
    {
        if (string.IsNullOrWhiteSpace(ModelPath) || !File.Exists(ModelPath))
        {
            return new ProgressionModelOutput
            {
                ModelName = "ProgressionNextTokenModel.onnx",
                IsAvailable = false,
                Status = "ONNX-файл модели пока не настроен. Используется демонстрационный fallback."
            };
        }

        return new ProgressionModelOutput
        {
            ModelName = Path.GetFileName(ModelPath),
            IsAvailable = false,
            Status = "Файл модели найден, но ONNX Runtime адаптер еще не подключен."
        };
    }
}
