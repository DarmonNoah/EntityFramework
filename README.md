# HospitalManagement — Système de Gestion Hospitalière

Application ASP.NET Core 8+ avec Entity Framework Core et SQLite, développée dans le cadre d'un TP de gestion hospitalière.

---

## Instructions d'exécution

### Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/)
- Entity Framework Core CLI

### Installation de l'outil EF Core CLI

```bash
dotnet tool install --global dotnet-ef
```

### Cloner / ouvrir le projet

```bash
cd HospitalManagement
```

### Restaurer les dépendances

```bash
dotnet restore
```

### Appliquer les migrations (création de la base de données)

```bash
dotnet ef database update
```

> La base de données SQLite `hospital.db` sera créée automatiquement à la racine du projet.

### Lancer l'application

```bash
dotnet run
```

L'API est disponible sur : **`http://localhost:5038`**  
L'interface Swagger est disponible sur : **`http://localhost:5038/swagger`**

---

## Lancer les tests unitaires

```bash
cd HospitalManagement.Tests
dotnet test
```

Résultat attendu : `Passed! - Failed: 0, Passed: 6`

---

## Architecture du projet

```
HospitalManagement/
├── Controllers/              # Endpoints REST (API)
│   ├── PatientsController.cs
│   ├── DoctorsController.cs
│   ├── DepartmentsController.cs
│   ├── ConsultationsController.cs
│   └── DashboardController.cs
│
├── Data/                     # Accès aux données
│   └── HospitalDbContext.cs  # DbContext EF Core + configuration des relations
│
├── DTOs/                     # Objets de transfert (vues et entrées)
│   ├── PatientDetailsDto.cs
│   ├── DoctorPlanningDto.cs
│   ├── DepartmentStatsDto.cs
│   └── CreateConsultationDto.cs
│
├── Migrations/               # Historique des migrations EF Core
│
├── Models/                   # Entités du domaine
│   ├── Patient.cs
│   ├── Doctor.cs
│   ├── Department.cs
│   ├── Consultation.cs
│   ├── Address.cs            # Owned Type (type complexe factorisé)
│   └── Staff.cs              # Héritage TPH (MedicalDoctor, Nurse, AdminStaff)
│
├── Repositories/             # Pattern Repository (abstraction EF Core)
│   ├── IRepository.cs
│   ├── IPatientRepository.cs
│   ├── IConsultationRepository.cs
│   ├── PatientRepository.cs
│   └── ConsultationRepository.cs
│
├── Services/                 # Logique métier
│   ├── IPatientService.cs
│   ├── PatientService.cs
│   ├── IConsultationService.cs
│   ├── ConsultationService.cs
│   ├── IDashboardService.cs
│   └── DashboardService.cs
│
├── Program.cs                # Point d'entrée, DI, configuration
└── hospital.db               # Base de données SQLite (générée automatiquement)

HospitalManagement.Tests/
├── PatientServiceTests.cs    # Tests unitaires patients
└── ConsultationServiceTests.cs # Tests unitaires consultations
```

---

## Migrations EF Core

| # | Nom | Contenu |
|---|-----|---------|
| 1 | `InitialCreate` | Entités `Patient` et `Department` avec contraintes d'unicité |
| 2 | `AddDoctors` | Entité `Doctor`, relation avec `Department`, responsable médical |
| 3 | `AddConsultations` | Entité `Consultation`, relations Many-to-Many, index composite |
| 4 | `AdvancedModeling` | Owned Type `Address`, héritage TPH `Staff`, sous-départements |
| 5 | `PerformanceAndConcurrency` | Index de performance, `RowVersion` pour la concurrence |

---

## Endpoints disponibles

### Patients
| Méthode | Route | Description |
|---------|-------|-------------|
| GET | `/api/Patients?page=1&pageSize=10` | Liste paginée triée alphabétiquement |
| GET | `/api/Patients/{id}` | Détail d'un patient |
| GET | `/api/Patients/search?name=Dupont` | Recherche par nom |
| POST | `/api/Patients` | Créer un patient |
| PUT | `/api/Patients/{id}` | Modifier un patient |
| DELETE | `/api/Patients/{id}` | Supprimer un patient |

### Départements
| Méthode | Route | Description |
|---------|-------|-------------|
| GET | `/api/Departments` | Liste des départements |
| GET | `/api/Departments/{id}` | Détail d'un département |
| POST | `/api/Departments` | Créer un département |

### Médecins
| Méthode | Route | Description |
|---------|-------|-------------|
| GET | `/api/Doctors` | Liste des médecins avec département |
| GET | `/api/Doctors/{id}` | Détail d'un médecin |
| POST | `/api/Doctors` | Créer un médecin |

### Consultations
| Méthode | Route | Description |
|---------|-------|-------------|
| POST | `/api/Consultations` | Planifier une consultation |
| PATCH | `/api/Consultations/{id}/status` | Modifier le statut |
| PATCH | `/api/Consultations/{id}/cancel` | Annuler une consultation |
| GET | `/api/Consultations/patient/{id}/upcoming` | Prochaines consultations d'un patient |
| GET | `/api/Consultations/doctor/{id}/today` | Consultations du jour d'un médecin |

### Dashboard
| Méthode | Route | Description |
|---------|-------|-------------|
| GET | `/api/Dashboard/patients/{id}` | Fiche complète patient + consultations |
| GET | `/api/Dashboard/doctors/{id}/planning` | Planning médecin + département |
| GET | `/api/Dashboard/departments/stats` | Statistiques par département |

---

## Choix d'architecture

### Clean Architecture simplifiée
Le projet est organisé en couches avec des responsabilités séparées :
- **Controllers** : reçoivent les requêtes HTTP, délèguent aux services, retournent les réponses
- **Services** : contiennent la logique métier, utilisent les repositories ou le contexte directement
- **Repositories** : abstraient l'accès à EF Core derrière des interfaces testables
- **DTOs** : isolent les vues de la représentation interne des entités

### Patterns utilisés
- **Repository Pattern** : isole EF Core derrière des interfaces pour faciliter les tests
- **Dependency Injection** : tous les services et repositories sont injectés via `Program.cs`
- **Owned Types** : l'adresse est un type valeur embarqué dans la table `Patients`
- **TPH (Table Per Hierarchy)** : le personnel médical partage une seule table avec discriminateur
- **DTO / Projection** : les vues dashboard utilisent des `Select()` directs en SQL pour éviter le problème N+1

### Stratégies EF Core
- `AsNoTracking()` systématique sur toutes les lectures
- Index composites sur les colonnes fréquemment requêtées
- `RowVersion` sur `Patient` pour détecter les conflits de concurrence
- `DeleteBehavior.Restrict` sur Doctor/Department pour protéger l'intégrité des données

---

## Dépendances principales

| Package | Version | Usage |
|---------|---------|-------|
| `Microsoft.EntityFrameworkCore` | 9.x | ORM principal |
| `Microsoft.EntityFrameworkCore.Sqlite` | 9.x | Provider SQLite |
| `Microsoft.EntityFrameworkCore.Tools` | 9.x | CLI migrations |
| `Swashbuckle.AspNetCore` | 6.x | Interface Swagger |
| `Microsoft.EntityFrameworkCore.InMemory` | 9.x | Base en mémoire pour les tests |
| `xunit` | 2.x | Framework de tests |
