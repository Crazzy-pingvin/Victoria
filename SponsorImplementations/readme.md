# Предысловие
Данное дополнение позволяет ввести в игру систему спонсорства.
На данный момент требуется ручной ввод.

# Спонсорка
Команды доступны с уровнем доступа Host.
Имеются следующие команды:

`clearsponsor <CKEY>` позволяет очищать полностью спонсорку с игрока.
- CKEY - ник игрока.

#

`addsponsor <CKEY> <приоритетный вход?> <Количества дополнительных слотов под персонажей> <Цвет в OOC> [Прототипы]`
Выдает игроку спонсорку с указаными конфигурациями.
- CKEY - ник игрока.
- Приоритетный вход - приоритет входа игрока при высоком онлайне. принимает true/false.
- Цвет в OOC отвечает за цвет в чате OOC.

Прототипов можно указать несклько.
В поле Прототипы принимаются список прототипов, что имеют поле `sponsorOnly:`.
Они будут указаны позже.

#

`setsponsorgroup <CKEY> <прототип группы>`
Выдает игроку спонсорку, выставляя конфигурации с прототипа.
- CKEY - ник игрока.
- Прототип группы имеет следующую структуру:
```yaml
- type: sponsorGroup
  id: <Айди прототипа. Позже его нужно указать в команде после CKEY>
  color: <цвет оос чата. Указывается примерно "#662311">
  extraCharSlots: <количество дополнительных слотов под персонажей. Пример 5 или любое число выше, либо равен 0>
  serverPriorityJoin: <приоритетный вход. Булевое значение. true/false>
  prototypes:
  - [Список прототипов]


```
На данный момент имеются прототипы с поддержкой спонсорки:
`SpeciesPrototype`
`LoadoutPrototype`
`MarkingPrototype`
`TraitPrototype`
`TTSVoicePrototype`


При желании сделать прототип лишь для спонсоров делаем такие манипулияции:
```yaml
# ПРИМЕР
- type: trait
  id: Accentless
  name: trait-accentless-name
  description: trait-accentless-desc
  category: SpeechTraits
  cost: 2
  sponsorOnly: true # Добавляем, если желаем, чтобы данный прототип имели только люди со спонсоркой.
  components:
  - type: Accentless
    removes:
    - type: LizardAccent
    - type: MothAccent
    - type: ReplacementAccent
      accent: dwarf
```
И далее указываем ID прототипа либо в команде, либо в прототипе группы.

`addsponsor <CKEY> <приоритетный вход?> <Количества дополнительных слотов под персонажей> <Цвет в OOC> Accentless #Вот тут`

```yaml
- type: sponsorGroup
  id: KrasnoGovori
  color: "#662311"
  extraCharSlots: 0
  serverPriorityJoin: false
  prototypes:
  - Accentless # Вот тут
```

# Отдельно про лоадауты
Чтобы добавить вещь как лоадаут, нам требуются некоторые махинации:

```yaml
# Rainbow
- type: loadout
  id: EblanKit
  effects:
  - !type:SponsorLoadoutEffect # Если хотим, чтобы была спонсорская шняга
  equipment:
    jumpsuit: ClothingUniformColorRainbow # Указываем уже свой прототип энтити по желанию
```

```yaml
# Меняем в Prototypes/Loadouts/loadout_groups.yml
- type: loadoutGroup
  id: Trinkets
  name: loadout-group-trinkets
  minLimit: 0
  maxLimit: 3
  loadouts:
  - EblanKit # Eblan
  - SpaceLaw # Corvax-HyperLink
  - FlowerWreath
...
```

Ну и чтобы лоадаут был доступен, потребуется выдать спонсорку с прототипом EblanKit(или что вы там укажете)

# Вот и сказочке конец

