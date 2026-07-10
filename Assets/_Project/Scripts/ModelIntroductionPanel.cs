using UnityEngine;
using UnityEngine.UI;

namespace SemiconductorTeaching
{
    public sealed class ModelIntroductionPanel : MonoBehaviour
    {
        public Text titleText;
        public Text descriptionText;

        public string waferTitle = "晶圆";
        public string waferDescription = "晶圆是制造半导体器件的基础材料，也是后续掺杂与形成 PN 结的载体。";
        public string pnJunctionTitle = "PN 结与单向导电性";
        public string pnJunctionDescription = "P 区与 N 区接触后，多数载流子扩散并形成耗尽层。正向偏置降低势垒，电流容易通过；反向偏置增大势垒，电流近似截止。";
        public string pTypeTitle = "P 型半导体";
        public string pTypeDescription = "P 型半导体通过掺入受主杂质形成，空穴是多数载流子，电子是少数载流子。";
        public string nTypeTitle = "N 型半导体";
        public string nTypeDescription = "N 型半导体通过掺入施主杂质形成，电子是多数载流子，空穴是少数载流子。";

        public string diffusionTitle = "载流子扩散";
        public string diffusionDescription = "N 区电子和 P 区空穴会因浓度差向 PN 结附近扩散，这是耗尽层形成的起点。";
        public string recombinationTitle = "电子与空穴复合";
        public string recombinationDescription = "电子和空穴在结区附近相遇并复合，移动载流子减少，中间区域逐渐变成耗尽层。";
        public string depletionTitle = "耗尽层形成";
        public string depletionDescription = "复合后留下不能自由移动的固定离子，形成内建电场，阻止载流子继续无限扩散。";
        public string fullAnimationTitle = "PN 结形成过程";
        public string fullAnimationDescription = "完整演示会依次表现扩散、复合和耗尽层形成，帮助理解 PN 结从接触到稳定的过程。";
        public string resetAnimationTitle = "演示复位";
        public string resetAnimationDescription = "模型已回到演示前状态，可以重新选择某一段过程观察。";

        public void Show(int modelIndex)
        {
            switch (modelIndex)
            {
                case 0:
                    SetContent(waferTitle, waferDescription);
                    break;
                case 1:
                    SetContent(pnJunctionTitle, pnJunctionDescription);
                    break;
                case 2:
                    SetContent(pTypeTitle, pTypeDescription);
                    break;
                case 3:
                    SetContent(nTypeTitle, nTypeDescription);
                    break;
            }
        }

        public void ShowAnimation(int animationIndex)
        {
            switch (animationIndex)
            {
                case 0:
                    SetContent(diffusionTitle, diffusionDescription);
                    break;
                case 1:
                    SetContent(recombinationTitle, recombinationDescription);
                    break;
                case 2:
                    SetContent(depletionTitle, depletionDescription);
                    break;
                case 3:
                    SetContent(fullAnimationTitle, fullAnimationDescription);
                    break;
                case 4:
                    SetContent(resetAnimationTitle, resetAnimationDescription);
                    break;
            }
        }

        void SetContent(string title, string description)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (descriptionText != null)
            {
                descriptionText.text = description;
            }
        }
    }
}
