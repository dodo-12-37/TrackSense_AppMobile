# 420-W57-SF_E23_4394_TrackSense_AppMobile
Dépôt de l'Application Mobile du Projet Synthèse pour le cours de Programmation, bases de données et serveurs – AEC (LEA.D4). 

Nous ferons la création d'un objet connecté nommé le TrackSense. Cette objet sera principalement utilisé sur un vélo ou un piéton.

# Débogage

## Android

Il y a plusieurs solutions pour déboguer une application Android. **Si vous voulez utiliser le bluetooth, vous devrez utiliser un appareil Android local**.

### Émulateur

|Avantage                |Désavantage                   |
|------------------------|------------------------------|
|Facile à mettre en place|Problèmes avec le bluetooth   |
|                        |Consomme beaucoup de ressource|

### Sous-système Windows pour Android (WSA) :

- Documentation : https://learn.microsoft.com/fr-fr/windows/android/wsa/
- Vidéo : https://www.youtube.com/watch?v=QpNYzigUdfg&ab_channel=JamesMontemagno
- Barista (Extension VS) : https://marketplace.visualstudio.com/items?itemName=Redth.WindowsSubsystemForAndroidVisualStudioExtension

|Avantage                               |Désavantage                |
|---------------------------------------|---------------------------|
|Lancement rapide (surtout avec Barista)|Requiert Windows 11        |
|                                       |Problèmes avec le bluetooth|

### Appareil Android local

- Documentation : https://developer.android.com/studio/debug/dev-options?hl=fr

|Avantage                |Désavantage                 |
|------------------------|----------------------------|
|Lancement rapide        |Requiert un appareil Android|
|Bluetooth fonctionnel   |                            |
|Facile à mettre en place|                            |

