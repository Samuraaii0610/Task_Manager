Objectifs pédagogiques 
 
Ce projet a pour but de vous faire concevoir et développer une application de 
gestion de tâches multiplateforme en utilisant les technologies suivantes : 
- C# avec le framework .NET MAUI (Multi-platform App UI) 
- Le pattern de conception MVVM (Model-View-ViewModel) 
- Entity Framework Core pour la gestion des données 
- Une base de données MySQL pour le stockage des tâches et de leurs métadonnées. 
 
Ce projet sera réalisé à 2 étudiants. Une gestion de projet devra être mise en place 
afin de suivre l’avancement et la répartition des tâches au sein de l’équipe. On 
optera pour une gestion de projet dite AGILE de type Scrum avec, par exemple, un 
sprint par semaine. 
Spécifications fonctionnelles 
L’application permettra à un utilisateur de : 
• Créer un compte et s’authentifier 
• Créer, modifier, supprimer des tâches 
• Ajouter des sous-tâches, étiquettes, commentaires 
• Suivre l’évolution d’une tâche via son statut 
• Afficher l’historique des modifications 
• Visualiser les tâches selon leur priorité, échéance ou catégorie 
• Consulter une vue détaillée de chaque tâche 
• D’associer une tâche à un projet 
• De confier une tâche à une autre personne (le réalisateur) 
 
Modèle de données 
Chaque tâche contient les champs suivants : 
• titre : Titre court de la tâche 
• description : Détail de la tâche 
• dateCreation : Date de création de la tâche 
• echeance : Date limite de la tâche 
• statut : "à faire", "en cours", "terminée", "annulée" 
• priorite : "basse", "moyenne", "haute", "critique" 
• auteur : nom, prénom, email 
• Réalisateur : nom, prénom, email 
• categorie : perso, travail, projet, etc. 
• etiquettes : Liste de mots-clés 
• sousTaches : titre, statut, échéance (facultatif) 
• commentaires : auteur, date, contenu 
Technologies imposées 
 
• C# avec le framework .NET MAUI 
• Entity Framework Core avec MySQL (Pomelo.EntityFrameworkCore.MySql) 
• Design Pattern MVVM (CommunityToolkit.MVVM recommandé) 
• GitHub pour les dépôts de code (Git) et pour la gestion de projet (Boards). 
Étapes de réalisation 
 
1. Configuration du projet MAUI et connexion MySQL avec EF Core 
2. Réalisation du MCD et création des entités et du DbContext 
3. Développement des vues et des ViewModels 
4. Intégration du CRUD et des relations complexes (étiquettes, sous-tâches, etc.) 
5. Ajout de la logique de filtrage, tri, et affichage 
5. Gestion des comptes utilisateurs et authentification 
6. Tests de fonctionnement 
Livrables attendus 
Pour la fin de la semaine 16 : 
• Code source complet structuré en MVVM 
• Script SQL de création de la base 
• Documentation technique (modèle, architecture, captures d’écran) 
• Rapport synthétique sur les choix techniques et difficultés rencontrées