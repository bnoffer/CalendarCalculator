# CalendarCalculator
C# Class library for calculating the actual amount of working days in a month

This project is derived from:
http://www.mycsharp.de/wbb2/thread.php?postid=3618571#post3618571

## Usage example

**NOTE:** The information below are a quick and dirty translation from the above German forum thread. Feel free to contribute a improved usage guide.

Basis of the construct is a universal holiday matrix off two XML files. The first XML file describes the possible geographical Regions (for example, in our federal states) with the Country and Language as Global Keys / matchcodes. The structure is as follows (excerpt):

```xml
<?xml version="1.0" encoding="utf-8"?>
<States.Config>
  <Country>
    <CountryCode>DE</CountryCode>
    <Language>DE</Language>
    <States>
      <State>
        <StateID>0</StateID>
        <StateName>Catalog</StateName>
      </State>
      <State>
        <StateID>1</StateID>
        <StateName>Deutschland Gesamt</StateName>
      </State>
      <State>
        <StateID>2</StateID>
        <StateName>Bayern</StateName>
      </State>
    </States>
  </Country>
</States.Config>
```

With the country code (here: "DE") and the language code (here: "DE") shall be the scope of a (federal) states list fixed. So, too, within a country different languages and mapped within a country on the basis of language even different forms.

The XML file for the holidays has a similar structure (extract): Country Code and language must match on both Files!

```xml
?xml version="1.0" encoding="utf-8"?>
<Holidays.Config>
  <Country>
    <CountryCode>DE</CountryCode>
    <Language>DE</Language>
    <Catalog>
      <Holiday>
        <HolidayID>10</HolidayID>
        <HolidayName>Neujahr</HolidayName>
        <HolidayFormula>F:1.1</HolidayFormula>
        <HolidayDuration>1</HolidayDuration>
        <HolidayValidFrom>01.01.1900</HolidayValidFrom>
        <HolidayValidTo>31.12.2099</HolidayValidTo>
        <StateIDList>0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17</StateIDList>
      </Holiday>
      <Holiday>
        <HolidayID>20</HolidayID>
        <HolidayName>HeiligeDreiKönige</HolidayName>
        <HolidayFormula>F:6.1</HolidayFormula>
        <HolidayDuration>1</HolidayDuration>
        <HolidayValidFrom>01.01.1900</HolidayValidFrom>
        <HolidayValidTo>31.12.2099</HolidayValidTo>
        <StateIDList>0,2,3,15</StateIDList>
      </Holiday>
     </Catalog>
  </Country>
</Holidays.Config>
```

The StateIDList specifies for which (federal states of the respective holiday is valid. In this case, "0" is the minimum which is 0 for the catalog, i.e., in This includes all holidays. With Duration can be even "Holidays" realize that are more than one day.

With the integrated formula parser can own formulas for Holiday calculation and give to calculate new holidays.

Building a holiday formula is as follows: "Type: modifier"

Examples:
F: 1.1 // Fixed holiday on 1.1 (New Year)
E: 0 // Easter Sunday
E: 1 // Easter Monday
E: -2 // Good Friday
RP: 0 // Bededag
MD: 0 // Mother
AD: 0 // Fourth Advent
AD: -7 // Third Advent

Except for the fixed holiday "F", which with a modifier always day and month Point are given separately, all other types of holiday ( "Floating") an offset which is calculated on the basis of a different holiday can be (usually from Easter Sunday).

Sounds possibly complicated the handling is however quite simple:

```javascript
HolidayMatrix holidayMatrix = new HolidayMatrix("DE", "DE", @"D:\states.config.xml", @"D:\holidays.config.xml");
holidayMatrix.CurrentStateID = 2;  // Bayern als Bundesland setzen
```

And now you can calculate:

```javascript
CalendarCalculator calendar = new CalendarCalculator( holidayMatrix );
```

In addition the following properties are availible:

```javascript
int days = calendar.GetWorkingDays(new DateTime(2010, 3, 31), new DateTime( 2010,5, 3)); 
```


```javascript
HolidayMatrix.Holidays  // Liefert Liste aller Feiertage des gewählten Landes, Untergruppe Sprache und Untergruppe Bundesland/State

CalendarCalculator.IsHoliday( DateTime )
CalendarCalculator.GetHolidayName( DateTime )

CalendarCalculator.SaturdayIsWorkingDay = false;  // Samstag Arbeitstag ja/nein
CalendarCalculator.SundayIsWorkingDay = false;    // Sonntag Arbeitstag ja/nein 
```

You can find addtitional information int he source code.