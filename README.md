# To-Do List Applicatie

Een console-gebaseerd taakbeheersysteem gebouwd met C# en .NET 9.0. Deze applicatie stelt gebruikers in staat om taken en teamleden te beheren met configureerbare datastructuur backends.

## Functies

- **Taakbeheer**: Maak, update en volg taken met beschrijvingen, prioriteiten en status
- **Teambeheer**: Beheer teamleden en wijs ze toe aan taken
- **Taakafhankelijkheden**: Definieer afhankelijkheden tussen taken
- **Meerdere Datastructuur Backends**: Kies tussen Array, LinkedList, Binary Tree of HashMap implementaties
- **JSON Persistentie**: Sla taken en leden op in JSON-formaat
- **Console Interface**: Gebruiksvriendelijke commandoregelpinterface voor alle bewerkingen

## Projectstructuur

```
ToDoList/
├── Program.cs                 # Toepassingsinvoerpunt
├── Data/                      # JSON databestanden
│   ├── members.json           # Opgeslagen teamleden
│   └── tasks.json             # Opgeslagen taken
├── Data Structures/           # Aangepaste verzamelingen implementaties
│   ├── MyArray.cs
│   ├── MyBinarySearchTree.cs
│   ├── MyHashMap.cs
│   └── MyLinkedList.cs
├── Interface/                 # Service en repository interfaces
│   ├── IMemberService.cs
│   ├── ITaskService.cs
│   ├── ITaskView.cs
│   ├── IRepository.cs
│   ├── IMyCollection.cs
│   ├── IMyHashMap.cs
│   └── IMyIterator.cs
├── Model/                     # Gegevensmodellen
│   ├── TaskItem.cs
│   ├── Member.cs
│   └── IHasId.cs
├── Service/                   # Business logic services
│   ├── TaskService.cs
│   └── MemberService.cs
├── Repository/                # Gegevenspersistentielaag
│   └── JsonRepository.cs
└── View/                      # Console UI componenten
    ├── ConsoleTaskView.cs
    ├── FormatHelpers.cs
    └── Options.cs
```

## Aan de slag

### Vereisten

- .NET 9.0 SDK of runtime
- Visual Studio 2022 of een C# compatibele IDE

### Bouwen

```bash
dotnet build
```

### Uitvoeren

```bash
dotnet run
```

Wanneer u de applicatie start, wordt u gevraagd uw voorkeursdatastructuur te selecteren:

1. **Array** - Eenvoudige op array gebaseerde verzameling
2. **LinkedList** - Gekoppelde lijstimplementatie
3. **BinaryTree** - Binaire zoekboomimplementatie
4. **HashMap** - Hash map implementatie

## Gegevensmodellen

### TaskItem
- **Id**: Unieke identificator
- **Description**: Taakbeschrijving
- **Status**: Taakstatus (bijv. in wacht, in uitvoering, voltooid)
- **Priority**: Taakprioriteit
- **AssignedMembers**: Verzameling van lid-ID's toegewezen aan de taak
- **DependantOn**: Verzameling van taak-ID's waarvan deze taak afhangt

### Member
- **Id**: Unieke identificator
- **Name**: Ledennaam
- **Password**: Ledenwachtwoord

## Architectuur

De applicatie volgt een servicegeoriënteerde architectuur met duidelijke scheiding van belangen:

- **Repository Pattern**: Geabstraheerde gegevenspersistentie via `IRepository<T>`
- **Service Laag**: Bedrijfslogica geïmplementeerd in `TaskService` en `MemberService`
- **View Laag**: Console UI beheerd door `ConsoleTaskView`
- **Aangepaste Verzamelingen**: Plug-in datastructuurimplementaties

## Technologiestapel

- **Taal**: C# 13
- **.NET Framework**: .NET 9.0
- **Gegevensformaat**: JSON
- **Architectuur**: Service-Georiënteerde Architectuur (SOA)
