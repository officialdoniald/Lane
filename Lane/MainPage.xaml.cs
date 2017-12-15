using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using SQLite.Net;
using System.Collections.ObjectModel;
using SQLite.Net.Platform.WinRT;
using Windows.Storage;
using SQLite.Net.Attributes;
using System.Threading.Tasks;

namespace Lane
{
    public sealed partial class MainPage : Page
    {
        /*Év napjainak besorolása*/
        string[] munkanapok = new string[]
        { "munkanapokon","11:23","11:24","11:25","11:26","11:27","11:30","12:1","12:2","12:3","12:4"
        ,"12:7","12:8","12:9","12:10","12:11","12:12","12:14","12:15","12:16","12:17","12:18"};
        string[] szabadnapok_szombat = new string[]
        { "szabadnapokon","11:28","12:5","12:19","11:21"};
        string[] szombat = new string[]
        { "szombat","11:28","12:5","12:19","11:21"};
        string[] munkaszuneti_vasarnap = new string[]
        { "munkaszüneti napokon","11:22","11:29","12:6","12:13","12:20","12:27","12:24","12:25","12:26"};
        string[] vasarnap = new string[]
        { "vasárnap","11:22","11:29","12:6","12:13","12:20","12:27","12:24","12:25","12:26"};
        string[] oktatasi_szunet = new string[]
        { "oktatási szünet","12:21","12:22","12:23","12:24","12:25","12:26","12:27","12:28","12:29","12:30","12:31"};
        string[] tanszunet = new string[]
        { "tanszünet"};

        string[] milyen_kozlekedesi_eszkozok_vannak = new string[]
        {"1","2","3","3F","4","5","7","8","9","10","19","13","13A","20","20A","21","36","60","60Y","64","70","71","72","73","73Y","74","74A","75","76","77","77A","78","78A","79","79H","7F","84","84A","90","90F","90H","91E","92E","93E","94E","Auchan járat"};
        string[] milyen_megallohelyek_vannak = new string[]
        { "Acél u.","Agyagos u.","Akácfa u.","Alkotmány u.","Alsó kikötő sor","Anna-kút (Tisza Lajos krt.)","Anna-kút","Apály u.","Attila utca","Aranyosi utca","Artézi kút","Auchan Áruház","Árok u.","Back Bernát utca","Bajai út","Bakay Nándor utca","Baktó,Völgyérhát u.","Balajthy utca","Balatoni u.","Barázda u.","Bartók tér","Basahíd utca","Belvárosi temető","Belvárosi temető II.","Berlini krt.","Beszterce utca","Bécsi krt.","Béketelep, MEDIKÉMIA","Béketelepi Ált. Isk.","Bognár u.","Brassói utca","Brüsszeli krt.","Budapesti krt.","Budapesti út","Budapesti út (Dorozsmai út)","Centrum Áruház","Centrum Áruház (Mikszáth u.)","Cincér utca","Cinke u.","Csaba u.","Csaba utca","Csallóközi u.","Csanádi u.","Csanádi utca","Csap u.","Csatorna","Csemegi-tó","Cserepes sor","Cserje sor","Csillag tér","Csillag tér (Budapesti krt.)","Csillag tér (Lugas u.)","Csipkebogyó utca","Csonka u.","Csongrádi sgt.","Damjanich u.","Damjanich utca","Dankó Pista utca","Deák Ferenc Gimnázium","Deli Károly u.","Derkovits fasor","Diadal u.","Diófa utca (Derkovits fasor)","Diófa utca (Szőregi út)","Diófa vendéglő","Diófa Vendéglő","Dobó u.","Dózsa utca","Dráva u.","Dugonics tér","Dugonics tér (Dáni utca)","Dugonics tér (Tisza Lajos krt.)","Duna u.","Első Szegedi Ipari Park","Erdélyi tér","Erdő u.","Erdő utca","Erdőtarcsa u.","Etelka sor","Etelka sor (Felső Tisza-part)","Európa liget","Fecske u.","Fehérparti lejáró","Fehértói halgazdaság","Fonógyári út","Fonógyári úti Iparváros","Földhivatal","Földvári u.","Fő fasor","Füvészkert","Galamb utca","Gábor Áron u.","Gál utca","Gárdonyi Géza u.","Gát utca","Gém u.","Gém utca","Glattfelder Gyula tér","Gólya u.","Gőzmalom u.","Gumigyár","Gumigyár, buszforduló","Gyálarét","Gyár u. (Magyar u.)","Gyár u. (Szerb u.)","Gyümölcs u.","Hajnal utca (ideiglenes)","Hajnal utca(ideiglenes)","Hajós u.","Hargitai u.","Hatházak","Háló utca","Hétvezér u.","Hétvezér utca","Holt-Tisza","Honfoglalás u.","Honvéd tér","Honvéd tér (Tisza Lajos krt.)","Huszár u. (Okmányiroda)","Huszka Jenő utca","II. Kórház","Ikarusz köz","Ipoly sor","Iskola u.","Jerney János Ált. Isk.","József Attila sgt. (Budapesti krt.)","József Attila sgt.(Budapesti krt.)","József Attila sgt. (Retek utca)","Ifjúsági Ház","Kalász u.","Kamaratöltés","Kamarási u.","Katalin utca","Kavics u.","Kálvária sgt.","Kálvária sgt. (Vásárhelyi Pál u.)","Kálvária tér","Kálvária tér II.","Kecskés","Kecskés telep, Bódi Vera u.","Kecskés telep, Gera Sándor u.","Kenyérgyári út","Kereskedelmi rakt.","Kereskedő köz (EXPO)","Kiskertek","Kiskundorozsma, ABC","Kiskundorozsma, Czékus u.","Kiskundorozsma, Fürdő","Kiskundorozsma, vá.bej.","Kiskundorozsma, Vásártér","Kisstadion","Kisteleki u.","Kísérleti gazdaság","Klára utca","Klinikák","Kokárda u.","Kollégiumi út","Kolozsvári tér","Koppány köz","Kossuth Lajos Ált. Isk.","Koszorú u.","Körtöltés utca","Közép fasor","Közép fasor (Bérkert u.)","Lengyel u.","Lidicei tér","Londoni krt.","Londoni krt. (Bakay Nándor u.)","Londoni krt. (Kálvária sgt.)","Lugas u.","Lugas utca","Makkosház","Makkosházi krt.","Makkosházi krt. (Csongrádi sgt.)","Makkosházi krt. (Rókusi krt.)","Malom (Dorozsmai út)","Malom (Jerney u.)","Marostői utca","Mars tér (Attila utca)","Mars tér (aut. áll.)","Mars tér (Aut. áll.)","Mars tér (Mikszáth u.)","Mars tér (Szt. Rókus tér)","Mars tér (üzletsor)","Mars tér(üzletsor)","Moszkvai krt.","Móravárosi Bevásárlóközpont","Mura utca","Nagyszombati u. (páratlan oldal)","Nagyszombati u. (páros oldal)","Napfény köz","Napfényfürdő","Napos út (Dorozsmai út)","Negyvennyolcas u.","Nemes takács u. (ideiglenes)","Nyári tanya","Nyilassy utca","Nyilassy u.","Ortutay utca","Öthalmi Diáklakások","Petőfi Sándor sgt.","Petőfitelep, Fő tér","Pinty u.","Pulz utca","Pipiske utca","Radnóti u.","Rákóczi u. (Vám tér)","Remény utca (ideiglenes)","Rendező tér","Rengey u.","Repülőtér","Repülőtér bej.út","Retek utca","Rév u.","Rókusi II. sz. Ált. Isk.","Rókusi víztorony","Római krt.(Szilléri sgt.)","Római krt. (Szilléri sgt.)","Rózsa u.","Rózsa utca","Rózsa u. (Csongrádi sgt.)","Rózsatő u. (Magyar u.)","Rózsatő u. (Szerb u.)","Sajka utca","Sándor utca","Sárkány u.","Sportcsarnok (Székely sor)","Sportcsarnok (Temesvári krt.)","Somogyi utca","Subasa","Szabadkai út","Szabadsajtó u.","Szabadság tér","Szalámigyár","Szamos u.","Szatymazi utca","Szeged(Rókus), vá.bej.út","Szeged, Szélső sor","Szeged Plaza","Szegedi Ipari Logisztikai Központ","Személy pu.","Szeged pu.","Szent Ferenc u.","Szent János tér","Szent György tér","Szent-Györgyi Albert u.","Szentmihály","Szentmihály, malom","Széchenyi István u.","Széchenyi tér","Széchenyi tér (Kelemen u.)","Széksósi út","Szél u.","Szivárvány kitérő","Szöri u.","Szövetség u.","Szőreg, ABC","Szőreg, malom","Szőregi Szabadidőpark","SZTK Rendelő","Szúnyog u.","Tabán u. (Felső Tisza-part)","Tabán utca (Felső Tisza-part)","Tanács u.","Tarján","Tarján, víztorony","Tarján, Víztorony tér","Tassi ház","Tavasz utca","Tápé, Ált. Isk.","Tápé, Csatár u.","Temesvári krt. (Népkert sor)","Thököly u.","Tisza Volán Zrt.","Torontál tér (P+R)","Töltés u.","Traktor u.","Tücsök utca","Tündér utca","Tűzoltó utca","Új-Petőfitelep","Újszeged, Gabonakutató","Újszeged, Gyermekkorház","Újszeged, víztorony","Vadaspark","Vadkerti tér","Vadkerti tér, buszforduló","Vaspálya u.","Városi Stadion","Vásárhelyi Pál út","Vásárhelyi Pál utca","Vásárhelyi Pál utca (Kossuth L. sgt.)","Vásárhelyi Pál u. (Bakay Nándor u.)","Vásárhelyi Pál u. (Kossuth L. sgt.)","Vásárhelyi Pál u. (Pulz u.)","Verbéna u.","Veresács utca","Vértó","Vértói út","Vitéz utca","Zágráb u.","Zápor út","Zsámbokréti sor"};

        /*1. Deklaráljuk az egyes járatok megállóhelyeit.*/
        /*Foggalmam sincs hogy ezeknél most hogyan: 75,64*/

        /*Buszok*/

        /*Auchan*/
        string[] auchan_jarat = new string[]
        { "Auchan járat;0,2,4,6,7,8,9,11,12,14,15,16,18,19,21,22,23,24,25,26,28,30,31,32,34,35,36,39,40","Auchan áruház","Budapesti út (Dorozsmai út)","Szeged(Rókus), vá. bej út.","Kisteleki utca","Rókusi víztorony","Rókus II. sz. Ált. Isk.","Vértó","Makkosházi krt.","Agyagos utca","József Attila sgt. (Budapesti krt.)","Tarján, víztorony tér","Csillag tér (Budapesti krt.)","Szamos utca","József Attila sgt. (Retek utca)","Gál utca","Római krt. (Szilléri sgt.)","Fecske utca","Csillag tér (Budapesti krt.)","Tarján, víztorony tér","József Attila sgt. (Budapesti krt.)","Agyagos utca","Makkosházi krt. (Rókusi krt.)","Vértó","Rókus II. sz. Ált. Isk.","Rókusi víztorony","Kisteleki utca","Szeged(Rókus), vá. bej út.","Budapesti út (Dorozsmai út)","Auchan áruház"};

        /*7F*/
        string[] het_f_marstertol = new string[]
        {"7F;0,1,2,5,7,9,10,12,13,15,16,17,18,19,20,21,22","Mars tér (üzletsor)","Tavasz utca","Damjanich utca", "Szeged (Rókus), vá.bej.út","Fonógyári krt.","Budapesti út","Kollégiumi út","Kiskundorozsma, vá.bej.","Tassi ház","Csatorna","Malom (Dorozsmai út)","Kiskundorozsma, ABC","Brassói utca","Széksósi út","Csipkebogyó utca","Subasa","Kiskundorozsma, Fürdő"};
        string[] het_f_kiskundorozsma_furdotol = new string[]
        {"7F;0,1,2,3,4,5,6,7,9,10,12,14,16,17,18,19,20,22","Kiskundorozsma, Fürdő","Subasa","Csipkebogyó utca","Széksósi út","Brassói utca","Kiskundorozsma, ABC","Malom (Dorozsmai út)","Csatorna","Tassi ház","Kiskundorozsma, vá.bej.","Kollégiumi út","Budapesti út","Fonógyári út.","Szeged(Rókus), vá.bej.út","Vásárhelyi Pál utca (Kossuth L. sgt.)","Damjanich utca","Tavasz utca","Mars tér (üzletsor)"};

        /*13*/
        string[] tizenharom_viztoronytol = new string[]
        {"13;0,1,3,5,7,8,10,12,13,14,15,16,17,18,19,20,21,22,23,24","Tarján, Víztorony tér","József Attila sgt. (Budapesti krt.)","Deák Ferenc Gimnázium","Retek utca","Sándor utca","Berlini	krt.","Hétvezér u.","Mars tér (aut.	áll.)","Londoni krt. (Bakay Nándor u.)","Huszár u. (Okmányiroda)","Mura utca" ,"Tisza Volán Zrt." ,"Kenyérgyári út" ,"II. Kórház" ,"Kálvária tér" ,"Remény utca (ideiglenes)" ,"Hajnal utca (ideiglenes)","Szél u." ,"Cserepes sor" ,"Móravárosi Bevásárlóközpont"};
        string[] tizenharom_napfenyparktol = new string[]
        {"13;0,1,2,3,5,7,8,9,10,11,12,13,14,15,17,18,20,21,22,24,25","Móravárosi Bevásárlóközpont","Cserepes sor","Gólya u.","Kolozsvári tér","Nemes takács u. (ideiglenes)","Kálvária tér II.","Kórház","Kálvária sgt. (Vásárhelyi Pál u.)","Tisza Volán Zrt.","Vásárhelyi Pál u. (Bakay Nándor u.)","Mura utca","Huszár	u. (Okmányiroda)","Londoni krt. (Bakay Nándor u.)","Mars tér (aut. áll.)","Hétvezér u.","Berlini krt.","Dankó Pista utca","Retek utca","Deák Ferenc Gimnázium","József Attila sgt. (Budapesti krt.)","Tarján, Víztorony tér" };

        /*13A*/
        string[] tizenharom_a_marster_bevnyitva = new string[]
        {"13A;0,1,2,3,4,5,6,7,8,9,10,11,12,13","Mars tér(üzletsor)","Mars tér(aut.áll.)","Londoni krt. (Bakay Nándor u.)","Huszár u. (Okmányiroda)","Mura utca","Tisza Volán Zrt.","Kenyérgyári út","II.Kórház","Kálvária tér","Remény utca(ideiglenes)","Hajnal utca(ideiglenes)","Szél u.","Cserepes sor","Móravárosi Bevásárlóközpont"};
        string[] tizenharom_a_marster_bevzarva = new string[]
        {"13A;0,1,2,3,4,5,6,7,8,9,10,11,12,13","Mars tér(üzletsor)","Mars tér(aut.áll.)","Londoni krt. (Bakay Nándor u.)","Huszár u. (Okmányiroda)","Mura utca","Tisza Volán Zrt.","Kenyérgyári út","II.Kórház","Kálvária tér","Remény utca(ideiglenes)","Hajnal utca(ideiglenes)","Szél u.","Cserepes sor","Gólya u."};
        string[] tizenharom_a_bevtol_bevnyitva = new string[]
        {"13A;0,1,2,3,5,7,8,9,10,11,12,13,14,15,16","Móravárosi Bevásárlóközpont","Cserepes sor","Gólya u.","Kolozsvári tér","Nemes takács u. (ideiglenes)","Kálvária tér","II. Kórház","Kálvária sgt. (Vásárhelyi Pál u.)","Tisza Volán Zrt.","Vásárhelyi Pál u. (Bakay Nándor u.)","Mura utca","Huszár u. (Okmányiroda)","Londoni krt. (Bakay Nándor u.)","Mars tér (Attila utca)","Mars tér (üzletsor)"};
        string[] tizenharom_a_bevtol_bevzarva = new string[]
        {"13A;0,1,3,5,6,7,8,9,10,11,12,13,14","Gólya u.","Kolozsvári tér","Nemes takács u. (ideiglenes)","Kálvária tér","II. Kórház","Kálvária sgt. (Vásárhelyi Pál u.)","Tisza Volán Zrt.","Vásárhelyi Pál u. (Bakay Nándor u.)","Mura utca","Huszár u. (Okmányiroda)","Londoni krt. (Bakay Nándor u.)","Mars tér (Attila utca)","Mars tér (üzletsor)" };

        /*20*/
        string[] huszas_petofitol = new string[]
        {"20;0,1,3,5,6,8,9,10,12,14,16,17,19,20,22,24,25,26,27,28","Petőfitelep, Fő tér","Gábor Áron u.","Csillag tér (Lugas u.)","Szamos u.","József Attila sgt. (Retek utca)","Lengyel u.","Glattfelder Gyula tér","Anna-kút (Tisza Lajos krt.)","Centrum Áruház","Dugonics tér (Tisza Lajos krt.)","Honvéd tér (Tisza Lajos krt.)","SZTK Rendelő","Bécsi krt.","Szent Ferenc u.","Személy pu.","Szabadsajtó u.","Csonka u.","Szabadság tér","Vadkerti tér","Vadkerti tér, buszforduló" };
        string[] huszas_vadkertitol = new string[]
        { "20;0,1,3,4,5,6,8,9,11,12,14,15,17,19,21,23,24,25,27,28","Vadkerti tér, buszforduló","Vadkerti tér","Szabadság tér","Csonka u.","Szabadsajtó u.","Személy pu.","Szent Ferenc u.","Bécsi krt.","SZTK Rendelő","Dugonics tér","Dugonics tér (Tisza Lajos krt.)","Centrum Áruház","Anna-kút (Tisza Lajos krt.)","Glattfelder Gyula tér","Dankó Pista utca","József Attila sgt. (Retek utca)","Szamos u.","Csillag tér (Lugas u.)","Gábor Áron u.","Petőfitelep, Fő tér"};

        /*20A*/
        string[] husz_a_petofitol = new string[]
        {"20A;0,1,3,5,6,8,9,10,12,14,15", "Petőfitelep, Fő tér","Gábor Áron u.","Csillag tér (Lugas u.)","Szamos u.","József Attila  sgt. (Retek  utca)","Lengyel u.","Glattfelder Gyula tér","Anna-kút (Tisza Lajos krt.)","Centrum Áruház","Dugonics tér (Tisza Lajos krt.)","Honvéd tér"};
        string[] husz_a_honvedtol = new string[]
        {"20A;0,1,3,4,6,7,9,11,12,13,15,16","Honvéd tér","Dugonics tér","Dugonics tér (Tisza Lajos krt.)","Centrum Áruház","Anna-kút (Tisza Lajos krt.)","Glattfelder Gyula tér","Dankó Pista utca","József Attila sgt. (Retek utca)","Szamos u.","Csillag tér (Lugas u.)","Gábor Áron u.","Petőfitelep, Fő tér"};

        /*21*/
        string[] huszonegy_petofitol = new string[]
        {"21;0,1,2,3,4,5,6,8,9,11,13,14,18,20,21,22,23,24", "Új-Petőfitelep","Balatoni u.","Acél u.","Kalász u.","Lidicei tér","Csap u.","Csillag tér (Lugas u.)","Szamos u.","József Attila sgt. (Retek utca)","Rózsa u. (Csongrádi sgt.)","Berlini krt.","Hétvezér u.","Mars tér (aut. áll.)","Kálvária sgt.","Petőfi Sándor sgt.","Bécsi krt.","Szent Ferenc u.","Személy pu."};
        string[] huszonegy_palyaudvartol = new string[]
        { "21;0,1,2,3,5,8,10,12,13,14,15,17,19,20,21,22,23,24","Személy pu.","Szent Ferenc u.","Bécsi krt.","Petőfi Sándor sgt.","Kálvária sgt.","Mars tér (aut. áll.)","Hétvezér u.","Gém u.","Rózsa u.","József Attila sgt. (Retek utca)","Szamos u.","Csillag tér (Lugas u.)","Gábor Áron u.","Lidicei tér","Kalász u.","Acél u.","Balatoni u.","Új-Petőfitelep"};

        /*36*/
        string[] harminchat_honvedtol = new string[]
        { "36;0,1,2,3,4,5,7,8,9,10,11,13,14,15,16,17,19,20,21,22,23,24,25,26,27,28,29","Honvéd tér","Dugonics tér","Dugonics tér (Dáni utca)","Londoni krt. (Kálvária sgt.)","Kálvária tér","II. Kórház","Vadaspark","Bajai út","Belvárosi temető","Ikarusz köz","Fonógyári úti Iparváros","Budapesti út","Kollégiumi út","Kiskundorozsma, vá.bej.","Tassi ház","Csatorna","Malom (Jerney u.)","Jerney János Ált. Isk.","Huszka Jenő utca","Széchenyi István u.","Basahíd utca","Szent János tér","Balajthy utca","Negyvennyolcas u.","Kiskundorozsma, Vásártér","Erdőtarcsa u.","Kiskundorozsma, Czékus u."};
        string[] harminchat_kkdorozsmatol = new string[]
        {"36;0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,17,18,19,20,21,23,24,25,26,27,28", "Kiskundorozsma, Czékus u.","Erdőtarcsa u.","Kiskundorozsma, Vásártér","Negyvennyolcas u.","Balajthy utca","Szent János tér","Basahíd utca","Széchenyi István u.","Huszka Jenő utca","Jerney János Ált. Isk.","Malom (Dorozsmai út)","Csatorna","Tassi ház","Kiskundorozsma, vá.bej.","Kollégiumi út","Budapesti út","Fonógyári úti Iparváros","Ikarusz köz","Belvárosi temető","Bajai út","Vadaspark","II. Kórház","Kálvária tér","Földhivatal","Londoni krt. (Kálvária sgt.)","Dugonics tér (Dáni utca)","Honvéd tér" };

