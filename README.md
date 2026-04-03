Navrhněte a implementujte aplikaci (C#/WPF) pro zobrazení výškové mapy a její analýzu v zadané kruhové oblasti, které:

1. Z datového souboru načtěte výškovou mapu území. Data jsou v textovém souboru, nejdříve je hlavička v této podobě:
   - ncols počet sloupců matice
   - nrows počet řádků matice
   - xllcorner x-ová souřadnice levého spodního rohu výškové mapy
   - yllcorner y-ová souřadnice levého spodního rohu výškové mapy
   - cellsize – rozteč mezi body výškové mapy
   - Dále pak následují výšky v jednotlivých bodech výškové mapy, jejich počet by měl být ncols x nrows.

2. Tuto výškovou mapu vykreslete v okně (černobíle - nejnižší body budou mít barvu černou, nejvyšší body barvu bílou).

3. Ve statusu (např. v labelu umístěném v dolní části okna aplikace) se budou průběžně zobrazovat souřadnice a výška místa, nad kterým je kurzor myši. Souřadnice můžou být i relativní (např. souřadnice bodu ve výškové mapě) ale lépe by bylo zobrazit souřadnice absolutní - tj. k souřadnici levého spodního rohu mapy přičítat přírůstek a výsledek zobrazit.

4. Aplikace umožní zadat kruhovou oblast v mapě (zobrazením barevně odlišené kružnice). Střed kružnice je zadán prvním kliknutím do mapy (levé tlačítko myši), poloměr druhým, přičemž:
   a. v mapě bude křížkem zobrazen středový bod, zadaný bod na obvodu a kružnice  
   b. bude ve formuláři vedle mapy zobrazena číselná hodnota - střed kružnice a její poloměr

5. Při implementaci použijte MVVM design pattern a pro UI interakci použijte binding properties pomocí ICommand, více např. zde https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm

Volitelné funkce:

- Pokuste se implementovat funkci zoom mapy (kolečko myši) a funkci pan (posun mapy při stisknutém pravém tlačítku myši)

Hodnoceno bude, kolik bodů zadání dokážete splnit a jakým způsobem (algoritmy, stábní kultura kódu). Je kladen důraz na samostatnou práci, z internetu nelze převzít celé kusy kódu nebo části řešení.
