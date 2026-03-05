# Entity Framework Core - Notes de cours

## ORM
ORM = Object Relational Mapping  
→ convertit objets C# ↔ tables SQL

Principe
- application manipule objets
- ORM génère SQL
- résultat converti en objets

Avantages
- moins de SQL
- code plus lisible
- migrations DB
- protection injection SQL

Inconvénients
- performance parfois moins bonne
- requêtes complexes difficiles
- perte de contrôle SQL

---

# Entity Framework

EF6
- basé ADO.NET
- Windows uniquement

EF Core
- réécriture complète
- cross platform
- open source
- meilleures performances
- approche Code First

---

# Structure projet recommandée

séparation des couches

Core
- entités
- logique métier

Infrastructure
- DbContext
- migrations
- repositories

API / UI
- controllers
- interface utilisateur

---

# Entités

classes POCO (Plain Old CLR Object)

exemple :

class Customer
- Id
- FirstName
- LastName
- Email

navigation

Customer
→ ICollection<Order>

---

# Conventions EF Core

clé primaire
- Id
- EntityNameId

relations
- propriétés de navigation

---

# Data Annotations vs Fluent API

Data Annotations

[Key]  
[Required]  
[MaxLength]

Fluent API

modelBuilder.Entity<Product>()
    .Property(p => p.Name)
    .IsRequired()

Fluent API
- plus flexible
- configuration centralisée

---

# Relations

Types

1:1  
1:N  
N:M  

exemple :

Customer
→ Orders

Order
→ CustomerId

---

# DbContext

représente une session avec la base

responsabilités
- connexion
- suivi entités
- requêtes LINQ
- transactions

exemple :

class ApplicationDbContext : DbContext

DbSet<Customer>
DbSet<Product>
DbSet<Order>

DbSet = table

---

# Cycle de vie entité

states

Detached  
Added  
Unchanged  
Modified  
Deleted  

SaveChanges()
→ génère SQL

---

# Configuration modèle

OnModelCreating()

sert à configurer :
- entités
- relations
- contraintes

ex :

modelBuilder.Entity<Customer>()
    .HasKey(c => c.Id)

---

# LINQ

Language Integrated Query

permet requêtes C#

sources
- collections
- bases de données

ex :

context.Products
    .Where(p => p.Price > 100)

---

# Execution des requêtes

lazy execution
→ exécutée seulement quand nécessaire

exécution immédiate

ToList()  
Count()  
First()

---

# CRUD

Create  
Read  
Update  
Delete  

---

# Create

context.Products.Add(product)

context.SaveChanges()

async

await context.Products.AddAsync(product)

---

# Read

context.Products.ToList()

avec filtre

context.Products
    .Where(p => p.Price > 100)

---

# Update

product = context.Products.Find(id)

modifier propriétés

context.SaveChanges()

---

# Delete

product = context.Products.Find(id)

context.Products.Remove(product)

context.SaveChanges()

---

# Change Tracking

EF suit les modifications des entités

states
- Added
- Modified
- Deleted

désactiver suivi

AsNoTracking()

utile pour lecture seule

---

# Bonnes pratiques

- filtrer avant projection
- éviter N+1 queries
- utiliser Include pour relations
- utiliser DTO
- utiliser AsNoTracking pour lecture

ex N+1 :

products = context.Products

foreach product
→ requête category

solution

Include(p => p.Category)

---

# Pagination

Skip + Take

query
.Skip((page-1)*pageSize)
.Take(pageSize)