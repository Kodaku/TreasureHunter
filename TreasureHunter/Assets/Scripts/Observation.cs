using UnityEngine;
using System.IO;
public class Observation
{
    int m_startX;
    int m_startY;
    float m_episodeDuration;
    float m_qMax;
    float m_qMin;
    int m_numberOfSteps;
    float m_qValue;
    bool m_concludedWithTreasure;

    public int startX
    {
        get { return m_startX; }
        set { m_startX = value; }
    }
    public int startY
    {
        get { return m_startY; }
        set { m_startY = value; }
    }
    public float episodeDuration
    {
        get { return m_episodeDuration; }
        set { m_episodeDuration = value; }
    }

    public float qMax
    {
        get { return m_qMax; }
        set { m_qMax = value; }
    }

    public float qMin
    {
        get { return m_qMin; }
        set { m_qMin = value; }
    }

    public int numberOfSteps
    {
        get { return m_numberOfSteps; }
        set { m_numberOfSteps = value; }
    }

    public float qValue
    {
        get { return m_qValue; }
        set { m_qValue = value; }
    }

    public bool concludedWithTreasure
    {
        get { return m_concludedWithTreasure; }
        set { m_concludedWithTreasure = value; }
    }

    public void SaveToFile(bool append = true)
    {
        string tsvPath = Application.dataPath + "/Resources/Observations.tsv";
        string tsvData = m_startX + "\t" +
                        m_startY + "\t" +
                        m_episodeDuration.ToString().Replace(",", ".") + "\t" +
                        m_numberOfSteps + "\t" +
                        m_qMin.ToString().Replace(",", ".") + "\t" +
                        m_qMax.ToString().Replace(",", ".") + "\t" +
                        qValue.ToString().Replace(",",".") + "\t" +
                        m_concludedWithTreasure.ToString().ToUpper();
        StreamWriter tsvWriter = new StreamWriter(tsvPath, append);
        tsvWriter.WriteLine(tsvData);
        tsvWriter.Flush();
        tsvWriter.Close();
    }
}