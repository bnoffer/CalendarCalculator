using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace interweb.CalendarCalculator
{
    /******************************************************************************************************************
     * The main calculation module. A holiday matrix is needed to use this module.
     * 
     * Example:
     * ========
     * Holidaymatrix h = new HolidayMatrix( "DE", DE", @"C:\xml_states.xml", @"C:\xml_holidays.xml" );
     * h.CurrentStateID = 2;  // Important, select the right state, otherwise the catalog => all holidays is valid! 
     * CalendarCalculator c = new CalendarCalculator( h );
     * 
     * // Get all working days between a timframe:
     * int days = c.GetWorkingDays(new DateTime(2009, 12, 6), new DateTime( 2010, 1, 10));
     * 
     * Last update: 15.05.2010
     ******************************************************************************************************************/
    //-----------------------------------------------------------------------------------------------------------------
    public class CalendarCalculator
    //-----------------------------------------------------------------------------------------------------------------
    {
        HolidayMatrix _holidayMatrix;
        bool _saturdayIsWorkingDay;
        bool _sundayIsWorkingDay;

        //-----------------------------------------------------------------------------------------------------------------
        public CalendarCalculator( HolidayMatrix holidayMatrix )
        //-----------------------------------------------------------------------------------------------------------------
        {
            this._holidayMatrix = holidayMatrix;
            this.SaturdayIsWorkingDay = false;
            this.SundayIsWorkingDay = false;
        }

        //------------------------------------------------------------------------------------------------------------------------------------
        public int GetWorkingDays(DateTime beginDate, DateTime endDate)   
        //------------------------------------------------------------------------------------------------------------------------------------
        {
            int daysDiff;                                            // Days gross between begin- and enddate
            int weekDayCount;                                        // Counter for Weekdays (0 = Sunday, 1 = Monday etc., with respect to DayOfWeek Enum)
            int workingDays;                                         // Counter working days
            int correctionIndex;                                     // Index to Weekdayarray for day count correction
            DateTime currentHolidayDate;                             // Date of current holiday
            int currentDuration;                                     // Duration of current holiday
            TimeSpan tsTicks;                                        // Difference begin- and enddate using ticks 
            int[] aWeekdays = { 0, 0, 0, 0, 0, 0, 0 };               // Array with counter for weekdays of the working days (Index 0 = all Sundays, 1 = all Mondays etc.)
            List<DateTime> checkedDates = new List<DateTime>();      // Array with holidays to ensure, that a holiday date gets considered only one time. 
                                                                     // E. g., when 2 holidays have the same date (e. g. fourth advent can be the same day as xmas).
                                                                     // Otherwise the working days would get reduced by 2, what is wrong.

            tsTicks = new TimeSpan(endDate.Ticks - beginDate.Ticks);
            daysDiff = (int)(tsTicks.TotalDays + 1);                 // Days total gross.
            weekDayCount = Math.Min(daysDiff - 1, 6);                // If Timespan less than 1 week

            for (int i = 0; i <= weekDayCount; i++)                  // Move sum of all weekdays (gross) to the weekday array...
            {
                aWeekdays[(int)(beginDate.AddDays(i).DayOfWeek)] = ((daysDiff - i) / 7) + 1;
            }

            // From the subsequent weekday of the end date, there is always on day too much. Correct this...
            correctionIndex = (int)endDate.DayOfWeek < (int)DayOfWeek.Saturday ? (int)endDate.DayOfWeek + 1 : (int)DayOfWeek.Sunday;
            if( aWeekdays[correctionIndex] > 0 )                     // When the timeframe is very short (less 1 week) and the respective
                aWeekdays[correctionIndex]--;                        // weekday array is zero, the correction must not made!
 
            // Loop all years and remove holidays from the weekday array...
            for (int currentYear = beginDate.Year; currentYear <= endDate.Year; currentYear++)
            {
                foreach (Holiday holiday in this._holidayMatrix.Holidays)
                {
                    currentHolidayDate = GetHolidayDate(holiday, currentYear);
                    currentDuration = holiday.HolidayDuration;

                    while (currentDuration > 0)
                    {
                        if (currentHolidayDate >= beginDate && currentHolidayDate <= endDate &&
                            beginDate >= holiday.HolidayValidFrom && endDate <= holiday.HolidayValidTo && !checkedDates.Contains(currentHolidayDate))
                        {
                            checkedDates.Add(currentHolidayDate);
                            aWeekdays[(int)currentHolidayDate.DayOfWeek]--;
                        }

                        currentDuration--;
                        if (currentDuration > 0)
                            currentHolidayDate.AddDays(1);
                    }
                }
            }

            // Counting working days is simple now...
            workingDays = aWeekdays[(int)DayOfWeek.Monday] + aWeekdays[(int)DayOfWeek.Tuesday] + aWeekdays[(int)DayOfWeek.Wednesday] +
                           aWeekdays[(int)DayOfWeek.Thursday] + aWeekdays[(int)DayOfWeek.Friday];

            if (this.SaturdayIsWorkingDay)
            {
                workingDays += aWeekdays[(int)DayOfWeek.Saturday];
            }

            if (this.SundayIsWorkingDay)
            {
                workingDays += aWeekdays[(int)DayOfWeek.Sunday];
            }

            return (workingDays);
        }

        //------------------------------------------------------------------------------------------------------------------------------------
        public bool IsHoliday(DateTime dateCheck)
        //------------------------------------------------------------------------------------------------------------------------------------
        {
            return (CheckIfHoliday(dateCheck) != null);
        }

        //------------------------------------------------------------------------------------------------------------------------------------
        public string GetHolidayName(DateTime dateCheck)
        //------------------------------------------------------------------------------------------------------------------------------------
        {
            Holiday holiday = CheckIfHoliday(dateCheck);

            if (holiday != null)
                return (holiday.HolidayName);
            else
                return ("");
        }

        //------------------------------------------------------------------------------------------------------------------------------------
        private Holiday CheckIfHoliday(DateTime dateCheck)
        //------------------------------------------------------------------------------------------------------------------------------------
        {
            foreach (Holiday holiday in this._holidayMatrix.Holidays)
            {
                if (dateCheck == GetHolidayDate(holiday, dateCheck.Year))
                    return (holiday);
            }
            return (null);
        }

        //------------------------------------------------------------------------------------------------------------------------------------
        public DateTime GetHolidayDate(Holiday holiday, int currentYear)
        //------------------------------------------------------------------------------------------------------------------------------------
        {
            DateTime easterSunday = GetEasterSunday(currentYear);              // Date Eastersunday
            DateTime dateHoliday = new DateTime(0);                            // Date of holiday

            int easterCode = easterSunday.Day - 1 + (easterSunday.Month * 31);
            int adventAndMothersDayCode = (currentYear - 1 + currentYear / 4) % 7;

            switch (holiday.HolidayFormulaType)                                // Calculate holiday based on formula... 
            {
                case "F":
                    dateHoliday = new DateTime(currentYear, holiday.HolidayFormulaMonth, holiday.HolidayFormulaDay);
                    break;

                case "E":
                    dateHoliday = easterSunday.AddDays( holiday.HolidayFormulaOffset );
                    break;

                case "MD":
                    dateHoliday = new DateTime(currentYear, 5, 14 - adventAndMothersDayCode).AddDays(holiday.HolidayFormulaOffset);
                    break;

                case "RP":
                    dateHoliday = new DateTime(currentYear, 11, 16 + (easterCode % 7)).AddDays(holiday.HolidayFormulaOffset);
                    break;

                case "AD":
                    dateHoliday = new DateTime(currentYear, 12, 24 - adventAndMothersDayCode).AddDays(holiday.HolidayFormulaOffset);
                    break;
            
            }
            return (dateHoliday);
        }

        //------------------------------------------------------------------------------------------------------------------------------------
        public DateTime GetEasterSunday(int currentYear)         // Found on Intranet in classic ASP, translated to C#
        //------------------------------------------------------------------------------------------------------------------------------------
        {
            int a, b, c, d, e, m, n, p;
            DateTime easterSunday;

            a = currentYear % 19;
            b = currentYear % 4;
            c = currentYear % 7;
            m = 24;
            n = 5;
            d = (19 * a + m) % 30;
            e = (2 * b + 4 * c + 6 * d + n) % 7;
            p = 22 + d + e;

            if (p > 31)
            {
                if (p == 56 && d == 28 && a > 10)
                {
                    easterSunday = new DateTime(currentYear, 4, 18);
                }
                else
                {
                    if (p == 57)
                    {
                        easterSunday = new DateTime(currentYear, 4, 19);
                    }
                    else
                    {
                        easterSunday = new DateTime(currentYear, 4, p - 31);
                    }
                }
            }
            else
            {
                easterSunday = new DateTime(currentYear, 3, p);
            }

            return (easterSunday);
        }


        //-----------------------------------------------------------------------------------------------------------------
        public bool SaturdayIsWorkingDay
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return this._saturdayIsWorkingDay; }
            set { this._saturdayIsWorkingDay = value; }
        }

        //-----------------------------------------------------------------------------------------------------------------
        public bool SundayIsWorkingDay
        //-----------------------------------------------------------------------------------------------------------------
        {
            get { return this._sundayIsWorkingDay; }
            set { this._sundayIsWorkingDay = value; }
        }
       
    }
}