        /*77*/
        string[] hetvenhet_baktotol = new string[]
        { "77;0,1,2,3,4,5,7,8,9,11,12,14,15,16,19,21,23,24,25,26","Baktó,Völgyérhát u.","Bognár u.","Kokárda u.","Gyümölcs u.","Diadal u.","Szeged, Szélső sor","József Attila sgt. (Budapesti krt.)","Tarján, víztorony","Csillag tér (Budapesti krt.)","Fecske u.","Római krt. (Szilléri sgt.)","Sándor utca","Berlini krt.","Hétvezér u.","Mars tér (aut. áll.)","Kálvária sgt.","Petőfi Sándor sgt.","Bécsi krt.","Szent Ferenc u.","Személy pu."};
        string[] hetvenhet_szemelyitol = new string[]
        { "77;0,1,2,3,5,8,10,12,13,14,15,17,19,20,21,22,23,24,25,26","Személy pu.","Szent Ferenc u.","Bécsi krt.","Petőfi Sándor sgt.","Kálvária sgt.","Mars tér (aut. áll.)","Hétvezér u.","Berlini krt.","Gál utca","Római  krt. (Szilléri sgt.)","Fecske u.","Csillag tér (Budapesti krt.)","Tarján, víztorony","Budapesti krt.","Szeged, Szélső sor","Alkotmány u.","Gyümölcs u.","Kokárda u.","Bognár u.","Baktó,Völgyérhát u."};
        string[] hetvenhet_tarjaniviztoronytol = new string[]
        {"77;0,1,3,4,6,7,8,11,13,15,16,17,18", "Tarján, víztorony","Csillag tér (Budapesti krt.)","Fecske u.","Római krt. (Szilléri sgt.)","Sándor utca","Berlini krt.","Hétvezér u.","Mars tér (aut. áll.)","Kálvária sgt.","Petőfi Sándor sgt.","Bécsi krt.","Szent Ferenc u.","Személy pu."};

        /*79*/
        string[] hetvenkilenc_auchan_nyitva_marstertol = new string[]
        {"79;0,1,2,5,7,9,11,12,13,14,16,17", "Mars tér (üzletsor)","Tavasz utca","Damjanich u.","Szeged(Rókus), vá.bej.út","Fonógyári út","Budapesti út (Dorozsmai út)","Auchan Áruház","Zápor út","Öthalmi Diáklakások","Gumigyár","Back Bernát utca","Szegedi Ipari Logisztikai Központ"};
        string[] hetvenkilenc_auchan_zarva_marstertol = new string[]
        { "79;0,1,2,5,7,9,10,11,12,14,15","Mars tér (üzletsor)","Tavasz utca","Damjanich u.","Szeged(Rókus), vá.bej.út","Fonógyári út","Budapesti út (Dorozsmai út)","Zápor út","Öthalmi Diáklakások","Gumigyár, buszforduló","Back Bernát utca","Szegedi Ipari Logisztikai Központ"};
        string[] hetvenkilenc_auchan_nyitva_logisztikatol = new string[]
        { "79;0,1,3,4,7,9,10,12,13,14,15,16,18","Szegedi Ipari Logisztikai Központ","Back Bernát utca","Gumigyár","Öthalmi Diáklakások","Auchan Áruház","Zápor út","Budapesti út (Dorozsmai út)","Fonógyári út","Szeged(Rókus), vá.bej.út","Vásárhelyi Pál u. (Kossuth L. sgt.)","Damjanich u.","Tavasz utca","Mars tér (üzletsor)"};
        string[] hetvenkilenc_auchan_zarva_logisztikatol = new string[]
        { "79;0,1,3,4,5,7,9,10,11,12,13,15","Szegedi Ipari Logisztikai Központ","Back Bernát utca","Gumigyár","Öthalmi Diáklakások","Zápor út","Budapesti út (Dorozsmai út)","Fonógyári út","Szeged(Rókus), vá.bej.út","Vásárhelyi Pál u. (Kossuth L. sgt.)","Damjanich u.","Tavasz utca","Mars tér (üzletsor)"};

        /*60*/
        string[] hatvan_marstertol = new string[]
        { "60;0,1,2,4,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23","Mars tér (Szt. Rókus tér)","Mars tér (aut. áll.)","Bartók tér","Széchenyi tér (Kelemen u.)","Torontál tér (P+R)","Sportcsarnok (Székely sor)","Kisstadion","Töltés u.","Diófa utca (Derkovits fasor)","Derkovits fasor","Traktor u.","Kamaratöltés","Kavics u.","Napfény köz","Barázda u.","Szőreg, ABC","Kossuth Lajos Ált. Isk.","Szőregi Szabadidőpark","Rózsatő u. (Szerb u.)","Gyár u. (Szerb u.)","Szőreg, malom"};
        string[] hatvan_szoregrol = new string[]
        { "60;0,1,2,3,4,5,6,7,8,9,10,11,12,13,15,17,20,21,23","Szőreg, malom","Gyár u. (Szerb u.)","Rózsatő u. (Szerb u.)","Szőregi Szabadidőpark","Kossuth Lajos Ált. Isk.","Szőreg, ABC","Barázda u.","Napfény köz","Kavics u.","Kamaratöltés","Traktor u.","Derkovits fasor","Diófa utca (Derkovits fasor)","Töltés u.","Sportcsarnok (Székely sor)","Torontál tér (P+R)","Széchenyi tér (Kelemen u.)","Centrum Áruház (Mikszáth u.)","Mars tér (Szt. Rókus tér)"};

        /*60Y*/
        string[] hatvan_y_marstertol = new string[]
        { "60Y;0,1,2,4,7,8,10,12,14,15,16,17,18,19,20,21,22,23,24","Mars tér (Szt. Rókus tér)","Mars tér (aut. áll.)","Bartók tér","Széchenyi tér (Kelemen u.)","Torontál tér (P+R)","Sportcsarnok (Székely sor)","Diófa utca (Szőregi út)","Aranyosi utca","Kamaratöltés","Kavics u.","Napfény köz","Barázda u.","Szőreg, ABC","Iskola u.","Vaspálya u.","Rózsatő u. (Magyar u.)","Gyár u. (Magyar u.)","Gőzmalom u.","Szőreg, malom"};
        string[] hatvan_y_szoregtol = new string[]
        {"60Y;0,1,2,3,4,5,7,8,9,10,11,13,14,15,15,20,22,24", "Szőreg, malom","Gőzmalom u.","Gyár u. (Magyar u.)","Rózsatő u. (Magyar u.)","Vaspálya u.","Iskola u.","Szőreg, ABC","Barázda u.","Napfény köz","Kavics u.","Kamaratöltés","Aranyosi utca","Diófa utca (Szőregi út)","Sportcsarnok (Székely sor)","Torontál tér (P+R)","Széchenyi tér (Kelemen u.)","Centrum Áruház (Mikszáth u.)","Mars tér (Szt. Rókus tér)"};

        /*70*/
        string[] hetven_marstertol = new string[]
        { "70;0,1,2,4,7,8,10,11,12,13","Mars tér (Szt. Rókus tér)","Mars tér (aut. áll.)","Bartók tér","Széchenyi tér (Kelemen u.)","Torontál tér (P+R)","Újszeged, Gabonakutató","Alsó kikötő sor","Hatházak","Akácfa u.","Füvészkert"};
        string[] hetven_fuveszkert = new string[]
        { "70;0,1,2,3,4,6,8,9,11","Füvészkert","Akácfa u.","Hatházak","Alsó kikötő sor","Újszeged, Gabonakutató","Torontál tér (P+R)","Széchenyi tér (Kelemen u.)","Centrum Áruház (Mikszáth u.)","Mars tér (Szt. Rókus tér)"};

        /*71*/
        string[] hetvenegy_marstertol = new string[]
        { "71;0,2,4,7,9,10,11,13,14,15,16,17,18,19,20","Mars tér (Mikszáth u.)","Bartók tér","Széchenyi tér (Kelemen u.)","Napfényfürdő","Temesvári krt. (Népkert sor)","Közép fasor (Bérkert u.)","Marostői utca","Szöri u.","Szövetség u.","Tanács u.","Thököly u.","Cinke u.","Pipiske utca","Klára utca","Katalin utca"};
        string[] hetvenegy_katalinutcatol = new string[]
        { "71;0,1,2,3,4,5,6,7,8,9,11,13,16,18,19","Katalin utca","Klára utca","Pipiske utca","Cinke u.","Thököly u.","Tanács u.","Szövetség u.","Szöri u.","Marostői utca","Közép fasor (Bérkert u.)","Temesvári krt. (Népkert sor)","Napfényfürdő","Széchenyi tér (Kelemen u.)","Centrum Áruház (Mikszáth u.)","Mars tér (Mikszáth u.)"};

        /*72*/
        string[] hetvenketto_marstertol = new string[]
        { "72;0,2,4,7,9,11,12,13,14,15,16,17,18,19","Mars tér (Mikszáth u.)","Bartók tér","Széchenyi tér (Kelemen u.)","Torontál tér (P+R)","Sportcsarnok (Székely sor)","Közép fasor","Újszeged, víztorony","Radnóti u.","Thököly u.","Cinke u.","Pipiske utca","Hargitai u.","Pinty u.","Erdélyi tér"};
        string[] hetvenketto_erdelyitertol = new string[]
        { "72;0,1,2,3,4,5,7,8,9,11,12,15,17,18","Erdélyi tér","Pinty u.","Hargitai u.","Pipiske utca","Cinke u.","Radnóti u.","Újszeged, víztorony","Közép fasor","Fő fasor","Sportcsarnok (Székely sor)","Torontál tér (P+R)","Széchenyi tér (Kelemen u.)","Centrum Áruház (Mikszáth u.)","Mars tér (Mikszáth u.)"};

        /*73*/
        string[] hetvenharom_marstertol = new string[]
        { "73;0,2,3,4,5,6,7,8,9,10,12,13,15,16,17","Mars tér (üzletsor)","Hétvezér u.","Berlini krt.","Gál utca","Római krt. (Szilléri sgt.)","Hajós u.","Tabán u. (Felső Tisza-part)","Etelka sor (Felső Tisza-part)","Városi Stadion","Duna u.","Beszterce utca","Árok u.","Rév u.","Honfoglalás u.","Tápé, Csatár u."};
        string[] hetvenharom_taperol = new string[]
        { "73;0,1,2,4,5,7,8,9,10,11,12,13,14,15,17","Tápé, Csatár u.","Honfoglalás u.","Rév u.","Árok u.","Beszterce utca","Duna u.","Városi Stadion","Etelka sor (Felső Tisza-part)","Tabán u. (Felső Tisza-part)","Hajós u.","Római krt. (Szilléri sgt.)","Sándor utca","Berlini krt.","Hétvezér u.","Mars tér (üzletsor)"};

        /*73Y*/
        string[] hetvenharom_y_marstertol = new string[]
        {"73Y;0,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18", "Mars tér (üzletsor)","Hétvezér u.","Berlini krt.","Gál utca","Római krt. (Szilléri sgt.)","Hajós u.","Tabán u. (Felső Tisza-part)","Etelka sor (Felső Tisza-part)","Városi Stadion","Duna u.","Petőfitelep, Fő tér","Zágráb u.","Dráva u.","Tápé, Ált. Isk.","Nyilassy u.","Rév u.","Honfoglalás u.","Tápé, Csatár u."};
        string[] hetvenharom_y_taperol = new string[]
        { "73Y;0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,18","Tápé, Csatár u.","Honfoglalás u.","Rév u.","Nyilassy u.","Táoé, Ált. Isk.","Dráva u.","Zágráb u.","Petőfitelep, Fő tér","Duna u.","Városi Stadion","Etelka sor (Felső Tisza-part)","Tabán u. (Felső Tisza-part)","Hajós u.","Római krt. (Szilléri sgt.)","Sándor utca","Berlini krt.","Hétvezér u.","Mars tér (üzletsor)"};

        /*74*/
        string[] hetvennegy_marstertol = new string[]
        { "74;0,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21","Mars tér (Mikszáth u.)","Bartók tér","Dugonics tér (Tisza Lajos krt.)","Honvéd tér (Tisza Lajos krt.)","SZTK Rendelő","Bécsi krt.","Szent Ferenc u.","Dobó u.","Sárkány u.","Vadkerti tér","Kamarási u.","Rendező tér","Holt-Tisza","Cincér utca","Szúnyog u.","Kiskertek","Tücsök utca","Apály u.","Deli Károly u.","Koszorú u.","Gyálarét"};
        string[] hetvennegy_gyalaret = new string[]
        { "74;0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,20,21","Gyálarét","Koszorú u.","Deli Károly u.","Apály u.","Tücsök utca","Kiskertek","Szúnyog u.","Cincér utca","Holt-Tisza","Rendező tér","Kamarási u.","Vadkerti tér","Sárkány u.","Dobó u.","Szent Ferenc u.","Bécsi krt.","SZTK Rendelő","Dugonics tér","Dugonics tér (Tisza Lajos krt.)","Centrum Áruház (Mikszáth u.)","Mars tér (Mikszáth u.)"};

        /*76*/
        string[] hetvenhat_marstertol = new string[]
        { "76;0,1,2,3,4,5,6,8,9,11,13,14,15,16,18,19","Mars tér (Mikszáth u.)","Bartók tér","Dugonics tér (Tisza Lajos krt.)","Moszkvai krt.","Rákóczi u.(Vám tér)","Szabadkai út","Szalámigyár","Kecskés telep, Gera Sándor u.","Kecskés telep, Bódi Vera u.","Fehérparti lejáró","Verbéna u.","Artézi kút","Gárdonyi Géza u.","Szentmihály, malom","Koppány köz","Szentmihály"};
        string[] hetvenhat_szentmihaly = new string[]
        { "76;0,2,4,6,7,8,9,10,11,13,14,15,16,17,19,20","Szentmihály","Koppány köz","Szentmihály, malom","Gárdonyi Géza u.","Artézi kút","Verbéna u.","Fehérparti lejáró","Kecskés telep, Bódi Vera u.","Kecskés telep, Gera Sándor u.","Szalámigyár","Rákóczi u.(Vám tér)","Moszkvai krt.","Földvári u.","Dugonics tér (Tisza Lajos krt.)","Centrum Áruház (Mikszáth u.)","Mars tér (Mikszáth u.)" };

        /*77A*/
        string[] hetvenhet_a_baktotol = new string[]
        { "77A;0,1,2,3,4,5,7,8,9,11,12,14,15,16,19","Baktó,Völgyérhát u.","Bognár u.","Kokárda u.","Gyümölcs u.","Diadal u.","Szeged, Szélső sor","József Attila sgt.(Budapesti krt.)","Tarján, víztorony","Csillag tér (Budapesti krt.)","Fecske u.","Római krt.(Szillérisgt.)","Sándor utca","Berlini krt.","Hétvezér u.","Mars tér (aut. áll.)"};
        string[] hetvenhet_a_marstertol = new string[]
        { "77A;0,2,4,5,6,7,9,11,12,13,14,15,16,17,18","Mars tér (aut. áll.)","Hétvezér u.","Berlini krt.","Gál utca","Római krt.(Szilléri sgt.)","Fecske u.","Csillag tér (Budapesti krt.)","Tarján, víztorony","József Attila sgt.(Budapesti krt.)","Szeged, Szélső sor","Alkotmány u.","Gyümölcs u.","Kokárda u.","Bognár u.","Baktó,Völgyérhát u."};

        /*78*/
        string[] hetvennyolc_marstertol = new string[]
        { "78;0,1,2,4,6,7,8,9,10,11,12,13,16,18,19,20,21,22,24,26","Mars tér (üzletsor)","Tavasz utca","Damjanich u.","Szeged(Rókus), vá.bej.út","Napos út (Dorozsmai út)","Cserje sor","Béketelepi Ált. Isk.","Nagyszombati u. (páratlan oldal)","Rengey u.","Zsámbokréti sor","Béketelep, MEDIKÉMIA","Vértói út","Vértó","Rókusi II. sz. Ált. Isk.","Rókusi víztorony","Kisteleki u.","Vásárhelyi Pál u. (Kossuth L. sgt.)","Damjanich u.","Tavasz utca","Mars tér (üzletsor)"};

        /*78A*/
        string[] hetvennyolc_a_marstertol = new string[]
        { "78A;0,1,2,5,6,7,9,11,13,14,15,16,17,18,19,21,22,23,24,26","Mars tér (üzletsor)","Tavasz utca","Damjanich u.","Kisteleki u.","Rókusi víztorony","Rókusi II. sz. Ált. Isk.","Vértói út","Béketelep, MEDIKÉMIA","Csallóközi u.","Rengey u.","Nagyszombati u. (páros oldal)","Béketelepi Ált. Isk.","Cserje sor","Első Szegedi Ipari Park","Tűzoltó utca","Szeged(Rókus), vá.bej.út","Vásárhelyi Pál u. (Kossuth L. sgt.)","Damjanich u.","Tavasz utca","Mars tér (üzletsor)"};

        /*79H*/
        string[] hetvenkilenc_h_marstertol = new string[]
        { "79H;0,1,2,5,7,9,11,12,13,16,19,21","Mars tér (üzletsor)","Tavasz utca","Damjanich  u.","Szeged(Rókus), vá.bej.út","Fonógyári út","Budapesti út (Dorozsmai út)","Zápor út","Öthalmi Diáklakások","Gumigyár","Nyári tanya","Kísérleti gazdaság","Fehértói halgazdaság"};
        string[] hetvenkilenc_h_fehertoitol = new string[]
        { "79H;0,3,5,7,8,10,11,13,14,15,16,17,19","Fehértói halgazdaság","Kísérleti gazdaság","Nyári tanya","Gumigyár","Öthalmi Diáklakások","Zápor út","Budapesti út (Dorozsmai út)","Fonógyári út","Szeged(Rókus), vá.bej.út","Vásárhelyi Pál u. (Kossuth L. sgt.)","Damjanich u.","Tavasz utca","Mars tér (üzletsor)"};

        /*74A*/
        string[] hetvennegy_a_marstertol = new string[]
        { "74A;0,2,4,5,6,7,8,9,10,11,12,13,14","Mars tér (Mikszáth u.)","Bartók tér","Dugonics tér (Tisza Lajos krt.)","Honvéd tér (Tisza Lajos krt.)","SZTK Rendelő","Bécsi krt.","Szent Ferenc u.","Dobó u.","Sárkány u.","Vadkerti tér","Kamarási u.","Rendező tér","Holt-Tisza"};
        string[] hetvennegy_a_holtiszatol = new string[]
        { "74A;0,1,2,3,4,5,6,8,9,10,11,13,14","Holt-Tisza","Rendező tér","Kamarási u.","Vadkerti tér","Sárkány u.","Dobó u.","Szent Ferenc u.","Bécsi krt.","SZTK Rendelő","Dugonics tér","Dugonics tér (Tisza Lajos krt.)","Centrum Áruház (Mikszáth u.)","Mars tér (Mikszáth u.)"};

        /*84*/
        string[] nyolcvannegy_makoshaztol = new string[]
        {"84;0,1,2,3,4,6,7,8,10,11,13,15,16,17", "Makkosház","Ipoly sor","Makkosházi krt.","Agyagos u.","József Attila sgt. (Budapesti krt.)","Tarján, víztorony","Csillag tér (Budapesti krt.)","Fecske u.","Sajka utca","Szent-Györgyi Albert u.","Fő fasor","Sportcsarnok (Temesvári krt.)","Csanádi u.","Újszeged, Gabonakutató"};
        string[] nyolcvannegy_gabonakutatotol = new string[]
        { "84;0,1,2,3,5,7,8,9,10,11,12,14,15,16","Újszeged, Gabonakutató","Csanádi u.","Sportcsarnok (Temesvári krt.)","Fő fasor","Szent-Györgyi Albert u.","Római krt. (Szilléri sgt.)","Fecske u.","Csillag tér (Budapesti krt.)","Tarján, víztorony","József Attila sgt. (Budapesti krt.)","Agyagos u.","Makkosházi krt. (Csongrádi sgt.)","Ipoly sor","Makkosház"};

        /*84A*/
        string[] nyolcvannegy_a_tarjanviztoronytol = new string[]
        { "84A;0,1,2,4,5,7,8,9,10","Tarján, víztorony","Csillag tér (Budapesti krt.)","Fecske u.","Sajka utca","Szent-Györgyi Albert u.","Fő fasor","Sportcsarnok (Temesvári krt.)","Csanádi u.","Újszeged, Gabonakutató"};
        string[] nyolcvannegy_a_gabonakutatotol = new string[]
        { "84A;0,1,2,3,5,7,8,9,10","Újszeged, Gabonakutató","Csanádi u.","Sportcsarnok (Temesvári krt.)","Fő fasor","Szent-Györgyi Albert u.","Római krt. (Szilléri sgt.)","Fecske u.","Csillag tér (Budapesti krt.)","Tarján, víztorony" };

        /*90*/
        string[] kilencven_lugasutcatol = new string[]
        {"90;0,1,3,5,7,9,10,11,13,15,16,17,18,19,20,21,22,23,24,25,26,27,28", "Lugas u.","Csillag tér (Budapesti krt.)","Tarján, víztorony","József Attila sgt. (Budapesti krt.)","Agyagos u.","Makkosházi krt. (Rókusi krt.)","Vértó","Rókusi II. sz. Ált. Isk.","Rókusi víztorony","Kisteleki u.","Vásárhelyi Pál u. (Pulz u.)","Csemegi-tó","Tisza Volán Zrt.","Kenyérgyári út","II. Kórház","Kálvária tér","Remény utca (ideiglenes)","Hajnal utca (ideiglenes)","Szél u.","Cserepes sor","Rákóczi u. (Vám tér)","Szent Ferenc u.","Személy pu."};
        string[] kilencven_szemelypalyaudvartol = new string[]
        { "90;0,1,2,3,4,6,8,9,10,11,12,13,15,17,18,19,21,22,24,26,27,28","Személy pu.","Szent Ferenc u.","Rákóczi u. (Vám tér)","Gólya u.","Kolozsvári tér","Nemes takács u. (ideiglenes)","Kálvária tér","II. Kórház","Kálvária sgt. (Vásárhelyi Pál u.)","Tisza Volán Zrt.","Csemegi-tó","Vásárhelyi Pál u. (Pulz u.)","Kisteleki u.","Rókusi víztorony","Rókusi II. sz. Ált. Isk.","Vértó","Makkosházi krt.","Agyagos u.","József Attila sgt. (Budapesti krt.)","Tarján, víztorony","Csillag tér (Budapesti krt.)","Lugas u."};

