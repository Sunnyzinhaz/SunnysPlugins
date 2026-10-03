using System;
using System.Text;

namespace CustomItems.Core;

public class BarGenerator(int progressBarLength, string fillCharacter = BarGenerator.BasicCharacter, string emptyCharacter = BarGenerator.BasicCharacter)
{
    public const string BasicCharacter = "||||||||||||||||||||||||||||||||||||||||";

    public int ProgressBarLength { get; set; } = progressBarLength;
    public int ProgressBarFill { get; set; } = 0;
    public string FillCharacter { get; set; } = fillCharacter;
    public string EmptyCharacter { get; set; } = emptyCharacter;
    //public float Cspace { get; set; } = -4.5f;
    public float Size { get; set; } = 40f;
    public float CspaceIncrementPerSize { get; set; } = -0.204347826087f;
    // -0.204347826087f
    // -0.217391304348f

    public Func<int, int, (string fillColor, string emptyColor)> RenderColor = (index, length) =>
    {
        int R = (int)Math.Min(255f, 71f * (index / (float)length + 1f));
        int G = (int)Math.Min(255f, 191f * (index / (float)length + 1f));
        int B = (int)Math.Min(255f, 255f * (index / (float)length + 1f));
        int Empty = (int)Math.Min(255f, 71f * (length - index));

        return ($"#{R:X2}{G:X2}{B:X2}ff", $"#FFFFFF{Empty:X2}");
    };

    public string Generate()
    {
        StringBuilder progressBar = new();
        float cspace = CspaceIncrementPerSize * Size;

        progressBar.Append($"<cspace={cspace}><size={Size}>");

        for (int i = 0; i < ProgressBarLength; i++)
        {
            var colors = RenderColor(i, ProgressBarLength);

            if (i <= ProgressBarFill && ProgressBarFill > 0)
                progressBar.Append($"<color={colors.fillColor}>{FillCharacter}</color>");
            else
                progressBar.Append($"<color={colors.emptyColor}>{EmptyCharacter}</color>");
        }

        progressBar.Append("</size></cspace>");

        return progressBar.ToString();
    }

    public string Update(int newFill)
    {
        ProgressBarFill = newFill;
        return Generate();
    }
}