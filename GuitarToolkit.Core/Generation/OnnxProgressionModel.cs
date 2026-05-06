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
                Status = "ONNX model file is not configured yet. Demo fallback is used."
            };
        }

        return new ProgressionModelOutput
        {
            ModelName = Path.GetFileName(ModelPath),
            IsAvailable = false,
            Status = "ONNX Runtime adapter is reserved for the trained model integration."
        };
    }
}