        /*90F*/
        string[] kilencven_f_petofiteleptol = new string[]
        { "90F;0,2,3,4,5,6,7,9,11,13,15,16,17,19,21,22,23,24,25,26,27,28,29,30,31,32,33,34","Petőfitelep, Fő tér","Duna u.","Városi Stadion","Etelka sor","Erdő u.","Csaba u.","Csillag tér (Budapesti krt.)","Tarján, víztorony","József Attila sgt. (Budapesti krt.)","Agyagos u.","Makkosházi krt. (Rókusi krt.)","Vértó","Rókusi II. sz. Ált. Isk.","Rókusi víztorony","Kisteleki u.","Vásárhelyi Pál u. (Pulz u.)","Csemegi-tó","Tisza Volán Zrt.","Kenyérgyári út","II. Kórház","Kálvária tér","Remény utca (ideiglenes)","Hajnal utca (ideiglenes)","Szél u.","Cserepes sor","Rákóczi u. (Vám tér)","Szent Ferenc u.","Személy pu."};
        string[] kilencven_f_szemelypalyaudvartol = new string[]
        {"90F;0,1,2,3,4,6,8,9,10,11,12,13,15,17,18,19,21,22,24,26,27,28,29,30,32,33,35", "Személy pu.","Szent Ferenc u.","Rákóczi u. (Vám tér)","Gólya u.","Kolozsvári tér","Nemes takács u. (ideiglenes)","Kálvária tér","II. Kórház","Kálvária sgt. (Vásárhelyi Pál u.)","Tisza Volán Zrt.","Csemegi-tó","Vásárhelyi Pál u. (Pulz u.)","Kisteleki u.","Rókusi víztorony","Rókusi II. sz. Ált. Isk.","Vértó","Makkosházi krt.","Agyagos u.","József Attila sgt. (Budapesti krt.)","Tarján, víztorony","Csillag tér (Budapesti krt.)","Csaba u.","Erdő u.","Etelka sor (Felső Tisza-part)","Városi Stadion","Duna u.","Petőfitelep, Fő tér"};

        /*90H*/
        string[] kilencven_h_lugautcatol = new string[]
        {"90H;0,1,2,4,6,8,9,10,11,12,13,15,17,18,19,20,22,24,25", "Lugas u.","Csillag tér (Budapesti krt.)","Tarján, víztorony","József Attila sgt. (Budapesti krt.)","Agyagos u.","Makkosházi krt. (Rókusi krt.)","Vértó","Rókusi II. sz. Ált. Isk.","Rókusi víztorony","Kisteleki u.","Szeged(Rókus), vá.bej.út","Fonógyári út","Budapesti út (Dorozsmai út)","Auchan Áruház","Zápor út","Öthalmi Diáklakások","Gumigyár, buszforduló","Back Bernát utca","Szegedi Ipari Logisztikai Központ"};
        string[] kilencven_h_szlogisztikatol = new string[]
        {"90H;0,1,2,4,5,6,7,11,13,14,16,17,18,20,22,23,24,25", "Szegedi Ipari Logisztikai Központ","Back Bernát utca","Gumigyár, buszforduló","Öthalmi Diáklakások","Zápor út","Budapesti út (Dorozsmai út)","Fonógyári út","Szeged(Rókus), vá.bej.út","Kisteleki u.","Rókusi víztorony","Rókusi II. sz. Ált. Isk.","Vértó","Makkosházi krt.","Agyagos u.","József Attila sgt. (Budapesti  krt.)","Tarján, víztorony","Csillag tér (Budapesti krt.)","Lugas u."};

        /*91E*/
        string[] kilencvenegye_szechenyitol = new string[] 
        { "91E;0,2,4,6,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,28,29,30,31,32,34,35,36,37,38,39,40,42,44,46,48,49","Széchenyi tér (Kelemen u.)","Torontál tér (P+R)","Sportcsarnok (Székely sor)","Közép fasor","Újszeged, víztorony","Radnóti u.","Thököly u.","Cinke u.","Pipiske utca","Hargitai u.","Pinty u.","Erdélyi tér","Traktor u.","Kamaratöltés","Kavics u.","Napfény köz","Barázda u.","Szőreg, ABC","Kossuth Lajos Ált. Isk.","Szőregi Szabadidőpark","Rózsatő u. (Szerb u.)","Gyár u. (Szerb u.)","Szőreg, malom","Gőzmalom u.","Gyár u. (Magyar u.)","Rózsatő u. (Magyar u.)","Vaspálya u.","Iskola u.","Szőreg, ABC","Barázda u.","Napfény köz","Kavics u.","Kamaratöltés","Aranyosi utca","Diófa utca (Szőregi út)","Sportcsarnok (Székely sor)","Torontál tér (P+R)","Széchenyi tér (Kelemen u.)","Centrum Áruház (Mikszáth u.)","Mars tér (Mikszáth u.)"};
        /*92E*/
        string[] kilencvenkettoe_szechenyitol = new string[]
        { "92E;0,1,2,4,6,7,8,9,11,12,13,14,15,16,17,19,20,21,22,23,24,25,27,28","Széchenyi tér (Kelemen u.)","Múzeum","Dózsa utca","Glattfelder Gyula tér","Gál utca","Római krt. (Szilléri sgt.)","Fecske u.","Csillag tér (Budapesti krt.)","Tarján, Víztorony tér","Csillag tér (Budapesti krt.)","Csaba u.","Erdő u.","Etelka sor (Felső Tisza-part)","Városi Stadion","Duna u.","Petőfitelep, Fő tér","Gábor Áron u.","Lidicei tér","Kalász u.","Acél u.","Diadal u.","Szeged, Szélső sor","József Attila sgt. (Budapesti krt.)","Tarján, Víztorony tér"};
        /*93E*/
        string[] kilencvenharome_szechenyitol = new string[] 
        { "93E;0,1,2,3,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31","Széchenyi tér (Kelemen u.)","Centrum Áruház (Mikszáth u.)","Mars tér (Mikszáth u.)","Mars tér (aut. áll.)","Hétvezér u.","Gém u.","Rózsa u. (Csongrádi sgt.)","Diófa Vendéglő","Makkosházi krt. (Rókusi krt.)","Vértó","Rókusi II. sz. Ált. Isk.","Rókusi víztorony","Kisteleki u.","Szeged(Rókus), vá.bej.út","Fonógyári út","Budapesti út","Kollégiumi út","Kiskundorozsma, vá.bej.","Tassi ház","Csatorna","Malom","Jerney János Ált. Isk.","Huszka Jenő utca","Széchenyi István u.","Basahíd utca","Szent János tér","Balajthy utca","Negyvennyolcas u.","Kiskundorozsma, Vásártér","Erdőtarcsa u.","Kiskundorozsma, Czékus u."};
        /*94E*/
        string[] kilencvennegye_szechenyitol = new string[] 
        { "94E;0,2,3,4,6,7,9,10,11,15,16","Széchenyi tér (Kelemen u.)","Dugonics tér (Tisza Lajos krt.)","Honvéd tér (Tisza Lajos krt.)","SZTK Rendelő","Bécsi krt.","Szent Ferenc u.","Dobó u.","Sárkány u.","Vadkerti tér","Kecskés telep, Gera Sándor u.","Kecskés telep, Bódi Vera u."};
        
        /*Trolik*/
        /*5*/
        string[] otostroli_kortoltestol = new string[]
        {"5;0,1,2,3,4,5,6,7,8,10,11,13,14,15,16", "Körtöltés utca","Rókusi víztorony","Rókusi II. sz. Ált.","Vértó","Makkosházi krt.","Diófa vendéglő","Rózsa utca","Berlini körút","Hétvezér utca","Mars tér (Aut. áll.)","Bartók tér","Széchenyi tér","Torontál tér (P+R)","Csanádi utca","Újszeged, Gyermekkorház"};
        string[] otostroli_gyermekkorhaztol = new string[]
        {"5;0,1,2,4,6,7,8,10,11,12,14,15,16,17", "Újszeged, Gyermekkorház","Csanádi utca","Torontál tér (P+R)","Széchenyi tér","Tisza Lajos krt. (Mikszáth u.)","Mars tér (Aut. áll.)","Hétvezér utca","Gém utca","Rózsa utca","Diófa vendéglő","Makkosházi krt.","Vértó","Rókusi II.sz.Ált.","Körtöltés utca"};

        /*7*/
        string[] hetestroli_bakaytol = new string[]
        { "7;0,1,2,3,4,5,6,7,8,9,10","Bakay Nándor utca","Vásárhelyi Pál út","Mura utca","Huszár utca (Okmányiroda)","Londoni körút","Attila utca","Bartók tér","Széchenyi tér (Kelemen utca)","Torontál tér (P+R)","Csanádi utca","Újszeged, Gyermekkorház"};
        string[] hetestroli_gyermekkorhaztol = new string[]
        {"7;0,1,2,4,5,6,7,8,9,10,11,12", "Újszeged, Gyermekkorház","Csanádi utca","Torontál tér (P+R)","Széchenyi tér (Kelemen utca)","Mars tér (Attila utca)","Londoni krt.","Huszár utca (Okmányiroda)","Mura utca","Vásárhelyi Pál út","Bakay Nándor utca"};

        /*8*/
        string[] nyolcastol_makkoshaztol = new string[]
        {"8;0,1,2,3,4,5,6,7,8,9,10,11,12,13", "Makkosház","Gát u.","Ortutay utca","Makkosházi krt.","Diófa Vendéglő","Rózsa utca","Tündér utca","Szent István tér","Anna-kút (Tisza Lajos krt.)","Centrum Áruház","Dugonics tér","Honvéd tér","Aradi vértanúk tere","Klinikák"};
        string[] nyolcastol_klinikaktol = new string[]
        { "8;0,1,2,3,4,5,6,7,8,9,10,11,12,13","Klinikák","SZTK Rendelő","Dugonics tér","Dugonics tér (Tisza Lajos krt.)","Centrum Áruház","Anna-kút (Tisza Lajos krt.)","Szent István tér","Gém utca","Rózsa utca","Diófa Vendéglő","Makkosházi krt.","Ortutay utca","Gát utca","Makkosház"};

        /*10*/
        string[] tizes_viztoronytol = new string[]
        { "10;0,1,2,4,6,7,8,9,10,11,12,13,14","Tarján, Víztorony tér","Csillag tér","Fecske utca","Római krt. (Szilléri sgt.)","Sándor utca","Csongrádi sgt.","Szent István tér","Anna-kút","Centrum Áruház","Dugonics tér","Honvéd tér","Aradi vértanúk tere","Klinikák"};
        string[] tizes_klinikaktol = new string[]
        { "10;0,1,2,3,4,5,6,7,9,10,11,12,13","Klinikák","SZTK Rendelő","Dugonics tér","Dugonics tér (Tisza Lajos krt.)","Centrum Áruház","Anna-kút (Tisza Lajos krt.)","Szent István tér","Berlini körűt","Gál utca","Római krt. (Szilléri sgt.)","Fecske utca","Csillag tér","Tarján, Víztorony tér"};

        /*9*/
        string[] kilences_lugasutcatol = new string[]
        { "9;0,1,2,3,3,4,5,5,6,7,9,10,11,12,13,14,15,16","Lugas utca","Csillag tér","Csaba utca","Erdő utca","Etelka sor","Tabán utca (Felső Tisza-part)","Háló utca","Ifjúsági Ház","Dózsa utca","Széchenyi tér","Tisza Lajos körút (Mikszáth u.)","Mars tér (Aut. áll.)","Hétvezér utca","Gém utca","Rózsa utca","Diófa Vendéglő","Makkosházi krt.","Vértói út"};
        string[] kilences_vertotol = new string[]
        { "9;0,1,2,3,4,5,6,7,8,10,11,12,13,14,15,16,17,18,19,20","Vértói út","Vértó","Makkosházi krt.","Diófa Vendéglő","Rózsa utca","Berlini krt.","Hétvezér utca","Mars tér (Aut. áll.)","Bartók tér","Széchenyi tér","Múzeum","Dózsa utca","Ifjúsági Ház","Háló utca","Tabán utca (Felső Tisza-part)","Etelka sor","Erdő utca","Csaba utca","Csillag tér","Lugas utca"};

        /*19*/
        string[] tizenkilences_viztoronytol = new string[]
        { "19;0,1,2,3,4,6,6,7,8,9,11,12,13,15,16,17,18,19,21,22","Tarján, Víztorony tér","Csillag tér","Csaba utca","Erdő utca","Etelka sor","Tabán utca (Felső Tisza-part)","Háló utca","Ifjúsági Ház","Dózsa utca","Széchenyi tér","Tisza Lajos körút (Mikszáth u.)","Mars tér (Aut. áll.)","Hétvezér utca","Gém utca","Rózsa utca","Diófa Vendéglő","Makkosházi körút","Ortutay utca","Gát utca","Makkosház"};
        string[] tizenkilences_makkoshaztol = new string[]
        { "19;0,1,2,4,5,6,7,8,10,11,13,14,15,16,17,18,19,20,21,22,23","Makkosház","Gát utca","Ortutay utca","Makkosházi krt.","Diófa Vendéglő","Rózsa utca","Berlini krt.","Hétvezér utca","Mars tér (Aut. áll.)","Bartók tér","Széchenyi tér","Múzeum","Dózsa utca","Ifjúsági Ház","Háló utca","Tabán utca (Felső Tisza-part)","Etelka sor","Erdő utca","Csaba utca","Csillag tér","Tarján, Víztorony tér"};

        /*Villamosok*/
        string[] egyes_szemelyitol = new string[] 
        { "1;0,1,2,3,4,5,7,8,9,10,12,13,14,15","Személy pu.","Galamb utca","Bécsi krt.","Aradi vértanúk tere","Somogyi utca","Széchenyi tér","Anna-kút","Rókusi templom","Tavasz utca","Damjanich u.","Vásárhelyi Pál utca","Pulz utca","Rókus pu.","Szeged Plaza"};
        string[] egyes_plazatol = new string[] 
        { "1;0,1,2,3,4,5,6,8,9,10,11,13,14,15","Szeged Plaza","Rókus pu.","Pulz utca","Vásárhelyi Pál utca","Damjanich u.","Tavasz utca","Rókusi templom","Anna-kút","Széchenyi tér","Somogyi utca","Aradi vértanúk tere","Bécsi krt.","Bem utca","Személy pu."};
        string[] kettes_szemelyitol = new string[] 
        { "2;0,1,2,3,4,5,7,8,9,10,12,13,14,16,17,18","Személy pu.","Galamb utca","Bécsi krt.","Aradi vértanúk tere","Somogyi utca","Széchenyi tér","Anna-kút","Rókusi templom","Tavasz utca","Damjanich u.","Vásárhelyi Pál utca","Szatymazi utca","Rókusi víztorony","Rókus II. sz. Ált.","Vértó","Európa liget"};
        string[] kettes_europaligettol = new string[] 
        { "2;0,1,2,4,5,7,8,9,10,12,13,14,15,17,18,19","Európa liget","Vértó","Rókus II. sz. Ált.","Rókusi víztorony","Szatymazi utca","Vásárhelyi Pál utca","Damjanich u.","Tavasz utca","Rókusi templom","Anna-kút","Széchenyi tér","Somogyi utca","Aradi vértanúk tere","Bécsi krt.","Bem utca","Személy pu."};
        string[] negyes_tarjantol = new string[] 
        { "4;0,1,2,3,4,5,7,8,10,11,13,15,17,18,19,20,21","Tarján","Budapesti krt.","Deák Ferenc Gimnázium","Rózsa utca","Kecskeméti utca","Brüsszeli krt.","Szent György tér","Glattfelder Gyula tér","Anna-kút","Centrum Áruház","Dugonics tér","Vitéz utca","Szivárvány kitérő","Vám tér","Szabadkai út","Szalámigyár","Kecskés"};
        string[] negyes_kecskestol = new string[] 
        { "4;0,1,2,3,4,6,8,10,11,13,14,16,17,18,19,20,21","Kecskés","Szalámigyár","Szabadkai út","Vám tér","Szivárvány kitérő","Vitéz utca","Dugonics tér","Centrum Áruház","Anna-kút","Glattfelder Gyula tér","Szent György tér","Brüsszeli krt.","Kecskeméti utca","Rózsa utca","Deák Ferenc Gimnázium","Budapesti krt.","Tarján"};
        string[] haromas_tarjantol = new string[] 
        { "3;0,1,2,3,4,5,7,8,10,11,13,15,16,17,18,20","Tarján","Budapesti krt.","Deák Ferenc Gimnázium","Rózsa utca","Kecskeméti utca","Brüsszeli krt.","Szent György tér","Glattfelder Gyula tér","Anna-kút","Centrum Áruház","Dugonics tér","Londoni krt.","Veresács utca","Kálvária tér","II. Kórház","Vadaspark"};
        string[] haromas_vadasparktol = new string[] 
        { "3;0,2,3,4,5,7,9,10,12,13,15,16,17,18,19,20","Vadaspark","II. Kórház","Kálvária tér","Veresács utca","Londoni krt.","Dugonics tér","Centrum Áruház","Anna-kút","Glattfelder Gyula tér","Szent György tér","Brüsszeli krt.","Kecskeméti utca","Rózsa utca","Deák Ferenc Gimnázium","Budapesti krt.","Tarján"};
        string[] haromfes_tarjantol = new string[] 
        { "3F;0,1,2,3,4,5,7,8,10,11,13,15,16,17,18,20,23,24,25,26","Tarján","Budapesti krt.","Deák Ferenc Gimnázium","Rózsa utca","Kecskeméti utca","Brüsszeli krt.","Szent György tér","Glattfelder Gyula tér","Anna-kút","Centrum Áruház","Dugonics tér","Londoni krt.","Veresács utca","Kálvária tér","II. Kórház","Vadaspark","Belvárosi temető","Belvárosi temető II.","Kereskedő köz","Fonógyári út"};
        string[] haromfes_fonogyaritol = new string[] 
        { "3F;0,1,2,3,6,8,9,10,11,13,15,16,18,19,21,22,23,24,25,26","Fonógyári út","Kereskedő köz","Belvárosi temető II.","Belvárosi temető","Vadaspark","II. Kórház","Kálvária tér","Veresács utca","Londoni krt.","Dugonics tér","Centrum Áruház","Anna-kút","Glattfelder Gyula tér","Szent György tér","Brüsszeli krt.","Kecskeméti utca","Rózsa utca","Deák Ferenc Gimnázium","Budapesti krt.","Tarján"};
        /**/
        /*1. vége*/

        /*2. Deklaráljuk az egyes járműtípusoknál, hogy melyik órákban vannak jelen.*/
        public int[] auchan_orak = new int[]
        { 7,8,9,10,11,12,13,14,15,16,17,18,19};

        public int[] trolik_villamosok_orak = new int[]
        { 3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,0};
        /*2. vége*/

        /*3. Deklaráljuk az egyes járatoknál, hogy mikor indulnak az egyes végállomásokon.*/
        string[] _Auchan_nyitva_8tol = new string[] /*itt majd regge 7:43kor a szamos utcatol indul !*/
        { "Auchan járat;Auchan áruház;munkanapokon","25","25","25","25","25","25","25","25","25","25","25","25"};

        string[] _7_F_munkanap_marstertol_munkanapokon = new string[]
        {"7F;Mars tér;munkanapokon","","","25,55","35","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","45","","" };
        string[] _7_F_nyari_tanszunetben_munkaszuneti_napokonSzeged_helyi_marstertol = new string[]
        {"7F;Mars tér;Nyári tanszünetben munkaszüneti napok on (Szeged helyi)","","","","05","05","05,35","05,35","05,35","05","05","05","05,35","05,35","05,35","05,35","05,35","05","05","05","05","45","",""};
        string[] _7_F_nyari_tanszunetben_szabadnapokon_Szeged_helyi_marstertol = new string[]
        {"7F;Mars tér;Nyári tanszünetben szabadna pokon (Szeged helyi)","","","","05","05","05,35","05,35","05,35","05","05","05","05,35","05,35","05,35","05,35","05,35","05","05","05","05","45","",""};
        string[] _7_F_tanevben_munkaszuneti_napokon_Szeged_helyi_marstertol = new string[]
        {"7F;Mars tér;Tanév tartama alatt munkaszüneti napokon (Szeged helyi)","","","","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","45","",""};
        string[] _7_F_tanevben_szabadnapokon_Szeged_helyi_marstertol = new string[]
        {"7F;Mars tér;Tanév tartama alatt szabadnapokon (Szeged helyi)","","","","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","45","",""};
        string[] _7_F_munkanap_furdotol_munkanapokon = new string[]
        {"7F;Mars tér;munkanapokon","","","50","20","00,30","00,30","30","30","30","30","30","30","30","30","30","30","30","30","30","45","","","" };
        string[] _7_F_nyari_tanszunetben_munkaszuneti_napokonSzeged_helyi_furdotol = new string[]
        {"7F;Mars tér;Nyári tanszünetben munkaszüneti napok on (Szeged helyi)","","","","30","30","30","00,30","00,30","00,30","30","30","30","00,30","00,30","00,30","00,30","00,30","30","30","30","","",""};
        string[] _7_F_nyari_tanszunetben_szabadnapokon_Szeged_helyi_furdotol = new string[]
        {"7F;Mars tér;Nyári tanszünetben szabadnapo kon (Szeged helyi)","","","","30","30","30","00,30","00,30","00,30","30","30","30","00,30","00,30","00,30","00,30","00,30","30","30","30","","",""};
        string[] _7_F_tanevben_munkaszuneti_napokon_Szeged_helyi_furdotol = new string[]
        {"7F;Mars tér;Tanév tartama alatt munkaszüneti napokon (Szeged helyi)","","","","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","","",""};
        string[] _7_F_tanevben_szabadnapokon_Szeged_helyi_furdotol = new string[]
        {"7F;Mars tér;Tanév tartama alatt szabadnapokon (Szeged helyi)","","","","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","","",""};

        string[] _13_munkanapokon_viztoronytol = new string[]
        {"13;Tarján, Víztorony tér;munkanapokon","","","28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","","","","","","",""};
        string[] _13_munkanapokon_napfényparktol = new string[]
        {"13;Móravárosi Bevásárlóközpont;munkanapokon","","","53","13,33,53","13,33,53","13,33,53","13,33,53","13,33,53","13,33,53","13,33,53","13,33,53","13,33,53","13,33,53","13,33,53","13,33,53","13,33,53","","","","","","",""};

