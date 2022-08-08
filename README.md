# Theme park simulator

This project was prepared by 3 of us in a university course. The aim of the project was to solve a complex problem over a whole semester, following an agile methodology. The game was created as a WPF project in C# with MVVM architecture. I was responsible for creating the viewmodel and contributed to the model.

##Short description of the game - in hungarian

A játékos egy jól körülhatárolt területet kap, amelyen építkezhet, és elkészítheti a saját vidámparkját. A parkban elhelyezhet különféle játékokat, vendéglátóhelyeket, illetve építhet utakat is. A park építésére a játékos kap egy kezdőtőkét. Amennyiben a játékos úgy érzi, hogy készen van az építkezéssel, a parkot megnyithatja a nyilvánosság előtt. Ekkor a parkba vendégek érkeznek, akik igénybe vehetik a szolgáltatásokat. A parkba való belépéskor a vendégeknek belépőt kell fizetniük.  Mind a játékok használatáért, mind pedig a szolgáltatásokért fizetendő pénz összegét a játékos határozza meg (tehát bizonyos játékok akár benne lehetnek a belépőjegy árában is).
A játékot valós idejűként kell elkészíteni, de a valósághoz képest természetesen az idő jóval gyorsabban fog telni. A megnyitott vidámpark folyamatosan üzemel, és mindig nappal van.

Játékok
A játékok építése pénzbe és időbe kerül. Játékot csak szabad területre lehet építenünk. A játékok üzemeltetése pénzbe kerül, melyet használatonként vonunk le a játékos pénzéből. Emellett a játékoknak van egy rendszeres fenntartási költsége is, amelyet a használattól függetlenül kell a játékosnak kifizetni. (Például üzemeltető személyzet bére.) A játékoknak van kapacitása, hogy egyszerre hány vendég tudja azokat igénybe venni, illetve egy kör mennyi ideig tart. Azok, akik a játékra nem férnek fel, a játéknál várakoznak. Amennyiben nincs elegendő várakozó, legyen beállítható, hogy a játékok minimum hány százalékos kihasználtságnál indulnak el. Egy játéknak értelemszerűen a következő státuszai lehetnek: üzemel, várakozik, épül.

Vendéglátóhelyek
A vendégek bizonyos időközönként megéheznek vagy megszomjaznak, ezért szükséges, hogy legyenek számukra büfék, édességárusok, jégkásastandok, üdítőárusok stb. A vendéglátóhelyek hasonlóan működnek, mint a játékok: egyszerre adott számú vendéget tudnak kiszolgálni, a kiszolgálásnak van egy adott ideje, a többieknek sorba kell állni. A vendéglátóhelyek üzemeltetési díja is hasonlóan működik, mint a játékoké (rendszeres díj + eladott termékek költsége).

