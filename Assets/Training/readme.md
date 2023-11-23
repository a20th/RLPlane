# Ágenstanítás megfigyelések

## Tesztelések
Először megpróbáltam csak arra betanítani az ágenst, hogy nézzen a pontra. A pont egyenlőre ugyanazon a helyen álldogál.
| Tanítás neve | Verdikt | 
|-|-|
| Run1 | Sikertelen tesztfutás, túl nagy lépésszám summary_freq túlontúl alacsony |
| Run2 | Az ágens betanul, de a tanulási görbébe beszakadások vannak, habár a lehető legjobb rewardot hozza (max_step = 1000, potential reward/step = 1); summary_freq még mindig alacsony;  |

Ezután következő környezetet próbáltam betanítani: Az ágens még mindig egy helyben áll, csak forog, de a pont már random ugrál. summary_freq javítva, potential reward/step = 0.1

| Tanítás neve | Verdikt | 
|-|-|
| Run3 | Az ágens nem tanult be, feltehetőleg a kevés összlépésszám miatt |+
| Run4 | Az ágens nem tanult be, feltehetőleg a kevés összlépésszám miatt |