        string[] _13_A_marster_bevnyitva_munkanapokon = new string[]
        { "13A;Mars tér(üzletsor);munkanapokon","","","", "", "", "", "", "", "", "", "", "", "", "", "", "","30","00", "00", "00", "00","","" };
        string[] _13_A_marster_bevnyitva_szabadnapokon = new string[]
        { "13A;Mars tér(üzletsor);szombat","","", "", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00", "00", "00", "00","","" };
        string[] _13_A_marster_bevnyitva_munkaszuneti_napokon = new string[]
        { "13A;Mars tér(üzletsor);vasárnap","","", "", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00", "00", "00", "00","","" };
        string[] _13_A_bevtol_bevnyitva_munkanapokon = new string[]
        { "13A;Móravárosi Bevásárlóközpont;munkanapokon","","", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "13,43", "13", "13", "13", "13","","" };
        string[] _13_A_bevtol_bevnyitva_szabadnapokon = new string[]
        { "13A;Móravárosi Bevásárlóközpont;szombat","","", "", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13", "13", "13", "13","","" };
        string[] _13_A_bevtol_bevnyitva_munkaszuneti_napokon = new string[]
        { "13A;Móravárosi Bevásárlóközpont;vasárnap","","", "", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13,43", "13", "13", "13", "13","","" };
        string[] _13_A_marster_bevzarva_munkaszuneti_napokon = new string[]
        { "13A;Mars tér(üzletsor);vasárnap","","", "", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00,30", "00", "00", "00", "00","","" };
        string[] _13_A_bevtol_bevzarva_munkaszuneti_napokon = new string[]
        { "13A;Gólya utca;vasárnap","","", "", "15,45", "15,45", "15,45", "15,45", "15,45", "15,45", "15,45", "15,45", "15,45", "15,45", "15,45", "15,45", "15,45", "15,45", "15", "15", "15", "15","","" };

        string[] _20_petofitol_munkanapokon = new string[]
        {"20;Petőfitelep, Fő tér;munkanapokon","","","30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","20,50","20,50","20",""};
        string[] _20_petofitol_iskolai_eloadas_napokon = new string[]
        {"20;Petőfitelep, Fő tér;iskolai előadási napokon","","","","","20,40","","","","","","","","","","","","","","","","","",""};
        string[] _20_petofitol_szabadnapokon = new string[]
        {"20;Petőfitelep, Fő tér;szombat","","","50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20",""};
        string[] _20_petofitol_munkaszuneti_napokon = new string[]
        { "20;Petőfitelep, Fő tér;vasárnap","","","50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20",""};
        string[] _20_vadkertitol_munkanapokon = new string[]
        { "20;Vadkerti tér, buszforduló;munkanapokon","","","40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,50","20,50","25",""};
        string[] _20_vadkertitol_iskolai_eloadas_napokon = new string[]
        { "20;Vadkerti tér, buszforduló;iskolai előadási napokon","","","","","50","10","","","","","","","","","","","","","","","","",""};
        string[] _20_vadkertitol_szabadnapokon = new string[]
        { "20;Vadkerti tér, buszforduló;szombat","","","50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","25",""};
        string[] _20_vadkertitol_munkaszuneti_napokon = new string[]
        { "20;Vadkerti tér, buszforduló;vasárnap","","","50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","25",""};

        string[] _20_A_petofitol_tanevtartalmaalatt_munkanap = new string[]
        { "20A;Petőfitelep, Fő tér;tanév tartalma alatt munkanapokon","","","40","00,20,40","00,20,40","00,20,40","00,20","","","","40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00","","","","","",""};
        string[] _20_A_honvedtol_tanevtartalmaalatt_munkanap = new string[]
        { "20A;Honvéd tér;tanév tartalma alatt munkanapokon","","","","00,20,40","00,20,40","00,20,40","00,20,40","","","","","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20","","","","","",""};

        string[] _21_petofitol_munkanapokon = new string[]
        { "21;Petőfitelep, Fő tér;munkanapokon","","","28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","08,28,48","28,58","28,58","28,58","28","",""};
        string[] _21_petofitol_iskolai_eloadasi_napokon = new string[]
        { "21;Petőfitelep, Fő tér;iskolai előadási napokon","","","","","18,38,58","18,38,58","","","","","","","","","","","","","","","","",""};
        string[] _21_petofitol_szabadnapokon = new string[]
        { "21;Petőfitelep, Fő tér;szombat","","","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28","",""};
        string[] _21_petofitol_munaszuneti_napokon = new string[]
        { "21;Petőfitelep, Fő tér;vasárnap","","","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28","",""};
        string[] _21_palyaudvartol_munkanapokon = new string[]
        { "21;Személy pu.;munkanapokon","","","38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","28,58","28,58","28,58","28,58","",""};
        string[] _21_palyaudvartol_iskolai_eloadasi_napokon = new string[]
        { "21;Személy pu.;iskolai előadási napokon","","","","","48","08,28,48","08,28","","","","","","","","","","","","","","","",""};
        string[] _21_palyaudvartol_szabadnapokon = new string[]
        { "21;Személy pu.;szombat","","","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","",""};
        string[] _21_palyaudvartol_munaszuneti_napokon = new string[]
        { "21;Személy pu.;vasárnap","","","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","",""};

        string[] _36_honvedrol_szabadnapokon = new string[]
        { "36;Honvéd tér;szombat","","","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","58","58","58","58","58","58","58","58","58","58","",""};
        string[] _36_honvedrol_munkaszuneti_napokon = new string[]
        { "36;Honvéd tér;vasárnap","","","58","58","58","58","58","58","58","58","58","58","58","58","58","58","58","58","58","58","58","",""};
        string[] _36_honvedrol_nyari_tanszunetben_munkanapokon = new string[]
        { "36;Honvéd tér;Nyári tanszünetben munkanapokon (Szeged helyi)","","","","","","","40","20","00,40","20","00","","","","","","40","00,20","00,40","20","00,58","",""};
        string[] _36_honvedrol_tanev_alatt_munkanapokon = new string[]
        { "36;Honvéd tér;Tanév tartama alatt munkanapokon (Szeged helyi)","","","","","","","40","20","00,40","20","00","","","","","","40","00,20","00,40","20","00,58","",""};
        string[] _36_kkdorozsmarol_szabadnapokon = new string[]
        { "36;Kiskundorozsma, Czékus u.;szombat","","","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28","28","28","28","28","28","28","28","28","28","",""};
        string[] _36_kkdorozsmarol_munkaszuneti_napokon = new string[]
        { "36;Kiskundorozsma, Czékus u.;vasárnap","","","28","28","28","28","28","28","28","28","28","28","28","28","28","28","28","28","28","28","28","",""};
        string[] _36_kkdorozsmarol_nyari_tanszunetben_munkanapokon = new string[]
        { "36;Kiskundorozsma, Czékus u.;Nyári tanszünetben munkanapokon (Szeged helyi)","","","","","","","10,50","30","10,50","30","","","","","","","10,30,50","30","10,50","30","10,50","",""};
        string[] _36_kkdorozsmarol_tanev_alatt_munkanapokon = new string[]
        { "36;Kiskundorozsma, Czékus u.;Tanév tartama alatt munkanapokon (Szeged helyi)","","","","","","","10,50","30","10,50","30","","","","","","","10,30,50","30","10,50","30","10,50","",""};

        string[] _79_auchan_nyitva_marstertol_munkanapokon = new string[]
        { "79;Mars tér (üzletsor);munkanapokon","","","","","","","30","00,30","00,30","00,30","00,30","","30","00,30","00,30","","00,30","00,30","00,30","30","00","",""};
        string[] _79_auchan_nyitva_marstertol_szabadnapokon = new string[]
        { "79;Mars tér (üzletsor);szombat","","","","","00,30","30","30","30","30","30","30","30","30","30","30","","00,30","30","30","","00","",""};
        string[] _79_auchan_nyitva_marstertol_munkaszuneti_napokon = new string[]
        { "79;Mars tér (üzletsor);vasárnap","","","","","00,30","30","30","30","30","30","30","30","30","30","30","","00,30","30","30","","00","",""};
        string[] _79_auchan_zarva_marstertol_munkaszuneti_napokon = new string[]
        { "79;Mars tér (üzletsor);vasárnap","","","","","00,30","30","30","30","30","30","30","30","30","30","30","","00,30","30","30","","00","",""};
        string[] _79_auchan_nyitva_logisztikatol_munkanapokon = new string[]
        { "79;Szegedi Ipari Logisztikai Központ;munkanapokon","","","","","","","50","20,50","20,50","20,50","20,50","","50","20,50","20,50","","20,50","20,50","20,50","50","20","",""};
        string[] _79_auchan_nyitva_logisztikatol_szabadnapokon = new string[]
        { "79;Szegedi Ipari Logisztikai Központ;szombat","","","","","","50","50","50","50","50","50","50","50","50","50","50","","50","50","50","","",""};
        string[] _79_auchan_nyitva_logisztikatolmunkaszuneti_napokon = new string[]
        { "79;Szegedi Ipari Logisztikai Központ;munkaszüneti napokon","","","","","","","50","50","50","50","50","50","50","50","50","50","","50","50","","","",""};
        string[] _79_auchan_zarva_logisztikatol_munkaszuneti_napokon = new string[]
        { "79;Szegedi Ipari Logisztikai Központ;munkaszüneti napokon","","","","","50","50","50","50","50","50","50","50","50","50","50","50","","50","50","50","","",""};

        string[] _77_baktotol_munkanapokon = new string[]
        { "77;Baktó,Völgyérhát u.;munkanapokon","","","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","","",""};
        string[] _77_baktotol_szabadnapokon = new string[]
        { "77;Baktó,Völgyérhát u.;szombat","","","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","","",""};
        string[] _77_baktotol_munkaszuneti_napokon = new string[]
        { "77;Baktó,Völgyérhát u.;vasárnap","","","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","57","","",""};
        string[] _77_szemelyitol_munanapokon = new string[]
        { "77;Személy pu.;munkanapokon","","","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","","",""};
        string[] _77_szemelyitol_szabadnapokon = new string[]
        { "77;Személy pu.;szombat","","","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","","",""};
        string[] _77_szemelyitol_munkaszuneti_napokon = new string[]
        { "77;Személy pu.;vasárnap","","","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","","",""};
        string[] _77_tarjanviztoronytol_munkanapokon = new string[]
        { "77;Tarján, víztorony;munkanapokon","","","05","","","","","","","","","","","","","","","","","","","",""};
        string[] _77_tarjanviztoronytol_szabadnapokon = new string[]
        { "77;Tarján, víztorony;szombat","","","05","","","","","","","","","","","","","","","","","","","",""};
        string[] _77_tarjanviztoronytol_munkaszuneti_napokon = new string[]
        { "77;Tarján, víztorony;vasárnap","","","05","","","","","","","","","","","","","","","","","","","",""};

        string[] _60_marstertol_szabadnapokon = new string[]
        { "60;Mars tér (Szt. Rókus tér);szombat","","","35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","35","35","35","35","35","35","35","35","35","35","",""};
        string[] _60_marstertol_munkaszuneti_napokon = new string[]
        { "60;Mars tér (Szt. Rókus tér);vasárnap","","","35","35","35","35","35","35","35","35","35","35","35","35","35","35","35","35","35","35","35","",""};
        string[] _60_marstertol_nyariszunet_munkanapokon = new string[]
        { "60;Mars tér (Szt. Rókus tér);nyári tanszünetben munk anapokon","","","35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","15,35","35","35","35","35","",""};
        string[] _60_marstertol_tanevben_munkanapokon = new string[]
        { "60;Mars tér (Szt. Rókus tér);tanév tartama alatt munkanapokon","","","35","05,35,55","15,35,55","15,35","05,35","05,35","05,35","05,35","05,35","05,35,55","15,35,55","15,35,55","15,35,55","15,35","15,35","35","35","35","35","",""};
        string[] _60_szoregtol_szabadnapokon = new string[]
        { "60;Szőreg, malom;szombat","","","45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","30","30","30","30","30","30","30","30","30","30","",""};
        string[] _60_szoregtol_munkaszuneti_napokon = new string[]
        { "60;Szőreg, malom;vasárnap","","","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","",""};
        string[] _60_szoregtol_nyariszunet_munkanapokon = new string[]
        { "60;Szőreg, malom;nyári tanszünetben munkana pokon","","","45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,40","20","00,40","30","30","30","",""};
        string[] _60_szoregtol_tanevben_munkanapokon = new string[]
        { "60;Szőreg, malom;tanév tartama alatt munkanapokon","","","45","15,45","10,30,50","10,30,50","15,45","15,45","15,45","15,45","15,45","15,45","10,30,50","10,30,50","10,30,50","10,30,50","20","00,40","30","30","30","",""};

        string[] _60_Y_marstertol_szabadnapokon = new string[]
        { "60Y;Mars tér (Szt. Rókus tér);szombat","","","50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20","05","05","05","05","05","05","05","05","05","05,55","",""};
        string[] _60_Y_marstertol_munkaszuneti_napokon = new string[]
        { "60Y;Mars tér (Szt. Rókus tér);vasárnap","","","","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05","05,55","",""};
        string[] _60_Y_marstertol_nyariszunet_munkanapokon = new string[]
        { "60Y;Mars tér (Szt. Rókus tér);nyári tanszünetben munkan apokon","","","50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","15,55","35","15","05","05","05,55","",""};
        string[] _60_Y_marstertol_tanevben_munkanapokon = new string[]
        { "60Y;Mars tér (Szt. Rókus tér);tanév tartama alatt munkanapokon","","","50","20,45","05,25,45","05,25,50","20,50","20,50","20,50","20,50","20,50","20,45","05,25,45","05,25,45","05,25,45","05,25,55","35","15","05","05","05,55","",""};
        string[] _60_Y_szoregtol_szabadnapokon = new string[]
        { "60Y;Szőreg, malom;szombat","","","25","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00","00","00","00","00","00","00","00","00","00","",""};
        string[] _60_Y_szoregtol_munkaszuneti_napokon = new string[]
        { "60Y;Szőreg, malom;vasárnap","","","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00","",""};
        string[] _60_Y_szoregtol_nyariszunet_munkanapokon = new string[]
        { "60Y;Szőreg, malom;nyári tanszünetben munkanap okon","","","25","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,40","20","00","00","00","",""};
        string[] _60_Y_szoregtol_tanevben_munkanapokon = new string[]
        { "60Y;Szőreg, malom;tanév tartama alatt munkanapokon","","","25","00,30","00,20,40","00,20,40","00,30","00,30","00,30","00,30","00,30","00,30","00,20,40","00,20,40","00,20,40","00,20,40","00,40","20","00","00","00","",""};

        string[] _70_marstertol_munkanapokon = new string[]
        { "70;Mars tér (Szt. Rókus tér);munkanapokon","","","45","15,45","15,45","15,45","45","45","45","45","45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","45","45","45","",""};
        string[] _70_marstertol_szabadnapokon = new string[]
        { "70;Mars tér (Szt. Rókus tér);szombat","","","45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","45","45","45","",""};
        string[] _70_marstertol_munkaszuneti_napokon = new string[]
        { "70;Mars tér (Szt. Rókus tér);vasárnap","","","45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","45","45","45","",""};
        string[] _70_fuveszkerttol_munkanapokon = new string[]
        { "70;Füvészkert;munkanapokon","","","","00,30","00,30","00,30","00","00","00","00","00","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00","00","00","00",""};
        string[] _70_fuveszkerttol_szabadnapokon = new string[]
        { "70;Füvészkert;szombat","","","","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00","00","00","00",""};
        string[] _70_fuveszkerttol_munkaszuneti_napokon = new string[]
        { "70;Füvészkert;vasárnap","","","","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00","00","00","00",""};

        string[] _71_marstertol_szabadnapokon = new string[]
        { "71;Mars tér (Mikszáth u.);szombat","","","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","",""};
        string[] _71_marstertol_munkaszuneti_napokon = new string[]
        { "71;Mars tér (Mikszáth u.);vasárnap","","","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","",""};
        string[] _71_marstertol_nyari_tanszunet_munkanapokon = new string[]
        { "71;Mars tér (Mikszáth u.);nyári tanszünet munk anapokon","","","20,40","00,20,40","00,20,35,50","05,20,35,50","05,20,40","05,20,40","05,20,40","05,20,40","05,20,40","05,20,40","05,20,40","05,20,40","05,20,40","05,20,40","05,20,40","05,20,40","00,25,55","25,55","25,55","",""};
        string[] _71_marstertol_tanevtartalmaalatt_munkanapokon = new string[]
        { "71;Mars tér (Mikszáth u.);tanév tartalma alatt munkanapokon","","","20,40","00,20,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","05,20,40","05,20,40","00,25,55","25,55","25,55","",""};
        string[] _71_katalinutcatol_szabadnapokon = new string[]
        { "71;Katalin utca;szombat","","","45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15",""};
        string[] _71_katalinutcatol_munkaszuneti_napokon = new string[]
        { "71;Katalin utca;vasárnap","","","45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15",""};
        string[] _71_katalinutcatol_nyari_tanszunet_munkanapokon = new string[]
        { "71;Katalin utca;nyári tanszünet mu nkanapokon","","","40","00,20,40","00,20,40,55","10,25,40,55","10,25,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,45","15,45","15,45","15",""};
        string[] _71_katalinutcatol_tanevtartalmaalatt_munkanapokon = new string[]
        { "71;Katalin utca;tanév tartalma alatt munkanapokon","","","40","00,20,40","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,40","00,20,40","00,20,45","15,45","15,45","15",""};

        string[] _72_marstertol_szabadnapokon = new string[]
        { "72;Mars tér (Mikszáth u.);szombat","","","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10",""};
        string[] _72_marstertol_munkaszuneti_napokon = new string[]
        { "72;Mars tér (Mikszáth u.);vasárnap","","","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10",""};
        string[] _72_marstertol_nyari_tanszunet_munkanapokon = new string[]
        { "72;Mars tér (Mikszáth u.);nyári tanszünet munk anapokon","","","10,30,50","10,30,50","10,30,45","00,15,30,45","00,15,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,40","10,40","10,40","10",""};
        string[] _72_marstertol_tanevtartalmaalatt_munkanapokon = new string[]
        { "72;Mars tér (Mikszáth u.);tanév tartalma alatt munkanapokon","","","10,30,50","10,30,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","10,30,50","10,30,50","10,40","10,40","10,40","10",""};
        string[] _72_erdelyitertol_szabadnapokon = new string[]
        { "72;Erdélyi tér;szombat","","","30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30",""};
        string[] _72_erdelyitertol_munkaszuneti_napokon = new string[]
        { "72;Erdélyi tér;vasárnap","","","30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30",""};
        string[] _72_erdelyitertol_nyari_tanszunet_munkanapokon = new string[]
        { "72;Erdélyi tér;nyári tanszünet munka napokon","","","30,50","10,30,50","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","10,25,40,55","10,25,40,55","10,25,40,55","10,25,40,55","10,25,40,55","10,25,40,55","10,25,40,55","10,25,40,55","10,25,40,55","10,30,50","10,30,50","10,30","00,30","00,30","00,30",""};
        string[] _72_erdelyitertol_tanevtartalmaalatt_munkanapokon = new string[]
        { "72;Erdélyi tér;tanév tartalma alatt munkanapokon","","","30,50","10,30,50","10,30,50","05,20,35,50","05,20,35,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30","00,30","00,30","00,30",""};

        string[] _73_marstertol_szabadnapokon = new string[]
        { "73;Mars tér (üzletsor);szombat","","","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","30","30","30","30","30","30","30","30","30","",""};
        string[] _73_marstertol_munkaszuneti_napokon = new string[]
        { "73;Mars tér (üzletsor);vasárnap","","","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30","",""};
        string[] _73_marstertol_nyari_tanszunet_munkanapokon = new string[]
        { "73;Mars tér (üzletsor);nyári tanszünet munkan apokon","","","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","30","30","30","",""};
        string[] _73_marstertol_tanevtartalmaalatt_munkanapokon = new string[]
        { "73;Mars tér (üzletsor);tanév tartalma alatt munkanapokon","","","00,30","00,30,50","10,30,50","10,30","00,30","00,30","00,30","00,30","00,30","00,30,50","10,30,50","10,30,50","10,30,50","10,30","00,30","00,30","30","30","30","",""};
        string[] _73_taperol_szabadnapokon = new string[]
        { "73;Tápé, Csatár u.;szombat","","","35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","20","20","20","20","20","20","20","20","20","20",""};
        string[] _73_taperol_munkaszuneti_napokon = new string[]
        { "73;Tápé, Csatár u.;vasárnap","","","20","20","20","20","20","20","20","20","20","20","20","20","20","20","20","20","20","20","20","20",""};
        string[] _73_taperol_nyari_tanszunet_munkanapokon = new string[]
        { "73;Tápé, Csatár u.;nyári tanszünet munkan apokon","","","35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","20","20","20","20",""};
        string[] _73_taperol_tanevtartalmaalatt_munkanapokon = new string[]
        { "73;Tápé, Csatár u.;tanév tartalma alatt munkanapokon","","","35","05,35","00,20,40","00,20,40","05,35","05,35","05,35","05,35","05,35","05,35","00,20,40","00,20,40","00,20,40","00,20,40","05,35","05,35","20","20","20","20",""};

        string[] _73Y_marstertol_szabadnapokon = new string[]
        { "73Y;Mars tér (üzletsor);szombat","","","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15","00","00","00","00","00","00","00","00","00","00",""};
        string[] _73Y_marstertol_munkaszuneti_napokon = new string[]
        { "73Y;Mars tér (üzletsor);vasárnap","","","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00","00",""};
        string[] _73Y_marstertol_nyari_tanszunet_munkanapokon = new string[]
        { "73Y;Mars tér (üzletsor);nyári tanszünet munka napokon","","","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15","00","00","00","00",""};
        string[] _73Y_marstertol_tanevtartalmaalatt_munkanapokon = new string[]
        { "73Y;Mars tér (üzletsor);tanév tartalma alatt munkanapokon","","","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15,45","15","00","00","00","00",""};
        string[] _73Y_taperol_szabadnapokon = new string[]
        { "73Y;Tápé, Csatár u.;szombat","","","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","50","50","50","50","50","50","50","50","50","",""};
        string[] _73Y_taperol_munkaszuneti_napokon = new string[]
        { "73Y;Tápé, Csatár u.;vasárnap","","","50","50","50","50","50","50","50","50","50","50","50","50","50","50","50","50","50","50","50","",""};
        string[] _73Y_taperol_nyari_tanszunet_munkanapokon = new string[]
        { "73Y;Tápé, Csatár u.;nyári tanszünet munka napokon","","","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","50","50","50","",""};
        string[] _73Y_taperol_tanevtartalmaalatt_munkanapokon = new string[]
        { "73Y;Tápé, Csatár u.;tanév tartalma alatt munkanapokon","","","20,50","20,50","10,30,50","10,30,50","20,50","20,50","20,50","20,50","20,50","20,50","10,30,50","10,30,50","10,30,50","10,30,50","20,50","20,50","50","50","50","",""};

        string[] _74_marstertol_szabadnapokon = new string[]
        { "74;Mars tér (Mikszáth u.);szombat","","","40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","",""};
        string[] _74_marstertol_munkaszuneti_napokon = new string[]
        { "74;Mars tér (Mikszáth u.);vasárnap","","","40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","10,40","",""};
        string[] _74_marstertol_nyari_tanszunet_munkanapokon = new string[]
        { "74;Mars tér (Mikszáth u.);nyári tanszünet munka napokon","","","40","00,20,40","00,20,40","00,20,40","00,20,40","10,40","10,40","10,40","10,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","10,40","10,40","10,40","10,45","",""};
        string[] _74_marstertol_tanevtartalmaalatt_munkanapokon = new string[]
        { "74;Mars tér (Mikszáth u.);tanév tartalma alatt munkanapokon","","","40","00,20,40","00,20,40","00,20,40","00,20,40","10,40","10,40","10,40","10,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","10,40","10,40","10,40","10,45","",""};
        string[] _74_gyalaretrol_szabadnapokon = new string[]
        { "74;Gyálarét;szombat","","","35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05","06",""};
        string[] _74_gyalaretrol_munkaszuneti_napokon = new string[]
        { "74;Gyálarét;vasárnap","","","35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05,35","05","06",""};
        string[] _74_gyalaretrol_nyari_tanszunet_munkanapokon = new string[]
        { "74;Gyálarét;nyári tanszünet munkanap okon","","","35","05,25,45","05,25,45","05,25,45","05,25,45","05,35","05,35","05,35","05,35","05,25,45","05,25,45","05,25,45","05,25,45","05,25,45","05,25,45","05,35","05,35","05,35","05","06",""};
        string[] _74_gyalaretrol_tanevtartalmaalatt_munkanapokon = new string[]
        { "74;Gyálarét;tanév tartalma alatt munkanapokon","","","35","05,25,45","05,25,45","05,25,45","05,25,45","05,35","05,35","05,35","05,35","05,25,45","05,25,45","05,25,45","05,25,45","05,25,45","05,25,45","05,35","05,35","05,35","05","06",""};

        string[] _76_marstertol_szabadnapokon = new string[]
        { "76;Mars tér (Mikszáth u.);szombat","","","55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","",""};
        string[] _76_marstertol_munkaszuneti_napokon = new string[]
        { "76;Mars tér (Mikszáth u.);vasárnap","","","55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","25,55","",""};
        string[] _76_marstertol_tanevtartalmaalatt_munkanapokon = new string[]
        { "76;Mars tér (Mikszáth u.);tanév tartalma alatt munkanapokon","","","35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,25,55","25,55","25,55","25,55","25,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","25,55","25,55","25,55","25,55","25,55","",""};
        string[] _76_marstertol_nyari_tanszunet_munkanapokon = new string[]
        { "76;Mars tér (Mikszáth u.);nyári tanszünet munkan apokon","","","35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,25,55","25,55","25,55","25,55","25,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","25,55","25,55","25,55","25,55","25,55","",""};
        string[] _76_szentmihalytol_szabadnapokon = new string[]
        { "76;Szentmihály;szombat","","","50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","",""};
        string[] _76_szentmihalytol_munkaszuneti_napokon = new string[]
        { "76;Szentmihály;vasárnap","","","50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","",""};
        string[] _76_szentmihalytol_tanevtartalmaalatt_munkanapokon = new string[]
        { "76;Szentmihály;tanév tartalma alatt munkanapokon","","","35,55","10,25,40,55","10,25,40,55","10,25,40,55","10,25,50","20,50","20,50","20,50","20,50","20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,50","20,50","20,50","20,50","20,50","",""};
        string[] _76_szentmihalytol_nyari_tanszunet_munkanapokon = new string[]
        { "76;Szentmihály;nyári tanszünet munka napokon","","","35,55","10,25,40,55","10,25,40,55","10,25,40,55","10,25,50","20,50","20,50","20,50","20,50","20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,50","20,50","20,50","20,50","20,50","",""};

        string[] _77A_baktotol_munkanapokon = new string[]
        { "77A;Baktó,Völgyérhát u.;munkanapokon","","","27","17,37","17,37","17,37","17,37","17,37","17,37","17,37","17,37","17,37","17,37","17,37","17,37","17,37","17,37","17,37","27","27","27","05",""};
        string[] _77A_baktotol_szabadnapokon = new string[]
        { "77A;Baktó,Völgyérhát u.;szombat","","","27","27","27","27","27","27","27","27","27","27","27","27","27","27","27","27","27","27","27","05",""};
        string[] _77A_baktotol_munkaszuneti_napokon = new string[]
        { "77A;Baktó,Völgyérhát u.;vasárnap","","","27","27","27","27","27","27","27","27","27","27","27","27","27","27","27","27","27","27","27","05",""};
        string[] _77A_marstertol_munkanapokon = new string[]
        { "77A;Mars tér (aut. áll.);munkanapokon","","","08,58","08,58","08,58","08,58","08,58","08,58","08,58","08,58","08,58","08,58","08,58","08,58","08,58","08,58","08,58","18","08","08","08","",""};
        string[] _77A_marstertol_szabadnapokon = new string[]
        { "77A;Mars tér (aut. áll.);szombat","","","08","08","08","08","08","08","08","08","08","08","08","08","08","08","08","08","08","08","08","",""};
        string[] _77A_marstertol_munkaszuneti_napokon = new string[]
        { "77A;Mars tér (aut. áll.);vasárnap","","","08","08","08","08","08","08","08","08","08","08","08","08","08","08","08","08","08","08","08","",""};

        string[] _78_marstertol_munkanapokon = new string[]
        { "78;Mars tér (üzletsor);munkanapokon","","","","15,45","15,45","15","15","15","15","15","15","15","15","15","15","15","15","15","15","15","05,45","",""};
        string[] _78_marstertol_szabadnapokon = new string[]
        { "78;Mars tér (üzletsor);szabadnapokon","","","","15","15","15","15","15","15","15","15","15","15","15","15","15","15","15","15","15","05,45","",""};
        string[] _78_marstertol_munkaszuneti_napokon = new string[]
        { "78;Mars tér (üzletsor);vasárnap","","","25","15","15","15","15","15","15","15","15","15","15","15","15","15","15","15","15","15","45","",""};

        string[] _78A_marstertol_munkanapokon = new string[]
        { "78A;Mars tér (üzletsor);munkanapokon","","","25","","25,55","25,45","45","45","45","45","45","45","45","45","45","45","45","","","","","",""};
        string[] _78A_marstertol_szabadnapokon = new string[]
        { "78A;Mars tér (üzletsor);szombat","","","25","","","45","45","45","45","45","45","","","","","","","","","","","",""};

        string[] _79H_marstertol_iskolai_eloadas_napokon = new string[]
        { "79H;Mars tér (üzletsor);iskolai előadas napokon","","","","","35","","","","","","","","","35","","","","","","","","",""};
        string[] _79H_fehertoitol_iskolai_eloadas_napokon = new string[]
        { "79H;Fehértói halgazdaság;iskolai előadas napokon","","","","","50","","","","","","","","","","00","","","","","","","",""};

        string[] _74A_marstertol_iskolai_eloadas_napokon = new string[]
        { "74A;Mars tér (Mikszáth u.);iskolai előadas napokon","","","","","10,30,50","10,30,50","10","","","","","10,30,50","10,30,50","10,30,50","10,30,50","10","","","","","","",""};
        string[] _74A_holttiszatol_iskolai_eloadas_napokon = new string[]
        { "74A;Holt-Tisza;iskolai előadas napokon","","","","","30,50","10,30,50","10,30","","","","","25,45","05,25,45","05,25,45","05,25,45","05,25","","","","","","",""};

        string[] _84_makkoshaztol_iskolai_eloadas_napokon = new string[]
        { "84;Makkosház;iskolai előadas napokon","","","","","10,30,50","10,30,50","10,30","","","","","","","","","","","","","","","",""};
        string[] _84_makkoshaztol_szabadnapokon = new string[]
        { "84;Makkosház;szombat","","","","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00","",""};
        string[] _84_makkoshaztol_munkaszuneti_napokon = new string[]
        { "84;Makkosház;vasárnap","","","","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00,30","00","",""};
        string[] _84_makkoshaztol_tanevtartalmaalatt_munkanapokon = new string[]
        { "84;Makkosház;tanév tartalma alatt munkanapokon","","","20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00","",""};
        string[] _84_makkoshaztol_nyari_tanszunet_munkanapokon = new string[]
        { "84;Makkosház;nyári tanszünet munkana pokon","","","20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00","",""};
        string[] _84_gabonakutatotol_iskolai_eloadas_napokon = new string[]
        { "84;Újszeged, Gabonakutató;iskolai előadas napokon","","","","","30,50","10,30,50","10,30,50","","","","","","","","","","","","","","","",""};
        string[] _84_gabonakutatotol_szabadnapokon = new string[]
        { "84;Újszeged, Gabonakutató;szombat","","","","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20","",""};
        string[] _84_gabonakutatotol_munkaszuneti_napokon = new string[]
        { "84;Újszeged, Gabonakutató;vasárnap","","","","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20,50","20","",""};
        string[] _84_gabonakutatotol_tanevtartalmaalatt_munkanapokon = new string[]
        { "84;Újszeged, Gabonakutató;tanév tartalma alatt munkanapokon","","","40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20","",""};
        string[] _84_gabonakutatotol_nyari_tanszunet_munkanapokon = new string[]
        { "84;Újszeged, Gabonakutató;nyári tanszünet munkana pokon","","","40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20","",""};

        string[] _84A_tarjaniviztoronytol_iskolai_eloadas_napokon = new string[]
        { "84A;Tarján, víztorony;iskolai előadas napokon","","","","","40,50","00,10,20,30","","","","","","15,35,55","15","","","","","","","","","",""};
        string[] _84A_gabonakutatotol_iskolai_eloadas_napokon = new string[]
        { "84A;Újszeged, Gabonakutató;iskolai előadas napokon","","","","","55","05,15,25,35,45","","","","","","30,50","10,30","","","","","","","","","",""};

        string[] _90_lugasutcatol_munkanapokon = new string[]
        { "90;Lugas u.;munkanapokon","","","","25,45","05,25,45","05,25,45","05,45","25","05,45","25","05,45","20,50","20,50","20,50","20,50","20,50","20,50","25","05,45","25","05,45","",""};
        string[] _90_lugasutcatol_szabadnapokon = new string[]
        { "90;Lugas u.;szombat","","","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","",""};
        string[] _90_lugasutcatol_munkaszuneti_napokon = new string[]
        { "90;Lugas u.;vasárnap","","","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","",""};
        string[] _90_szemelyipalyaudvartol_munkanapokon = new string[]
        { "90;Személy pu.;munkanapokon","","","","05,25,45","05,25,45","05,25,45","05,25,45","05,35","15,55","35","15,55","35","05,35","05,35","05,35","05,35","05,35","05,35","15,35","35","15,35","",""};
        string[] _90_szemelyipalyaudvartol_szabadnapokon = new string[]
        { "90;Személy pu.;szombat","","","58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","",""};
        string[] _90_szemelyipalyaudvartol_munkaszuneti_napokon = new string[]
        { "90;Személy pu.;vasárnap","","","58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","28,58","",""};

        string[] _90F_petofiteleptol_munkanapokon = new string[23]
        { "90F;Petőfitelep, Fő tér;munkanapokon","","30,50","10,30,50","10,30,50","10,30,50","10,30","00,40","20","00,40","20","00,30","00,30","00,30","00,30","00,30","00,30","00,40","20","00,40","20","",""};
        string[] _90F_szemelypalyaudvartol_munkanapokon = new string[23]
        { "90F;Személy pu.;munkanapokon","","15,55","15,35,55","15,35,55","15,35,55","15,35","15,35","35","15,35","35","15,35","20,50","20,50","20,50","20,50","20,50","20,55","35","15,55","35","",""};

        string[] _90H_lugasutcatol_munkanapokon = new string[23]
        { "90H;Lugas u.;munkanapokon","","","15","","","","","","","","","","","","","","","","15","","",""};
        string[] _90H_szegedilogkozpont_munkanapokon = new string[23]
        { "90H;Szegedi Ipari Logisztikai Központ; munkanapokon","","","","15","","","","","","","","","","","","","","","","15","",""};

        string[] _91E_szechenyitol_munkanapokon = new string[24] 
        { "91E;Széchenyi tér (Kelemen u.);szombat","00","","","","","","","","","","","","","","","","","","","","","",""};
        string[] _91E_szechenyitol_szabadnapokon = new string[]
        { "91E;Széchenyi tér (Kelemen u.);vasárnap","00","","","","","","","","","","","","","","","","","","","","","",""};

        string[] _92E_szechenyitol_munkanapokon = new string[]
        { "92E;Széchenyi tér (Kelemen u.);szombat","00","","","","","","","","","","","","","","","","","","","","","",""};
        string[] _92E_szechenyitol_szabadnapokon = new string[]
        { "92E;Széchenyi tér (Kelemen u.);vasárnap","00","","","","","","","","","","","","","","","","","","","","","",""};

        string[] _93E_szechenyitol_munkanapokon = new string[]
        { "93E;Széchenyi tér (Kelemen u.);szombat","00","","","","","","","","","","","","","","","","","","","","","",""};
        string[] _93E_szechenyitol_szabadnapokon = new string[]
        { "93E;Széchenyi tér (Kelemen u.);vasárnap","00","","","","","","","","","","","","","","","","","","","","","",""};

        string[] _94E_szechenyitol_munkanapokon = new string[]
        { "94E;Széchenyi tér (Kelemen u.);szombat","00","","","","","","","","","","","","","","","","","","","","","",""};
        string[] _94E_szechenyitol_szabadnapokon = new string[]
        { "94E;Széchenyi tér (Kelemen u.);vasárnap","00","","","","","","","","","","","","","","","","","","","","","",""};

        string[] _5_kortoltestol_tanitasi_munkanap = new string[]
        { "5;Körtöltés utca;tanítási munkanapokon","","30","00,20,40","00,15,25,35,45,55","05,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,07,05,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,52","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,37,52","07,22,37,52","07,22,40","00,30",""};
        string[] _5_kortoltestol_iskolaszuneti_munkanap = new string[]
        { "5;Körtöltés utca;iskolaszüneti munkan apokon","","30","00,20,40","00,15,25,35,45,55","05,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,07,05,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,52","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,37,52","07,22,37,52","07,22,40","00,30",""};
        string[] _5_kortoltestol_szombat = new string[]
        { "5;Körtöltés utca;szombat","","20","00,20,40","00,20,40","00,20,40,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","07,22,37,52","07,22,37,52","07,22,40","00,30",""};
        string[] _5_kortoltestol_vasarnap = new string[]
        { "5;Körtöltés utca;vasárnap","","20","00,20,40","00,20,40","00,20,40,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","00,15,25,35,45,55","07,22,37,52","07,22,37,52","07,22,40","00,30",""};
        string[] _5_gyermekkorhaztol_tanitasi_munkanap = new string[]
        { "5;Újszeged, Gyermekkorház;tanítási munkanapokon","","35","20,40","00,20,35,45,55","05,15,25,35,45,55","00,10,15,25,30,40,45,55","00,10,15,25,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,15,25,30,40,45,55","00,10,15,25,30,40,45,55","00,10,15,25,30,40,45,55","00,10,15,25,30,40,45,55","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,15,30,45","00,15,30,45","00,20,50",""};
        string[] _5_gyermekkorhaztol_iskolaszuneti_munkanap = new string[]
        { "5;Újszeged, Gyermekkorház;iskolaszüneti munkan apokon","","35","20,40","00,20,35,45,55","05,15,25,35,45,55","00,10,15,25,30,40,45,55","00,10,15,25,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,15,25,30,40,45,55","00,10,15,25,30,40,45,55","00,10,15,25,30,40,45,55","00,10,15,25,30,40,45,55","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,15,30,45","00,15,30,45","00,20,50",""};
        string[] _5_gyermekkorhaztol_szombat = new string[]
        { "5;Újszeged, Gyermekkorház;szombat","","35","20,40","00,20,40","00,20,40","00,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,30,45","00,15,30,45","00,15,30,45","00,20,50",""};
        string[] _5_gyermekkorhaztol_vasarnap = new string[]
        { "5;Újszeged, Gyermekkorház;vasárnap","","35","20,40","00,20,40","00,20,40","00,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,28,38,48,58","08,18,30,45","00,15,30,45","00,15,30,45","00,20,50",""};

        string[] _7_bakaytol_tanitasi_munkanap = new string[]
        { "7;Bakay Nándor utca;tanítási munkanapokon","","","","","32,47","02,17,32,47","02,17,37","07,37","07,37","07,37","07,37","07,37","02,17,37","02,17,37","02,17,38","08,28,58","28","","","","","",""};
        string[] _7_bakaytol_iskolaszuneti_munkanap = new string[]
        { "7;Bakay Nándor utca;iskolaszüneti munka napokon","","","","","32,47","02,17,32,47","02,17,37","07,37","07,37","07,37","07,37","07,37","02,17,37","02,17,37","02,17,38","08,28,58","28","","","","","",""};
        string[] _7_gyermekkorhaztol_tanitasi_munkanap = new string[]
        { "7;Újszeged, Gyermekkorház;tanítási munkanapokon","","","","","30,50","05,20,35,50","05,20,35,55","25,55","25,55","25,55","25,55","25,55","20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,25,45","15","","","","",""};
        string[] _7_gyermekkorhaztol_iskolaszuneti_munkanap = new string[]
        { "7;Újszeged, Gyermekkorház;iskolaszüneti munkanapok on","","","","","30,50","05,20,35,50","05,20,35,55","25,55","25,55","25,55","25,55","25,55","20,35,50","05,20,35,50","05,20,35,50","05,20,35,50","05,25,45","15","","","","",""};

        string[] _8_makkoshaztol_tanitasi_munkanapokon = new string[]
        { "8;Makkosház;tanítási munkanapokon","","","20,40","00,20,40","00,20,34,44,53,57","03,07,13,17,23,27,33,37,43,47,53,57","01,09,16,24,31,39,46,54","04,14,24,34,44,54","04,14,24,34,44,54","04,14,24,34,44,54","04,14,24,34,44,54","04,14,24,34,44,54","01,09,16,24,31,39,46,54","01,09,16,24,31,39,46,54","01,09,16,24,31,39,46,54","01,09,16,24,34,44,54","04,14,24,34,44,54","04,14,24,34,44,54","04,20,40","00,20,40","00,20,40","00",""};
        string[] _8_makkoshaztol_iskolaszuneti_munkanapokon = new string[]
        { "8;Makkosház;iskolaszüneti munka napokon","","","20,40","00,20,40","00,20,40,54","04,14,24,34,44,54","04,14,24,36,48","04,14,24,36,48","04,14,24,36,48","04,14,24,36,48","04,14,24,36,48","04,14,24,36,48","00,12,24,34,44,54","04,14,24,34,44,54","04,14,24,34,44,54","04,14,24,36,48","00,12,24,38,53","08,23,38,53","08,20,40","00,20,40","00,20,40","00",""};
        string[] _8_makkoshaztol_szombat = new string[]
        { "8;Makkosház;szombat","","","20,40","00,20,40","00,20,40","00,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,40","00,20,40","00,20,40","00",""};
        string[] _8_makkoshaztol_vasarnap = new string[]
        { "8;Makkosház;vasárnap","","","20,40","00,20,40","00,20,40","00,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,38,53","08,20,40","00,20,40","00,20,40","00",""};
        string[] _8_klinikaktol_tanitasi_munkanapokon = new string[]
        { "8;Klinikák;tanítási munkanapokon","","","35,55","15,35,55","15,35,55","05,15,19,25,29,35,39,45,49,55,59","05,09,15,19,23,32,38,46,53","01,08,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,23,31,38,46,53","01,08,16,23,31,38,46,53","01,08,16,23,31,38,46,53","01,08,16,23,31,38,46,53","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,40,55","15,35,55","15,35,55","15",""};
        string[] _8_klinikaktol_iskolaszuneti_munkanapokon = new string[]
        { "8;Klinikák;iskolaszüneti munkana pokon","","","35,55","15,35,55","15,35,55","15,25,35,45,55","05,15,25,35,45,56","08,20,32,44,56","08,20,32,44,56","08,20,32,44,56","08,20,32,44,56","08,20,32,44,56","08,20,32,44,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,56","08,20,32,44,58","12,27,42,57","12,27,40,55","15,35,55","15,35,55","00",""};
        string[] _8_klinikaktol_szombat = new string[]
        { "8;Klinikák;szombat","","","35,55","15,35,55","15,35,55","15,35,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,20,35,55","15,35,55","15,35,55","00",""};
        string[] _8_klinikaktol_vasarnap = new string[]
        { "8;Klinikák;vasárnap","","","35,55","15,35,55","15,35,55","15,35,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,19,34,49","04,20,35,55","15,35,55","15,35,55","00",""};

        string[] _10_viztoronytol_tanitasi_munkanapokon = new string[]
        { "10;Tarján, Víztorony tér;tanítási munkanapokon","","","28,48","08,28,48","08,28,39,49,56","01,06,11,16,21,26,31,36,41,46,51,56","01,06,14,21,29,36,44,51,59","09,19,29,39,49,59","09,19,29,39,49,59","09,19,29,39,49,59","09,19,29,39,49,59","09,19,29,39,49,58","06,14,21,29,36,44,51,58","06,14,21,29,36,44,51,58","06,14,21,29,36,44,51,58","06,14,21,29,39,49,59","09,19,29,39,49,59","09,19,29,39,49,59","09,28,48","08,28,48","08,28,48","20",""};
        string[] _10_viztoronytol_iskolaszuneti_munkanapokon = new string[]
        { "10;Tarján, Víztorony tér;iskolaszüneti mun kanapokon","","","28,48","08,28,48","08,28,48","09,19,29,39,49,59","09,19,29,43,55","07,19,31,43,55","07,19,31,43,55","07,19,31,43,55","07,19,31,43,55","07,19,31,43,55","07,19,31,39,49,59","09,19,29,39,49,59","09,19,29,39,49,59","09,19,29,43,55","07,19,31,46","01,16,31,46","01,16,28,48","08,28,48","08,28,48","20",""};
        string[] _10_viztoronytol_szombat = new string[]
        { "10;Tarján, Víztorony tér;szombat","","","28,48","08,28,48","08,28,48","08,28,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,16,30,45","08,28,48","08,28,48","20",""};
        string[] _10_viztoronytol_vasarnap = new string[]
        { "10;Tarján, Víztorony tér;vasárnap","","","28,48","08,28,48","08,28,48","08,28,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,16,30,45","08,28,48","08,28,48","20",""};
        string[] _10_klinikatol_tanitasi_munkanapokon = new string[]
        { "10;Klinikák;tanítási munkanapokon","","","45","05,25,45","05,25,45","00,10,17,22,27,32,42,47,52,57","02,07,12,17,22,27,35,42,50,57","05,12,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,19,27,35,42,50,57","05,12,19,27,35,42,50,57","05,12,19,27,35,42,50,57","05,12,19,27,35,42,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,45","05,25,45","05,25,45","05,35",""};
        string[] _10_klinikatol_iskolaszuneti_munkanapokon = new string[]
        { "10;Klinikák;iskolaszüneti munkanap okon","","","45","05,25,45","05,25,45","05,20,30,40,50","00,10,20,30,40,50","02,14,26,38,50","02,14,26,38,50","02,14,26,38,50","02,14,26,38,50","02,14,26,38,50","02,14,26,38,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","02,14,26,38,50","05,20,35,50","05,20,35,45","05,25,45","05,25,45","05,35",""};
        string[] _10_klinikatol_szombat = new string[]
        { "10;Klinikák;szombat","","","45","05,25,45","05,25,45","05,25,45,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,45","05,27,45","05,27,45","05,35",""};
        string[] _10_klinikatol_vasarnap = new string[]
        { "10;Klinikák;vasárnap","","","45","05,25,45","05,25,45","05,25,45,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,42,57","12,27,45","05,27,45","05,27,45","05,35",""};

        string[] _9_lugasutcatol_tanitasi_munkanapokon = new string[]
        { "9;Lugas utca;tanítási munkanapokon","","","56","26,49","09,29,49","06,16,26,36,46,56","06,16,28,43,58","13,31,51","11,31,51","11,31,51","11,31,51","11,28,42,54","06,18,30,42,54","06,18,30,42,54","06,18,30,42,54","06,18,30,43,58","13,29,49","09,29,49","09,29,49","09,29,53","26","00",""};
        string[] _9_lugasutcatol_iskolaszunetii_munkanapokon = new string[]
        { "9;Lugas utca;iskolaszüneti munka napokon","","","56","26,49","09,29,49","09,28,43,58","13,28,43,58","13,31,51","11,31,51","11,31,51","11,31,51","11,31,51","11,31,51","11,31,51","11,31,51","11,31,51","11,31,51","11,31,51","11,31,51","11,29,56","26","00",""};
        string[] _9_lugasutcatol_szombat = new string[]
        { "9;Lugas utca;szombat","","","56","26,56","26,56","26,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,56","26","00",""};
        string[] _9_lugasutcatol_vasarnap = new string[]
        { "9;Lugas utca;vasárnap","","","56","26,56","26,56","26,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,49","09,29,56","26","00",""};
        string[] _9_vertotol_tanitasi_munkanapokon = new string[]
        { "9;Vértói út;tanítási munkanapokon","","","32","02,22,42","02,22,36,46,56","06,16,26,36,46","00,15,30,45","02,22,42","02,22,42","02,22,42","02,22,42","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,15,30,45","02,22,42","02,22,42","02,22,42","02,32","02,35","",""};
        string[] _9_vertotol_iskolaszunetii_munkanapokon = new string[]
        { "9;Vértói út;iskolaszüneti munkan apokon","","","32","02,22,42","02,22,42","00,15,30,45","00,15,30,45","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,32","02,35","",""};
        string[] _9_vertotol_szombat = new string[]
        { "9;Vértói út;szombat","","","32","02,32","02,32","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,32","02,35","",""};
        string[] _9_vertotol_vasarnap = new string[]
        { "9;Vértói út;vasárnap","","","32","02,32","02,32","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,22,42","02,32","02,35","",""};

        string[] _19_viztoronytol_tanitasi_munkanapokon = new string[]
        { "19;Tarján, Víztorony tér;tanítási munkanapokon","","","40","10,38,58","18,38","00,10,20,30,40,50","00,10,20,35,50","05,20,40","00,20,40","00,20,40","00,20,40","00,20,35,47,59","11,23,35,47,59","11,23,35,47,59","11,23,35,47,59","11,23,35,50","05,20,35,58","18,38,58","18,38,58","18,40","10,40","",""};
        string[] _19_viztoronytol_iskolaszunetii_munkanapokon = new string[]
        { "19;Tarján, Víztorony tér;iskolaszüneti munka napokon","","","40","10,38,58","18,38,58","18,35,50","05,20,35,50","05,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,18,40","10,40","",""};
        string[] _19_viztoronytol_szombat = new string[]
        { "19;Tarján, Víztorony tér;szombat","","","40","10,40","10,40","10,38,50","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,40","10,40","",""};
        string[] _19_viztoronytol_vasarnap = new string[]
        { "19;Tarján, Víztorony tér;vasárnap","","","40","10,40","10,40","10,38,50","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,38,58","18,40","10,40","",""};
        string[] _19_makkoshaztol_tanitasi_munkanapokon = new string[]
        { "19;Makkosház;tanítási munkanapokon","","","15,45","10,30,50","10,30,40,50","00,10,20,30,40,50","05,20,35,50","10,30,50","10,30,50","10,30,50","10,30,50","05,17,29,41,53","05,17,29,41,53","05,17,29,41,53","05,17,29,41,53","05,20,35,50","05,30,50","10,30,50","10,30,50","15,45","15","",""};
        string[] _19_makkoshaztol_iskolaszunetii_munkanapokon = new string[]
        { "19;Makkosház;iskolaszüneti munkana pokon","","","15,45","10,30,50","10,30,50","05,20,35,50","05,20,35,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","15,45","15","",""};
        string[] _19_makkoshaztol_szombat = new string[]
        { "19;Makkosház;szombat","","","15,45","15,45","15,45","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","15,45","15","",""};
        string[] _19_makkoshaztol_vasarnap = new string[]
        { "19;Makkosház;vasárnap","","","15,45","15,45","15,45","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","15,45","15","",""};

        /*Villamosok*/
        string[] _1_szemelyitol_tanitasi_munkanapokon = new string[] 
        { "1;Személy pu.;tanítási munkanap","","","","10,55","59","59","59","59","59","59","59","59","59","59","59","59","59","59","59","59","","",""};
        string[] _1_szemelyitol_iskolaszuneti_munkanapokon = new string[] 
        { "1;Személy pu.;iskolaszüneti mu nkanap","","","","10,55","59","59","59","59","59","59","59","59","59","59","59","59","59","59","59","59","","",""};
        string[] _1_szemelyitol_szombat = new string[] 
        { "1;Személy pu.;szombat","","","","10,55","59","59","59","59","59","59","59","59","59","59","59","59","59","59","59","59","","",""};
        string[] _1_szemelyitol_vasarnap = new string[] 
        { "1;Személy pu.;vasárnap","","","","10,55","59","59","59","59","59","59","59","59","59","59","59","59","59","59","59","59","","",""};
        string[] _1_plazatol_tanitasi_munkanapokon = new string[]
        { "1;Szeged Plaza;tanítási munkanap","","","50","37","37","37","37","37","37","37","37","37","37","37","37","37","37","37","37","37","","",""};
        string[] _1_plazatol_szombat = new string[]
        { "1;Szeged Plaza;szombat","","","50","37","37","37","37","37","37","37","37","37","37","37","37","37","37","37","37","37","","",""};
        string[] _1_plazatol_vasarnap = new string[]
        { "1;Szeged Plaza;vasárnap","","","50","37","37","37","37","37","37","37","37","37","37","37","37","37","37","37","37","37","","",""};

        string[] _2_palyaudvartol_tanitasi_munkanapokon = new string[] 
        { "2;Személy pu.;tanítási munkanap","","","25,45","05,10,25,45,55","00,15,25,35,45,55,59","02,10,17,25,32,38,44,50,56,59","02,08,14,20,25,32,40,47,55,59","02,10,17,25,32,40,47,55,59","02,10,17,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,32,40,47,55,59","02,10,17,25,32,40,47,55,59","02,10,17,25,32,40,47,55,59","02,10,17,25,32,40,47,55,59","02,10,17,25,32,40,47,55,59","02,10,17,25,35,45,55,59","05,15,25,35,45,55,59","10,25,40,55,59","10,25,40,55,59","10,25,45","10,25,45","05"};
        string[] _2_palyaudvartol_iskolaszuneti_munkanapokon = new string[] 
        { "2;Személy pu.;iskolaszüneti mun kanap","","","25,45","05,10,25,45,55","00,15,25,35,45,55,59","02,10,17,25,32,40,47,55,59","02,10,17,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,32,40,47,55,59","02,10,17,25,32,40,47,55,59","02,10,17,25,32,40,47,55,59","02,10,17,25,35,45,55,59","05,15,25,35,45,55,59","10,25,40,55,59","10,25,40,55,59","10,25,40,55,59","10,25,45","10,25,45","05"}; 
        string[] _2_palyaudvartol_szombat = new string[] 
        { "2;Személy pu.;szombat","","","25,45","05,10,25,45,55","05,25,45,59","05,20,35,50,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,40,55,59","10,25,40,55,59","10,25,40,55,59","10,25,45,59","05,25,45","05,25,45","05"};
        string[] _2_palyaudvartol_vasárnap = new string[] 
        { "2;Személy pu.;vasárnap","","","25,45","05,10,25,45,55","05,25,45,59","05,20,35,50,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,35,45,55,59","05,15,25,40,55,59","10,25,40,55,59","10,25,40,55,59","10,25,45,59","05,25,45","05,25,45","05"};
        string[] _2_europaligettol_tanitasi_munkanapokon = new string[]
        { "2;Európa liget;tanítási munkanapokon","","","00,20,40","00,20,35,50","00,10,20,30,37,45,52","00,06,12,18,24,30,36,42,48,54","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,10,20,30,40,50","00,10,20,30,45","00,15,30,45","00,15,30,45","00,20,40","00,20,45",""};
        string[] _2_europaligettol_iskolaszuneti_munkanapokon = new string[]
        { "2;Európa liget;iskolaszüneti munkan apokon","","","00,20,40","00,20,40","00,10,20,30,37,45,52","00,07,15,22,30,37,45,52","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,07,15,22,30,37,45,52","00,10,20,30,40,50","00,10,20,30,45","00,15,30,45","00,15,30,45","00,15,30,45","00,20,40","00,20,45",""};
        string[] _2_europaligettoll_szombat = new string[]
        { "2;Európa liget;szombat","","","00,20,40","00,20,40","00,20,40,55","10,25,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,15,30,45","00,15,30,45","00,15,30,45","00,20,40","00,20,40","00,20,45",""};
        string[] _2_europaligettol_vasárnap = new string[]
        { "2;Európa liget;vasárnap","","","00,20,40","00,20,40","00,20,40,55","10,25,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,10,20,30,40,50","00,15,30,45","00,15,30,45","00,15,30,45","00,20,40","00,20,40","00,20,45",""};

        string[] _3_tarjantol_tanitasi_munkanapokon = new string[] 
        { "3;Tarján;tanítási munkanapokon","","","40","00,20,40","00,18,42","05,25,45","06,30,54","18,42","06,30","18,42","06,30","18,30,42,54","06,18,30,42,54","06,18,30,42,54","06,18,30,42,54","06,18,30,42,54","07,22,40","00,20,40","00,20,40","00","","",""};
        string[] _3_tarjantol_iskolaszuneti_munkanapokon = new string[] 
        { "3;Tarján;iskolaszüneti munka napokon","","","40","00,20,40","00,18,42","06,30,54","18,42","06,30,54","18,42","06,30,54","18,42","06,18,30,42,54","06,18,30,42,54","06,18,30,42,54","06,18,30,42,54","06,18,30,42,54","07,22,40","00,20,40","00,20,40","00","","",""};
        string[] _3_tarjantol_szombat = new string[] 
        { "3;Tarján;szombat","","","40","00,20,40","00,20,40","00,20,40","","","","","","","","","","40","00,20,40","00,20,40","00,20,40","00","","",""};
        string[] _3_tarjantol_vasarnap = new string[] 
        { "3;Tarján;vasárnap","","","40","00,20,40","00,20,40","00,20,40","","","","","","","","","","40","00,20,40","00,20,40","00,20,40","00","","",""};
        string[] _3_vadasparktol_tanitasi_munkanapokon = new string[] 
        { "3;Vadaspark;tanítási munakapokon","","","46","06,26,46","06,26,46","00,12,32,52","12,36","00,24,48","12,36","00,24,48","12,36","00,24,48","12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","06,26,46","06,26,46","06,26","","",""};
        string[] _3_vadasparktol_iskolaszuneti_munkanapokon = new string[] 
        { "3;Vadaspark;iskolaszüneti muna kapokon","","","46","06,26,46","06,26,46","00,12,36","00,24,48","12,36","00,24,48","12,36","00,24,48","12,36","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","06,26,46","06,26,46","06,26","","",""};
        string[] _3_vadasparktol_szombat = new string[] 
        { "3;Vadaspark;szombat","","","46","06,26,46","06,26,46","06,26,46","06,26","","","","","","","","","","26,46","06,26,46","06,26,46","06,26","","",""};
        string[] _3_vadasparktol_vasarnap = new string[] 
        { "3;Vadaspark;vasárnap","","","46","06,26,46","06,26,46","06,26,46","06,26","","","","","","","","","","26,46","06,26,46","06,26,46","06,26","","",""};

        string[] _3F_tarjantol_tanitasi_munkanapokon = new string[]
        { "3F;Tarján;tanítási munkanapokon","","","","","30,54","15,35,55","18,42","06,30,54","18,42","06,30,54","18,42","06","","","","","","","","","","",""};
        string[] _3F_tarjantol_iskolaszuneti_munkanapokon = new string[]
        { "3F;Tarján;iskolaszüneti munkan apokon","","","","","30,54","18,42","06,30,54","18,42","06,30,54","18,42","06,30,54","","","","","","","","","","","",""};
        string[] _3F_tarjantol_szombat = new string[]
        { "3F;Tarján;szombat","","","","","","","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20","","","","","","",""};
        string[] _3F_tarjantol_vasarnap = new string[]
        { "3F;Tarján;vasárnap","","","","","","","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20","","","","","","",""};
        string[] _3F_fonogyaritol_tanitasi_munkanapokon = new string[] 
        { "3F;Fonógyári út;tanítási munkanapokon","","","","","","16,36,56","18,42","06,30,54","18,42","06,30,54","18,42","06,30,54","","","","","","","","","","",""};
        string[] _3F_fonogyaritol_iskolaszuneti_munkanapokon = new string[] 
        { "3F;Fonógyári út;iskolaszüneti muna kanpoko","","","","","","18,42","06,30,54","18,42","06,30,54","18,42","06,30,54","18,42","","","","","","","","","","",""};
        string[] _3F_fonogyaritol_szombat = new string[]
        { "3F;Fonógyári út;szombat","","","","","","","40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00","","","","","",""};
        string[] _3F_fonogyaritol_vasarnap = new string[]
        { "3F;Fonógyári út;vasárnap","","","","","","","40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00,20,40","00","","","","","",""};

        string[] _4_tarjantol_tanitasi_munkanapokon = new string[] 
        { "4;Tarján;tanítási munkanapokon","","","30,50","10,30,50","08,24,36,48","00,10,20,30,40,50","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,15,30,50","10,30,50","10,30,50","10,30,50","10,35","",""};
        string[] _4_tarjantol_iskolaszuneti_munkanapokon = new string[] 
        { "4;Tarján;iskolaszüneti munkan apokon","","","30,50","10,30,50","10,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,12,24,36,48","00,15,30,50","10,30,50","10,30,50","10,30,50","10,35","",""};
        string[] _4_tarjantol_szombat = new string[] 
        { "4;Tarján;szombat","","","30,50","10,30,50","10,30,50","10,30,50","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,45,55","05,15,25,35,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,35","",""};
        string[] _4_tarjantol_vasarnap = new string[]
        { "4;Tarján;vasárnap","","","30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,30,50","10,35","",""};
        string[] _4_kecskestol_tanitasi_munkanapokon = new string[]
        { "4;Kecskés;tanítási munkan apokon","","","55","15,35,55","15,36,52","04,16,26,36,46,56","06,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,42,52","15,35,55","15,35,55","15,35,55","15,35","",""};
        string[] _4_kecskestol_iskolaszuneti_munkanapokon = new string[]
        { "4;Kecskés;iskolaszüneti munkan apokon","","","55","15,35,55","15,36,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","04,16,28,40,52","15,35,55","15,35,55","15,35,55","15,35","",""};
        string[] _4_kecskestol_szombat = new string[]
        { "4;Kecskés;szombat","","","55","15,35,55","15,35,55","15,35,55","15,31,41,51","01,11,21,31,41,51","01,11,21,31,41,51","01,11,21,31,41,51","01,15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35","",""};
        string[] _4_kecskestol_vasarnap = new string[]
        { "4;Kecskés;vasárnap","","","55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35,55","15,35","",""};

        /*3. vége*/

        public List<string[]> megallok_listaja_buszonkent = new List<string[]>();
        public List<string[]> milyen_megallok_jaratoknként = new List<string[]>();
        public List<string[]> ev = new List<string[]>();

        public int ora = DateTime.Now.Hour;
        public int nap = DateTime.Now.Day;
        public int perc = DateTime.Now.Minute;
        public int honap = DateTime.Now.Month;

        public ObservableCollection<Menetrend> menetrendlist_itemsource = new ObservableCollection<Menetrend>();
        public string kedvencjarat = "";
        public string kedvencmegallo = "";
        public MainPage()
        {
            megallok_listaja_buszonkent.Add(_Auchan_nyitva_8tol);
            megallok_listaja_buszonkent.Add(_7_F_munkanap_marstertol_munkanapokon);
            megallok_listaja_buszonkent.Add(_7_F_nyari_tanszunetben_munkaszuneti_napokonSzeged_helyi_marstertol);
            megallok_listaja_buszonkent.Add(_7_F_nyari_tanszunetben_szabadnapokon_Szeged_helyi_marstertol);
            megallok_listaja_buszonkent.Add(_7_F_tanevben_munkaszuneti_napokon_Szeged_helyi_marstertol);
            megallok_listaja_buszonkent.Add(_7_F_tanevben_szabadnapokon_Szeged_helyi_marstertol);
            megallok_listaja_buszonkent.Add(_7_F_munkanap_furdotol_munkanapokon);
            megallok_listaja_buszonkent.Add(_7_F_nyari_tanszunetben_munkaszuneti_napokonSzeged_helyi_furdotol);
            megallok_listaja_buszonkent.Add(_7_F_nyari_tanszunetben_szabadnapokon_Szeged_helyi_furdotol);
            megallok_listaja_buszonkent.Add(_7_F_tanevben_munkaszuneti_napokon_Szeged_helyi_furdotol);
            megallok_listaja_buszonkent.Add(_7_F_tanevben_szabadnapokon_Szeged_helyi_furdotol);
            megallok_listaja_buszonkent.Add(_13_munkanapokon_viztoronytol);
            megallok_listaja_buszonkent.Add(_13_munkanapokon_napfényparktol);
            megallok_listaja_buszonkent.Add(_13_A_marster_bevnyitva_munkanapokon);
            megallok_listaja_buszonkent.Add(_13_A_marster_bevnyitva_szabadnapokon);
            megallok_listaja_buszonkent.Add(_13_A_marster_bevnyitva_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_13_A_bevtol_bevnyitva_munkanapokon);
            megallok_listaja_buszonkent.Add(_13_A_bevtol_bevnyitva_szabadnapokon);
            megallok_listaja_buszonkent.Add(_13_A_bevtol_bevnyitva_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_13_A_marster_bevzarva_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_13_A_bevtol_bevzarva_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_20_petofitol_munkanapokon);
            megallok_listaja_buszonkent.Add(_20_petofitol_iskolai_eloadas_napokon);
            megallok_listaja_buszonkent.Add(_20_petofitol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_20_petofitol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_20_vadkertitol_munkanapokon);
            megallok_listaja_buszonkent.Add(_20_vadkertitol_iskolai_eloadas_napokon);
            megallok_listaja_buszonkent.Add(_20_vadkertitol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_20_vadkertitol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_20_A_petofitol_tanevtartalmaalatt_munkanap);
            megallok_listaja_buszonkent.Add(_20_A_honvedtol_tanevtartalmaalatt_munkanap);
            megallok_listaja_buszonkent.Add(_21_petofitol_munkanapokon);
            megallok_listaja_buszonkent.Add(_21_petofitol_iskolai_eloadasi_napokon);
            megallok_listaja_buszonkent.Add(_21_petofitol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_21_petofitol_munaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_21_palyaudvartol_munkanapokon);
            megallok_listaja_buszonkent.Add(_21_palyaudvartol_iskolai_eloadasi_napokon);
            megallok_listaja_buszonkent.Add(_21_palyaudvartol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_21_palyaudvartol_munaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_36_honvedrol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_36_honvedrol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_36_honvedrol_nyari_tanszunetben_munkanapokon);
            megallok_listaja_buszonkent.Add(_36_honvedrol_tanev_alatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_36_kkdorozsmarol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_36_kkdorozsmarol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_36_kkdorozsmarol_nyari_tanszunetben_munkanapokon);
            megallok_listaja_buszonkent.Add(_36_kkdorozsmarol_tanev_alatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_79_auchan_nyitva_marstertol_munkanapokon);
            megallok_listaja_buszonkent.Add(_79_auchan_nyitva_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_79_auchan_nyitva_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_79_auchan_zarva_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_79_auchan_nyitva_logisztikatol_munkanapokon);
            megallok_listaja_buszonkent.Add(_79_auchan_nyitva_logisztikatol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_79_auchan_nyitva_logisztikatolmunkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_79_auchan_zarva_logisztikatol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_77_baktotol_munkanapokon);
            megallok_listaja_buszonkent.Add(_77_baktotol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_77_baktotol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_77_szemelyitol_munanapokon);
            megallok_listaja_buszonkent.Add(_77_szemelyitol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_77_szemelyitol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_77_tarjanviztoronytol_munkanapokon);
            megallok_listaja_buszonkent.Add(_77_tarjanviztoronytol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_77_tarjanviztoronytol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_60_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_60_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_60_marstertol_nyariszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_60_marstertol_tanevben_munkanapokon);
            megallok_listaja_buszonkent.Add(_60_szoregtol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_60_szoregtol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_60_szoregtol_nyariszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_60_szoregtol_tanevben_munkanapokon);
            megallok_listaja_buszonkent.Add(_60_Y_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_60_Y_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_60_Y_marstertol_nyariszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_60_Y_marstertol_tanevben_munkanapokon);
            megallok_listaja_buszonkent.Add(_60_Y_szoregtol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_60_Y_szoregtol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_60_Y_szoregtol_nyariszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_60_Y_szoregtol_tanevben_munkanapokon);
            megallok_listaja_buszonkent.Add(_70_marstertol_munkanapokon);
            megallok_listaja_buszonkent.Add(_70_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_70_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_70_fuveszkerttol_munkanapokon);
            megallok_listaja_buszonkent.Add(_70_fuveszkerttol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_70_fuveszkerttol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_71_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_71_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_71_marstertol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_71_marstertol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_71_katalinutcatol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_71_katalinutcatol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_71_katalinutcatol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_71_katalinutcatol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_72_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_72_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_72_marstertol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_72_marstertol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_72_erdelyitertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_72_erdelyitertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_72_erdelyitertol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_72_erdelyitertol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_73_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_73_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_73_marstertol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_73_marstertol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_73_taperol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_73_taperol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_73_taperol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_73_taperol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_73Y_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_73Y_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_73Y_marstertol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_73Y_marstertol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_73Y_taperol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_73Y_taperol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_73Y_taperol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_73Y_taperol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_74_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_74_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_74_marstertol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_74_marstertol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_74_gyalaretrol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_74_gyalaretrol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_74_gyalaretrol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_74_gyalaretrol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_76_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_76_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_76_marstertol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_76_marstertol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_76_szentmihalytol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_76_szentmihalytol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_76_szentmihalytol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_76_szentmihalytol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_77A_baktotol_munkanapokon);
            megallok_listaja_buszonkent.Add(_77A_baktotol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_77A_baktotol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_77A_marstertol_munkanapokon);
            megallok_listaja_buszonkent.Add(_77A_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_77A_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_78_marstertol_munkanapokon);
            megallok_listaja_buszonkent.Add(_78_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_78_marstertol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_78A_marstertol_munkanapokon);
            megallok_listaja_buszonkent.Add(_78A_marstertol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_79H_marstertol_iskolai_eloadas_napokon);
            megallok_listaja_buszonkent.Add(_79H_fehertoitol_iskolai_eloadas_napokon);
            megallok_listaja_buszonkent.Add(_74A_marstertol_iskolai_eloadas_napokon);
            megallok_listaja_buszonkent.Add(_74A_holttiszatol_iskolai_eloadas_napokon);
            megallok_listaja_buszonkent.Add(_84_makkoshaztol_iskolai_eloadas_napokon);
            megallok_listaja_buszonkent.Add(_84_makkoshaztol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_84_makkoshaztol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_84_makkoshaztol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_84_makkoshaztol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_84_gabonakutatotol_iskolai_eloadas_napokon);
            megallok_listaja_buszonkent.Add(_84_gabonakutatotol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_84_gabonakutatotol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_84_gabonakutatotol_tanevtartalmaalatt_munkanapokon);
            megallok_listaja_buszonkent.Add(_84_gabonakutatotol_nyari_tanszunet_munkanapokon);
            megallok_listaja_buszonkent.Add(_84A_tarjaniviztoronytol_iskolai_eloadas_napokon);
            megallok_listaja_buszonkent.Add(_84A_gabonakutatotol_iskolai_eloadas_napokon);
            megallok_listaja_buszonkent.Add(_90_lugasutcatol_munkanapokon);
            megallok_listaja_buszonkent.Add(_90_lugasutcatol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_90_lugasutcatol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_90_szemelyipalyaudvartol_munkanapokon);
            megallok_listaja_buszonkent.Add(_90_szemelyipalyaudvartol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_90_szemelyipalyaudvartol_munkaszuneti_napokon);
            megallok_listaja_buszonkent.Add(_90F_petofiteleptol_munkanapokon);
            megallok_listaja_buszonkent.Add(_90F_szemelypalyaudvartol_munkanapokon);
            megallok_listaja_buszonkent.Add(_90H_lugasutcatol_munkanapokon);
            megallok_listaja_buszonkent.Add(_90H_szegedilogkozpont_munkanapokon);
            megallok_listaja_buszonkent.Add(_5_kortoltestol_tanitasi_munkanap);
            megallok_listaja_buszonkent.Add(_5_kortoltestol_iskolaszuneti_munkanap);
            megallok_listaja_buszonkent.Add(_5_kortoltestol_szombat);
            megallok_listaja_buszonkent.Add(_5_kortoltestol_vasarnap);
            megallok_listaja_buszonkent.Add(_5_gyermekkorhaztol_tanitasi_munkanap);
            megallok_listaja_buszonkent.Add(_5_gyermekkorhaztol_iskolaszuneti_munkanap);
            megallok_listaja_buszonkent.Add(_5_gyermekkorhaztol_szombat);
            megallok_listaja_buszonkent.Add(_5_gyermekkorhaztol_vasarnap);
            megallok_listaja_buszonkent.Add(_7_bakaytol_tanitasi_munkanap);
            megallok_listaja_buszonkent.Add(_7_bakaytol_iskolaszuneti_munkanap);
            megallok_listaja_buszonkent.Add(_7_gyermekkorhaztol_tanitasi_munkanap);
            megallok_listaja_buszonkent.Add(_7_gyermekkorhaztol_iskolaszuneti_munkanap);
            megallok_listaja_buszonkent.Add(_8_makkoshaztol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_8_makkoshaztol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_8_makkoshaztol_szombat);
            megallok_listaja_buszonkent.Add(_8_makkoshaztol_vasarnap);
            megallok_listaja_buszonkent.Add(_8_klinikaktol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_8_klinikaktol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_8_klinikaktol_szombat);
            megallok_listaja_buszonkent.Add(_8_klinikaktol_vasarnap);
            megallok_listaja_buszonkent.Add(_10_viztoronytol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_10_viztoronytol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_10_viztoronytol_szombat);
            megallok_listaja_buszonkent.Add(_10_viztoronytol_vasarnap);
            megallok_listaja_buszonkent.Add(_10_klinikatol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_10_klinikatol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_10_klinikatol_szombat);
            megallok_listaja_buszonkent.Add(_10_klinikatol_vasarnap);
            megallok_listaja_buszonkent.Add(_9_lugasutcatol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_9_lugasutcatol_iskolaszunetii_munkanapokon);
            megallok_listaja_buszonkent.Add(_9_lugasutcatol_szombat);
            megallok_listaja_buszonkent.Add(_9_lugasutcatol_vasarnap);
            megallok_listaja_buszonkent.Add(_9_vertotol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_9_vertotol_iskolaszunetii_munkanapokon);
            megallok_listaja_buszonkent.Add(_9_vertotol_szombat);
            megallok_listaja_buszonkent.Add(_9_vertotol_vasarnap);
            megallok_listaja_buszonkent.Add(_19_viztoronytol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_19_viztoronytol_iskolaszunetii_munkanapokon);
            megallok_listaja_buszonkent.Add(_19_viztoronytol_szombat);
            megallok_listaja_buszonkent.Add(_19_viztoronytol_vasarnap);
            megallok_listaja_buszonkent.Add(_19_makkoshaztol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_19_makkoshaztol_iskolaszunetii_munkanapokon);
            megallok_listaja_buszonkent.Add(_19_makkoshaztol_szombat);
            megallok_listaja_buszonkent.Add(_19_makkoshaztol_vasarnap);
            megallok_listaja_buszonkent.Add(_1_szemelyitol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_1_szemelyitol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_1_szemelyitol_szombat);
            megallok_listaja_buszonkent.Add(_1_szemelyitol_vasarnap);
            megallok_listaja_buszonkent.Add(_1_plazatol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_1_plazatol_szombat);
            megallok_listaja_buszonkent.Add(_1_plazatol_vasarnap);
            megallok_listaja_buszonkent.Add(_2_palyaudvartol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_2_palyaudvartol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_2_palyaudvartol_szombat);
            megallok_listaja_buszonkent.Add(_2_palyaudvartol_vasárnap);
            megallok_listaja_buszonkent.Add(_2_europaligettol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_2_europaligettol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_2_europaligettoll_szombat);
            megallok_listaja_buszonkent.Add(_2_europaligettol_vasárnap);
            megallok_listaja_buszonkent.Add(_3_tarjantol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_3_tarjantol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_3_tarjantol_szombat);
            megallok_listaja_buszonkent.Add(_3_tarjantol_vasarnap);
            megallok_listaja_buszonkent.Add(_3_vadasparktol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_3_vadasparktol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_3_vadasparktol_szombat);
            megallok_listaja_buszonkent.Add(_3_vadasparktol_vasarnap);
            megallok_listaja_buszonkent.Add(_3F_tarjantol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_3F_tarjantol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_3F_tarjantol_szombat);
            megallok_listaja_buszonkent.Add(_3F_tarjantol_vasarnap);
            megallok_listaja_buszonkent.Add(_3F_fonogyaritol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_3F_fonogyaritol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_3F_fonogyaritol_szombat);
            megallok_listaja_buszonkent.Add(_3F_fonogyaritol_vasarnap);
            megallok_listaja_buszonkent.Add(_4_tarjantol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_4_tarjantol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_4_tarjantol_szombat);
            megallok_listaja_buszonkent.Add(_4_tarjantol_vasarnap);
            megallok_listaja_buszonkent.Add(_4_kecskestol_tanitasi_munkanapokon);
            megallok_listaja_buszonkent.Add(_4_kecskestol_iskolaszuneti_munkanapokon);
            megallok_listaja_buszonkent.Add(_4_kecskestol_szombat);
            megallok_listaja_buszonkent.Add(_4_kecskestol_vasarnap); 
            megallok_listaja_buszonkent.Add(_91E_szechenyitol_munkanapokon);
            megallok_listaja_buszonkent.Add(_91E_szechenyitol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_92E_szechenyitol_munkanapokon);
            megallok_listaja_buszonkent.Add(_92E_szechenyitol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_93E_szechenyitol_munkanapokon);
            megallok_listaja_buszonkent.Add(_93E_szechenyitol_szabadnapokon);
            megallok_listaja_buszonkent.Add(_94E_szechenyitol_munkanapokon);
            megallok_listaja_buszonkent.Add(_94E_szechenyitol_szabadnapokon);

            milyen_megallok_jaratoknként.Add(auchan_jarat);
            milyen_megallok_jaratoknként.Add(het_f_marstertol);
            milyen_megallok_jaratoknként.Add(het_f_kiskundorozsma_furdotol);
            milyen_megallok_jaratoknként.Add(tizenharom_viztoronytol);
            milyen_megallok_jaratoknként.Add(tizenharom_napfenyparktol);
            milyen_megallok_jaratoknként.Add(tizenharom_a_marster_bevnyitva);
            milyen_megallok_jaratoknként.Add(tizenharom_a_marster_bevzarva);
            milyen_megallok_jaratoknként.Add(tizenharom_a_bevtol_bevnyitva);
            milyen_megallok_jaratoknként.Add(tizenharom_a_bevtol_bevzarva);
            milyen_megallok_jaratoknként.Add(huszas_petofitol);
            milyen_megallok_jaratoknként.Add(huszas_vadkertitol);
            milyen_megallok_jaratoknként.Add(husz_a_petofitol);
            milyen_megallok_jaratoknként.Add(husz_a_honvedtol);
            milyen_megallok_jaratoknként.Add(huszonegy_petofitol);
            milyen_megallok_jaratoknként.Add(huszonegy_palyaudvartol);
            milyen_megallok_jaratoknként.Add(harminchat_honvedtol);
            milyen_megallok_jaratoknként.Add(harminchat_kkdorozsmatol);
            milyen_megallok_jaratoknként.Add(hetvenhet_baktotol);
            milyen_megallok_jaratoknként.Add(hetvenhet_szemelyitol);
            milyen_megallok_jaratoknként.Add(hetvenhet_tarjaniviztoronytol);
            milyen_megallok_jaratoknként.Add(hetvenkilenc_auchan_nyitva_marstertol);
            milyen_megallok_jaratoknként.Add(hetvenkilenc_auchan_zarva_marstertol);
            milyen_megallok_jaratoknként.Add(hetvenkilenc_auchan_nyitva_logisztikatol);
            milyen_megallok_jaratoknként.Add(hetvenkilenc_auchan_zarva_logisztikatol);
            milyen_megallok_jaratoknként.Add(hatvan_marstertol);
            milyen_megallok_jaratoknként.Add(hatvan_szoregrol);
            milyen_megallok_jaratoknként.Add(hatvan_y_marstertol);
            milyen_megallok_jaratoknként.Add(hatvan_y_szoregtol);
            milyen_megallok_jaratoknként.Add(hetven_marstertol);
            milyen_megallok_jaratoknként.Add(hetven_fuveszkert);
            milyen_megallok_jaratoknként.Add(hetvenegy_marstertol);
            milyen_megallok_jaratoknként.Add(hetvenegy_katalinutcatol);
            milyen_megallok_jaratoknként.Add(hetvenketto_marstertol);
            milyen_megallok_jaratoknként.Add(hetvenketto_erdelyitertol);
            milyen_megallok_jaratoknként.Add(hetvenharom_marstertol);
            milyen_megallok_jaratoknként.Add(hetvenharom_taperol);
            milyen_megallok_jaratoknként.Add(hetvenharom_y_marstertol);
            milyen_megallok_jaratoknként.Add(hetvenharom_y_taperol);
            milyen_megallok_jaratoknként.Add(hetvennegy_marstertol);
            milyen_megallok_jaratoknként.Add(hetvennegy_gyalaret);
            milyen_megallok_jaratoknként.Add(hetvenhat_marstertol);
            milyen_megallok_jaratoknként.Add(hetvenhat_szentmihaly);
            milyen_megallok_jaratoknként.Add(hetvenhet_a_baktotol);
            milyen_megallok_jaratoknként.Add(hetvenhet_a_marstertol);
            milyen_megallok_jaratoknként.Add(hetvennyolc_marstertol);
            milyen_megallok_jaratoknként.Add(hetvennyolc_a_marstertol);
            milyen_megallok_jaratoknként.Add(hetvenkilenc_h_marstertol);
            milyen_megallok_jaratoknként.Add(hetvenkilenc_h_fehertoitol);
            milyen_megallok_jaratoknként.Add(hetvennegy_a_marstertol);
            milyen_megallok_jaratoknként.Add(hetvennegy_a_holtiszatol);
            milyen_megallok_jaratoknként.Add(nyolcvannegy_makoshaztol);
            milyen_megallok_jaratoknként.Add(nyolcvannegy_gabonakutatotol);
            milyen_megallok_jaratoknként.Add(nyolcvannegy_a_tarjanviztoronytol);
            milyen_megallok_jaratoknként.Add(nyolcvannegy_a_gabonakutatotol);
            milyen_megallok_jaratoknként.Add(kilencven_lugasutcatol);
            milyen_megallok_jaratoknként.Add(kilencven_szemelypalyaudvartol);
            milyen_megallok_jaratoknként.Add(kilencven_f_petofiteleptol);
            milyen_megallok_jaratoknként.Add(kilencven_f_szemelypalyaudvartol);
            milyen_megallok_jaratoknként.Add(kilencven_h_lugautcatol);
            milyen_megallok_jaratoknként.Add(kilencven_h_szlogisztikatol); 
            milyen_megallok_jaratoknként.Add(kilencvenegye_szechenyitol);
            milyen_megallok_jaratoknként.Add(kilencvenkettoe_szechenyitol);
            milyen_megallok_jaratoknként.Add(kilencvenharome_szechenyitol);
            milyen_megallok_jaratoknként.Add(kilencvennegye_szechenyitol);
            milyen_megallok_jaratoknként.Add(otostroli_kortoltestol);
            milyen_megallok_jaratoknként.Add(otostroli_gyermekkorhaztol);
            milyen_megallok_jaratoknként.Add(hetestroli_bakaytol);
            milyen_megallok_jaratoknként.Add(hetestroli_gyermekkorhaztol);
            milyen_megallok_jaratoknként.Add(nyolcastol_makkoshaztol);
            milyen_megallok_jaratoknként.Add(nyolcastol_klinikaktol);
            milyen_megallok_jaratoknként.Add(tizes_viztoronytol);
            milyen_megallok_jaratoknként.Add(tizes_klinikaktol);
            milyen_megallok_jaratoknként.Add(kilences_lugasutcatol);
            milyen_megallok_jaratoknként.Add(kilences_vertotol);
            milyen_megallok_jaratoknként.Add(tizenkilences_viztoronytol);
            milyen_megallok_jaratoknként.Add(tizenkilences_makkoshaztol);
            milyen_megallok_jaratoknként.Add(egyes_szemelyitol);
            milyen_megallok_jaratoknként.Add(egyes_plazatol);
            milyen_megallok_jaratoknként.Add(kettes_szemelyitol);
            milyen_megallok_jaratoknként.Add(kettes_europaligettol);
            milyen_megallok_jaratoknként.Add(negyes_tarjantol);
            milyen_megallok_jaratoknként.Add(negyes_kecskestol);
            milyen_megallok_jaratoknként.Add(haromas_tarjantol);
            milyen_megallok_jaratoknként.Add(haromas_vadasparktol);
            milyen_megallok_jaratoknként.Add(haromfes_tarjantol);
            milyen_megallok_jaratoknként.Add(haromfes_fonogyaritol);

            this.InitializeComponent();
            kedvencekleiras.Visibility = Visibility.Visible;
            kedvencekcrollvieer.IsEnabled = true; 

            ev.Add(munkanapok);
            ev.Add(szabadnapok_szombat);
            ev.Add(munkaszuneti_vasarnap);
            ev.Add(tanszunet);
            ev.Add(oktatasi_szunet);
            ev.Add(szombat);
            ev.Add(vasarnap);

            lekerdez_elhagytam_dbt();

            menetrendlist.ItemsSource = menetrendmenu(milyen_kozlekedesi_eszkozok_vannak, milyen_megallohelyek_vannak, megallok_listaja_buszonkent, milyen_megallok_jaratoknként);
            
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();

            nemkellstatusbar();
        }
        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "sample.sqlite"));

        public SQLiteConnection dbConn;

        public DispatcherTimer dispatcherTimer = new DispatcherTimer();

        Kedvencek kedv;

        private void insertUser(string j, string m)
        {
            dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), DB_PATH);
            dbConn.CreateTable<Kedvencek>();
            Kedvencek kedv = new Kedvencek()
            {
                Kedvenc_jarat = j,
                Kedvenc_megallo = m
            };

            dbConn.Insert(kedv);
        }

        private List<Kedvencek> getUser()
        {
            List<Kedvencek> list;
            dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), DB_PATH);
            dbConn.CreateTable<Kedvencek>();
            list = dbConn.Query<Kedvencek>("select * from kedvencek");
            if (kedv != null)
            {
                return list;
            }
            else
            {
                return list;
            }

        }

