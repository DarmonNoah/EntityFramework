# ANALYSE — Système de Gestion Hospitalière

## 1. Avantages et inconvénients du modèle

### Avantages

**Modélisation claire et cohérente**
Le modèle reflète fidèlement le domaine métier : chaque entité (Patient, Doctor, Department, Consultation) a une responsabilité unique et bien définie. Les relations sont explicites et documentées.

**Intégrité des données garantie**
- Les index uniques sur `FileNumber` et `Email` empêchent les doublons de patients
- Le `DeleteBehavior.Restrict` sur Doctor/Department protège contre les suppressions accidentelles
- L'index composite `(PatientId, DoctorId, Date)` évite les doublons de consultation

**Owned Type pour l'adresse**
Factoriser l'adresse en `Owned Type` permet de la réutiliser sans créer de table supplémentaire, tout en gardant une structure orientée objet propre dans le code.

**Héritage TPH pour le personnel**
La stratégie Table Per Hierarchy pour `Staff` / `MedicalDoctor` / `Nurse` / `AdminStaff` offre de bonnes performances en lecture (une seule table, pas de jointure) et une migration simple.

**Hiérarchie de départements**
La relation auto-référentielle sur `Department` permet une organisation en arborescence illimitée sans complexité supplémentaire.

**Concurrence gérée**
Le `[Timestamp]` / `RowVersion` sur `Patient` détecte les conflits d'édition simultanée sans verrouillage pessimiste, ce qui est adapté à un environnement multi-utilisateurs.

### Inconvénients

**SQLite en développement**
SQLite est pratique pour le développement mais ne supporte pas toutes les contraintes (ex : CHECK constraints, PRAGMA dans les transactions). En production, SQL Server ou PostgreSQL seraient plus adaptés.

**Héritage TPH avec colonnes nullables**
La table `Staff` contient des colonnes nullables pour chaque sous-type (`Specialty`, `LicenseNumber`, `Service`, `Grade`, `Function`). Si les types de personnel se multiplient, la table devient peu lisible et comporte beaucoup de `NULL`.

**Absence de soft delete**
Les suppressions sont définitives. Dans un contexte médical, il serait préférable d'archiver les données plutôt que de les supprimer physiquement (traçabilité légale).

**Pas d'audit trail**
Le modèle ne trace pas qui a créé ou modifié une entité, ni quand. Dans un système médical, cela peut être une exigence réglementaire (RGPD, HDS).

**Adresse uniquement sur Patient**
L'`Owned Type` Address n'est pas encore utilisé sur Department. La factorisation est donc partielle.

---

## 2. Optimisations pour 100 000 patients

### Base de données
- **Migrer vers SQL Server ou PostgreSQL** : meilleur support des index, partitionnement, et performances en concurrence élevée
- **Index couvrants** : ajouter des index couvrants sur les colonnes fréquemment lues ensemble (ex : `LastName`, `FirstName`, `FileNumber`)
- **Partitionnement** : partitionner la table `Consultations` par année/mois car elle sera la plus volumineuse
- **Archivage** : déplacer les consultations de plus de 5 ans dans une table d'archive pour alléger les requêtes courantes

### Application
- **Cache distribué** (Redis) : mettre en cache les données peu changeantes (liste des départements, médecins actifs)
- **Pagination obligatoire** : ne jamais charger tous les patients, toujours paginer avec `Skip/Take`
- **AsNoTracking** systématique sur toutes les lectures
- **Projections** : toujours utiliser `Select(new DTO {...})` plutôt que de charger les entités complètes
- **Requêtes compilées** (`EF.CompileAsyncQuery`) pour les requêtes très fréquentes

### Infrastructure
- **Read replicas** : séparer les lectures (tableau de bord, statistiques) des écritures (création de dossiers)
- **Full-text search** : utiliser les capacités full-text de PostgreSQL ou SQL Server pour la recherche par nom, bien plus efficace qu'un `LIKE '%name%'`

---

## 3. Implémentation d'un système de rendez-vous en ligne

### Nouveaux besoins fonctionnels
- Les patients peuvent réserver eux-mêmes leurs créneaux
- Les médecins définissent leurs disponibilités
- Le système doit gérer les conflits en temps réel
- Notifications automatiques (rappels, annulations)

### Modifications du modèle

```csharp
// Nouveau : créneaux de disponibilité d'un médecin
public class TimeSlot
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool IsAvailable { get; set; } = true;
}

// Nouveau : compte patient pour l'accès en ligne
public class PatientAccount
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

// Enrichissement de Consultation
public class Consultation
{
    // ... champs existants ...
    public int? TimeSlotId { get; set; }          // Lien vers le créneau réservé
    public TimeSlot? TimeSlot { get; set; }
    public bool IsOnlineBooking { get; set; }      // Réservé en ligne ou non
    public DateTime? BookedAt { get; set; }        // Horodatage de la réservation
    public string? CancellationReason { get; set; }
}
```

### Architecture recommandée
- **API Gateway** : séparer l'API publique (patients) de l'API interne (médecins/admin)
- **Gestion des conflits** : utiliser des transactions sérialisables pour la réservation de créneaux
- **Notifications** : intégrer un service de messagerie (email/SMS) via un pattern Observer ou une queue (RabbitMQ, Azure Service Bus)
- **WebSockets** : pour afficher les créneaux disponibles en temps réel sans rechargement

---

## 4. Impact sur le modèle si on ajoutait la facturation

### Nouvelles entités nécessaires

```csharp
// Facture liée à une consultation
public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty; // Unique
    public int ConsultationId { get; set; }
    public Consultation Consultation { get; set; } = null!;
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
}

// Ligne de facturation (acte médical, médicament, etc.)
public class InvoiceLine
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal VatRate { get; set; }
}

public enum InvoiceStatus { Draft, Issued, Paid, Cancelled }
```

### Impacts sur le modèle existant
- **Patient** : ajouter les informations de couverture sociale et mutuelle (`SocialSecurityNumber`, `InsuranceProvider`)
- **Consultation** : ajouter un lien vers la facture générée + le tarif appliqué
- **Doctor** : ajouter le tarif horaire ou par acte
- **Department** : ajouter un centre de coût pour la comptabilité analytique

### Contraintes à respecter
- Les factures ne doivent **jamais être supprimées** (obligation légale) → soft delete uniquement
- Toute modification de montant doit générer un **avoir** plutôt qu'une modification directe
- Les montants doivent être stockés en `decimal(18,2)` avec la devise explicite
- Archivage obligatoire pendant 10 ans minimum (réglementation comptable française)

---

## Conclusion

Ce modèle constitue une base solide pour un système hospitalier de taille moyenne. Les principaux axes d'amélioration pour une mise en production seraient : migrer vers PostgreSQL, implémenter un audit trail complet, ajouter le soft delete, et séparer les préoccupations de lecture/écriture via un pattern CQRS pour les vues analytiques.
