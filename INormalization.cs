using System;

namespace ai_learning
{
    public interface INormalization
    {
        void GaussNormal(double[][] data, int column);
        void MinMaxNormal(double[][] data, int column);
        void ShowMatrix(double[][] matrix, int decimals);
        void ShowData(string[] rawData);
        void EncodeFile(string originalFile, string encodedFile, int column, string encodingType);
        string EffectsEncoding(int index, int N);
        string Dummyencoding(int index, int N);

    }
}