Utak
A parkban utakat kell építenünk. A vendégek csak azokat a helyeket tudják elérni, amelyekhez vezet út a park bejáratától. Az utak építése pénzbe kerül. (Például szegmensenként bizonyos összegbe.

Vendégek
A vendégek csak az utakon tudnak közlekedni és véletlenszerűen választanak célt maguknak. (De nem véletlenszerűen bolyonganak, hanem célirányosan közlekednek.)
A vendégek bizonyos időközönként szeretnének játszani vagy enni, inni, ehhez van egy hangulat és egy jóllakottság értékük. A hangulatuk folyamatosan, lassan csökken, egy játékot használva növekedik. (A játéktól függően eltérő mértékben). Ha az általuk meglátogatott helynél lassan  kerülnek sorra, akkor a hangulatuk gyorsabban kezd csökkeni. Ha a hangulatuk nullára esik, akkor elhagyják a parkot. A jóllakottság értékük szintén folyamatosan, lassan csökken, egy vendéglátóhelyen vásárolva növekedik. (A vendéglátóhelytől függően eltérő mértékben.) Ha a vendégek már nagyon éhesek, szomjasak, akkor az a hangulatukat is elrontja.
A vendégeknél a hajlandóság egy szolgáltatás igénybevételére függ attól, hogy az mennyibe kerül. (A drágább szolgáltatásokat csak a vendégek kisebb hányada fogja igénybe venni.) Ez a hajlandóság már a parkba történő belépésre is vonatkozik (ha van belépődíj). A vendégeknél egy kezdeti (akár eltérő) pénzösszeg áll rendelkezésre. Ha ez elfogy, hazamennek.

Bónusz feladatok

Növények [0.5 pont]
A játékos parkosíthat is: a parkban ültethetünk fákat, bokrokat, illetve füvesíthetünk is a szabad területeken. Ezek pénzbe kerülnek, de javítják a közelben lévő vendégek hangulatát.

Takarítás [1 pont]
A vendégek az étel és ital elfogyasztása után a csomagolóanyagokat hajlamosak eldobni séta közben. Ha találnak a környezetükben szemetest, akkor oda, egyébként az úton. A szemetes vidámpark látványa minden látogató hangulatát rontja. A park útjainak takarítását a takarító személyzet végzi, és amennyiben szemetes útszakaszra érnek, azt feltakarítják. Többet is alkalmazhatunk, akik autonóm módon járják a park útjait, de akár át is helyezhetjük őket a játék futása közben, amennyiben máshol van rájuk szükség.

Karbantartás [1 pont]
A játékok elromolhatnak. A meghibásodott játékokat a karbantartó tudja megjavítani. Több karbantartót is alkalmazhatunk, akik autonóm módon járják a parkot (az utakon), feladat hiányában céltalanul. Amennyiben egy játék elromlik, a legközelebbi karbantartó a játékhoz megy és megjavítja. A javításnak költsége és ideje van. A meghibásodott vagy javítás alatt álló játék nem fogad vendégeket.

Kampány [0.5 pont]
Megadott összegért a játékos reklámkampányt indíthat a közeli településeken. A kampány keretében kuponok kerülnek kiosztásra, amelyek beválthatóak egy ingyenes körre egy játékra vagy a parkba való belépésre. A kampány elindítása után egy ideig több vendég keresi fel a parkot, egy részük pedig rendelkezni fog a kuponnal. A kupon egyszer használatos.

Perzisztencia [0.5 pont]
Egy adott játékállást legyen lehetőség elmenteni, majd később egy kiválasztott játékállást visszatölteni és a játékot folytatni.

Adrenalin [0.5 pont]
A játékoknak van egy előre definiált adrenalin faktora, hogy mennyire ijesztőek. A vendégek is rendelkeznek egy preferált adrenalin értékkel. A vendégek a preferenciájukhoz közeli játékokra nagyobb hajlandósággal ülnek fel (így akár többet is hajlandóak fizetni érte), a preferenciájuktól távoliakra kisebb hajlandósággal (unalmasnak vagy túl ijesztőnek találják).

Áramellátás [0.5 pont]
A játékok árammal működnek, ezért áramellátást kell biztosítani a megfelelő helyeken. Áramot egyrészt a vidámpark bejárata automatikusan generál, másrészt helyezhetők el áramforrások a parkban, tetszőleges helyen, pénzért cserébe. Ezek az áramforrások megadott méretű területet képesek lefedni. Azok a játékok és vendéglátóhelyek, amik nem jutnak áramhoz, nem működnek.

Mosdók [0.5 pont]
A vendégek időnként szükségesnek érzik mosdók használatát. A játékos pénzért, tetszőlegesen helyezhet el mosdókat a területen melyeket úttal kell elérhetővé tenni. A vendégek ezen értéke hasonlóan az éhség/szomjúság értékhez folyamatosan nő és szélsőséges esetben a hangulatot is befolyásolja. Egy vendég minél jobban igényli a mosdó használatát, annál nagyobb valószínűséggel választja azt következő úti céljának. A mosdóknak van üzemeltetési költsége de fizetőssé is tehetők.

Víz / vízi játékok [0.5 pont] -> Sandy háza
A játékos tetszőleges alakú víz alatti üvegbuborékokat alakíthat ki melyre szárazföldi játékok helyezhetők. Hagyományos játékot nem lehet buborékba építeni és buborékos játékot sem lehet vízbe tenni. A buborékhoz speciális, drágább út (airlock) építhető, amelyekkel a vendégek részére megközelíthetővé tehetőek a hagyományos játékok.

Cápa vendég [0.5 pont]
Emberek mellett a vízalatti vidámparkot cápák is látogatják, mint vendégek.
Ha a cápa típusú vendég jóllakottsága nullára csökken sorbaállás közben, akkor van esély arra, hogy az előtte lévő sorban állót megeszi, ezzel enyhítve éhségét.
