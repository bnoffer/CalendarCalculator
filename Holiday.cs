using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace interweb.CalendarCalculator
{
    /******************************************************************************************************************
     * This class holds the whole structure of a holiday
     * 
     * Basic structure of a holiday formula:    "type:modifier"
     * Basic rule: The Fixed holiday type "F" needs day and month modifier seperated by ".", 
     * all others an int offset value.
     * 
     * F:25.12      => Fixed holiday on 25 Dec.
     * E:-49        => Floating holiday 49 days before Easter Sunday
     * E:1          => Floating holiday 1 day after Easter Sunday
     * E:0          => Easter Sunday
     * MD:0         => Mothers Day
     * AD:0         => 4th Advdent
     * AD:-7        => 3rd Advent
     * RP:0         => RepentanceAndPrayer (German "Buß und Bettag")
     * 
     * Last update: 15.05.2010
     ******************************************************************************************************************/
    //-----------------------------------------------------------------------------------------------------------------
    public class Holiday
    //-----------------------------------------------------------------------------------------------------------------
    {
        int _holidayID;
        string _holidayName;
        string _holidayFormulaType;
        int _holidayFormulaOffset;
        int _holidayFormulaDay;
        int _holidayFormulaMonth;
        int _holidayDuration;
        DateTime _holidayValidFrom;
        DateTime _holidayValidTo;

        //-----------------------------------------------------------------------------------------------------------------
        public Holiday(int holidayID, string holidayName, string holidayFormula, int holidayDuration, DateTime holidayValidFrom, DateTime holidayValidTo)
        //-----------------------------------------------------------------------------------------------------------------
        {
            string[] formula = holidayFormula.Trim().ToUpper().Split(':');
            string[] dayAndMonth;

            this._holidayID = holidayID;
            this._holidayName = holidayName;
            this._holidayDuration = holidayDuration;
            this._holidayValidFrom = holidayValidFrom;
            this._holidayValidTo = holidayValidTo;

            this._holidayFormulaType = formula[0];

            if (this._holidayFormulaType == "F")
            {
                dayAndMonth = formula[1].Split('.');
                this._holidayFormulaOffset = 0;
                this._holidayFormulaDay = int.Parse(dayAndMonth[0]);
                this._holidayFormulaMonth = int.Parse(dayAndMonth[1]);
            }
            else
            {
                this._holidayFormulaOffset = int.Parse( formula[1] );
                this._holidayFormulaDay = 0;
                this._holidayFormulaMonth = 0;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public int HolidayID
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return( this._holidayID ); }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public string HolidayName
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return( this._holidayName ); }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public string HolidayFormulaType
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return( this._holidayFormulaType ); }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public int HolidayFormulaOffset
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return (this._holidayFormulaOffset); }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public int HolidayFormulaDay
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return (this._holidayFormulaDay); }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public int HolidayFormulaMonth
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return (this._holidayFormulaMonth); }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public string HolidayFormula
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return( this._holidayFormulaType == "F" ? 
                "F:" + this._holidayFormulaDay.ToString() + "." + this._holidayFormulaMonth.ToString() :
                this._holidayFormulaType + ":" + this._holidayFormulaOffset.ToString() );             
                }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public int HolidayDuration
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return( this._holidayDuration ); }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public DateTime HolidayValidFrom
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return this._holidayValidFrom; }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public DateTime HolidayValidTo
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return( this._holidayValidTo ); }
        }

    }
}
