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
        public string pnJunctionTitle = "PN结与单向导电性";
        public string pnJunctionDescription = "P区与N区接触后，多数载流子扩散并形成耗尽层。\n正向偏置降低势垒，电流容易通过；反向偏置增大势垒，电流近似截止。";
        public string pTypeTitle = "P型半导体";
        public string pTypeDescription = "P型半导体通过掺入受主杂质形成，空穴是多数载流子，电子是少数载流子。";
        public string nTypeTitle = "N型半导体";
        public string nTypeDescription = "N型半导体通过掺入施主杂质形成，电子是多数载流子，空穴是少数载流子。";

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

        void SetContent(string title, string description)
        {
            if (titleText != null)
                titleText.text = title;
            if (descriptionText != null)
                descriptionText.text = description;
        }
    }
}