        private async void DispatcherTimer_Tick(object sender, object e)
        {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            int ora2 = DateTime.Now.Hour;
            int perc2 = DateTime.Now.Minute;
            int honap2 = DateTime.Now.Month;
            int nap2 = DateTime.Now.Day;
            if (ora != ora2 || perc != perc2 || honap != honap2 || nap != nap2)
            {
                dispatcherTimer.Stop();
                ora = ora2;
                perc = perc2;
                honap = honap2;
                nap = nap2;
                menetrendlist.ItemsSource = null;
                menetrendlist.ItemsSource = menetrendlist.ItemsSource = menetrendmenu(milyen_kozlekedesi_eszkozok_vannak, milyen_megallohelyek_vannak, megallok_listaja_buszonkent, milyen_megallok_jaratoknként);
                dispatcherTimer.Start();
            }
        }


        async void nemkellstatusbar()
        {
            try
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }
            catch { }
        }

        private void hamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            menuSplitPane.IsPaneOpen = !menuSplitPane.IsPaneOpen;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (kedvencek.IsSelected == true)
            {
                kedvencekleiras.Visibility = Visibility.Visible;
                kedvencekcrollvieer.IsEnabled = true;
                jaratokcrollvieer.IsEnabled = false;
                megallokcrollvieer.IsEnabled = false;
                elhagytamlistacrollvieer.IsEnabled = false;
                leirasleiras.Visibility = Visibility.Collapsed;
                menetrendleiras.Visibility = Visibility.Collapsed;
                elhagytamleiras.Visibility = Visibility.Collapsed;
                elerhetosegleiras.Visibility = Visibility.Collapsed;
                megallok_szerinti_listazas.Visibility = Visibility.Collapsed;
            }
            else if (leiras.IsSelected == true)
            {
                kedvencekcrollvieer.IsEnabled = false;
                jaratokcrollvieer.IsEnabled = false;
                megallokcrollvieer.IsEnabled = false;
                elhagytamlistacrollvieer.IsEnabled = false;
                kedvencekleiras.Visibility = Visibility.Collapsed;
                leirasleiras.Visibility = Visibility.Visible;
                menetrendleiras.Visibility = Visibility.Collapsed;
                elhagytamleiras.Visibility = Visibility.Collapsed;
                elerhetosegleiras.Visibility = Visibility.Collapsed;
                megallok_szerinti_listazas.Visibility = Visibility.Collapsed;
            }
            else if (menetrend.IsSelected == true)
            {
                kedvencekcrollvieer.IsEnabled = false;
                jaratokcrollvieer.IsEnabled = true;
                megallokcrollvieer.IsEnabled = false;
                elhagytamlistacrollvieer.IsEnabled = false;
                kedvencekleiras.Visibility = Visibility.Collapsed;
                leirasleiras.Visibility = Visibility.Collapsed;
                menetrendleiras.Visibility = Visibility.Visible;
                elhagytamleiras.Visibility = Visibility.Collapsed;
                elerhetosegleiras.Visibility = Visibility.Collapsed;
                megallok_szerinti_listazas.Visibility = Visibility.Collapsed;
            }
            else if (elhagytam.IsSelected == true)
            {
                kedvencekcrollvieer.IsEnabled = false;
                jaratokcrollvieer.IsEnabled = false;
                megallokcrollvieer.IsEnabled = false;
                elhagytamlistacrollvieer.IsEnabled = true;
                kedvencekleiras.Visibility = Visibility.Collapsed;
                leirasleiras.Visibility = Visibility.Collapsed;
                menetrendleiras.Visibility = Visibility.Collapsed;
                elhagytamleiras.Visibility = Visibility.Visible;
                elerhetosegleiras.Visibility = Visibility.Collapsed;
                megallok_szerinti_listazas.Visibility = Visibility.Collapsed;
            }
            else if (megallokszerint.IsSelected == true)
            {
                kedvencekcrollvieer.IsEnabled = false;
                jaratokcrollvieer.IsEnabled = false;
                elhagytamlistacrollvieer.IsEnabled = false;
                megallokcrollvieer.IsEnabled = true;
                kedvencekleiras.Visibility = Visibility.Collapsed;
                leirasleiras.Visibility = Visibility.Collapsed;
                menetrendleiras.Visibility = Visibility.Collapsed;
                elhagytamleiras.Visibility = Visibility.Collapsed;
                elerhetosegleiras.Visibility = Visibility.Collapsed;
                megallok_szerinti_listazas.Visibility = Visibility.Visible;
            }
            else
            {
                kedvencekcrollvieer.IsEnabled = false;
                jaratokcrollvieer.IsEnabled = false;
                elhagytamlistacrollvieer.IsEnabled = false;
                megallokcrollvieer.IsEnabled = false;
                kedvencekleiras.Visibility = Visibility.Collapsed;
                leirasleiras.Visibility = Visibility.Collapsed;
                megallok_szerinti_listazas.Visibility = Visibility.Collapsed;
                menetrendleiras.Visibility = Visibility.Collapsed;
                elhagytamleiras.Visibility = Visibility.Collapsed;
                elerhetosegleiras.Visibility = Visibility.Visible;
            }
        }

        public string milyennap = "";

        ObservableCollection<Menetrend> menetrendmenu(string[] milyen_jarmuvek_vannak, string[] milyen_megallohelyek_vannak, List<string[]> megallok_erintve, List<string[]> milyen_megallok_jaratoknként)
        {
            ObservableCollection<Menetrend> menetrend2 = new ObservableCollection<Menetrend>();
            Menetrend men = new Menetrend();
            
            for (int i = 0; i < ev.Count; i++)/*Megnézzük hogy ezen a napon milyen nap van: munkaszüneti, szabadnap, ilyesmi*/
            {
                for (int k = 1; k < ev[i].Length; k++)
                {
                    if (nap == Convert.ToInt32(ev[i][k].Split(':')[1]) && honap == Convert.ToInt32(ev[i][k].Split(':')[0]))
                    {
                        milyennap = ev[i][0];
                    }
                }
            }
            
            for (int i = 0; i < milyen_jarmuvek_vannak.Length;i++)
            {
                for (int j =1;j< megallok_listaja_buszonkent.Count;j++)
                {
                    string maga_a_busz = megallok_listaja_buszonkent[j][0].Split(';')[0];
                    string indulas = megallok_listaja_buszonkent[j][0].Split(';')[1];
                    string busznap = megallok_listaja_buszonkent[j][0].Split(';')[2];
                    if (busznap.Contains(milyennap))
                    {
                        for (int k =1; k < megallok_listaja_buszonkent[j].Length;k++)
                        {
                            if (megallok_listaja_buszonkent[j][k] != "" && ora <= k+1)
                            {
                                string veglegesmikor = "";

                                bool nezi_kilephet_e = false;

                                if (megallok_listaja_buszonkent[j][k].Length == 2)
                                {
                                    int tomb = Convert.ToInt32(megallok_listaja_buszonkent[j][k]);
                                    if (tomb > perc && ora == k+ 1)
                                    {
                                        nezi_kilephet_e = true;
                                        veglegesmikor = (tomb-perc) +" perc";
                                    }
                                    else if (ora < k + 1)
                                    {
                                        nezi_kilephet_e = true;
                                        int orabusz = (k + 1) * 60 + tomb;
                                        int oraaktual = (ora * 60) + perc;
                                        int orakul = orabusz - oraaktual;
                                        if ((orakul / 60) == 0)
                                        {
                                            veglegesmikor = (orakul - (orakul / 60) * 60) + " perc";
                                        }
                                        else
                                        {
                                            veglegesmikor = (orakul / 60) + " óra " + (orakul - (orakul / 60) * 60) + " perc";
                                        }
                                        
                                    }
                                }
                                else
                                {
                                    string[] tombb = megallok_listaja_buszonkent[j][k].Split(',');
                                    for (int z = 0; z < tombb.Length;z++)
                                    {
                                        int tomb = Convert.ToInt32(tombb[z]);
                                        if (tomb > perc && ora == k + 1)
                                        {
                                            nezi_kilephet_e = true;
                                            veglegesmikor = (tomb - perc) + " perc";
                                            z = tombb.Length;
                                        }
                                        else if (ora < k + 1)
                                        {
                                            nezi_kilephet_e = true;
                                            int orabusz = (k + 1) * 60 + tomb;
                                            int oraaktual = (ora * 60) + perc;
                                            int orakul = orabusz - oraaktual;
                                            if ((orakul / 60) == 0)
                                            {
                                                veglegesmikor = (orakul - (orakul / 60) * 60) + " perc";
                                            }
                                            else
                                            {
                                                veglegesmikor = (orakul / 60) + " óra " + (orakul - (orakul / 60) * 60) + " perc";
                                            }
                                            z = tombb.Length;
                                        }
                                    }

                                }
                                if (nezi_kilephet_e)
                                {
                                    Menetrend m = new Menetrend();
                                    m.Hanyasjarat = maga_a_busz;
                                    m.Honnanindul = indulas;
                                    m.Mikorindul = veglegesmikor;
                                    menetrend2.Add(m);
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            }
            menetrend_szerint_list.ItemsSource = megallok_szerint(menetrend2);
            return menetrend2;
        }
        string ered = "";
        ObservableCollection<Megallok> megallok_szerint(ObservableCollection<Menetrend> menetrend_coll)
        {
            ObservableCollection<Megallok> megallok_coll = new ObservableCollection<Megallok>();
            Megallok megall_class;
            
            string ered = "";
            bool bemente = false;

            for (int i =0; i < milyen_megallohelyek_vannak.Length;i++)
            {
                for (int k = 1; k <milyen_megallok_jaratoknként.Count; k++)
                {
                    string namostmarlegyenjojarat = milyen_megallok_jaratoknként[k][0].Split(';')[0];
                    string namostmarlegyenjohonnanindul = milyen_megallok_jaratoknként[k][1];
                    string[] namostmarlegyenjohanypercalatt = milyen_megallok_jaratoknként[k][0].Split(';')[1].Split(',');
                    for (int j = 1; j < milyen_megallok_jaratoknként[k].Length; j++)
                    {
                        if (milyen_megallok_jaratoknként[k][j] == milyen_megallohelyek_vannak[i])
                        {
                            for (int l = 1; l < megallok_listaja_buszonkent.Count; l++)
                            {
                                if (megallok_listaja_buszonkent[l][0].Split(';')[1] == namostmarlegyenjohonnanindul && megallok_listaja_buszonkent[l][0].Split(';')[2].Contains(milyennap) && megallok_listaja_buszonkent[l][0].Split(';')[0] == namostmarlegyenjojarat)
                                {
                                    for (int p = 1; p < megallok_listaja_buszonkent[l].Length; p++)
                                    {
                                        bemente = false;
                                        if (megallok_listaja_buszonkent[l][p].Length == 2)
                                        {
                                            string tomb = megallok_listaja_buszonkent[l][p];
                                            int ertek = Convert.ToInt32(tomb);
                                            int pluszperc = Convert.ToInt32(namostmarlegyenjohanypercalatt[j-1]);
                                            int rendesorapercben = ora * 60 + perc;
                                            int buszaktualisorapercben = (p + 1) * 60 + ertek + pluszperc;
                                            int kulonbseg = rendesorapercben- buszaktualisorapercben;
                                            if (kulonbseg<=0)
                                            {
                                                bemente = true;
                                                kulonbseg = Math.Abs(kulonbseg);
                                                if (kulonbseg / 60 == 0)
                                                {
                                                    ered += namostmarlegyenjojarat + " " + " Innen: " + namostmarlegyenjohonnanindul + " : " + (kulonbseg - (kulonbseg / 60) * 60) + " perc \n";
                                                }
                                                else
                                                {
                                                    ered += namostmarlegyenjojarat + " " + " Innen: " + namostmarlegyenjohonnanindul + " : " + kulonbseg / 60 + " óra " + (kulonbseg - (kulonbseg / 60) * 60) + " perc \n";
                                                }
                                                break;
                                            }
                                        }
                                        else if(megallok_listaja_buszonkent[l][p].Length > 2)
                                        {
                                            string[] tomb = megallok_listaja_buszonkent[l][p].Split(',');
                                            int ertek = 0;
                                            for (int asd = 0; asd < tomb.Length; asd++)
                                            {
                                                ertek = Convert.ToInt32(tomb[asd]);
                                                int rendesorapercben = ora * 60 + perc;
                                                int pluszperc = Convert.ToInt32(namostmarlegyenjohanypercalatt[j-1]);
                                                int buszaktualisorapercben = (p + 1) * 60 + ertek + pluszperc;
                                                int kulonbseg = rendesorapercben - buszaktualisorapercben;
                                                if (kulonbseg <= 0)
                                                {
                                                    bemente = true;
                                                    kulonbseg = Math.Abs(kulonbseg);
                                                    if (kulonbseg / 60 == 0)
                                                    {
                                                        ered += namostmarlegyenjojarat + " " + " Innen: " + namostmarlegyenjohonnanindul + " : " + (kulonbseg - (kulonbseg / 60) * 60) + " perc \n";
                                                    }
                                                    else
                                                    {
                                                        ered += namostmarlegyenjojarat + " " + " Innen: " + namostmarlegyenjohonnanindul+ " : " + kulonbseg / 60 + " óra " + (kulonbseg - (kulonbseg / 60) * 60) + " perc \n";
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        if (bemente == true)
                                        {
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                megall_class = new Megallok();
                megall_class.Megallo = milyen_megallohelyek_vannak[i];
                megall_class.Jaratok_mikor = ered;
                megallok_coll.Add(megall_class);
                ered = "";
            }
            string[] auchanmegallo = megallok_listaja_buszonkent[0];
            string[] auchanmikor = milyen_megallok_jaratoknként[0];
            bool bementodaahovaegyszerkell = false;
            if (auchanmegallo[0].Split(';')[2].Contains(milyennap))
            {
                for (int i = 0; i < milyen_megallohelyek_vannak.Length; i++)
                {
                    bementodaahovaegyszerkell = false;
                    if (milyen_megallohelyek_vannak[i] == "Szamos u." && ora <= 7)
                    {
                        int oraauchanpercben = 7 * 60 + 43;
                        int eredetiora = ora * 60 + perc;
                        int kul = eredetiora - oraauchanpercben;
                        if (kul <= 0 && kul / 60 != 0)
                        {
                            bementodaahovaegyszerkell = true;
                            kul = Math.Abs(kul);
                            ered = "Auchan járat " + Math.Abs(kul / 60) + " óra " + (kul - ((kul / 60) * 60)) + " perc " + "Innen: Szamos utca" + '\n';
                            for (int o = 0; o < megallok_coll.Count; o++)
                            {
                                if (megallok_coll[o].Megallo == "Szamos u.")
                                {
                                    megallok_coll[o].Jaratok_mikor += (ered);
                                    break;
                                }
                            }
                        }
                        else if (kul <= 0 && kul / 60 == 0)
                        {
                            bementodaahovaegyszerkell = true;
                            kul = Math.Abs(kul);
                            ered = "Auchan járat " + (kul - ((kul / 60) * 60)) + " perc " + "Innen: Szamos utca" + '\n';
                            for (int o = 0; o < megallok_coll.Count; o++)
                            {
                                if (megallok_coll[o].Megallo == "Szamos u.")
                                {
                                    megallok_coll[o].Jaratok_mikor += (ered);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int k = 1; k < auchanmikor.Length; k++)
                        {
                            if (auchanmikor[k].ToLower() == milyen_megallohelyek_vannak[i].ToLower() && ora > 7)
                            {
                                for (int p = 1; p < auchanmegallo.Length; p++)
                                {
                                    string asdasd = auchanmikor[0].Split(';')[1];
                                    int asdas = Convert.ToInt32(asdasd.Split(',')[k - 1]);
                                    int oraauchanpercben = (p + 7) * 60 + Convert.ToInt32(auchanmegallo[p]) + asdas;
                                    int eredetiora = ora * 60 + perc;
                                    int kul = eredetiora - oraauchanpercben;
                                    if (kul <= 0 && kul / 60 != 0)
                                    {
                                        kul = Math.Abs(kul);
                                        ered = "Auchan járat " + kul / 60 + " óra " + (kul - ((kul / 60) * 60)) + " perc " + "Innen: " + auchanmikor[k] + '\n';
                                        for (int o = 0; o < megallok_coll.Count; o++)
                                        {
                                            if (megallok_coll[o].Megallo.ToLower() == auchanmikor[k].ToLower())
                                            {
                                                megallok_coll[o].Jaratok_mikor += ( ered);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                    else if (kul <= 0 && kul / 60 == 0)
                                    {
                                        kul = Math.Abs(kul);
                                        ered = "Auchan járat " + (kul - ((kul / 60) * 60)) + " perc " + "Innen: " + auchanmikor[k] + '\n';
                                        for (int o = 0; o < megallok_coll.Count; o++)
                                        {
                                            if (megallok_coll[o].Megallo.ToLower() == auchanmikor[k].ToLower())
                                            {
                                                megallok_coll[o].Jaratok_mikor += (ered);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (bementodaahovaegyszerkell == true)
                    {
                        break;
                    }
                }
            }
            foreach (var item in megallok_coll)
            {
                if (item.Megallo.ToLower() == "Auchan áruház".ToLower() && item.Jaratok_mikor.Contains("Auchan járat"))
                {
                    Menetrend m = new Menetrend();
                    m.Hanyasjarat = "Auchan járat";
                    m.Honnanindul = "Auchan áruház";
                    string elsomikorindul = item.Jaratok_mikor.Substring(item.Jaratok_mikor.IndexOf("Auchan járat") + "Auchan járat".Length + 1);
                    string igazielsomikorindul = elsomikorindul.Remove(elsomikorindul.IndexOf(" Innen:"))+"\n";
                    if (ora != 19)
                    {
                        string harmadikmikorindul = elsomikorindul.Substring(elsomikorindul.IndexOf("Auchan járat") + "Auchan járat".Length + 1);
                        string igaziharmadikmikorindul = harmadikmikorindul.Remove(harmadikmikorindul.IndexOf(" Innen:"));
                        m.Mikorindul = igazielsomikorindul + igaziharmadikmikorindul;
                    }
                    else if (ora == 19 && perc < 26)
                    {
                        string harmadikmikorindul = elsomikorindul.Substring(elsomikorindul.IndexOf("Auchan járat") + "Auchan járat".Length + 1);
                        string igaziharmadikmikorindul = harmadikmikorindul.Remove(harmadikmikorindul.IndexOf(" Innen:"));
                        m.Mikorindul = igazielsomikorindul + igaziharmadikmikorindul;
                    }
                    else
                    {
                        m.Mikorindul = igazielsomikorindul;
                    }
                    menetrend_coll.Add(m);
                    menetrend_szerint_list.ItemsSource = null;
                    UpdateLayout();
                    menetrend_szerint_list.ItemsSource = menetrend_coll;
                }
            }
            return megallok_coll;
        }

        private void kedvencekjaratokcombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int k = kedvencekjaratokcombobox.SelectedIndex;
            if (k != 0)
            {
                kedvencjarat = milyen_kozlekedesi_eszkozok_vannak[k - 1];
            }
            else kedvencjarat = "";
        }

        private void kedvencekmegallokcombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int k = kedvencekmegallokcombobox.SelectedIndex;
            if (k != 0)
            {
                kedvencmegallo = milyen_megallohelyek_vannak[k - 1];
            }
            else kedvencmegallo = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((kedvencjarat=="" && kedvencmegallo != "") ||(kedvencjarat != "" && kedvencmegallo == ""))
            {
                badcontent.Text = "Sikeres Felvétel!";
                insertUser(kedvencjarat, kedvencmegallo);
            }
            else
            {
                badcontent.Text = "Csakegy üreset adj meg!";
            }
        }

        private ObservableCollection<Menetrend> menetrendkedvencekrendezve_fv(List<Kedvencek> k)
        {
            ObservableCollection<Menetrend> m = menetrendmenu(milyen_kozlekedesi_eszkozok_vannak, milyen_megallohelyek_vannak, megallok_listaja_buszonkent, milyen_megallok_jaratoknként);
            ObservableCollection<Menetrend> mm = new ObservableCollection<Menetrend>();
            Menetrend me;
            foreach (var item in m)
            {
                foreach (var itemkedvenc in k)
                {
                    if (item.Hanyasjarat == itemkedvenc.Kedvenc_jarat)
                    {
                        me = new Menetrend();
                        me.Hanyasjarat = item.Hanyasjarat;
                        me.Honnanindul = item.Honnanindul;
                        me.Mikorindul = item.Mikorindul;
                        mm.Add(me);
                    }
                }
            }

            return mm;
        }

        private ObservableCollection<Megallok> jaratokkedvencekrendezve_fv(List<Kedvencek> k)
        {
            ObservableCollection<Megallok> m = megallok_szerint(menetrendmenu(milyen_kozlekedesi_eszkozok_vannak, milyen_megallohelyek_vannak, megallok_listaja_buszonkent, milyen_megallok_jaratoknként));
            ObservableCollection<Megallok> mm = new ObservableCollection<Megallok>();
            Megallok me;
            foreach (var item in m)
            {
                foreach (var itemkedvenc in k)
                {
                    if (item.Megallo == itemkedvenc.Kedvenc_megallo)
                    {
                        me = new Megallok();
                        me.Megallo = item.Megallo;
                        me.Jaratok_mikor = item.Jaratok_mikor;
                        mm.Add(me);
                    }
                }
            }

            return mm;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int k = kedvencval.SelectedIndex;
            if (k==0)
            {
                ha_a_menetrend_szerint.Visibility = Visibility.Visible;
                ha_a_jaratok_szerint.Visibility = Visibility.Collapsed;
                menetrendkedvencekrendezve.ItemsSource = menetrendkedvencekrendezve_fv(getUser());
            }
            else
            {
                ha_a_menetrend_szerint.Visibility = Visibility.Collapsed;
                ha_a_jaratok_szerint.Visibility = Visibility.Visible;
                jaratokkedvencekrendezve.ItemsSource = jaratokkedvencekrendezve_fv(getUser());
            }
        }

        public async void connection(string ki,string mit, string hol,string elerhetoseg)
        {
            lane_Db item = new lane_Db
            {
                ki=ki,
                mit=mit,
                hol=hol,
                id= elerhetoseg
            };
            try
            {
                await App.MobileService.GetTable<lane_Db>().InsertAsync(item);
                lekerdez_elhagytam_dbt();
                UpdateLayout();
            }
            catch (Exception)
            {
            }
            
        }

        public async void lekerdez_elhagytam_dbt()
        {
            ldb_coll = null;
            ldb_coll = new ObservableCollection<lane_Db>();
            var ldb = await App.MobileService.GetTable<lane_Db>().ReadAsync();
            lane_Db r = new lane_Db();
            foreach (var item in ldb)
            {
                r = new lane_Db();
                r.hol = item.hol;
                r.id = item.id;
                r.ki = item.ki;
                r.mit = item.mit;
                ldb_coll.Add(r);
            }
            elhagytam_list.ItemsSource = ldb_coll;
        }
        public ObservableCollection<lane_Db> ldb_coll = new ObservableCollection<lane_Db>();

        private void elveszettbutton_Click(object sender, RoutedEventArgs e)
        {
            if (ki.Text == "" || mit.Text == "" || hol.Text == "" || id.Text == "")
            {
                figyelmeztetes.Text = "Töltsd ki mindet!";
            }
            else
            {
                connection(ki.Text, mit.Text, hol.Text, id.Text);
                figyelmeztetes.Text = "Sikeresen hozzáadtuk!";
            }
        }

        private void frissitobutton_Click(object sender, RoutedEventArgs e)
        {
            lekerdez_elhagytam_dbt();
        }
    }

    public class Menetrend
    {
        public string Hanyasjarat { get; set; }
        public string Honnanindul { get; set; }
        public string Mikorindul { get; set; }
    }

    public class Megallok
    {
        public string Megallo { get; set; }
        public string Jaratok_mikor { get; set; }
    }

    public class Kedvencek
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(500)]
        public string Kedvenc_jarat { get; set; }

        [MaxLength(500)]
        public string Kedvenc_megallo { get; set; }
    }

    public class lane_Db
    {
        public string id { get; set; }
        public string mit { get; set; }
        public string hol { get; set; }
        public string ki { get; set; }
    }

}
