using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVReaderScript {

	public static void ReadEquipmentData(string equipmentID, EquipmentBaseScript equipmentReference)
    {

        // Open the csv
        //TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/ResourcesImportData/EquipmentData.csv");

        TextAsset data = Resources.Load<TextAsset>("ImportData/EquipmentData");

        // Find the start index of the first bit of data we are looking for
        int equipmentDataStartIndex = data.text.IndexOf(',', data.text.IndexOf(equipmentID)) + 1;

        // Find end index
        int equipmentDataEndIndex = data.text.IndexOf(',', equipmentDataStartIndex);

        // Grab name info
        equipmentReference.equipmentName = data.text.Substring(equipmentDataStartIndex, equipmentDataEndIndex - equipmentDataStartIndex);

        // Update start index
        equipmentDataStartIndex = equipmentDataEndIndex + 1;

        // Find new end index
        equipmentDataEndIndex = data.text.IndexOf(',', equipmentDataStartIndex);

        // Grab description info
        equipmentReference.equipmentDescription = data.text.Substring(equipmentDataStartIndex, equipmentDataEndIndex - equipmentDataStartIndex);

        // Update start index
        equipmentDataStartIndex = equipmentDataEndIndex + 1;

        // Find new end index
        equipmentDataEndIndex = data.text.IndexOf(',', equipmentDataStartIndex);

        // Grab stat info
        equipmentReference.equipmentStats = data.text.Substring(equipmentDataStartIndex, equipmentDataEndIndex - equipmentDataStartIndex);

        // Update start index
        equipmentDataStartIndex = equipmentDataEndIndex + 1;

        // Find new end index
        equipmentDataEndIndex = data.text.IndexOf(',', equipmentDataStartIndex);

        // Grab stat info
        equipmentReference.equipmentJudgementName = data.text.Substring(equipmentDataStartIndex, equipmentDataEndIndex - equipmentDataStartIndex);

        // Update start index
        equipmentDataStartIndex = equipmentDataEndIndex + 1;

        // Find new end index
        equipmentDataEndIndex = data.text.IndexOf(",", equipmentDataStartIndex);

        // Grab stat info
        equipmentReference.equipmentJudgementStats = data.text.Substring(equipmentDataStartIndex, equipmentDataEndIndex - equipmentDataStartIndex);

        // Update start index
        equipmentDataStartIndex = equipmentDataEndIndex + 1;

        // Find new end index
        equipmentDataEndIndex = data.text.IndexOf("\n", equipmentDataStartIndex);

        // Grab sprite asset
        equipmentReference.equipmentIcon = Resources.Load<Sprite>(data.text.Substring(equipmentDataStartIndex, equipmentDataEndIndex - equipmentDataStartIndex - 1));
        equipmentReference.equipmentJudgementIcon = Resources.Load<Sprite>(data.text.Substring(equipmentDataStartIndex, equipmentDataEndIndex - equipmentDataStartIndex - 1) + "Highlight");

        // Close data
        Resources.UnloadUnusedAssets();
    }

    public static void ReadNarrativeData(string scenarioNumber, NarrativeManagerScript narrativeManagerReference)
    {
        TextAsset data = Resources.Load<TextAsset>("ImportData/NarrativeScenarios/NarrativeScenario" + scenarioNumber);

        // WILL NEED TO FIGURE OUT LOCALISATION THINGS LATER

        int dataStartIndex;
        int dataEndIndex;
        int paragraphNumber = 1;
        int dataLastParagraphIndex = data.text.LastIndexOf("\n");

        bool endReached = false; 

        // Start looping to retrieve data
        while (endReached == false)
        {
            // Find the portrait and deal with that first
            dataStartIndex = data.text.IndexOf("*N" + paragraphNumber.ToString());

            // Check if end reached
            if (dataStartIndex < 5)
            {
                break;
            }

            dataStartIndex = data.text.IndexOf(',', dataStartIndex) + 1;

            dataEndIndex = data.text.IndexOf(',', dataStartIndex);

            narrativeManagerReference.AddScenarioPortrait(data.text.Substring(dataStartIndex, dataEndIndex - dataStartIndex));

            // Then get the paragraph
            dataStartIndex = dataEndIndex + 1;

            dataEndIndex = data.text.IndexOf(',', dataStartIndex);

            narrativeManagerReference.AddScenarioParagraph(ConvertCommas(data.text.Substring(dataStartIndex, dataEndIndex - dataStartIndex)));
            

            // Check if we're at the end
            if (dataEndIndex > dataLastParagraphIndex)
            {
                endReached = true;
            }
            else
            {
                paragraphNumber += 1;
                // Cautionary break point so it doesnt loop forever
                if (paragraphNumber > 40)
                {
                    Debug.Log("CSV Reader has looped too many times reading narrative data");
                    break;
                }
            }
        }

        // Unload the resource
        Resources.UnloadUnusedAssets();
    }

    static public string ConvertCommas(string textData)
    {
        return textData.Replace(';', ',');
    }
}
