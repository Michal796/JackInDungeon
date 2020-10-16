# JackInDungeon
UWAGA
Domyślna wartość rozdzielczości dla gry JackInDungeon wynosi 1920x1080.

Wszystkie elementy graficzne w grze zostały udostępnione przez autora książki "Projektowanie gier przy użyciu środowiska Unity i języka C#" J. G. Bonda. Autor udostępnił również pliki tekstowe na podstawie których program wygenerował dwie mapy (poziom pokazowy i grywalny), oraz klasę Utils jako narzędzie do opracowania projektu.

Założenia gry: gra polega na eksploracji lochów bohaterem nazwanym Jack. W grze dostępne są dwa poziomy: pierwszy (pokazowy) ma na celu szybką prezentację mechanik w grze, a drugi jest grywalnym poziomem, który nadaje się do eksploracji.
Bohater wyposażony jest w miecz, który stanowi jego podstawową broń. Podczas gry, znaleźć również linkę z hakiem (w pokazowym poziomie, linka leży w pobliżu poczatkowego położenia bohatera), która umożliwia bohaterowi atakowanie przeciwników na odległość, a także przyciąganie się do ścian. Linka z hakiem umożliwia bohaterowi pokonywanie terenu, który normalnie jest dla niego niemożliwy do przebycia (na przykład czerwone pola lawy).
Jeśli bohater przyciągnie się do ściany, a miejsce w którym się znajdzie będzie niebezpieczene (np. będzie to wspomniana lawa), położenie bohatera zostanie zresetowane, a on sam otrzyma karę w postaci obrażeń.  Przeciwnikami w grze są szkielety. Zderzenie ze szkieletem spowoduje zranienie bohatera, a także jego chwilową nieśmiertelność oraz odrzucenie. Pokonany szkielet może upuścić przedmiot (uzupełnienie zdrowia lub klucz).
W grze dostępne są zamknięte drzwi, które należy otworzyć przy pomocy klucza. Klucze mogą wypadać z pokonanych wrogów, ale można je znaleźć również eksplorując mapę.

Sterowanie: sterowanie postacią w grze odbywa się przy pomocy STRZAŁEK KIERUNKOWYCH,
Atak mieczem wykonuje się przyciskiem 'Z',
Atak lub przyciągnięcie się do ściany hakiem z linką wykonuje się przyciskiem 'X' (najpierw należy podnieść hak)

Skrypty: 
- CamFollowJack - kamera podążająca za postacią do kolejnych pomieszczeń.
- DamageEffect - klasa przypisana do wrogów występujących w grze oraz do miecza głównego bohatera; przechowuje informacje o skutkach ataku.
- Enemy - podstawowa klasa wroga.
- GateKeeper - skrypt dołączony do głównego bohatera, umożliwia mu otwieranie drzwi przy pomocy kluczy.
- Grapple - skrypt obsługujący hak z linką głównego bohatera.
- GridMove - klasa sprawia, że każdy obiekt korzystający z interfejsu IFancerMoving porusza się z wykorzystaniem siatki, co umożliwia precyzyjne przechodzenie przez drzwi.
- GuiPanel - skrypt odpowiedzialny za wyświetlanie informacji o bohaterze, z boku ekranu.
- IFancingMove - interfejs, który umożliwia określonym obiektom ruch z wykorzystaniem siatki.
- IKeyMaster - interfejs umożliwiający otwieranie drzwi bohaterowi, jeśli posiada klucz.
- InRoom - ta klasa sprawia, że wrogowie nie opuszczają pomieszczeń, a także określa pozycję obiektu (numer pokoju oraz pozycję lokalną).
- Jack - skrypt odpowiada za zarządzanie głównym bohaterem gry.
- PickLevelMenu - odpowiada za wybór poziomu na samym początku gry.
- PickUp - klasa przedmiotów, które można podnosić.
- Skeletos - klasa dziedzicząca po klasie Enemy, konkretny rodzaj przeciwnika.
- SwordController - klasa obsługuje miecz postaci.
- Tile - klasa każdego obiektu typu Sprite, z którego zbudowana jest mapa.
- TileCamera - klasa odpowiada za przetwarzanie oraz przechowywanie obiektów typu Tile.
- Utils - klasa udostępniona przez autora wyżej wymienionej książki jako narzędzie do opracowania projektu.
