using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace interweb.CalendarCalculator
{
    /******************************************************************************************************************
     * This class reads states and holidays from XML files and stores them for holiday calculation.
     * 
     * Structure of the State XML-file: ID 0 is reserved for the catalog, the catalog holds all holidays.
     * for a whole country/languange section. All IDs must be unique!
     * 
     * <?xml version="1.0" encoding="utf-8"?>
     * <States.Config>
     *   <Country>
     *     <CountryCode>DE</CountryCode>
     *     <Language>DE</Language>
     *     <States>
     *       <State>
     *         <StateID>0</StateID>
     *         <StateName>Catalog</StateName>
     *       </State>
     *       <State>
     *         <StateID>1</StateID>
     *         <StateName>Germany - Basic Holidays</StateName>
     *       </State>
     *       <State>
     *         <StateID>2</StateID>
     *         <StateName>Bavaria</StateName>
     *       </State>
     *     </States>
     *   </Country>
     * </States.Config>
     * 
     * 
     * Structure of the Holiday XML-file: Formula structure is described within the holiday class.
     * The StateIDList holds all State IDs, on which the respective holiday is a valid holiday.
     * 
     * <?xml version="1.0" encoding="utf-8"?>
     * <Holidays.Config>
     *   <Country>
     *     <CountryCode>DE</CountryCode>
     *     <Language>DE</Language>
     *     <Catalog>
     *       <Holiday>
     *         <HolidayID>10</HolidayID>
     *         <HolidayName>Neujahr</HolidayName>
     *         <HolidayFormula>F:1.1</HolidayFormula>
     *         <HolidayDuration>1</HolidayDuration>
     *         <HolidayValidFrom>01.01.1900</HolidayValidFrom>
     *         <HolidayValidTo>31.12.2099</HolidayValidTo>
     *         <StateIDList>0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17</StateIDList>
     *       </Holiday>
     *     </Catalog>
     *   </Country>
     * </Holidays.Config>
     * 
     * 
     * Last update: 15.05.2010
     ******************************************************************************************************************/
    //-----------------------------------------------------------------------------------------------------------------
    public class HolidayMatrix
    //-----------------------------------------------------------------------------------------------------------------
    {
        string _countryCode;
        string _language;
        int _currentStateID;
        Dictionary<State, List<Holiday>> _holidayMatrix;

        //-----------------------------------------------------------------------------------------------------------------
        public HolidayMatrix( string countryCode, string language, string pathXMLstates, string pathXMLholidays )
        //-----------------------------------------------------------------------------------------------------------------
        {
            XDocument xdoc;                                // Holds the respective XML document
            Holiday holiday;                               // temp. class to hold a holiday
            string[] stateIDs;                             // current state ID list from XML file, splitted

            this._countryCode = countryCode.Trim().ToUpper();
            this._language = language.Trim().ToUpper();
            this._currentStateID = 0;
            this._holidayMatrix = new Dictionary<State, List<Holiday>>();      // The key of the Dictionary is a State object!

            xdoc = XDocument.Load(pathXMLstates);          // Read XML file with state informations...

            IEnumerable<XElement> allElementsLanguage = xdoc.Root.Elements("Country").Where(x =>
                       ((string)x.Element("CountryCode")).Trim().ToUpper() == this._countryCode &&
                       ((string)x.Element("Language")).Trim().ToUpper() == this._language).Descendants("State");
   
                                                           // Create state object from each record and add to the matrix...
            foreach (XElement stateItem in allElementsLanguage)
            {
                this._holidayMatrix.Add(new State( int.Parse(stateItem.Element("StateID").Value), stateItem.Element("StateName").Value.ToString()), 
                    new List<Holiday>() );
            }

            xdoc = XDocument.Load(pathXMLholidays);       // Read XML file with holiday informations...

            IEnumerable<XElement> allHolidays = xdoc.Root.Elements("Country").Where(x =>
                      ((string)x.Element("CountryCode")).Trim().ToUpper() == this._countryCode && 
                      ((string)x.Element("Language")).Trim().ToUpper() == this._language).Descendants("Catalog").Descendants("Holiday");

            foreach (XElement holidayItem in allHolidays) // Create holiday object from each record and add to the matrix...
            {
                holiday = new Holiday( int.Parse( holidayItem.Element("HolidayID").Value ), holidayItem.Element("HolidayName").Value.ToString(),
                    holidayItem.Element("HolidayFormula").Value.ToString(),
                    int.Parse( holidayItem.Element("HolidayDuration").Value ), 
                    Convert.ToDateTime( holidayItem.Element("HolidayValidFrom").Value ),
                    Convert.ToDateTime( holidayItem.Element("HolidayValidTo").Value ) );

                                                          // Split state ID List and distribute current holiday to each state from ID list...
                stateIDs = holidayItem.Element("StateIDList").Value.ToString().Split(',');

                for( int i = 0; i < stateIDs.Length; i++ )
                {
                    if (_holidayMatrix.ContainsKey(new State(int.Parse(stateIDs[i]), "")))         // If state is in matrix, add holiday...
                    {
                        this._holidayMatrix[new State(int.Parse(stateIDs[i]), "")].Add(holiday);
                    }
                    else                                                                           // Otherwise add unknown state and add holiday...
                    {
                        this._holidayMatrix.Add(new State(int.Parse(stateIDs[i]), "Unknown State"), new List<Holiday>());
                        this._holidayMatrix[new State(int.Parse(stateIDs[i]), "")].Add(holiday);
                    }
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public string CountryCode
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return this._countryCode; }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public string Language
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return this._language; }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public List<Holiday> Holidays
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return( this._holidayMatrix[ new State( this.CurrentStateID, "" ) ] ); }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public int CurrentStateID
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return this._currentStateID; }
            set { this._currentStateID = value; }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public string CurrentStateName
        //-----------------------------------------------------------------------------------------------------------------
        {
            // Search in the key collection (state objects) of the matrix for the current state and get the right Name from element #0...
            get { return (this._holidayMatrix.Keys.Where( k => k.StateID == this.CurrentStateID)).ElementAt(0).StateName; }
        }
    }
}
