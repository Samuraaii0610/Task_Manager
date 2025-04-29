# Task Manager

## Description du projet
Task Manager est une application de gestion de tâches développée avec .NET MAUI. Elle permet aux utilisateurs de créer, visualiser, modifier et supprimer des tâches, ainsi que de gérer leur profil utilisateur.

## Fonctionnalités
- Authentification utilisateur (connexion/inscription)
- Gestion de tâches (création, modification, suppression)
- Profil utilisateur
- Interface utilisateur moderne et intuitive

## Prérequis
- Visual Studio 2022 ou plus récent
- .NET 8.0 SDK
- MySQL Server (local ou distant)

## Configuration de la base de données
1. Assurez-vous que MySQL Server est installé et en cours d'exécution
2. Créez une base de données nommée "taskmanager"
3. Par défaut, l'application utilise les paramètres de connexion suivants:
   - Serveur: localhost
   - Base de données: taskmanager
   - Utilisateur: root
   - Mot de passe: (vide)

Si vous souhaitez modifier ces paramètres, vous pouvez le faire dans le fichier `TaskManager/MauiProgram.cs`.

## Lancement de l'application
1. Ouvrez la solution `Task_Manager.sln` dans Visual Studio
2. Assurez-vous que le projet `TaskManager` est défini comme projet de démarrage
3. Sélectionnez la plateforme cible (Windows)
4. Appuyez sur F5 ou cliquez sur le bouton "Démarrer" pour lancer l'application

## Migrations Entity Framework
Si vous modifiez le modèle de données, vous devrez mettre à jour la base de données:

```bash
# Dans la console du Gestionnaire de Package
Add-Migration NomDeLaMigration
Update-Database
```

## Structure du projet
- **TaskManager**: Projet principal contenant l'interface utilisateur et la logique de l'application
- **TaskManager.Data**: Projet contenant les modèles de données et le contexte de base de données
